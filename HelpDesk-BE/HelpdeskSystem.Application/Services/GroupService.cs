using AutoMapper;
using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Categories;
using HelpdeskSystem.Common.DTOs.Groups;
using HelpdeskSystem.Common.DTOs.SubCategories;
using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Application.Services;

public class GroupService : IGroupService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GroupService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto)
    {
        bool groupExists = await _uow.Groups.ExistsAsync(g => g.Id == dto.GroupId && !g.IsDeleted);
        if (!groupExists)
            return ApiResponse<CategoryDto>.FailureResponse("Group not found.");

        bool exists = await _uow.Categories.ExistsAsync(c => c.Name.ToLower() == dto.Name.Trim().ToLower() && c.GroupId == dto.GroupId && !c.IsDeleted);
        if (exists)
            return ApiResponse<CategoryDto>.FailureResponse("A category with this name already exists in this group.");

        Category category = new Category { Name = dto.Name.Trim(), GroupId = dto.GroupId };
        await _uow.Categories.AddAsync(category);
        await _uow.SaveChangesAsync();

        return ApiResponse<CategoryDto>.SuccessResponse(
            _mapper.Map<CategoryDto>(category), "Category created successfully.");
    }

    public async Task<ApiResponse<GroupDto>> CreateGroupAsync(CreateGroupDto dto)
    {
        bool exists = await _uow.Groups.ExistsAsync(g => g.Name.ToLower() == dto.Name.Trim().ToLower() && !g.IsDeleted);
        if (exists)
            return ApiResponse<GroupDto>.FailureResponse("A group with this name already exists.");

        Group group = new Group { Name = dto.Name.Trim() };
        await _uow.Groups.AddAsync(group);
        await _uow.SaveChangesAsync();

        return ApiResponse<GroupDto>.SuccessResponse(
            _mapper.Map<GroupDto>(group), "Group created successfully.");
    }

    public async Task<ApiResponse<SubCategoryDto>> CreateSubCategoryAsync(CreateSubCategoryDto dto)
    {
        bool groupExists = await _uow.Categories.ExistsAsync(c => c.Id == dto.CategoryId && !c.IsDeleted);
        if (!groupExists)
            return ApiResponse<SubCategoryDto>.FailureResponse("Category not found.");

        bool exists = await _uow.SubCategories.ExistsAsync(s => s.Name.ToLower() == dto.Name.Trim().ToLower() && s.CategoryId == dto.CategoryId && !s.IsDeleted);
        if (exists)
            return ApiResponse<SubCategoryDto>.FailureResponse("A sub category with this name already exists in this category.");

        SubCategory subCategory = new SubCategory { Name = dto.Name.Trim(), CategoryId = dto.CategoryId };
        await _uow.SubCategories.AddAsync(subCategory);
        await _uow.SaveChangesAsync();

        return ApiResponse<SubCategoryDto>.SuccessResponse(
            _mapper.Map<SubCategoryDto>(subCategory), "Sub category created successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
    {
        Category? category = await _uow.Categories.FindOneAsync(c => c.Id == id && !c.IsDeleted, includes: q => q.Include(c => c.SubCategories));
        if (category is null)
            return ApiResponse<bool>.FailureResponse("Category not found.");

        bool hasTickets = await _uow.Tickets.ExistsAsync(t => t.CategoryId == id && !t.IsDeleted);
        if (hasTickets)
            return ApiResponse<bool>.FailureResponse("Cannot delete category — it has tickets assigned to it.");

        foreach (SubCategory subCats in category.SubCategories)
            await DeleteSubCategoryAsync(subCats.Id);

        _uow.Categories.Delete(category);
        await _uow.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Category deleted successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteGroupAsync(int id)
    {
        Group? group = await _uow.Groups.FindOneAsync(g => g.Id == id && !g.IsDeleted,
            includes: q => q.Include(g => g.Categories));
        if (group is null)
            return ApiResponse<bool>.FailureResponse("Group not found.");

        bool hasTickets = await _uow.Tickets.ExistsAsync(t => t.GroupId == id && !t.IsDeleted);
        if (hasTickets)
            return ApiResponse<bool>.FailureResponse("Cannot delete group — it has tickets assigned to it.");

        foreach (Category cat in group.Categories)
            await DeleteCategoryAsync(cat.Id);

        _uow.Groups.Delete(group);
        await _uow.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Group deleted successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteSubCategoryAsync(int id)
    {
        SubCategory? subCategory = await _uow.SubCategories.FindOneAsync(s => s.Id == id && !s.IsDeleted);
        if (subCategory is null)
            return ApiResponse<bool>.FailureResponse("Subcategory not found.");

        bool hasTickets = await _uow.Tickets.ExistsAsync(t => t.SubCategoryId == id && !t.IsDeleted);
        if (hasTickets)
            return ApiResponse<bool>.FailureResponse("Cannot delete subcategory — it has tickets assigned to it.");

        _uow.SubCategories.Delete(subCategory);
        await _uow.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Subcategory deleted successfully.");
    }

    public async Task<ApiResponse<List<GroupDto>>> GetAllGroupsAsync()
    {
        IEnumerable<Group> groups = await _uow.Groups.GetAllAsync(g => !g.IsDeleted);
        List<GroupDto> dtos = _mapper.Map<List<GroupDto>>(groups);
        return ApiResponse<List<GroupDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<List<CategoryDto>>> GetCategoriesByGroupAsync(int groupId)
    {
        bool exists = await _uow.Groups.ExistsAsync(g => g.Id == groupId && !g.IsDeleted);
        if (!exists)
            return ApiResponse<List<CategoryDto>>.FailureResponse("Select valid Group.");

        IEnumerable<Category> categories = await _uow.Categories.FindAsync(c => c.GroupId == groupId && !c.IsDeleted);
        List<CategoryDto> dtos = _mapper.Map<List<CategoryDto>>(categories);
        return ApiResponse<List<CategoryDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<PagedResponse<CategoryDto>>> GetCategoriesByGroupPagedAsync(int groupId, int page, int pageSize, string? search)
    {
        bool exists = await _uow.Groups.ExistsAsync(g => g.Id == groupId && !g.IsDeleted);
        if (!exists)
            return ApiResponse<PagedResponse<CategoryDto>>.FailureResponse("Select valid Group.");

        IQueryable<Category> query = _uow.Categories.Query().Where(c => c.GroupId == groupId && !c.IsDeleted
            && (search == null || EF.Functions.ILike(c.Name, $"%{search}%")));
        (IEnumerable<Category> Items, int TotalCount) = await _uow.Categories.GetPaginatedAsync(query: query, orderBy: o => o.OrderBy(x => x.Name), pageNumber: page, pageSize: pageSize);

        List<CategoryDto> dtos = _mapper.Map<List<CategoryDto>>(Items);

        PagedResponse<CategoryDto> response = new PagedResponse<CategoryDto>
        {
            Items = dtos,
            TotalCount = TotalCount,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponse<PagedResponse<CategoryDto>>.SuccessResponse(response);
    }

    public async Task<ApiResponse<PagedResponse<GroupDto>>> GetGroupsPagedAsync(int page, int pageSize, string? search)
    {
        IQueryable<Group> query = _uow.Groups.Query().Where(g => !g.IsDeleted && 
            (search == null || EF.Functions.ILike(g.Name, $"%{search}%")));
        (IEnumerable<Group> Items, int TotalCount) = await _uow.Groups.GetPaginatedAsync(query: query, orderBy: o => o.OrderBy(x => x.Name), pageNumber: page, pageSize: pageSize);

        List<GroupDto> dtos = _mapper.Map<List<GroupDto>>(Items);

        PagedResponse<GroupDto> response = new PagedResponse<GroupDto>
        {
            Items = dtos,
            TotalCount = TotalCount,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponse<PagedResponse<GroupDto>>.SuccessResponse(response);
    }

    public async Task<ApiResponse<List<SubCategoryDto>>> GetSubCategoriesByCategoryAsync(int categoryId)
    {
        bool exists = await _uow.Categories.ExistsAsync(c => c.Id == categoryId && !c.IsDeleted);
        if (!exists)
            return ApiResponse<List<SubCategoryDto>>.FailureResponse("Select valid Category.");

        IEnumerable<SubCategory> subs = await _uow.SubCategories.FindAsync(s => s.CategoryId == categoryId && !s.IsDeleted);
        List<SubCategoryDto> dtos = _mapper.Map<List<SubCategoryDto>>(subs);
        return ApiResponse<List<SubCategoryDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<PagedResponse<SubCategoryDto>>> GetSubCategoriesByCategoryPagedAsync(int categoryId, int page, int pageSize, string? search)
    {
        bool exists = await _uow.Categories.ExistsAsync(c => c.Id == categoryId && !c.IsDeleted);
        if (!exists)
            return ApiResponse<PagedResponse<SubCategoryDto>>.FailureResponse("Select valid Category.");

        IQueryable<SubCategory> query = _uow.SubCategories.Query().Where(s => s.CategoryId == categoryId && !s.IsDeleted
            && (search == null || EF.Functions.ILike(s.Name, $"%{search}%")));
        (IEnumerable<SubCategory> Items, int TotalCount) = await _uow.SubCategories.GetPaginatedAsync(query: query, orderBy: o => o.OrderBy(x => x.Name), pageNumber: page, pageSize: pageSize);

        List<SubCategoryDto> dtos = _mapper.Map<List<SubCategoryDto>>(Items);

        PagedResponse<SubCategoryDto> response = new PagedResponse<SubCategoryDto>
        {
            Items = dtos,
            TotalCount = TotalCount,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponse<PagedResponse<SubCategoryDto>>.SuccessResponse(response);
    }

    public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        Category? category = await _uow.Categories.FindOneAsync(c => c.Id == id && !c.IsDeleted);
        if (category is null)
            return ApiResponse<CategoryDto>.FailureResponse("Category not found.");

        bool exists = await _uow.Categories.ExistsAsync(c => c.Name.ToLower() == dto.Name.Trim().ToLower() && c.GroupId == category.GroupId && !c.IsDeleted && c.Id != id);
        if (exists)
            return ApiResponse<CategoryDto>.FailureResponse("A category with this name already exists in this group.");

        category.Name = dto.Name.Trim();
        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync();

        return ApiResponse<CategoryDto>.SuccessResponse(
            _mapper.Map<CategoryDto>(category), "Category updated successfully.");
    }

    public async Task<ApiResponse<GroupDto>> UpdateGroupAsync(int id, UpdateGroupDto dto)
    {
        Group? group = await _uow.Groups.FindOneAsync(g => g.Id == id && !g.IsDeleted);
        if (group is null)
            return ApiResponse<GroupDto>.FailureResponse("Group not found.");

        bool exists = await _uow.Groups.ExistsAsync(g => g.Name.ToLower() == dto.Name.Trim().ToLower() && !g.IsDeleted && g.Id != id);
        if (exists)
            return ApiResponse<GroupDto>.FailureResponse("A group with this name already exists.");

        group.Name = dto.Name.Trim();
        _uow.Groups.Update(group);
        await _uow.SaveChangesAsync();

        return ApiResponse<GroupDto>.SuccessResponse(
            _mapper.Map<GroupDto>(group), "Group updated successfully.");
    }

    public async Task<ApiResponse<SubCategoryDto>> UpdateSubCategoryAsync(int id, UpdateSubCategoryDto dto)
    {
        SubCategory? subCategory = await _uow.SubCategories.FindOneAsync(s => s.Id == id && !s.IsDeleted);
        if (subCategory is null)
            return ApiResponse<SubCategoryDto>.FailureResponse("Subcategory not found.");

        bool exists = await _uow.SubCategories.ExistsAsync(s => s.Name.ToLower() == dto.Name.Trim().ToLower() && s.CategoryId == subCategory.CategoryId && !s.IsDeleted && s.Id != id);
        if (exists)
            return ApiResponse<SubCategoryDto>.FailureResponse("A subcategory with this name already exists in this category.");

        subCategory.Name = dto.Name.Trim();
        _uow.SubCategories.Update(subCategory);
        await _uow.SaveChangesAsync();

        return ApiResponse<SubCategoryDto>.SuccessResponse(
            _mapper.Map<SubCategoryDto>(subCategory), "Subcategory updated successfully.");
    }
}
