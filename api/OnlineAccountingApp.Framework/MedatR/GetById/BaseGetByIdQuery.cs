using MediatR;
using OnlineAccountingApp.Framework.MedatR.Common;

namespace OnlineAccountingApp.Framework.MedatR.GetById;

public abstract class BaseGetByIdQuery<TResponse> : IRequest<TResponse>, IHasId
{
    public string Id { get; set; } = default!;
}
