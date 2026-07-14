using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Common;

namespace HelpdeskSystem.Domain.Entities;

public class Notification : BaseEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? TicketId { get; set; }
    public NotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;

    // Navigation properties
    public User User { get; set; } = null!;
    public Ticket? Ticket { get; set; }
}
