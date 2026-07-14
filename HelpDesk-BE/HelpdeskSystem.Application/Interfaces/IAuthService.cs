using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Auth;

namespace HelpdeskSystem.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequestDto);
}
