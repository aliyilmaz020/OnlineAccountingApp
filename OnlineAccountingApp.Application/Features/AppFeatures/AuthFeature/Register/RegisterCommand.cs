using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Login;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Register;

public sealed class RegisterCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
