using OnlineAccountingApp.Domain.Entities.Identity;

namespace OnlineAccountingApp.Application.Services.AppServices;

public sealed record AccessToken(string Token, DateTime ExpiresAt);

public interface ITokenService
{
    AccessToken CreateAccessToken(AppUser user, IEnumerable<string> roles);

    string CreateRefreshToken();

    DateTime GetRefreshTokenExpiry();
}
