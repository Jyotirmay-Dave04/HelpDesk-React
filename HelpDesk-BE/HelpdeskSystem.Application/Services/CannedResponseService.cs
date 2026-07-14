using AutoMapper;
using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.CannedResponses;
using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.UnitOfWork;

namespace HelpdeskSystem.Application.Services;

public class CannedResponseService : ICannedResponseService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CannedResponseService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CannedResponseResponseDto>> CreateAsync(CreateCannedResponseDto dto)
    {
        CannedResponse cannedResponse = new CannedResponse{
            Title = dto.Title.Trim(),
            Body = dto.Body.Trim()
        };

        await _uow.CannedResponses.AddAsync(cannedResponse);
        await _uow.SaveChangesAsync();

        CannedResponseResponseDto response = _mapper.Map<CannedResponseResponseDto>(cannedResponse);
        return ApiResponse<CannedResponseResponseDto>.SuccessResponse(response);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        CannedResponse? cannedResponse = await _uow.CannedResponses.FindOneAsync(cr => cr.Id == id && !cr.IsDeleted);
        if(cannedResponse is null)
            return ApiResponse<bool>.FailureResponse("Canned Response not found");
        
        _uow.CannedResponses.Delete(cannedResponse);
        await _uow.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<List<CannedResponseResponseDto>>> GetAllAsync()
    {
        IEnumerable<CannedResponse> cannedResponses = await _uow.CannedResponses.GetAllAsync(cr => !cr.IsDeleted, orderBy: o => o.OrderBy(cr => cr.Title));

        List<CannedResponseResponseDto> dtos = _mapper.Map<List<CannedResponseResponseDto>>(cannedResponses);

        return ApiResponse<List<CannedResponseResponseDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<CannedResponseResponseDto>> UpdateAsync(int id, UpdateCannedResponseDto dto)
    {
        CannedResponse? cannedResponse = await _uow.CannedResponses.FindOneAsync(cr => !cr.IsDeleted && cr.Id == id);
        if(cannedResponse is null)
            return ApiResponse<CannedResponseResponseDto>.FailureResponse("Canned Response not found");
        
        cannedResponse.Title = dto.Title.Trim();
        cannedResponse.Body = dto.Body.Trim();
        _uow.CannedResponses.Update(cannedResponse);
        await _uow.SaveChangesAsync();

        CannedResponseResponseDto response = _mapper.Map<CannedResponseResponseDto>(cannedResponse);
        return ApiResponse<CannedResponseResponseDto>.SuccessResponse(response);
    }
}
