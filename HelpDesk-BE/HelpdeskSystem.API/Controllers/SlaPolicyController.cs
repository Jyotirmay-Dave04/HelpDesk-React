using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.SlaPolicies;
using HelpdeskSystem.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SlaPolicyController : ControllerBase
    {
        private readonly ISlaPolicyService _slaPolicyService;

        public SlaPolicyController(ISlaPolicyService slaPolicyService)
        {
            _slaPolicyService = slaPolicyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ApiResponse<List<SlaPolicyResponseDto>> result = await _slaPolicyService.GetAllSlaPoliciesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{priority}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Update(Priority priority, [FromBody] UpdateSlaPolicyDto dto)
        {
            ApiResponse<SlaPolicyResponseDto> result = await _slaPolicyService.UpdateSlaPolicyDto(priority, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
