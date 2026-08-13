using MediatR;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Framework.MedatR.Delete;

public abstract class BaseDeleteCommandHandler<TCommand, TEntity>(IUnitOfWork unitOfWork)
    : IRequestHandler<TCommand, bool>
    where TCommand : BaseDeleteCommand
    where TEntity : BaseEntity
{
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected IRepository<TEntity> Repository => UnitOfWork.Repository<TEntity>();

    public async Task<bool> Handle(TCommand request, CancellationToken cancellationToken)
    {
        TEntity entity = await GetExistingEntityAsync(request, cancellationToken);

        await BeforeDeleteAsync(entity, request, cancellationToken);

        await UnitOfWork.BeginTransactionAsync(cancellationToken);
        bool result = await DeleteEntityAsync(entity, cancellationToken);

        await AfterDeleteAsync(entity, request, cancellationToken);

        await UnitOfWork.CommitAsync(cancellationToken);

        return result;
    }

    protected virtual async Task<TEntity> GetExistingEntityAsync(TCommand request, CancellationToken cancellationToken)
    {
        TEntity? entity = await Repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            throw new BusinessException(GetNotFoundErrorCode(), GetNotFoundErrorMessage());
        }

        return entity;
    }

    protected virtual string GetNotFoundErrorCode() => AppErrorCodes.Common.ValidationFailed;

    protected virtual string GetNotFoundErrorMessage() => "Entity not found.";

    /// <summary>Defaults to soft delete; override to perform a hard delete or additional cleanup.</summary>
    protected virtual Task<bool> DeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
        => Repository.SoftDeleteAsync(entity, cancellationToken);

    /// <summary>Injection point executed before deletion.</summary>
    protected virtual Task BeforeDeleteAsync(TEntity entity, TCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;

    /// <summary>Injection point executed after deletion and before commit.</summary>
    protected virtual Task AfterDeleteAsync(TEntity entity, TCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
