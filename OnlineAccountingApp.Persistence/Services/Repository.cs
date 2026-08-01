using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Persistence.Services;

public class Repository<TEntity, TContext>(TContext context) : IRepository<TEntity>
    where TEntity : BaseEntity
    where TContext : DbContext
{
    private static readonly Func<TContext, string, Task<TEntity?>> GetById = EF.CompileAsyncQuery((TContext dbContext, string id) =>
        dbContext.Set<TEntity>().FirstOrDefault(e => e.Id == id));

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<TEntity> entry = await context.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task CreateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
    }

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<TEntity> entry = context.Set<TEntity>().Update(entity);
        return Task.FromResult(entry.Entity);
    }

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        context.Set<TEntity>().UpdateRange(entities);
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<TEntity> entry = context.Set<TEntity>().Remove(entity);
        return Task.FromResult(entry.State == EntityState.Deleted);
    }

    public Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        context.Set<TEntity>().RemoveRange(entities);
        return Task.FromResult(true);
    }

    public Task<bool> SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.Deleted = true;
        entity.EditDate = DateTime.UtcNow;
        context.Set<TEntity>().Update(entity);
        return Task.FromResult(true);
    }

    public async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, object>>[]? includes = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = ApplyIncludes(context.Set<TEntity>(), includes);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetById(context, id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>[]? includes = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = ApplyIncludes(context.Set<TEntity>(), includes);
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

    public async Task<PagedResult<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>[]? includes = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = ApplyIncludes(context.Set<TEntity>(), includes);
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        int totalCount = await query.CountAsync(cancellationToken);
        List<TEntity> items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TEntity>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate is null
            ? await context.Set<TEntity>().CountAsync(cancellationToken)
            : await context.Set<TEntity>().CountAsync(predicate, cancellationToken);
    }

    private static IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, Expression<Func<TEntity, object>>[]? includes)
    {
        if (includes is null)
        {
            return query;
        }

        foreach (Expression<Func<TEntity, object>> include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }
}
