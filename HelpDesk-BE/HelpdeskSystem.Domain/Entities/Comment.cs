using HelpdeskSystem.Domain.Common;

namespace HelpdeskSystem.Domain.Entities;

public class Comment : BaseEntity
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int AuthorId { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsInternal { get; set; } = false;

    // Navigation properties
    public Ticket Ticket { get; set; } = null!;
    public User Author { get; set; } = null!;
}
