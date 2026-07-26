using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Persistence.Context;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Persistence.Services;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : BaseEntity
{
    private static readonly Func<AppDbContext, string, Task<T?>> GetById = EF.CompileAsyncQuery((AppDbContext dbContext, string id) =>
        dbContext.Set<T>().FirstOrDefault(e => e.Id == id));

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<T> entry = await context.Set<T>().AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task CreateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await context.Set<T>().AddRangeAsync(entities, cancellationToken);
    }

    public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<T> entry = context.Set<T>().Update(entity);
        return Task.FromResult(entry.Entity);
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        context.Set<T>().UpdateRange(entities);
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<T> entry = context.Set<T>().Remove(entity);
        return Task.FromResult(entry.State == EntityState.Deleted);
    }

    public Task<bool> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        context.Set<T>().RemoveRange(entities);
        return Task.FromResult(true);
    }

    public Task<bool> SoftDeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.Deleted = true;
        entity.EditDate = DateTime.UtcNow;
        context.Set<T>().Update(entity);
        return Task.FromResult(true);
    }

    public async Task<T?> GetAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>[]? includes = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = ApplyIncludes(context.Set<T>(), includes);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetById(context, id);
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>[]? includes = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = ApplyIncludes(context.Set<T>(), includes);
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>[]? includes = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    
    {
        IQueryable<T> query = ApplyIncludes(context.Set<T>(), includes);
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        int totalCount = await query.CountAsync(cancellationToken);
        List<T> items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await context.Set<T>().AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate is null
            ? await context.Set<T>().CountAsync(cancellationToken)
            : await context.Set<T>().CountAsync(predicate, cancellationToken);
    }

    private static IQueryable<T> ApplyIncludes(IQueryable<T> query, Expression<Func<T, object>>[]? includes)
    {
        if (includes is null)
        {
            return query;
        }

        foreach (Expression<Func<T, object>> include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }


}
