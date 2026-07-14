using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Categories;
using HelpdeskSystem.Common.DTOs.Groups;
using HelpdeskSystem.Common.DTOs.SubCategories;

namespace HelpdeskSystem.Application.Interfaces;

public interface IGroupService
{
    // Groups
    Task<ApiResponse<List<GroupDto>>> GetAllGroupsAsync();
    Task<ApiResponse<PagedResponse<GroupDto>>> GetGroupsPagedAsync(int page, int pageSize, string? search);
    Task<ApiResponse<GroupDto>> CreateGroupAsync(CreateGroupDto dto);
    Task<ApiResponse<GroupDto>> UpdateGroupAsync(int id, UpdateGroupDto dto);
    Task<ApiResponse<bool>> DeleteGroupAsync(int id);

    // Categories
    Task<ApiResponse<List<CategoryDto>>> GetCategoriesByGroupAsync(int groupId);
    Task<ApiResponse<PagedResponse<CategoryDto>>> GetCategoriesByGroupPagedAsync(int groupId, int page, int pageSize, string? search);
    Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto);
    Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
    Task<ApiResponse<bool>> DeleteCategoryAsync(int id);

    // SubCategories
    Task<ApiResponse<List<SubCategoryDto>>> GetSubCategoriesByCategoryAsync(int categoryId);
    Task<ApiResponse<PagedResponse<SubCategoryDto>>> GetSubCategoriesByCategoryPagedAsync(int categoryId, int page, int pageSize, string? search);
    Task<ApiResponse<SubCategoryDto>> CreateSubCategoryAsync(CreateSubCategoryDto dto);
    Task<ApiResponse<SubCategoryDto>> UpdateSubCategoryAsync(int id, UpdateSubCategoryDto dto);
    Task<ApiResponse<bool>> DeleteSubCategoryAsync(int id);
}
