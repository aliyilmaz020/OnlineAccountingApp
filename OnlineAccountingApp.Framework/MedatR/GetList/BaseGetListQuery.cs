using MediatR;
using OnlineAccountingApp.Framework.MedatR.Common;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Framework.MedatR.GetList;

public abstract class BaseGetListQuery<TResponse> : IRequest<PagedResult<TResponse>>, IPagedQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
