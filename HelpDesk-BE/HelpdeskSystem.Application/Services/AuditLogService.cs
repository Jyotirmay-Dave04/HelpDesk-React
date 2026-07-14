using AutoMapper;
using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.AuditLogs;
using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AuditLogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _uow = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<AuditLogResponseDto>>> GetByTicketIdAsync(int ticketId)
    {
        bool ticketExists = await _uow.Tickets.ExistsAsync(t => t.Id == ticketId && !t.IsDeleted);
        if(!ticketExists)
            return ApiResponse<List<AuditLogResponseDto>>.FailureResponse("Ticket not found.");
        
        IEnumerable<AuditLog> auditLogs = await _uow.AuditLogs.FindAsync(
            filter: a => a.TicketId == ticketId && !a.IsDeleted,
            orderBy: q => q.OrderByDescending(a => a.CreatedAt),
            includes: q => q.Include(a => a.ChangedByUser)
        );

        List<AuditLogResponseDto> response = _mapper.Map<List<AuditLogResponseDto>>(auditLogs);

        return ApiResponse<List<AuditLogResponseDto>>.SuccessResponse(response);
    }
}
