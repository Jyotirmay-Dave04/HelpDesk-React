using HelpdeskSystem.Application.Helpers;
using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Auth;
using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJWTService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IJWTService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        User? user = await _unitOfWork.Users.FindOneAsync(u => EF.Functions.ILike(u.Email, loginRequestDto.Email.Trim()) && !u.IsDeleted);

        if(user is null || !PasswordHasher.Verify(loginRequestDto.Password, user.PasswordHash))
            return ApiResponse<AuthResponseDto>.FailureResponse("Invalid Credentials.");

        string token = _jwtService.GenerateToken(user);

        AuthResponseDto AuthResponse = new AuthResponseDto { Token = token };

        return ApiResponse<AuthResponseDto>.SuccessResponse(AuthResponse, "Login successful.");
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
    {
        bool exits = await _unitOfWork.Users.ExistsAsync(u => EF.Functions.ILike(u.Email, registerDto.Email.Trim()) && !u.IsDeleted);

        if(exits)
            return ApiResponse<AuthResponseDto>.FailureResponse("Email is already registered.");
        
        User user = new User 
        {
            Name = registerDto.Name,
            Email = registerDto.Email.Trim().ToLowerInvariant(),
            PasswordHash = PasswordHasher.Hash(registerDto.Password),
            Role = UserRole.Requester,
            CreatedBy = 0
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        string token = _jwtService.GenerateToken(user);

        AuthResponseDto AuthResponse = new AuthResponseDto { Token = token };

        return ApiResponse<AuthResponseDto>.SuccessResponse(AuthResponse, "Registration successful.");
    }
}
