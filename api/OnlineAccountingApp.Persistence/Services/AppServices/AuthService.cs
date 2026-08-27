using Microsoft.AspNetCore.Identity;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;
using DomainValidationException = OnlineAccountingApp.Domain.Exceptions.ValidationException;

namespace OnlineAccountingApp.Persistence.Services.AppServices;

public sealed class AuthService(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService) : IAuthService
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
        RefreshTokenRecord? record = await refreshTokenService.GetAsync(refreshToken, cancellationToken);
        if (record is null)
        {
            throw new BusinessException(AppErrorCodes.Auth.InvalidRefreshToken, "Refresh token is invalid, expired or already used.");
        }

        AppUser? user = await userManager.FindByIdAsync(record.UserId);
        if (user is null)
        {
            await refreshTokenService.RevokeAsync(refreshToken, cancellationToken);
            throw new BusinessException(AppErrorCodes.Auth.InvalidRefreshToken, "Refresh token is invalid, expired or already used.");
        }

        AccessToken accessToken = await CreateAccessTokenAsync(user);

        // Rotate: IssuedAtUtc is carried over, so the absolute session expiry never gets pushed forward.
        (string newRefreshToken, _) = await refreshTokenService.RotateAsync(refreshToken, record, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken.Token,
            RefreshToken = newRefreshToken,
            AccessTokenExpiresAt = accessToken.ExpiresAt
        };
    }

    public Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
        => refreshTokenService.RevokeAsync(refreshToken, cancellationToken);

    private async Task<AuthResponseDto> IssueTokensAsync(AppUser user, CancellationToken cancellationToken)
    {
        AccessToken accessToken = await CreateAccessTokenAsync(user);
        (string refreshToken, _) = await refreshTokenService.IssueAsync(user.Id, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken.Token,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessToken.ExpiresAt
        };
    }

    private async Task<AccessToken> CreateAccessTokenAsync(AppUser user)
    {
        IList<string> roles = await userManager.GetRolesAsync(user);
        return tokenService.CreateAccessToken(user, roles);
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
