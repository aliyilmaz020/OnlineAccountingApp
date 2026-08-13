using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.RefreshToken;

public sealed class RefreshTokenCommand : IRequest<AuthResponseDto>
{
    public string RefreshToken { get; set; }
}
