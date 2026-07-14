using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Common;

namespace HelpdeskSystem.Domain.Entities;

public class AuditLog : BaseEntity
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int? ChangedBy { get; set; }
    public AuditAction Action { get; set; }
    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Reason { get; set; }

    // Navigation properties
    public Ticket Ticket { get; set; } = null!;
    public User? ChangedByUser { get; set; } = null!;
}
