using System.Linq.Expressions;
using HelpdeskSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Infrastructure.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly HelpdeskDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(HelpdeskDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(List<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
    {
        return await _dbSet.AnyAsync(filter);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? includes = null)
    {
        IQueryable<T> query = _dbSet;

        query = query.Where(filter);

        if(orderBy is not null)     query = orderBy(query);

        if(includes is not null)    query = includes(query);

        return await query.ToListAsync();
    }

    public async Task<T?> FindOneAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? includes = null)
    {
        IQueryable<T> query = _dbSet;

        query = query.Where(filter);

        if(orderBy is not null)     query = orderBy(query);

        if(includes is not null)    query = includes(query);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? includes = null)
    {
        IQueryable<T> query = _dbSet;

        if(filter is not null)      query = query.Where(filter).AsNoTracking();

        if(orderBy is not null)     query = orderBy(query);

        if(includes is not null)    query = includes(query);

        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPaginatedAsync(IQueryable<T> query, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? includes = null, int pageNumber = 1, int pageSize = 10)
    {
        if(includes is not null)    query = includes(query);

        int TotalCount = await query.CountAsync();

        if(orderBy is not null) query = orderBy(query);

        List<T> Items = await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (Items, TotalCount);
    }

    public IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
}
