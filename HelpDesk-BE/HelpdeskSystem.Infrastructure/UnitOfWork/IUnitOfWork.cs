using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.Repository;

namespace HelpdeskSystem.Infrastructure.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<User> Users { get; }
    IGenericRepository<Ticket> Tickets { get; }
    IGenericRepository<Group> Groups { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<SubCategory> SubCategories { get; }
    IGenericRepository<Comment> Comments { get; }
    IGenericRepository<AuditLog> AuditLogs { get; }
    IGenericRepository<Notification> Notifications { get; }
    IGenericRepository<SlaPolicy> SlaPolicies { get; }
    IGenericRepository<CannedResponse> CannedResponses { get; }
    Task<int> SaveChangesAsync();
}
