using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Login;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;
using RefreshTokenEntity = OnlineAccountingApp.Domain.Entities.RefreshToken;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService,
    IUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        RefreshTokenEntity? storedToken = await refreshTokenService.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken is null || !storedToken.IsActive(DateTime.UtcNow))
        {
            throw new BusinessException(AppErrorCodes.Auth.InvalidRefreshToken, "Refresh token is invalid, expired or already used.");
        }

        AppUser? user = await userManager.FindByIdAsync(storedToken.AppUserId);
        if (user is null)
        {
            throw new BusinessException(AppErrorCodes.Auth.InvalidRefreshToken, "Refresh token is invalid, expired or already used.");
        }

        IList<string> roles = await userManager.GetRolesAsync(user);
        AccessToken accessToken = tokenService.CreateAccessToken(user, roles);

        // Rotate: the presented token is revoked and replaced, so replaying it fails.
        storedToken.RevokedAt = DateTime.UtcNow;

        var newRefreshToken = new RefreshTokenEntity
        {
            Token = tokenService.CreateRefreshToken(),
            AppUserId = user.Id,
            ExpiresAt = tokenService.GetRefreshTokenExpiry(),
            Status = true
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        await refreshTokenService.UpdateAsync(storedToken, cancellationToken);
        await refreshTokenService.CreateAsync(newRefreshToken, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken.Token,
            RefreshToken = newRefreshToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt
        };
    }
}
