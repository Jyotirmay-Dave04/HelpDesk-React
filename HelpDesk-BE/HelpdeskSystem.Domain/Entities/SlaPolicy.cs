using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Common;

namespace HelpdeskSystem.Domain.Entities;

public class SlaPolicy: BaseEntity
{
    public int Id { get; set; }
    public Priority Priority { get; set; }
    public int HoursToResolve { get; set; }
}
