namespace HelpdeskSystem.Common.DTOs.SlaPolicies;

public class SlaPolicyResponseDto
{
    public int Id { get; set; }
    public string Priority { get; set; } = string.Empty;
    public int HoursToResolve { get; set; }
}
