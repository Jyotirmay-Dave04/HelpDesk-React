using HelpdeskSystem.Domain.Common;

namespace HelpdeskSystem.Domain.Entities;

public class CannedResponse : BaseEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
