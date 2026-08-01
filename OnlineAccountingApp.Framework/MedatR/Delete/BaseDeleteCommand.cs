using MediatR;
using OnlineAccountingApp.Framework.MedatR.Common;

namespace OnlineAccountingApp.Framework.MedatR.Delete;

public abstract class BaseDeleteCommand : IRequest<bool>, IHasId
{
    public string Id { get; set; } = default!;
}
