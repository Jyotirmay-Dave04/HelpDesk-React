using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs.CannedResponses;
using HelpdeskSystem.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Agent)}")]
    public class CannedResponseController : ControllerBase
    {
        private readonly ICannedResponseService _cannedResponseService;

        public CannedResponseController(ICannedResponseService cannedResponseService)
        {
            _cannedResponseService = cannedResponseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _cannedResponseService.GetAllAsync();
            return Ok(result);
        }


        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCannedResponseDto dto)
        {
            var result = await _cannedResponseService.CreateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCannedResponseDto dto)
        {
            var result = await _cannedResponseService.UpdateAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _cannedResponseService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
