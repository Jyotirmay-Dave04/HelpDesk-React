using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Dashboard;
using HelpdeskSystem.Common.DTOs.Tickets;
using HelpdeskSystem.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUser;

        public DashboardController(IDashboardService dashboardService, ICurrentUserService currentUserService)
        {
            _dashboardService = dashboardService;
            _currentUser = currentUserService;
        }

        [HttpGet("adminKpis")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetAdminKpis()
        {
            ApiResponse<AdminKpiDto> result = await _dashboardService.GetAdminKpisAsync();
            return Ok(result);
        }

        [HttpGet("categoryDistribution")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetCategoryDistribution([FromQuery] string view = "monthly")
        {
            ApiResponse<List<CategoryDistributionDto>> result = await _dashboardService.GetCategoryDistributionAsync(view);
            return Ok(result);
        }

        [HttpGet("ticketTrends")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetTicketTrends([FromQuery] string view = "weekly")
        {
            ApiResponse<TicketTrendsDto> result = await _dashboardService.GetTicketTrendsAsync(view);
            return Ok(result);
        }

        [HttpGet("agentMetrics")]
        [Authorize(Roles = nameof(UserRole.Agent))]
        public async Task<IActionResult> GetAgentMetrics()
        {
            int agentId = _currentUser.GetUserId();
            ApiResponse<AgentMetricsDto> result = await _dashboardService.GetAgentMetricsAsync(agentId);
            return Ok(result);
        }

        [HttpGet("stats")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Agent)}")]
        public async Task<IActionResult> GetStats()
        {
            string role = _currentUser.GetUserRole();

            if(role == nameof(UserRole.Admin)){
                ApiResponse<AdminDashboardStatDto> result = await _dashboardService.GetAdminStatsAsync();
                return result.Success ? Ok(result) : BadRequest(result);
            }

            if(role == nameof(UserRole.Agent)){
                ApiResponse<AgentDashboardStatDto> result = await _dashboardService.GetAgentStatsAsync();
                return result.Success ? Ok(result) : BadRequest(result);
            }

            return Unauthorized();
        }

        [HttpGet("recent")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Agent)}")]
        public async Task<IActionResult> GetRecent()
        {
            ApiResponse<List<TicketListItemDto>> result = await _dashboardService.GetRecentAsync();
            return Ok(result);
        }
    }
}
