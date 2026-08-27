using MediatR;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Logout;

public sealed class LogoutCommand : IRequest<Unit>
{
    public string RefreshToken { get; set; }
}
