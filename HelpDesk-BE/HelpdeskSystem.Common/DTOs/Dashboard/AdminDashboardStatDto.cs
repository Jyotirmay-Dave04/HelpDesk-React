namespace HelpdeskSystem.Common.DTOs.Dashboard;

public class AdminDashboardStatDto
{
    public int Total { get; set; }
    public int Open { get; set; }
    public int Assigned { get; set; }
    public int InProgress { get; set; }
    public int OnHold { get; set; }
    public int Resolved { get; set; }
    public int Closed { get; set; }
    public int Rejected { get; set; }
    public int Cancelled { get; set; }
    public int CannotResolve { get; set; }
    public int SlaBreachedTotal { get; set; }
    public int SlaBreachedActive { get; set; }
    public int ReopenCount { get; set; }
}
