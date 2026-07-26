using Microsoft.EntityFrameworkCore.Storage;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Persistence.Context;

namespace OnlineAccountingApp.Persistence.Services.CompanyServices;

public sealed class CompanyUnitOfWork(CompanyDbContext context) : ICompanyUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = [];
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        Type entityType = typeof(T);
        if (!_repositories.TryGetValue(entityType, out object? repository))
        {
            repository = new Repository<T, CompanyDbContext>(context);
            _repositories[entityType] = repository;
        }

        return (IRepository<T>)repository;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction ??= await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            if (_transaction is not null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction?.Dispose();
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
