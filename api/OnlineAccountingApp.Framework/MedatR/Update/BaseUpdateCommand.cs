using MediatR;
using OnlineAccountingApp.Framework.MedatR.Common;

namespace OnlineAccountingApp.Framework.MedatR.Update;

public abstract class BaseUpdateCommand<TResponse> : IRequest<TResponse>, IHasId
{
    public string Id { get; set; } = default!;
}
