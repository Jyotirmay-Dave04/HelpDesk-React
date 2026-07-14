using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.Data;
using HelpdeskSystem.Infrastructure.Repository;

namespace HelpdeskSystem.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly HelpdeskDbContext _dbContext;

    public IGenericRepository<User> Users { get; }
    public IGenericRepository<Ticket> Tickets { get; }
    public IGenericRepository<Group> Groups { get; }
    public IGenericRepository<Category> Categories { get; }
    public IGenericRepository<SubCategory> SubCategories { get; }
    public IGenericRepository<Comment> Comments { get; }
    public IGenericRepository<AuditLog> AuditLogs { get; }
    public IGenericRepository<Notification> Notifications { get; }
    public IGenericRepository<SlaPolicy> SlaPolicies { get; }
    public IGenericRepository<CannedResponse> CannedResponses { get; }

    public UnitOfWork(HelpdeskDbContext dbContext)
    {
        _dbContext = dbContext;
        Users = new GenericRepository<User>(dbContext);
        Tickets = new GenericRepository<Ticket>(dbContext);
        Groups = new GenericRepository<Group>(dbContext);
        Categories = new GenericRepository<Category>(dbContext);
        SubCategories = new GenericRepository<SubCategory>(dbContext);
        Comments = new GenericRepository<Comment>(dbContext);
        AuditLogs = new GenericRepository<AuditLog>(dbContext);
        Notifications = new GenericRepository<Notification>(dbContext);
        SlaPolicies = new GenericRepository<SlaPolicy>(dbContext);
        CannedResponses = new GenericRepository<CannedResponse>(dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}
