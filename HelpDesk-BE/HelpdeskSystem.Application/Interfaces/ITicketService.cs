using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Tickets;

namespace HelpdeskSystem.Application.Interfaces;

public interface ITicketService
{
    Task<ApiResponse<TicketResponseDto>> CreateAsync(CreateTicketDto dto);
    Task<ApiResponse<TicketResponseDto>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResponse<TicketListItemDto>>> GetAllAsync(TicketFilterDto filter);
    Task<ApiResponse<TicketResponseDto>> UpdateAsync(int id, UpdateTicketDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<TicketResponseDto>> AssignAsync(int ticketId, AssignTicketDto dto);
    Task<ApiResponse<TicketResponseDto>> UpdateStatusAsync(int ticketId, UpdateStatusDto dto);
    Task<byte[]> ExportCsvAsync(TicketFilterDto filter);
}
