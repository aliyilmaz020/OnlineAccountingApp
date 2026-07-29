using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Login;
using OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.RefreshToken;
using OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Register;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Grpc.Protos;

namespace OnlineAccountingApp.Grpc.Services;

// No [Authorize] - mirrors AuthController's [AllowAnonymous] (Login/Register/RefreshToken are public).
public sealed class AuthGrpcService(IMediator mediator) : Auth.AuthBase
{
    public override async Task<AuthResponse> Login(LoginRequest request, ServerCallContext context)
    {
        AuthResponseDto result = await mediator.Send(
            new LoginCommand { Email = request.Email, Password = request.Password }, context.CancellationToken);

        return ToResponse(result);
    }

    public override async Task<AuthResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        AuthResponseDto result = await mediator.Send(
            new RegisterCommand { Email = request.Email, Password = request.Password }, context.CancellationToken);

        return ToResponse(result);
    }

    public override async Task<AuthResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        AuthResponseDto result = await mediator.Send(
            new RefreshTokenCommand { RefreshToken = request.RefreshToken }, context.CancellationToken);

        return ToResponse(result);
    }

    private static AuthResponse ToResponse(AuthResponseDto dto) => new()
    {
        AccessToken = dto.AccessToken,
        RefreshToken = dto.RefreshToken,
        AccessTokenExpiresAt = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.AccessTokenExpiresAt, DateTimeKind.Utc))
    };
}
