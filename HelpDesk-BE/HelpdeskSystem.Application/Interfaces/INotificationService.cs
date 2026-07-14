using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Comments;
using HelpdeskSystem.Common.DTOs.Notifications;
using HelpdeskSystem.Common.DTOs.Tickets;
using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.Application.Interfaces;

public interface INotificationService
{
    Task<ApiResponse<bool>> NotifyUserAsync(int userId, int? ticketId, NotificationType type, string message);
    Task<ApiResponse<bool>> NotifyRoleAsync(string role, int? ticketId, NotificationType type, string message);
    Task<ApiResponse<bool>> BroadcastCommentAsync(int ticketId, CommentResponseDto comment);
    Task<ApiResponse<bool>> BroadcastNewTicketAsync(TicketResponseDto ticket);
    Task<ApiResponse<bool>> BroadcastUpdatedTicketAsync(TicketResponseDto ticket);
    Task<ApiResponse<PagedResponse<NotificationResponseDto>>> GetUserNotificationsAsync(int userId, NotificationRequestFilterDto filterDto);
    Task<ApiResponse<int>> GetUnreadCountAsync(int userId);
    Task<ApiResponse<bool>> MarkAsReadAsync(int notificationId, int userId);
    Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId);
}
