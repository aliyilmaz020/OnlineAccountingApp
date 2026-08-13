using Mapster;
using MediatR;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Framework.MedatR.Create;

public abstract class BaseCreateCommandHandler<TCommand, TEntity, TResponse>(IUnitOfWork unitOfWork)
    : IRequestHandler<TCommand, TResponse>
    where TCommand : BaseCreateCommand<TResponse>
    where TEntity : BaseEntity
{
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected IRepository<TEntity> Repository => UnitOfWork.Repository<TEntity>();

    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        await EnsureDoesNotAlreadyExistAsync(request, cancellationToken);

        TEntity entity = await MapToEntityAsync(request, cancellationToken);

        await BeforeCreateAsync(entity, request, cancellationToken);

        await UnitOfWork.BeginTransactionAsync(cancellationToken);
        TEntity createdEntity = await Repository.CreateAsync(entity, cancellationToken);

        await AfterCreateAsync(createdEntity, request, cancellationToken);

        await UnitOfWork.CommitAsync(cancellationToken);

        return await MapToResponseAsync(createdEntity, cancellationToken);
    }

    /// <summary>Return the predicate that identifies a conflicting entity. Return null (default) to skip the existence check.</summary>
    protected virtual Expression<Func<TEntity, bool>>? GetExistsPredicate(TCommand request) => null;

    protected virtual async Task EnsureDoesNotAlreadyExistAsync(TCommand request, CancellationToken cancellationToken)
    {
        Expression<Func<TEntity, bool>>? predicate = GetExistsPredicate(request);
        if (predicate is null)
        {
            return;
        }

        bool alreadyExists = await Repository.ExistsAsync(predicate, cancellationToken);
        if (alreadyExists)
        {
            throw new BusinessException(GetAlreadyExistsErrorCode(), GetAlreadyExistsErrorMessage());
        }
    }

    protected virtual string GetAlreadyExistsErrorCode() => AppErrorCodes.Common.ValidationFailed;

    protected virtual string GetAlreadyExistsErrorMessage() => "Entity already exists.";

    protected virtual Task<TEntity> MapToEntityAsync(TCommand request, CancellationToken cancellationToken)
        => Task.FromResult(request.Adapt<TEntity>());

    protected virtual Task<TResponse> MapToResponseAsync(TEntity entity, CancellationToken cancellationToken)
        => Task.FromResult(entity.Adapt<TResponse>());

    /// <summary>Injection point executed after mapping and before persistence.</summary>
    protected virtual Task BeforeCreateAsync(TEntity entity, TCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;

    /// <summary>Injection point executed after persistence and before commit.</summary>
    protected virtual Task AfterCreateAsync(TEntity entity, TCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
