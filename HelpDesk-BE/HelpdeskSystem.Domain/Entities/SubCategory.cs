using HelpdeskSystem.Domain.Common;

namespace HelpdeskSystem.Domain.Entities;

public class SubCategory : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }

    // Navigation properties
    public Category Category { get; set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
