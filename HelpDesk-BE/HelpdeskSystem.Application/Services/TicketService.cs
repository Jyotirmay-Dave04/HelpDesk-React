using System.Text;
using AutoMapper;
using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Tickets;
using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Application.Services;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public TicketService(IUnitOfWork uow, ICurrentUserService currentUser, IMapper mapper, INotificationService notificationService)
    {
        _uow = uow;
        _currentUser = currentUser;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<ApiResponse<TicketResponseDto>> CreateAsync(CreateTicketDto dto)
    {
        // Parse priority enum
        if (!Enum.TryParse<Priority>(dto.Priority, true, out Priority priority))
            return ApiResponse<TicketResponseDto>.FailureResponse("Invalid priority value.");

        SlaPolicy? slaPolicy = await _uow.SlaPolicies.FindOneAsync(sp => sp.Priority == priority);

        bool groupExists = await _uow.Groups.ExistsAsync(g => g.Id == dto.GroupId && !g.IsDeleted);
        if(!groupExists)
            return ApiResponse<TicketResponseDto>.FailureResponse("Group does not found.");

        bool categoryExists = await _uow.Categories.ExistsAsync(c => c.Id == dto.CategoryId && !c.IsDeleted);
        if(!categoryExists)
            return ApiResponse<TicketResponseDto>.FailureResponse("Category does not found.");

        bool subCategoryExists = await _uow.SubCategories.ExistsAsync(sc => sc.Id == dto.SubCategoryId && !sc.IsDeleted);
        if(!subCategoryExists)
            return ApiResponse<TicketResponseDto>.FailureResponse("Sub Category does not found.");

        Ticket ticket = new Ticket
        {
            RequestedBy = _currentUser.GetUserId(),
            GroupId = dto.GroupId,
            CategoryId = dto.CategoryId,
            SubCategoryId = dto.SubCategoryId,
            Priority = priority,
            ServiceDetails = dto.ServiceDetails.Trim(),
            Status = TicketStatus.Open,
            SlaDeadline = DateTime.UtcNow.AddHours(slaPolicy?.HoursToResolve ?? 24),
            SlaBreached = false
        };

        await _uow.Tickets.AddAsync(ticket);
        await _uow.SaveChangesAsync();

        // Write AuditLog
        await WriteAuditLogAsync(ticket.Id, AuditAction.Created);

        TicketResponseDto response = _mapper.Map<TicketResponseDto>(ticket);

        await _notificationService.BroadcastNewTicketAsync(response);

        return ApiResponse<TicketResponseDto>.SuccessResponse(response, "Ticket created successfully.");
    }

    public async Task<ApiResponse<TicketResponseDto>> GetByIdAsync(int id)
    {
        Ticket? ticket = await _uow.Tickets.FindOneAsync(t => t.Id == id && !t.IsDeleted,
            includes: q => q.Include(t => t.Group).Include(t => t.Category).Include(t => t.SubCategory)
                            .Include(t => t.RequestedByUser).Include(t => t.AssignedToUser));
        if (ticket == null)
            return ApiResponse<TicketResponseDto>.FailureResponse("Ticket not found.");

        if (!CanAccessTicket(ticket))
            return ApiResponse<TicketResponseDto>.FailureResponse("Access denied.");

        TicketResponseDto response = _mapper.Map<TicketResponseDto>(ticket);

        return ApiResponse<TicketResponseDto>.SuccessResponse(response);
    }

    public async Task<ApiResponse<PagedResponse<TicketListItemDto>>> GetAllAsync(TicketFilterDto filter)
    {
        string role = _currentUser.GetUserRole();
        int userId = _currentUser.GetUserId();

        Func<IQueryable<Ticket>, IQueryable<Ticket>> ticketIncludes = query => query
            .Include(t => t.Group)
            .Include(t => t.Category)
            .Include(t => t.SubCategory)
            .Include(t => t.RequestedByUser)
            .Include(t => t.AssignedToUser);

        IQueryable<Ticket> query = _uow.Tickets.Query();

        // Base query — role filter
        switch (role)
        {
            case nameof(UserRole.Agent):
                query = filter.IsMyList ? query.Where(t => t.RequestedBy == userId && !t.IsDeleted) : query.Where(t => t.AssignedTo == userId && !t.IsDeleted);
                break;

            case nameof(UserRole.Requester):
                query = query.Where(t => t.RequestedBy == userId && !t.IsDeleted);
                break;

            case nameof(UserRole.Admin):
                query = filter.IsMyList ? query.Where(t => t.RequestedBy == userId && !t.IsDeleted) : query.Where(t => !t.IsDeleted);
                break;
        };

        // Apply optional filters
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(t =>
                EF.Functions.ILike(t.RequestedByUser.Name, $"%{filter.Search}%") ||
                EF.Functions.ILike(t.ServiceDetails, $"%{filter.Search}%"));

        if (!string.IsNullOrWhiteSpace(filter.Status) &&
            Enum.TryParse<TicketStatus>(filter.Status, true, out TicketStatus statusFilter))
            query = query.Where(t => t.Status == statusFilter);

        if (!string.IsNullOrWhiteSpace(filter.Priority) &&
            Enum.TryParse<Priority>(filter.Priority, true, out Priority priorityFilter))
            query = query.Where(t => t.Priority == priorityFilter);

        if (filter.GroupId.HasValue)
            query = query.Where(t => t.GroupId == filter.GroupId.Value);

        if (filter.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);

        if (filter.AssignedTo.HasValue)
            query = query.Where(t => t.AssignedTo == filter.AssignedTo.Value);

        if (filter.Breached.HasValue)
            query = query.Where(t => t.SlaBreached == filter.Breached.Value);

        if (filter.DateFrom.HasValue)
            query = query.Where(t => t.CreatedAt >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(t => t.CreatedAt <= filter.DateTo.Value);

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            bool asc = filter.SortDirection?.ToLower() != "desc";

            query = filter.SortBy.ToLower() switch
            {
                "createdat" => asc ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt),
                "sladeadline" => asc ? query.OrderBy(t => t.SlaDeadline) : query.OrderByDescending(t => t.SlaDeadline),
                "priority" => asc ? query.OrderBy(t => t.Priority) : query.OrderByDescending(t => t.Priority),
                "status" => asc ? query.OrderBy(t => t.Status) : query.OrderByDescending(t => t.Status),
                "resolvedat" => asc ? query.OrderBy(t => t.ResolvedAt) : query.OrderByDescending(t => t.ResolvedAt),
                _ => ApplyUrgencyOrder(query)
            };
        }
        else
        {
            query = ApplyUrgencyOrder(query);
        }

        // Pagination
        (IEnumerable<Ticket> paged, int total) = await _uow.Tickets.GetPaginatedAsync(
            query: query,
            includes: ticketIncludes,
            pageNumber: filter.Page,
            pageSize: filter.PageSize
        );

        List<TicketListItemDto> dtos = _mapper.Map<List<TicketListItemDto>>(paged);

        PagedResponse<TicketListItemDto> result = new PagedResponse<TicketListItemDto>
        {
            Items = dtos,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        return ApiResponse<PagedResponse<TicketListItemDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<TicketResponseDto>> UpdateAsync(int id, UpdateTicketDto dto)
    {
        Ticket? ticket = await _uow.Tickets.FindOneAsync(t => t.Id == id && !t.IsDeleted);
        if (ticket == null)
            return ApiResponse<TicketResponseDto>.FailureResponse("Ticket not found.");

        if (_currentUser.GetUserRole() != nameof(UserRole.Admin) && ticket.RequestedBy != _currentUser.GetUserId())
            return ApiResponse<TicketResponseDto>.FailureResponse("Access denied.");

        if (ticket.Status is not TicketStatus.Open or TicketStatus.ReOpen)
            return ApiResponse<TicketResponseDto>.FailureResponse("Ticket can only be edited while status is Open.");

        if (!Enum.TryParse<Priority>(dto.Priority, true, out Priority newPriority))
            return ApiResponse<TicketResponseDto>.FailureResponse("Invalid priority value.");

        SlaPolicy? slaPolicy = await _uow.SlaPolicies.FindOneAsync(sp => sp.Priority == newPriority);

        string oldPriority = ticket.Priority.ToString();
        string oldDetails = ticket.ServiceDetails;

        ticket.ServiceDetails = dto.ServiceDetails.Trim();
        ticket.Priority = newPriority;
        // Recalculate SLA if priority changed
        if (oldPriority != newPriority.ToString())
            ticket.SlaDeadline = DateTime.UtcNow.AddHours(slaPolicy?.HoursToResolve ?? 24);

        _uow.Tickets.Update(ticket);
        await _uow.SaveChangesAsync();

        if (oldPriority != dto.Priority.ToString())
            await WriteAuditLogAsync(id, AuditAction.Updated, "priority", oldPriority, dto.Priority.ToString());

        if (oldDetails != ticket.ServiceDetails)
            await WriteAuditLogAsync(id, AuditAction.Updated, "serviceDetails", oldDetails, ticket.ServiceDetails);

        TicketResponseDto response = _mapper.Map<TicketResponseDto>(ticket);

        await _notificationService.BroadcastUpdatedTicketAsync(response);

        return ApiResponse<TicketResponseDto>.SuccessResponse(response, "Ticket updated.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        if (_currentUser.GetUserRole() != nameof(UserRole.Admin))
            return ApiResponse<bool>.FailureResponse("Only admins can delete tickets.");

        Ticket? ticket = await _uow.Tickets.FindOneAsync(t => t.Id == id && !t.IsDeleted);
        if (ticket == null)
            return ApiResponse<bool>.FailureResponse("Ticket not found.");

        if (ticket.Status != TicketStatus.Closed && ticket.Status != TicketStatus.Rejected 
            && ticket.Status != TicketStatus.Cancelled && ticket.Status != TicketStatus.CannotResolve)
            return ApiResponse<bool>.FailureResponse($"Ticket is not terminated yet, so can not be deleted.");

        _uow.Tickets.Delete(ticket);
        await _uow.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Ticket deleted.");
    }

    public async Task<ApiResponse<TicketResponseDto>> AssignAsync(int ticketId, AssignTicketDto dto)
    {
        if (_currentUser.GetUserRole() != nameof(UserRole.Admin))
            return ApiResponse<TicketResponseDto>.FailureResponse("Only admins can assign tickets.");

        Ticket? ticket = await _uow.Tickets.FindOneAsync(
            filter: t => t.Id == ticketId && !t.IsDeleted,
            includes: q => q.Include(t => t.Group).Include(t => t.Category).Include(t => t.SubCategory).Include(t => t.RequestedByUser).Include(t => t.AssignedToUser)
        );

        if (ticket is null)
            return ApiResponse<TicketResponseDto>.FailureResponse("Ticket not found.");

        User? agent = await _uow.Users.FindOneAsync(u => u.Id == dto.AssignedToId && !u.IsDeleted);

        if (agent is null)
            return ApiResponse<TicketResponseDto>.FailureResponse("Agent not found.");

        if (agent.Role != UserRole.Agent)
            return ApiResponse<TicketResponseDto>.FailureResponse("Selected user is not an agent.");

        if (ticket.Status is TicketStatus.Resolved or TicketStatus.Closed or TicketStatus.Rejected or TicketStatus.Cancelled or TicketStatus.CannotResolve)
            return ApiResponse<TicketResponseDto>.FailureResponse("Terminated tickets cannot be assigned.");

        // Re-assignment not allowed while InProgress
        if (ticket.AssignedTo.HasValue && ticket.Status == TicketStatus.InProgress)
            return ApiResponse<TicketResponseDto>.FailureResponse("Re-assignment is not allowed while ticket is InProgress.");

        string? oldAssignedAgent = ticket.AssignedToUser?.Name;

        ticket.AssignedTo = dto.AssignedToId;

        // If first assignment from Open → Assigned
        if (ticket.Status == TicketStatus.Open || ticket.Status == TicketStatus.ReOpen)
            ticket.Status = TicketStatus.Assigned;

        _uow.Tickets.Update(ticket);
        await _uow.SaveChangesAsync();

        await WriteAuditLogAsync(ticket.Id, AuditAction.Assigned, "assignedTo", oldAssignedAgent, agent.Name);

        TicketResponseDto response = _mapper.Map<TicketResponseDto>(ticket);

        await _notificationService.NotifyUserAsync(dto.AssignedToId, ticket.Id, NotificationType.TicketAssigned, $"You have been assigned ticket #{ticket.Id}");
        await _notificationService.NotifyUserAsync(
            response.RequestedById, ticket.Id, NotificationType.TicketAssigned, $"Your ticket #{ticket.Id} has been assigned to {response.AssignedToName}.");

        await _notificationService.BroadcastUpdatedTicketAsync(response);

        return ApiResponse<TicketResponseDto>.SuccessResponse(response, "Ticket assigned successfully.");
    }

    public async Task<ApiResponse<TicketResponseDto>> UpdateStatusAsync(int ticketId, UpdateStatusDto dto)
    {
        if (!Enum.TryParse<TicketStatus>(dto.Status, true, out TicketStatus newStatus))
            return ApiResponse<TicketResponseDto>.FailureResponse("Invalid status.");

        Ticket? ticket = await _uow.Tickets.FindOneAsync(
            filter: t => t.Id == ticketId && !t.IsDeleted,
            includes: q => q.Include(t => t.Group).Include(t => t.Category).Include(t => t.SubCategory).Include(t => t.RequestedByUser).Include(t => t.AssignedToUser)
        );

        if (ticket is null)
            return ApiResponse<TicketResponseDto>.FailureResponse("Ticket not found.");

        // Validate transition
        bool isValidTransition = ValidTransitions.TryGetValue(ticket.Status, out List<TicketStatus>? allowedStatuses) && allowedStatuses.Contains(newStatus);

        if (!isValidTransition)
            return ApiResponse<TicketResponseDto>.FailureResponse($"Invalid status transition from {ticket.Status} to {newStatus}.");

        string oldStatus = ticket.Status.ToString();
        DateTime now = DateTime.UtcNow;

        if (_currentUser.GetUserRole() == nameof(UserRole.Requester))
        {
            if (newStatus != TicketStatus.Closed && newStatus != TicketStatus.Cancelled && newStatus != TicketStatus.ReOpen)
                return ApiResponse<TicketResponseDto>.FailureResponse($"Requester can not update ticket status to {newStatus}.");

            if (ticket.RequestedBy != _currentUser.GetUserId())
                return ApiResponse<TicketResponseDto>.FailureResponse("You did not requested this ticket.");
        }

        if (_currentUser.GetUserRole() == nameof(UserRole.Agent))
        {
            if (_currentUser.GetUserId() != ticket.AssignedTo)
                return ApiResponse<TicketResponseDto>.FailureResponse("Only the assigned agent can update ticket status.");

            if (newStatus is TicketStatus.ReOpen)
                return ApiResponse<TicketResponseDto>.FailureResponse("Agent can not re-open ticket.");
        }

        ticket.Status = newStatus;
        
        string? currentReason = null;
        if (ticket.Status == TicketStatus.OnHold || ticket.Status == TicketStatus.CannotResolve)
            currentReason = dto.Reason;
        
        bool wasPaused = PauseStates.Contains(Enum.Parse<TicketStatus>(oldStatus));
        bool willBePaused = PauseStates.Contains(ticket.Status);

        if(!wasPaused && willBePaused)
            ticket.SlaPausedAt = now;
        
        if(wasPaused && !willBePaused && ticket.SlaPausedAt.HasValue)
        {
            TimeSpan pausedDuration = now - ticket.SlaPausedAt.Value;
            long seconds = (long)pausedDuration.TotalSeconds;

            if (Enum.Parse<TicketStatus>(oldStatus) == TicketStatus.OnHold)
                ticket.OnHoldPausedSeconds += seconds;
            else
                ticket.PostResolutionPausedSeconds += seconds;

            ticket.SlaDeadline = ticket.SlaDeadline.Add(pausedDuration);
            ticket.SlaPausedAt = null;
        }

        if (ticket.Status == TicketStatus.ReOpen)
        {
            if(ticket.SlaBreached)
                ticket.BreachedBeforeReopen = true;
            ticket.ReopenCount += 1;
        }

        if (ticket.Status == TicketStatus.Resolved)
            ticket.ResolvedAt = DateTime.UtcNow;
        
        _uow.Tickets.Update(ticket);
        await _uow.SaveChangesAsync();

        await WriteAuditLogAsync(ticket.Id, AuditAction.StatusChanged, "status", oldStatus, newStatus.ToString(), currentReason);

        TicketResponseDto response = _mapper.Map<TicketResponseDto>(ticket);

        if (_currentUser.GetUserId() != response.RequestedById)
            await _notificationService.NotifyUserAsync(
                response.RequestedById, ticket.Id, NotificationType.StatusChanged, $"Your ticket #{ticket.Id} status changed to {ticket.Status}");

        if (_currentUser.GetUserId() != response.AssignedToId && response.AssignedToId.HasValue)
            await _notificationService.NotifyUserAsync(
                response.AssignedToId.Value, ticket.Id, NotificationType.StatusChanged, $"Status for ticket #{ticket.Id} changed to {ticket.Status}");

        if (ticket.Status == TicketStatus.ReOpen)
            await _notificationService.NotifyRoleAsync(
                nameof(UserRole.Admin), ticket.Id, NotificationType.StatusChanged, $"Ticket #{ticket.Id} is re-opened.");

        await _notificationService.BroadcastUpdatedTicketAsync(response);

        return ApiResponse<TicketResponseDto>.SuccessResponse(response, "Ticket status updated.");
    }

    public async Task<byte[]> ExportCsvAsync(TicketFilterDto filter)
    {
        IQueryable<Ticket> query = _uow.Tickets.Query()
            .Where(t => !t.IsDeleted)
            .Include(t => t.RequestedByUser)
            .Include(t => t.AssignedToUser)
            .Include(t => t.Category)
            .Include(t => t.SubCategory)
            .Include(t => t.Group);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(t =>
                EF.Functions.ILike(t.RequestedByUser.Name, $"%{filter.Search}%") ||
                EF.Functions.ILike(t.ServiceDetails, $"%{filter.Search}%"));

        if (!string.IsNullOrWhiteSpace(filter.Status) &&
            Enum.TryParse<TicketStatus>(filter.Status, true, out TicketStatus statusFilter))
            query = query.Where(t => t.Status == statusFilter);

        if (!string.IsNullOrWhiteSpace(filter.Priority) &&
            Enum.TryParse<Priority>(filter.Priority, true, out Priority priorityFilter))
            query = query.Where(t => t.Priority == priorityFilter);

        if (filter.GroupId.HasValue)
            query = query.Where(t => t.GroupId == filter.GroupId.Value);

        if (filter.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);

        if (filter.AssignedTo.HasValue)
            query = query.Where(t => t.AssignedTo == filter.AssignedTo.Value);

        if (filter.Breached.HasValue)
            query = query.Where(t => t.SlaBreached == filter.Breached.Value);

        if (filter.DateFrom.HasValue)
            query = query.Where(t => t.CreatedAt >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(t => t.CreatedAt <= filter.DateTo.Value);

        List<Ticket> tickets = await query
            .OrderByDescending(t => t.CreatedAt)
            .Take(5000)
            .ToListAsync();

        return BuildCsv(tickets);
    }


    // Helpers 
    private static readonly HashSet<TicketStatus> PauseStates = new()
    {
        TicketStatus.OnHold,
        TicketStatus.Resolved,
        TicketStatus.CannotResolve
    };

    private bool CanAccessTicket(Ticket ticket) => _currentUser.GetUserRole() switch
    {
        nameof(UserRole.Admin) => true,
        nameof(UserRole.Agent) => ticket.AssignedTo == _currentUser.GetUserId() || ticket.RequestedBy == _currentUser.GetUserId(),
        nameof(UserRole.Requester) => ticket.RequestedBy == _currentUser.GetUserId(),
        _ => false
    };

    private static byte[] BuildCsv(List<Ticket> tickets)
    {
        var sb = new StringBuilder();

        // Header row
        sb.AppendLine("ID,Description,Status,Priority,Group,Category,SubCategory,RequestedBy,AssignedTo,SlaDeadline,SlaBreached,CreatedAt,ResolvedAt");

        foreach (var t in tickets)
        {
            sb.AppendLine(string.Join(",",
                t.Id,
                CsvEscape(t.ServiceDetails),
                t.Status,
                t.Priority,
                CsvEscape(t.Group?.Name),
                CsvEscape(t.Category?.Name),
                CsvEscape(t.SubCategory?.Name),
                CsvEscape(t.RequestedByUser?.Name),
                CsvEscape(t.AssignedToUser?.Name ?? "Unassigned"),
                FormatDate(t.SlaDeadline),
                t.SlaBreached,
                FormatDate(t.CreatedAt),
                FormatDate(t.ResolvedAt)
            ));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string CsvEscape(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";

        value = value
            .Replace("\r\n", " ")
            .Replace("\n", " ")
            .Replace("\r", " ")
            .Replace("\t", " ");
        value = System.Text.RegularExpressions.Regex.Replace(value, @" {2,}", " ").Trim();

        if (value.Contains(',') || value.Contains('"'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private static readonly TimeZoneInfo AppTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");

    private static string FormatDate(DateTime? utcDate)
    {
        if (!utcDate.HasValue) return "";
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcDate.Value, AppTimeZone);
        return local.ToString("yyyy-MM-dd HH:mm");
    }

    private async Task WriteAuditLogAsync(
        int ticketId, AuditAction action, string? fieldName = null, string? oldVal = null, string? newVal = null, string? reason = null)
    {
        AuditLog log = new AuditLog
        {
            TicketId = ticketId,
            ChangedBy = _currentUser.GetUserId(),
            Action = action,
            FieldName = fieldName,
            OldValue = oldVal,
            NewValue = newVal,
            Reason = reason
        };
        await _uow.AuditLogs.AddAsync(log);
        await _uow.SaveChangesAsync();
    }

    private static IQueryable<Ticket> ApplyUrgencyOrder(IQueryable<Ticket> query)
    {
        return query
            .OrderBy(t =>
                // completed tickets always last
                t.Status == TicketStatus.Resolved || t.Status == TicketStatus.Closed || t.Status == TicketStatus.Rejected
                || t.Status == TicketStatus.Cancelled || t.Status == TicketStatus.CannotResolve
                    ? 100
                    : (t.Status == TicketStatus.Open || t.Status == TicketStatus.ReOpen)
                        ? t.SlaBreached ? 0 : 1
                        : t.Status == TicketStatus.InProgress
                            ? t.SlaBreached ? 3 : 4
                            : t.Status == TicketStatus.OnHold
                                ? t.SlaBreached ? 6 : 7
                                : t.Status == TicketStatus.Assigned
                                    ? t.SlaBreached ? 9 : 10
                                    : 50)
            // sort them by their sla deadline and non active goes to last
            .ThenBy(t =>
                t.Status != TicketStatus.Rejected && t.Status != TicketStatus.Resolved && t.Status != TicketStatus.Closed
                && t.Status != TicketStatus.Cancelled && t.Status != TicketStatus.CannotResolve
                    ? t.SlaDeadline : DateTime.MaxValue)
            // nearest deadline first inside same priority
            .ThenByDescending(t => t.ResolvedAt != null ? t.ResolvedAt : DateTime.MinValue);
    }

    private static readonly Dictionary<TicketStatus, List<TicketStatus>> ValidTransitions = new()
    {
        { TicketStatus.Open,            [ TicketStatus.Assigned, TicketStatus.Rejected, TicketStatus.Cancelled ] },
        { TicketStatus.Assigned,        [ TicketStatus.InProgress, TicketStatus.Rejected, TicketStatus.Cancelled ] },
        { TicketStatus.InProgress,      [ TicketStatus.OnHold, TicketStatus.Resolved, TicketStatus.Cancelled, TicketStatus.CannotResolve ] },
        { TicketStatus.OnHold,          [ TicketStatus.InProgress, TicketStatus.Resolved, TicketStatus.Cancelled, TicketStatus.CannotResolve ] },
        { TicketStatus.Resolved,        [ TicketStatus.Closed, TicketStatus.ReOpen ] },
        { TicketStatus.Closed,          [] },
        { TicketStatus.Rejected,        [] },
        { TicketStatus.Cancelled,       [] },
        { TicketStatus.ReOpen,          [ TicketStatus.Assigned, TicketStatus.Cancelled, TicketStatus.InProgress ] },
        { TicketStatus.CannotResolve,   [ TicketStatus.ReOpen ] }
    };
}
