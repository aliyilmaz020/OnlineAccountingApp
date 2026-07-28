using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Register;

public sealed class RegisterCommandHandler(IAuthService authService) : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        => authService.RegisterAsync(request.Email, request.Password, cancellationToken);
}
