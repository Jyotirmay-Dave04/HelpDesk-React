namespace HelpdeskSystem.Common.DTOs.Groups;

public class GetPagedGroupDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; } = string.Empty;
}
