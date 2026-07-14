using AutoMapper;
using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.User;
using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUserService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<bool>> ChangeUserRole(int id, ChangeUserRolePayload payload)
    {
        if (_currentUser.GetUserRole() != UserRole.Admin.ToString())
            return ApiResponse<bool>.FailureResponse("Access Denied!");

        User? existingUser = await _unitOfWork.Users.FindOneAsync(u => u.Id == id && !u.IsDeleted);

        if (existingUser is null)
            return ApiResponse<bool>.FailureResponse("User does not exists!");

        if (!Enum.TryParse<UserRole>(payload.Role, true, out UserRole newRole))
            return ApiResponse<bool>.FailureResponse("Please provide valid role.");

        IEnumerable<Ticket> assignedTickets = await _unitOfWork.Tickets.FindAsync(
            filter: t => t.AssignedTo == _currentUser.GetUserId()
                        && !t.IsDeleted
                        && t.Status != TicketStatus.Rejected
                        && t.Status != TicketStatus.Resolved
                        && t.Status != TicketStatus.Closed);
        if (assignedTickets.Any())
            return ApiResponse<bool>.FailureResponse("Agent has some tickets pending to resolve, try later.");

        existingUser.Role = newRole;

        _unitOfWork.Users.Update(existingUser);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Role updated successfully!");
    }

    public async Task<ApiResponse<bool>> DeleteUserById(int id)
    {
        if (_currentUser.GetUserRole() != UserRole.Admin.ToString())
            return ApiResponse<bool>.FailureResponse("Access Denied!");

        User? existingUser = await _unitOfWork.Users.FindOneAsync(u => u.Id == id && !u.IsDeleted);

        if (existingUser is null)
            return ApiResponse<bool>.FailureResponse("User does not exists!");

        _unitOfWork.Users.Delete(existingUser);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "User deleted successfully!");
    }

    public async Task<ApiResponse<PagedResponse<UserResponseDto>>> GetAgentsByWorkloadAsync(AssignAgentRequestDto dto)
    {
        if (_currentUser.GetUserRole() != nameof(UserRole.Admin))
            return ApiResponse<PagedResponse<UserResponseDto>>.FailureResponse("Access denied!");

        IQueryable<User> query = _unitOfWork.Users.Query();

        query = query.Where(u => !u.IsDeleted && u.Role == UserRole.Agent).Include(u => u.AssignedTickets);

        if (dto.Search is not null)
            query = query.Where(u => EF.Functions.ILike(u.Name, $"%{dto.Search}%"));

        int total = await query.CountAsync();

        var activeStatuses = new[] { TicketStatus.Assigned, TicketStatus.InProgress, TicketStatus.OnHold, TicketStatus.ReOpen };

        List<UserResponseDto> result = await query
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role.ToString(),
                ActiveTicketCount = u.AssignedTickets.Count(t => activeStatuses.Contains(t.Status)),
                CreatedAt = u.CreatedAt
            })
            .OrderBy(x => x.ActiveTicketCount)
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync();

        PagedResponse<UserResponseDto> dtos = new PagedResponse<UserResponseDto>
        {
            Items = result,
            TotalCount = total,
            Page = dto.Page,
            PageSize = dto.PageSize
        };

        return ApiResponse<PagedResponse<UserResponseDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<PagedResponse<UserResponseDto>>> GetAllUsersAsync(UserFilterRequest filter)
    {
        if (_currentUser.GetUserRole() != UserRole.Admin.ToString())
            return ApiResponse<PagedResponse<UserResponseDto>>.FailureResponse("Access Denied!");

        IQueryable<User> query = _unitOfWork.Users.Query();

        query = query.Where(u => !u.IsDeleted);

        if (filter.Role is not null)
        {
            if (Enum.TryParse<UserRole>(filter.Role, true, out UserRole role))
                query = query.Where(u => u.Role == role);
            else
                return ApiResponse<PagedResponse<UserResponseDto>>.FailureResponse("Role must be valid.");
        }

        if (filter.Search is not null)
            query = query.Where(u => EF.Functions.ILike(u.Name, $"%{filter.Search}%") || EF.Functions.ILike(u.Email, $"%{filter.Search}%"));

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            bool asc = filter.SortDirection?.ToLower() != "desc";

            query = filter.SortBy.ToLower() switch
            {
                "name" => asc ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name),
                "email" => asc ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
                "role" => asc ? query.OrderBy(u => u.Role) : query.OrderByDescending(u => u.Role),
                "createdAt" => asc ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt),
                _ => asc ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(u => u.CreatedAt);
        }

        (IEnumerable<User> paged, int total) = await _unitOfWork.Users.GetPaginatedAsync(
            query: query,
            pageNumber: filter.Page,
            pageSize: filter.PageSize
        );

        List<UserResponseDto> result = _mapper.Map<List<UserResponseDto>>(paged);

        PagedResponse<UserResponseDto> dtos = new PagedResponse<UserResponseDto>
        {
            Items = result,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        return ApiResponse<PagedResponse<UserResponseDto>>.SuccessResponse(dtos);
    }
}
