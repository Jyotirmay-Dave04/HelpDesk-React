using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            ApiResponse<AuthResponseDto> result = await _authService.RegisterAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            ApiResponse<AuthResponseDto> result = await _authService.LoginAsync(dto);
            return result.Success ? Ok(result) : Unauthorized(result);
        }
    }
}
