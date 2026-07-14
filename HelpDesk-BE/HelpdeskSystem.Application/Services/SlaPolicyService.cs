using AutoMapper;
using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.SlaPolicies;
using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.UnitOfWork;

namespace HelpdeskSystem.Application.Services;

public class SlaPolicyService : ISlaPolicyService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public SlaPolicyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _uow = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<SlaPolicyResponseDto>>> GetAllSlaPoliciesAsync()
    {
        IEnumerable<SlaPolicy> policies = await _uow.SlaPolicies.GetAllAsync(orderBy: o => o.OrderBy(sp => sp.Id));
        var dtos = _mapper.Map<List<SlaPolicyResponseDto>>(policies);
        return ApiResponse<List<SlaPolicyResponseDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<SlaPolicyResponseDto>> UpdateSlaPolicyDto(Priority priority, UpdateSlaPolicyDto dto)
    {
        SlaPolicy? policy = await _uow.SlaPolicies.FindOneAsync(sp => sp.Priority == priority);
        if (policy is null)
            return ApiResponse<SlaPolicyResponseDto>.FailureResponse("SLA policy not found for this priority.");
        
        if (dto.HoursToResolve <= 0)
            return ApiResponse<SlaPolicyResponseDto>.FailureResponse("Hours must be greater then zero.");
        
        policy.HoursToResolve = dto.HoursToResolve;
        _uow.SlaPolicies.Update(policy);
        await _uow.SaveChangesAsync();

        SlaPolicyResponseDto response = _mapper.Map<SlaPolicyResponseDto>(policy);
        return ApiResponse<SlaPolicyResponseDto>.SuccessResponse(response);
    }
}
