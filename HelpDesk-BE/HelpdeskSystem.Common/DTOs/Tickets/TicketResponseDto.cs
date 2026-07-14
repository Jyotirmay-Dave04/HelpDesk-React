using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.Common.DTOs.Tickets;

public class TicketResponseDto
{
    public int Id { get; set; }
    public string ServiceDetails { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
 
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
 
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
 
    public int SubCategoryId { get; set; }
    public string SubCategoryName { get; set; } = string.Empty;
 
    public int RequestedById { get; set; }
    public string RequesterName { get; set; } = string.Empty;
 
    public int? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }

    public DateTime? ResolvedAt { get; set; }
 
    public DateTime SlaDeadline { get; set; }
    public bool SlaBreached { get; set; }

    public bool BreachedBeforeReopen { get; set; }
    public int ReopenCount { get; set; }

    public bool IsSlaPaused { get; set; }
    public long OnHoldPausedSeconds { get; set; }
    public long PostResolutionPausedSeconds { get; set; }
 
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
