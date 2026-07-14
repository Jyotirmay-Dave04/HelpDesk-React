using System.Security.Claims;
using HelpdeskSystem.Application.Interfaces;

namespace HelpdeskSystem.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor) {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUserId()
    {
        string? claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        return int.TryParse(claim, out int id) ? id : 0;
    }

    public string GetUserName()
    {
        string? claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;

        return claim ?? "";
    }

    public string GetUserEmail()
    {
        string? claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

        return claim ?? "";
    }

    public string GetUserRole()
    {
        string? claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

        return claim ?? "";
    }
}
