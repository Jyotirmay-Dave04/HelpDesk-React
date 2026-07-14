using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.User;

namespace HelpdeskSystem.Application.Interfaces;

public interface IUserService
{
    Task<ApiResponse<PagedResponse<UserResponseDto>>> GetAllUsersAsync(UserFilterRequest dto);
    Task<ApiResponse<bool>> ChangeUserRole(int id, ChangeUserRolePayload newRole);
    Task<ApiResponse<bool>> DeleteUserById(int id);
    Task<ApiResponse<PagedResponse<UserResponseDto>>> GetAgentsByWorkloadAsync(AssignAgentRequestDto dto);
}
