using HelpdeskSystem.Domain.Common;

namespace HelpdeskSystem.Domain.Entities;

public class Group : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
