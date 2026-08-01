using Microsoft.AspNetCore.Identity;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using DomainValidationException = OnlineAccountingApp.Domain.Exceptions.ValidationException;
using RefreshTokenEntity = OnlineAccountingApp.Domain.Entities.RefreshToken;

namespace OnlineAccountingApp.Persistence.Services.AppServices;

public sealed class AuthService(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService,
    IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        AppUser? user = await userManager.FindByEmailAsync(email);
        if (user is null || !await userManager.CheckPasswordAsync(user, password))
        {
            throw new BusinessException(AppErrorCodes.Auth.InvalidCredentials, "Email or password is incorrect.");
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto> RegisterAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        AppUser? existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            throw new BusinessException(AppErrorCodes.Auth.UserAlreadyExists, "A user with the same email already exists.");
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = email,
            Email = email
        };

        ThrowIfFailed(await userManager.CreateAsync(user, password));

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        RefreshTokenEntity? storedToken = await refreshTokenService.GetByTokenAsync(refreshToken, cancellationToken);
        if (storedToken is null || !storedToken.IsActive(DateTime.UtcNow))
        {
            throw new BusinessException(AppErrorCodes.Auth.InvalidRefreshToken, "Refresh token is invalid, expired or already used.");
        }

        AppUser? user = await userManager.FindByIdAsync(storedToken.AppUserId);
        if (user is null)
        {
            throw new BusinessException(AppErrorCodes.Auth.InvalidRefreshToken, "Refresh token is invalid, expired or already used.");
        }

        AccessToken accessToken = await CreateAccessTokenAsync(user);

        // Rotate: the presented token is revoked and replaced, so replaying it fails.
        storedToken.RevokedAt = DateTime.UtcNow;

        RefreshTokenEntity newRefreshToken = BuildRefreshToken(user.Id);

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

    private async Task<AuthResponseDto> IssueTokensAsync(AppUser user, CancellationToken cancellationToken)
    {
        AccessToken accessToken = await CreateAccessTokenAsync(user);
        RefreshTokenEntity refreshToken = BuildRefreshToken(user.Id);

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

    private async Task<AccessToken> CreateAccessTokenAsync(AppUser user)
    {
        IList<string> roles = await userManager.GetRolesAsync(user);
        return tokenService.CreateAccessToken(user, roles);
    }

    private RefreshTokenEntity BuildRefreshToken(string appUserId)
    {
        return new RefreshTokenEntity
        {
            Token = tokenService.CreateRefreshToken(),
            AppUserId = appUserId,
            ExpiresAt = tokenService.GetRefreshTokenExpiry(),
            Status = true
        };
    }

    /// <summary>Surfaces Identity's own failures through the app's validation error shape.</summary>
    private static void ThrowIfFailed(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = result.Errors
            .GroupBy(error => error.Code)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

        throw new DomainValidationException(errors);
    }
}
