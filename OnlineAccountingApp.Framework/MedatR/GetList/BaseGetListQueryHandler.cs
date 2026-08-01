using Mapster;
using MediatR;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Framework.MedatR.GetList;

public abstract class BaseGetListQueryHandler<TQuery, TEntity, TResponse>(IUnitOfWork unitOfWork)
    : IRequestHandler<TQuery, PagedResult<TResponse>>
    where TQuery : BaseGetListQuery<TResponse>
    where TEntity : BaseEntity
{
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected IRepository<TEntity> Repository => UnitOfWork.Repository<TEntity>();

    public async Task<PagedResult<TResponse>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        PagedResult<TEntity> paged = await Repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            BuildPredicate(request),
            GetIncludes(request),
            cancellationToken: cancellationToken);

        return await MapToResponseAsync(paged, cancellationToken);
    }

    /// <summary>Override to filter/search the list (e.g. by a SearchTerm on the query). Return null (default) for no filtering.</summary>
    protected virtual Expression<Func<TEntity, bool>>? BuildPredicate(TQuery request) => null;

    /// <summary>Override to eager-load navigation properties.</summary>
    protected virtual Expression<Func<TEntity, object>>[]? GetIncludes(TQuery request) => null;

    protected virtual Task<PagedResult<TResponse>> MapToResponseAsync(PagedResult<TEntity> paged, CancellationToken cancellationToken)
    {
        return Task.FromResult(new PagedResult<TResponse>
        {
            Items = paged.Items.Adapt<List<TResponse>>(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        });
    }
}
