using MediatR;
using OnlineAccountingApp.Framework.MedatR.Common;

namespace OnlineAccountingApp.Framework.MedatR.Create;

public abstract class BaseCreateCommand<TResponse> : IRequest<TResponse>
{
}
