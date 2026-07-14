using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Common;

namespace HelpdeskSystem.Domain.Entities;

public class Ticket : BaseEntity
{
    public int Id { get; set; }
    public int RequestedBy { get; set; }
    public int? AssignedTo { get; set; }
    public int GroupId { get; set; }
    public int CategoryId { get; set; }
    public int SubCategoryId { get; set; }
    public Priority Priority { get; set; }
    public string ServiceDetails { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public DateTime SlaDeadline { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public bool SlaBreached { get; set; } = false;
    public DateTime? SlaPausedAt { get; set; }
    public long OnHoldPausedSeconds { get; set; } = 0;
    public long PostResolutionPausedSeconds { get; set; } = 0;
    public bool BreachedBeforeReopen { get; set; } = false;
    public int ReopenCount { get; set; } = 0;

    // Navigation properties
    public User RequestedByUser { get; set; } = null!;
    public User? AssignedToUser { get; set; }
    public Group Group { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public SubCategory SubCategory { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
