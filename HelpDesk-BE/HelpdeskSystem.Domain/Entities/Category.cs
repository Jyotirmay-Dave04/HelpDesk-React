using HelpdeskSystem.Domain.Common;

namespace HelpdeskSystem.Domain.Entities;

public class Category : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GroupId { get; set; }

    // Navigation properties
    public Group Group { get; set; } = null!;
    public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
