using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.Common.DTOs.Notifications;

public class NotificationResponseDto
{
    public int Id { get; set; }
    public int? TicketId { get; set; }
    public NotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}