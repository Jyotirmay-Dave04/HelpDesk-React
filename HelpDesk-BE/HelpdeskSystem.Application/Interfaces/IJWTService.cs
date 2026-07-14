using HelpdeskSystem.Domain.Entities;

namespace HelpdeskSystem.Application.Interfaces;

public interface IJWTService
{
    string GenerateToken(User user);
}
