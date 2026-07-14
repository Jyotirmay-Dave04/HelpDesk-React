using AutoMapper;
using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Dashboard;
using HelpdeskSystem.Common.DTOs.Tickets;
using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public DashboardService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _uow = unitOfWork;
        _currentUser = currentUserService;
        _mapper = mapper;
    }
    public async Task<ApiResponse<AdminKpiDto>> GetAdminKpisAsync()
    {
        IQueryable<Ticket> tickets = _uow.Tickets.Query().Where(t => !t.IsDeleted);

        double? avgSeconds = await _uow.AuditLogs.Query()
            .Where(a => a.Action == AuditAction.Assigned && !a.IsDeleted)
            .GroupBy(a => a.TicketId)
            .Select(g => new
            {
                TicketId = g.Key,
                FirstAssignedAt = g.Min(a => a.CreatedAt)
            })
            .Join(
                tickets,
                log => log.TicketId,
                t => t.Id,
                (log, t) => (double?)(log.FirstAssignedAt - t.CreatedAt).TotalSeconds
            )
            .AverageAsync();

        string frt = avgSeconds != null
        ? FormatDuration((long)avgSeconds.Value)
        : "N/A";

        // SLA Compliance
        int total = await tickets.CountAsync();
        int breached = await tickets.CountAsync(t => t.SlaBreached);
        float compliance = total > 0
            ? (float)(total - breached) / total * 100
            : 100f;

        return ApiResponse<AdminKpiDto>.SuccessResponse(new AdminKpiDto
        {
            AvgFirstResponseTime = frt,
            SlaComplianceRate = MathF.Round(compliance, 1)
        });
    }

    public async Task<ApiResponse<AdminDashboardStatDto>> GetAdminStatsAsync()
    {
        IQueryable<Ticket> query = _uow.Tickets.Query().Where(t => !t.IsDeleted);

        AdminDashboardStatDto stats = new AdminDashboardStatDto
        {
            Total = await query.CountAsync(),
            Open = await query.CountAsync(t => t.Status == TicketStatus.Open),
            Assigned = await query.CountAsync(t => t.Status == TicketStatus.Assigned),
            InProgress = await query.CountAsync(t => t.Status == TicketStatus.InProgress),
            OnHold = await query.CountAsync(t => t.Status == TicketStatus.OnHold),
            Resolved = await query.CountAsync(t => t.Status == TicketStatus.Resolved),
            Closed = await query.CountAsync(t => t.Status == TicketStatus.Closed),
            Rejected = await query.CountAsync(t => t.Status == TicketStatus.Rejected),
            Cancelled = await query.CountAsync(t => t.Status == TicketStatus.Cancelled),
            CannotResolve = await query.CountAsync(t => t.Status == TicketStatus.CannotResolve),
            SlaBreachedTotal = await query.CountAsync(t => t.SlaBreached || t.BreachedBeforeReopen),
            SlaBreachedActive = await query.CountAsync(t =>
                t.SlaBreached &&
                t.Status != TicketStatus.Resolved && t.Status != TicketStatus.Closed && t.Status != TicketStatus.Rejected
                && t.Status != TicketStatus.Cancelled && t.Status != TicketStatus.CannotResolve),
            ReopenCount = await query.CountAsync(t => t.ReopenCount > 0)
        };

        return ApiResponse<AdminDashboardStatDto>.SuccessResponse(stats);
    }

    public async Task<ApiResponse<AgentMetricsDto>> GetAgentMetricsAsync(int agentId)
    {
        var now = DateTime.UtcNow;

        var todayStart = DateTime.SpecifyKind(now.Date, DateTimeKind.Utc);
        var todayEnd   = todayStart.AddDays(1);
        var monthStart = DateTime.SpecifyKind(
            new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc);

        int solvedToday = await _uow.Tickets.Query()
            .CountAsync(t =>
                t.AssignedTo == agentId &&
                !t.IsDeleted &&
                t.ResolvedAt.HasValue &&
                t.ResolvedAt.Value >= todayStart &&
                t.ResolvedAt.Value < todayEnd);

        int slaBreachedMonth = await _uow.Tickets.Query()
            .CountAsync(t =>
                t.AssignedTo == agentId &&
                !t.IsDeleted &&
                t.SlaBreached &&
                t.CreatedAt >= monthStart);

        return ApiResponse<AgentMetricsDto>.SuccessResponse(new AgentMetricsDto
        {
            TicketsSolvedToday      = solvedToday,
            SlaBreachedCurrentMonth = slaBreachedMonth
        });
    }

    public async Task<ApiResponse<AgentDashboardStatDto>> GetAgentStatsAsync()
    {
        int userId = _currentUser.GetUserId();
        IQueryable<Ticket> query = _uow.Tickets.Query().Where(t => !t.IsDeleted && t.AssignedTo == userId);

        AgentDashboardStatDto stats = new AgentDashboardStatDto
        {
            AssignedToMe = await query.CountAsync(t => t.Status == TicketStatus.Assigned),
            InProgress = await query.CountAsync(t => t.Status == TicketStatus.InProgress),
            OnHold = await query.CountAsync(t => t.Status == TicketStatus.OnHold),
            Resolved = await query.CountAsync(t => t.Status == TicketStatus.Resolved),
            Closed = await query.CountAsync(t => t.Status == TicketStatus.Closed),
            CannotResolve = await query.CountAsync(t => t.Status == TicketStatus.CannotResolve),
            SlaBreachedActive = await query.CountAsync(t =>
                t.SlaBreached &&
                t.Status != TicketStatus.Resolved && t.Status != TicketStatus.Closed && t.Status != TicketStatus.Rejected
                && t.Status != TicketStatus.Cancelled && t.Status != TicketStatus.CannotResolve),
            SlaBreachedTotal = await query.CountAsync(t => t.SlaBreached || t.BreachedBeforeReopen),
            ReopenCount = await query.CountAsync(t => t.ReopenCount > 0)
        };

        return ApiResponse<AgentDashboardStatDto>.SuccessResponse(stats);
    }

    public async Task<ApiResponse<List<CategoryDistributionDto>>> GetCategoryDistributionAsync(string view)
    {
        DateTime now = DateTime.UtcNow;
        DateTime startDate;
        
        switch (view.ToLower())
        {
            case "weekly":  
                int dayOfWeek = (int)now.DayOfWeek;
                startDate = DateTime.SpecifyKind(now.Date.AddDays(dayOfWeek == 0 ? -6 : -dayOfWeek + 1), DateTimeKind.Utc);
                break;
            
            case "monthly":
                startDate = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc);
                break;
            
            case "yearly":
                startDate = DateTime.SpecifyKind(new DateTime(now.Year, 1, 1), DateTimeKind.Utc);
                break;
            
            default:
                return ApiResponse<List<CategoryDistributionDto>>.FailureResponse("Invalid view.");
        }

        List<CategoryDistributionDto> result = await _uow.Tickets.Query()
            .Where(t => !t.IsDeleted && t.CreatedAt >= startDate && t.CreatedAt <= now)
            .GroupBy(t => new { t.CategoryId, t.Category.Name })
            .Select(g => new CategoryDistributionDto
            {
                CategoryName = g.Key.Name,
                TicketCount = g.Count()
            })
            .OrderByDescending(x => x.TicketCount)
            .ToListAsync();

        return ApiResponse<List<CategoryDistributionDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<List<TicketListItemDto>>> GetRecentAsync()
    {
        int userId = _currentUser.GetUserId();
        string role = _currentUser.GetUserRole();

        Func<IQueryable<Ticket>, IQueryable<Ticket>> ticketIncludes = query => query
            .Include(t => t.Group)
            .Include(t => t.Category)
            .Include(t => t.SubCategory)
            .Include(t => t.RequestedByUser)
            .Include(t => t.AssignedToUser);

        IQueryable<Ticket> query = _uow.Tickets.Query().Where(t => !t.IsDeleted &&
            t.Status != TicketStatus.Rejected && t.Status != TicketStatus.Resolved && t.Status != TicketStatus.Closed
            && t.Status != TicketStatus.Cancelled && t.Status != TicketStatus.CannotResolve);

        if (role == nameof(UserRole.Requester))
            return ApiResponse<List<TicketListItemDto>>.FailureResponse("Access denied.");

        if (role == nameof(UserRole.Agent))
            query = query.Where(t => t.AssignedTo == userId);

        List<Ticket> tickets = await ticketIncludes(query)
            // Same order as ticket list
            .OrderBy(t =>
                (t.Status == TicketStatus.Open || t.Status == TicketStatus.ReOpen)
                    ? t.SlaBreached ? 0 : 1
                    : t.Status == TicketStatus.Assigned
                        ? t.SlaBreached ? 3 : 4
                        : t.Status == TicketStatus.InProgress
                            ? t.SlaBreached ? 6 : 7
                            : t.Status == TicketStatus.OnHold
                                ? t.SlaBreached ? 9 : 10
                                : 50)
            .ThenBy(t => t.SlaDeadline)
            .Take(5)
            .ToListAsync();

        List<TicketListItemDto> dtos = _mapper.Map<List<TicketListItemDto>>(tickets);

        return ApiResponse<List<TicketListItemDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<TicketTrendsDto>> GetTicketTrendsAsync(string view)
    {
        DateTime now = DateTime.UtcNow;
        TicketTrendsDto dto = new TicketTrendsDto();

        if (view == "weekly")
        {
            DateTime start = now.Date.AddDays(-6);
            IEnumerable<Ticket> tickets = await _uow.Tickets.FindAsync(t => !t.IsDeleted && t.CreatedAt >= start);

            for (int i = 6; i >= 0; i--)
            {
                DateTime day = now.Date.AddDays(-i);
                dto.Labels.Add(day.ToString("ddd"));
                dto.CreatedData.Add(tickets.Count(t => t.CreatedAt.Date == day));
                dto.ResolvedData.Add(tickets.Count(t => t.ResolvedAt.HasValue && t.ResolvedAt.Value.Date == day));
            }
        }
        else if (view == "monthly")
        {
            DateTime start = now.Date.AddDays(-29);
            IEnumerable<Ticket> tickets = await _uow.Tickets.FindAsync(t => !t.IsDeleted && t.CreatedAt >= start);

            for (int i = 29; i >= 0; i--)
            {
                DateTime day = now.Date.AddDays(-i);
                dto.Labels.Add(day.ToString("MMM d"));
                dto.CreatedData.Add(tickets.Count(t => t.CreatedAt.Date == day));
                dto.ResolvedData.Add(tickets.Count(t => t.ResolvedAt.HasValue && t.ResolvedAt.Value.Date == day));
            }
        }
        else
        {
            DateTime start = now.Date.AddMonths(-11).AddDays(1 - now.Day);
            IEnumerable<Ticket> tickets = await _uow.Tickets.FindAsync(t => !t.IsDeleted && t.CreatedAt >= start);

            for (int i = 11; i >= 0; i--)
            {
                DateTime month = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
                dto.Labels.Add(month.ToString("MMM"));
                dto.CreatedData.Add(tickets.Count(t =>
                    t.CreatedAt.Year == month.Year && t.CreatedAt.Month == month.Month));
                dto.ResolvedData.Add(tickets.Count(t =>
                    t.ResolvedAt.HasValue &&
                    t.ResolvedAt.Value.Year == month.Year &&
                    t.ResolvedAt.Value.Month == month.Month));
            }
        }

        return ApiResponse<TicketTrendsDto>.SuccessResponse(dto);
    }


    // Helper

    private static string FormatDuration(long totalSeconds)
    {
        if (totalSeconds < 60) return $"{totalSeconds}s";
        if (totalSeconds < 3600) return $"{totalSeconds / 60}m";
        var h = totalSeconds / 3600;
        var m = (totalSeconds % 3600) / 60;
        return m > 0 ? $"{h}h {m}m" : $"{h}h";
    }
}
