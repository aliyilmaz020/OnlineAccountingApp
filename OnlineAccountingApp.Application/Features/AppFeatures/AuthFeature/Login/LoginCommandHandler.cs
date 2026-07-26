using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;
// The sibling AuthFeature.RefreshToken namespace otherwise shadows the entity type.
using RefreshTokenEntity = OnlineAccountingApp.Domain.Entities.RefreshToken;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Login;

public sealed class LoginCommandHandler(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new BusinessException(AppErrorCodes.Auth.InvalidCredentials, "Email or password is incorrect.");
        }

        IList<string> roles = await userManager.GetRolesAsync(user);
        AccessToken accessToken = tokenService.CreateAccessToken(user, roles);

        var refreshToken = new RefreshTokenEntity
        {
            Token = tokenService.CreateRefreshToken(),
            AppUserId = user.Id,
            ExpiresAt = tokenService.GetRefreshTokenExpiry(),
            Status = true
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        await refreshTokenService.CreateAsync(refreshToken, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken.Token,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt
        };
    }
}
