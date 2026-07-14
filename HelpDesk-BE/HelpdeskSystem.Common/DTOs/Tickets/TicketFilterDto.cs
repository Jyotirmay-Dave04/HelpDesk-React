namespace HelpdeskSystem.Common.DTOs.Tickets;

public class TicketFilterDto
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public int? GroupId { get; set; }
    public int? CategoryId { get; set; }
    public int? AssignedTo { get; set; }
    public bool? Breached { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public bool IsMyList { get; set; } = false;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    private DateTime? _dateFrom;
    public DateTime? DateFrom
    {
        get => _dateFrom;
        set => _dateFrom = value.HasValue
            ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
            : null;
    }

    private DateTime? _dateTo;
    public DateTime? DateTo
    {
        get => _dateTo;
        set => _dateTo = value.HasValue
            ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc).AddDays(1).AddSeconds(-1)
            : null;
    }
}
