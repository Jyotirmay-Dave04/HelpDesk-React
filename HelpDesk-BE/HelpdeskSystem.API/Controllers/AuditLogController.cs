using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.AuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet("ticket/{id:int}")]
        public async Task<IActionResult> GetAuditsByTicketId(int id)
        {
            ApiResponse<List<AuditLogResponseDto>> result = await _auditLogService.GetByTicketIdAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
