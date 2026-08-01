using Mapster;
using MediatR;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Framework.MedatR.GetById;

public abstract class BaseGetByIdQueryHandler<TQuery, TEntity, TResponse>(IUnitOfWork unitOfWork)
    : IRequestHandler<TQuery, TResponse>
    where TQuery : BaseGetByIdQuery<TResponse>
    where TEntity : BaseEntity
{
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected IRepository<TEntity> Repository => UnitOfWork.Repository<TEntity>();

    public async Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken)
    {
        TEntity entity = await GetExistingEntityAsync(request, cancellationToken);
        return await MapToResponseAsync(entity, cancellationToken);
    }

    protected virtual async Task<TEntity> GetExistingEntityAsync(TQuery request, CancellationToken cancellationToken)
    {
        TEntity? entity = await Repository.GetAsync(BuildPredicate(request), GetIncludes(request), cancellationToken: cancellationToken);
        if (entity is null)
        {
            throw new BusinessException(GetNotFoundErrorCode(), GetNotFoundErrorMessage());
        }

        return entity;
    }

    /// <summary>Override to change how the entity is looked up (defaults to matching by Id).</summary>
    protected virtual Expression<Func<TEntity, bool>> BuildPredicate(TQuery request) => entity => entity.Id == request.Id;

    /// <summary>Override to eager-load navigation properties.</summary>
    protected virtual Expression<Func<TEntity, object>>[]? GetIncludes(TQuery request) => null;

    protected virtual string GetNotFoundErrorCode() => AppErrorCodes.Common.ValidationFailed;

    protected virtual string GetNotFoundErrorMessage() => "Entity not found.";

    protected virtual Task<TResponse> MapToResponseAsync(TEntity entity, CancellationToken cancellationToken)
        => Task.FromResult(entity.Adapt<TResponse>());
}
