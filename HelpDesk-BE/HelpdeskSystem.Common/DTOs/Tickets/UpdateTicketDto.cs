using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.Common.DTOs.Tickets;

public class UpdateTicketDto
{
    public string ServiceDetails { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}
