using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.SlaPolicies;
using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.Application.Interfaces;

public interface ISlaPolicyService
{
    Task<ApiResponse<List<SlaPolicyResponseDto>>> GetAllSlaPoliciesAsync();
    Task<ApiResponse<SlaPolicyResponseDto>> UpdateSlaPolicyDto(Priority priority, UpdateSlaPolicyDto dto);
}
