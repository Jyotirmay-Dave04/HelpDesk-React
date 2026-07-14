using AutoMapper;
using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Comments;
using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Application.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public CommentService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService, INotificationService notificationService)
    {
        _uow = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUserService;
        _notificationService = notificationService;
    }
    public async Task<ApiResponse<CommentResponseDto>> CreateAsync(CreateCommentDto createCommentDto)
    {
        Ticket? ticket = await _uow.Tickets.FindOneAsync(t => t.Id == createCommentDto.TicketId && !t.IsDeleted);
        if(ticket is null)
            return ApiResponse<CommentResponseDto>.FailureResponse("Ticket not found.");
        
         bool canAccessTicket = _currentUser.GetUserRole() switch {
            nameof(UserRole.Admin)      => true,
            nameof(UserRole.Agent)      => ticket.AssignedTo == _currentUser.GetUserId(),
            nameof(UserRole.Requester)  => ticket.RequestedBy == _currentUser.GetUserId(),
            _                           => false
        };

        if(!canAccessTicket)
            return ApiResponse<CommentResponseDto>.FailureResponse("Access denied.");
        
        bool isInternal = createCommentDto.IsInternal && _currentUser.GetUserRole() != nameof(UserRole.Requester);

        Comment newComment = new Comment {
            TicketId = createCommentDto.TicketId,
            AuthorId = _currentUser.GetUserId(),
            Body = createCommentDto.Body.Trim(),
            IsInternal = isInternal
        };

        await _uow.Comments.AddAsync(newComment);
        await _uow.SaveChangesAsync();

        Comment? comment = await _uow.Comments.FindOneAsync(c => c.Id == newComment.Id && !c.IsDeleted, includes: q => q.Include(c => c.Author));
        if(comment is null)
            return ApiResponse<CommentResponseDto>.FailureResponse("Failed to create or find comment.");
        
        CommentResponseDto response = _mapper.Map<CommentResponseDto>(comment);

        if(response.IsInternal) {
            if (_currentUser.GetUserRole() == nameof(UserRole.Agent))
                await _notificationService.NotifyRoleAsync(nameof(UserRole.Admin), ticket.Id, NotificationType.CommentAdded, $"Internal note added on ticket #{ticket.Id}");
            else if (ticket.AssignedTo.HasValue)
                await _notificationService.NotifyUserAsync(ticket.AssignedTo.Value, ticket.Id, NotificationType.CommentAdded, $"Internal note added on ticket #{ticket.Id}");
        } else {
            bool isFromRequester = _currentUser.GetUserId() == ticket.RequestedBy;
            int? notifyUserId = isFromRequester ? ticket.AssignedTo : ticket.RequestedBy;
            if (notifyUserId.HasValue)
                await _notificationService.NotifyUserAsync(notifyUserId.Value, ticket.Id, NotificationType.CommentAdded, $"New comment on ticket #{ticket.Id}");
        }

        await _notificationService.BroadcastCommentAsync(ticket.Id, response);

        return ApiResponse<CommentResponseDto>.SuccessResponse(response);
    }

    public async Task<ApiResponse<List<CommentResponseDto>>> GetByTicketIdAsync(int ticketId)
    {
        Ticket? ticket = await _uow.Tickets.FindOneAsync(t => t.Id == ticketId && !t.IsDeleted);
        if(ticket is null)
            return ApiResponse<List<CommentResponseDto>>.FailureResponse("Ticket not found.");
        
        bool canAccessTicket = _currentUser.GetUserRole() switch {
            nameof(UserRole.Admin)      => true,
            nameof(UserRole.Agent)      => ticket.AssignedTo == _currentUser.GetUserId(),
            nameof(UserRole.Requester)  => ticket.RequestedBy == _currentUser.GetUserId(),
            _                           => false
        };

        if(!canAccessTicket)
            return ApiResponse<List<CommentResponseDto>>.FailureResponse("Access denied.");

        IEnumerable<Comment> comments = await _uow.Comments.FindAsync(
            filter: c => c.TicketId == ticketId && !c.IsDeleted && (_currentUser.GetUserRole() != nameof(UserRole.Requester) || !c.IsInternal), 
            orderBy: q => q.OrderByDescending(c => c.CreatedAt),
            includes: q => q.Include(c => c.Author)
        );
        
        List<CommentResponseDto> response = _mapper.Map<List<CommentResponseDto>>(comments);

        return ApiResponse<List<CommentResponseDto>>.SuccessResponse(response);
    }
}
