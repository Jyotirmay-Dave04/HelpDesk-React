using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.CannedResponses;

namespace HelpdeskSystem.Application.Interfaces;

public interface ICannedResponseService
{
    Task<ApiResponse<List<CannedResponseResponseDto>>> GetAllAsync();
    Task<ApiResponse<CannedResponseResponseDto>> CreateAsync(CreateCannedResponseDto dto);
    Task<ApiResponse<CannedResponseResponseDto>> UpdateAsync(int id, UpdateCannedResponseDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}
