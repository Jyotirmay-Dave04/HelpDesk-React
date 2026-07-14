using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Dashboard;
using HelpdeskSystem.Common.DTOs.Tickets;

namespace HelpdeskSystem.Application.Interfaces;

public interface IDashboardService
{
    Task<ApiResponse<AdminKpiDto>> GetAdminKpisAsync();
    Task<ApiResponse<List<CategoryDistributionDto>>> GetCategoryDistributionAsync(string view);
    Task<ApiResponse<TicketTrendsDto>> GetTicketTrendsAsync(string view);
    Task<ApiResponse<AgentMetricsDto>> GetAgentMetricsAsync(int agentId);
    Task<ApiResponse<AdminDashboardStatDto>> GetAdminStatsAsync();
    Task<ApiResponse<AgentDashboardStatDto>> GetAgentStatsAsync();
    Task<ApiResponse<List<TicketListItemDto>>> GetRecentAsync();
}
