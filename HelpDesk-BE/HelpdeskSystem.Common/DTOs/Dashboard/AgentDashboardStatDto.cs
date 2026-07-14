namespace HelpdeskSystem.Common.DTOs.Dashboard;

public class AgentDashboardStatDto
{
    public int AssignedToMe { get; set; }
    public int InProgress { get; set; }
    public int OnHold { get; set; }
    public int Resolved { get; set; }
    public int Closed { get; set; }
    public int CannotResolve { get; set; }
    public int SlaBreachedActive { get; set; }
    public int SlaBreachedTotal { get; set; }
    public int ReopenCount { get; set; }
}
