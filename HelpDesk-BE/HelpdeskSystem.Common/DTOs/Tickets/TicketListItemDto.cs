using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.Common.DTOs.Tickets;

public class TicketListItemDto
{
    public int Id { get; set; }
    public string ServiceDetails { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string SubCategoryName { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public string? AssignedToName { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime SlaDeadline { get; set; }
    public bool SlaBreached { get; set; }
    public DateTime CreatedAt { get; set; }
}
