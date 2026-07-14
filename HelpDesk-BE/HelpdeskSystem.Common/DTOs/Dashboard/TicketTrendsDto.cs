namespace HelpdeskSystem.Common.DTOs.Dashboard;

public class TicketTrendsDto
{
    public List<string> Labels { get; set; } = [];
    public List<int> CreatedData { get; set; } = [];
    public List<int> ResolvedData { get; set; } = [];
}
