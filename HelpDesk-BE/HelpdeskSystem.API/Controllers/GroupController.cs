using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Categories;
using HelpdeskSystem.Common.DTOs.Groups;
using HelpdeskSystem.Common.DTOs.SubCategories;
using HelpdeskSystem.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
 
        public GroupController(IGroupService groupService)
            => _groupService = groupService;
    
        [HttpGet("groups")]
        public async Task<IActionResult> GetAllGroups()
        {
            ApiResponse<List<GroupDto>> result = await _groupService.GetAllGroupsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("groups/paged")]
        public async Task<IActionResult> GetGroupsPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 5, [FromQuery] string? search = null)
        {
            ApiResponse<PagedResponse<GroupDto>> result = await _groupService.GetGroupsPagedAsync(page, pageSize, search);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("groups/paged")]
        public async Task<IActionResult> GetGroupsPagedWithPost([FromBody] GetPagedGroupDto dto)
        {
            ApiResponse<PagedResponse<GroupDto>> result = await _groupService.GetGroupsPagedAsync(dto.Page, dto.PageSize, dto.Search);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("groups/create")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> CreateGroup(CreateGroupDto dto)
        {
            ApiResponse<GroupDto> result = await _groupService.CreateGroupAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("groups/update/{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> UpdateGroup(int id, UpdateGroupDto dto)
        {
            ApiResponse<GroupDto> result = await _groupService.UpdateGroupAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("groups/delete/{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            ApiResponse<bool> result = await _groupService.DeleteGroupAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    


        [HttpGet("categories/group/{groupId:int}")]
        public async Task<IActionResult> GetCategoriesByGroup(int groupId)
        {
            ApiResponse<List<CategoryDto>> result = await _groupService.GetCategoriesByGroupAsync(groupId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("categories/group/{groupId:int}/paged")]
        public async Task<IActionResult> GetCategoriesByGroupPaged(int groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5, [FromQuery] string? search = null)
        {
            ApiResponse<PagedResponse<CategoryDto>> result = await _groupService.GetCategoriesByGroupPagedAsync(groupId, page, pageSize, search);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("categories/group/{groupId:int}/paged")]
        public async Task<IActionResult> GetCategoriesByGroupPagedWithPost([FromBody] GetPagedCategoryDto dto)
        {
            ApiResponse<PagedResponse<CategoryDto>> result = await _groupService.GetCategoriesByGroupPagedAsync(dto.GroupId, dto.Page, dto.PageSize, dto.Search);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("categories/create")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
        {
            ApiResponse<CategoryDto> result = await _groupService.CreateCategoryAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("categories/update/{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto dto)
        {
            ApiResponse<CategoryDto> result = await _groupService.UpdateCategoryAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("categories/delete/{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            ApiResponse<bool> result = await _groupService.DeleteCategoryAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    


        [HttpGet("subcategories/category/{categoryId:int}")]
        public async Task<IActionResult> GetSubCategoriesByCategory(int categoryId)
        {
            ApiResponse<List<SubCategoryDto>> result = await _groupService.GetSubCategoriesByCategoryAsync(categoryId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("subcategories/category/{categoryId:int}/paged")]
        public async Task<IActionResult> GetSubCategoriesByCategoryPaged(int categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5, [FromQuery] string? search = null)
        {
            ApiResponse<PagedResponse<SubCategoryDto>> result = await _groupService.GetSubCategoriesByCategoryPagedAsync(categoryId, page, pageSize, search);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("subcategories/category/{categoryId:int}/paged")]
        public async Task<IActionResult> GetSubCategoriesByCategoryPagedWithPost([FromBody] GetPagedSubCategoryDto dto)
        {
            ApiResponse<PagedResponse<SubCategoryDto>> result = await _groupService.GetSubCategoriesByCategoryPagedAsync(dto.CategoryId, dto.Page, dto.PageSize, dto.Search);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("subcategories/create")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> CreateSubCategory(CreateSubCategoryDto dto)
        {
            ApiResponse<SubCategoryDto> result = await _groupService.CreateSubCategoryAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("subcategories/update/{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> UpdateSubCategory(int id, UpdateSubCategoryDto dto)
        {
            ApiResponse<SubCategoryDto> result = await _groupService.UpdateSubCategoryAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("subcategories/delete/{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            ApiResponse<bool> result = await _groupService.DeleteSubCategoryAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
