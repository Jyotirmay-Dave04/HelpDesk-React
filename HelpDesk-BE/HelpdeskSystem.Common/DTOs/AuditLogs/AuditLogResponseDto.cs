namespace HelpdeskSystem.Common.DTOs.AuditLogs;

public class AuditLogResponseDto
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int? ChangedBy { get; set; }
    public string? ChangedByName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
