namespace HelpdeskSystem.Common.DTOs.Tickets;

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
