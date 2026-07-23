namespace HelpdeskSystem.Common.DTOs.Categories;

public class GetPagedCategoryDto
{
    public int GroupId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; } = string.Empty;
}
