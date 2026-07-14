using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.Common.DTOs.User;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? ActiveTicketCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
