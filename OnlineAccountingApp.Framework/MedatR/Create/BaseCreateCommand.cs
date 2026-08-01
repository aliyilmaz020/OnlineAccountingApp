using MediatR;

namespace OnlineAccountingApp.Framework.MedatR.Create;

public abstract class BaseCreateCommand<TResponse> : IRequest<TResponse>
{
}
