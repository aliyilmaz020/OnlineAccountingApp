using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.RefreshToken;

public sealed class RefreshTokenCommandHandler(IAuthService authService) : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    public Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        => authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
}
