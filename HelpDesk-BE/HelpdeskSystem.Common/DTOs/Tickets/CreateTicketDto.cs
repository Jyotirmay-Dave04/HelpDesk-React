using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.Common.DTOs.Tickets;

public class CreateTicketDto
{
    public int GroupId { get; set; }
    public int CategoryId { get; set; }
    public int SubCategoryId { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string ServiceDetails { get; set; } = string.Empty;
}
