using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Persistence.Context;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Persistence.Services;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : BaseEntity
{
    public async Task<bool> CreateAsync(T entity)
    {
        EntityEntry<T> entry = await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        return entry.State == EntityState.Unchanged;
    }

    public async Task<bool> DeleteAsync(T entity)
    {
        EntityEntry<T> entry = context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
        return entry.State == EntityState.Detached;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await context.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
    {
        return await context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        EntityEntry<T> entry = context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
        return entry.State == EntityState.Unchanged;
    }
}
