namespace OnlineAccountingApp.Application.Services.AppServices;

public interface IRefreshTokenService : ICacheService
{
    Task<(string Token, RefreshTokenRecord Record)> IssueAsync(string userId, CancellationToken cancellationToken = default);

    Task<RefreshTokenRecord?> GetAsync(string token, CancellationToken cancellationToken = default);

    Task<(string Token, RefreshTokenRecord Record)> RotateAsync(string oldToken, RefreshTokenRecord existingRecord, CancellationToken cancellationToken = default);

    Task RevokeAsync(string token, CancellationToken cancellationToken = default);
}
