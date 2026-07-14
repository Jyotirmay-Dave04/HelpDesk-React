using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.AuditLogs;

namespace HelpdeskSystem.Application.Interfaces;

public interface IAuditLogService
{
    Task<ApiResponse<List<AuditLogResponseDto>>> GetByTicketIdAsync(int ticketId);
}
