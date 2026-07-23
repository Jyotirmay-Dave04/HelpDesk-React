namespace HelpdeskSystem.Common.DTOs.SubCategories;

public class GetPagedSubCategoryDto
{
    public int CategoryId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
}
