using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.User;
using HelpdeskSystem.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterRequest filter)
        {
            ApiResponse<PagedResponse<UserResponseDto>> result = await _userService.GetAllUsersAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("updateUserRole/{id:int}")]
        public async Task<IActionResult> UpdateUserRole(int id, ChangeUserRolePayload payload)
        {      
            ApiResponse<bool> result = await _userService.ChangeUserRole(id, payload);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            ApiResponse<bool> result = await _userService.DeleteUserById(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getAgents")]
        public async Task<IActionResult> GetAgents([FromQuery] AssignAgentRequestDto dto)
        {
            ApiResponse<PagedResponse<UserResponseDto>> result = await _userService.GetAgentsByWorkloadAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
