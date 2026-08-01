using Mapster;
using MediatR;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Framework.MedatR.Update;

public abstract class BaseUpdateCommandHandler<TCommand, TEntity, TResponse>(IUnitOfWork unitOfWork)
    : IRequestHandler<TCommand, TResponse>
    where TCommand : BaseUpdateCommand<TResponse>
    where TEntity : BaseEntity
{
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected IRepository<TEntity> Repository => UnitOfWork.Repository<TEntity>();

    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        TEntity entity = await GetExistingEntityAsync(request, cancellationToken);

        await EnsureNoConflictExistsAsync(request, entity, cancellationToken);

        await MapToEntityAsync(request, entity, cancellationToken);

        await BeforeUpdateAsync(entity, request, cancellationToken);

        await UnitOfWork.BeginTransactionAsync(cancellationToken);
        TEntity updatedEntity = await Repository.UpdateAsync(entity, cancellationToken);

        await AfterUpdateAsync(updatedEntity, request, cancellationToken);

        await UnitOfWork.CommitAsync(cancellationToken);

        return await MapToResponseAsync(updatedEntity, cancellationToken);
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

    /// <summary>Return the predicate that identifies a conflicting entity (e.g. same name, excluding the entity being updated). Return null (default) to skip the check.</summary>
    protected virtual Expression<Func<TEntity, bool>>? GetConflictPredicate(TCommand request, TEntity entity) => null;

    protected virtual async Task EnsureNoConflictExistsAsync(TCommand request, TEntity entity, CancellationToken cancellationToken)
    {
        Expression<Func<TEntity, bool>>? predicate = GetConflictPredicate(request, entity);
        if (predicate is null)
        {
            return;
        }

        bool conflictExists = await Repository.ExistsAsync(predicate, cancellationToken);
        if (conflictExists)
        {
            throw new BusinessException(GetConflictErrorCode(), GetConflictErrorMessage());
        }
    }

    protected virtual string GetConflictErrorCode() => AppErrorCodes.Common.ValidationFailed;

    protected virtual string GetConflictErrorMessage() => "Entity already exists.";

    protected virtual Task MapToEntityAsync(TCommand request, TEntity entity, CancellationToken cancellationToken)
    {
        request.Adapt(entity);
        return Task.CompletedTask;
    }

    protected virtual Task<TResponse> MapToResponseAsync(TEntity entity, CancellationToken cancellationToken)
        => Task.FromResult(entity.Adapt<TResponse>());

    /// <summary>Injection point executed after mapping and before persistence.</summary>
    protected virtual Task BeforeUpdateAsync(TEntity entity, TCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;

    /// <summary>Injection point executed after persistence and before commit.</summary>
    protected virtual Task AfterUpdateAsync(TEntity entity, TCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
