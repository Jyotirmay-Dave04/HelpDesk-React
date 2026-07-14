using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnumController : ControllerBase
    {
        [HttpGet("getPriorityEnums")]
        public IActionResult GetPriorityEnums()
        {
            string[] enums = Enum.GetNames(typeof(Priority));
            return Ok(ApiResponse<string[]>.SuccessResponse(enums));
        }

        [HttpGet("getTicketStatusEnums")]
        public IActionResult GetTicketStatusEnums()
        {
            string[] enums = Enum.GetNames(typeof(TicketStatus));
            return Ok(ApiResponse<string[]>.SuccessResponse(enums));
        }

        [HttpGet("getUserRoleEnums")]
        public IActionResult GetUserRoleEnums()
        {
            string[] enums = Enum.GetNames(typeof(UserRole));
            return Ok(ApiResponse<string[]>.SuccessResponse(enums));
        }

        [HttpGet("getAuditActionEnums")]
        public IActionResult GetAuditActionEnums()
        {
            string[] enums = Enum.GetNames(typeof(AuditAction));
            return Ok(ApiResponse<string[]>.SuccessResponse(enums));
        }
    }
}
