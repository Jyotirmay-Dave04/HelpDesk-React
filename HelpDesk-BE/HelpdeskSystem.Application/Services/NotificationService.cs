using AutoMapper;
using HelpdeskSystem.Application.Hubs;
using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Comments;
using HelpdeskSystem.Common.DTOs.Notifications;
using HelpdeskSystem.Common.DTOs.Tickets;
using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _uow;
    private readonly IHubContext<NotificationHub> _hub;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext, IMapper mapper)
    {
        _uow = unitOfWork;
        _hub = hubContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<bool>> BroadcastCommentAsync(int ticketId, CommentResponseDto comment)
    {
        bool ticketExists = await _uow.Tickets.ExistsAsync(t => t.Id == ticketId && !t.IsDeleted);
        if(!ticketExists)
            return ApiResponse<bool>.FailureResponse("Ticket not found!");
        
        await _hub.Clients.Group($"ticket-{ticketId}").SendAsync("RecieveComment", comment);

        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<bool>> BroadcastNewTicketAsync(TicketResponseDto ticket)
    {   
        await _hub.Clients.Group("admins").SendAsync("RecieveNewTicket", ticket);

        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<bool>> BroadcastUpdatedTicketAsync(TicketResponseDto ticket)
    {
        await _hub.Clients.Group("admins").SendAsync("ReciveUpdatedTicket", ticket);
        await _hub.Clients.Group($"user-{ticket.RequestedById}").SendAsync("ReciveUpdatedTicket", ticket);
        await _hub.Clients.Group($"user-{ticket.AssignedToId}").SendAsync("ReciveUpdatedTicket", ticket);

        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<int>> GetUnreadCountAsync(int userId)
    {
        User? user = await _uow.Users.FindOneAsync(u => u.Id == userId && !u.IsDeleted);
        if(user is null)
            return ApiResponse<int>.FailureResponse("User does not exist!");

        IQueryable<Notification> query = _uow.Notifications.Query();

        int count = await query.CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);

        return ApiResponse<int>.SuccessResponse(count);
    }

    public async Task<ApiResponse<PagedResponse<NotificationResponseDto>>> GetUserNotificationsAsync(int userId, NotificationRequestFilterDto filteDto)
    {
        bool userExists = await _uow.Users.ExistsAsync(u => u.Id == userId && !u.IsDeleted);
        if(!userExists)
            return ApiResponse<PagedResponse<NotificationResponseDto>>.FailureResponse("User does not exist!");

        IQueryable<Notification> query = _uow.Notifications.Query();
        query = query.Where(n => n.UserId == userId && !n.IsDeleted && !n.IsRead);

        (IEnumerable<Notification> Items, int total) = await _uow.Notifications.GetPaginatedAsync(
            query: query,
            orderBy: o => o.OrderByDescending(n => n.CreatedAt),
            pageNumber: filteDto.Page, 
            pageSize: filteDto.PageSize
        );

        List<NotificationResponseDto> paged = _mapper.Map<List<NotificationResponseDto>>(Items);

        PagedResponse<NotificationResponseDto> result = new PagedResponse<NotificationResponseDto>
        {
            Items = paged,
            TotalCount = total,
            Page = filteDto.Page,
            PageSize = filteDto.PageSize
        };

        return ApiResponse<PagedResponse<NotificationResponseDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId)
    {
        bool userExists = await _uow.Users.ExistsAsync(u => u.Id == userId && !u.IsDeleted);
        if(!userExists)
            return ApiResponse<bool>.FailureResponse("User does not exist!");

        IEnumerable<Notification> notifications = await _uow.Notifications.GetAllAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);

        foreach(Notification notification in notifications){
            notification.IsRead = true;
            _uow.Notifications.Update(notification);
        }
        await _uow.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<bool>> MarkAsReadAsync(int notificationId, int userId)
    {
        bool userExists = await _uow.Users.ExistsAsync(u => u.Id == userId && !u.IsDeleted);
        if(!userExists)
            return ApiResponse<bool>.FailureResponse("User does not exist!");

        Notification? notification = await _uow.Notifications.FindOneAsync(n => n.Id == notificationId && !n.IsRead && !n.IsDeleted);
        if (notification is null)
            return ApiResponse<bool>.FailureResponse("Notification not found or alreay read.");
        
        if(notification.UserId != userId)
            return ApiResponse<bool>.FailureResponse("Notification is not for given user.");
        
        notification.IsRead = true;
        _uow.Notifications.Update(notification);
        await _uow.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<bool>> NotifyRoleAsync(string role, int? ticketId, NotificationType type, string message)
    {
        if(string.IsNullOrEmpty(role) || !Enum.TryParse<UserRole>(role, true, out UserRole userRole))
            return ApiResponse<bool>.FailureResponse("Use valid role.");
        
        if(ticketId is not null){
            bool ticketExists = await _uow.Tickets.ExistsAsync(t => t.Id == ticketId && !t.IsDeleted);
            if(!ticketExists)
                return ApiResponse<bool>.FailureResponse("Ticket does not exists!");
        }

        IEnumerable<User> users = await _uow.Users.GetAllAsync(u => u.Role == userRole && !u.IsDeleted);

        List<Notification> notifications = users.Select(u => new Notification
        {
            UserId = u.Id,
            TicketId = ticketId,
            Type = type,
            Message = message,
            IsRead = false
        }).ToList();

        await _uow.Notifications.AddRangeAsync(notifications);
        await _uow.SaveChangesAsync();

        foreach (var notification in notifications)
        {
            NotificationResponseDto dto = _mapper.Map<NotificationResponseDto>(notification);
            await _hub.Clients.Group($"user-{notification.UserId}").SendAsync("ReceiveNotification", dto);
        }

        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<bool>> NotifyUserAsync(int userId, int? ticketId, NotificationType type, string message)
    {
        bool userExists = await _uow.Users.ExistsAsync(u => u.Id == userId && !u.IsDeleted);
        if(!userExists)
            return ApiResponse<bool>.FailureResponse("User does not exist!");

        if(ticketId is not null){
            bool ticketExists = await _uow.Tickets.ExistsAsync(t => t.Id == ticketId && !t.IsDeleted);
            if(!ticketExists)
                return ApiResponse<bool>.FailureResponse("Ticket does not exists!");
        }
        
        Notification notification = new Notification
        {
            UserId = userId,
            TicketId = ticketId,
            Type = type,
            Message = message,
            IsRead = false
        };
        await _uow.Notifications.AddAsync(notification);
        await _uow.SaveChangesAsync();

        NotificationResponseDto dto = _mapper.Map<NotificationResponseDto>(notification);
        await _hub.Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", dto);

        return ApiResponse<bool>.SuccessResponse(true);
    }
}
