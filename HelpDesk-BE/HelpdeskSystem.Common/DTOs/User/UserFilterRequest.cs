namespace HelpdeskSystem.Common.DTOs.User;

public class UserFilterRequest
{
    public string? Role { get; set; }
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
