using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Login;

public sealed class LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
