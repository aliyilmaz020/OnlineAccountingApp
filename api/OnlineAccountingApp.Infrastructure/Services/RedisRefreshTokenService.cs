using Microsoft.Extensions.Options;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Infrastructure.Options;
using System.Text.Json;

namespace OnlineAccountingApp.Infrastructure.Services;

public sealed class RedisRefreshTokenService(
    ICacheService cacheService,
    IOptions<JwtOptions> jwtOptions,
    ITokenService tokenService) : IRefreshTokenService
{
    private readonly JwtOptions _options = jwtOptions.Value;

    private static string KeyFor(string token) => $"refresh-token:{token}";

    public Task<bool> SetStringAsync(string key, string value, double? expiresIn = 500)
        => cacheService.SetStringAsync(key, value, expiresIn);

    public Task<string> GetValueAsync(string key) => cacheService.GetValueAsync(key);

    public Task Clear(string key) => cacheService.Clear(key);

    public async Task<(string Token, RefreshTokenRecord Record)> IssueAsync(string userId, CancellationToken cancellationToken = default)
    {
        DateTime issuedAt = DateTime.UtcNow;
        var record = new RefreshTokenRecord
        {
            UserId = userId,
            IssuedAtUtc = issuedAt,
            ExpiresAtUtc = issuedAt.AddMinutes(_options.RefreshTokenDays)
        };

        string token = tokenService.CreateRefreshToken();
        await WriteAsync(token, record);
        return (token, record);
    }

    public async Task<RefreshTokenRecord?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        string value = await cacheService.GetValueAsync(KeyFor(token));
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        RefreshTokenRecord? record = JsonSerializer.Deserialize<RefreshTokenRecord>(value);
        if (record is null || DateTime.UtcNow >= record.ExpiresAtUtc)
        {
            await cacheService.Clear(KeyFor(token));
            return null;
        }

        return record;
    }

    public async Task<(string Token, RefreshTokenRecord Record)> RotateAsync(string oldToken, RefreshTokenRecord existingRecord, CancellationToken cancellationToken = default)
    {
        var newRecord = new RefreshTokenRecord
        {
            UserId = existingRecord.UserId,
            IssuedAtUtc = existingRecord.IssuedAtUtc,
            ExpiresAtUtc = existingRecord.IssuedAtUtc.AddDays(_options.RefreshTokenDays)
        };

        string newToken = tokenService.CreateRefreshToken();

        await WriteAsync(newToken, newRecord);
        await cacheService.Clear(KeyFor(oldToken));

        return (newToken, newRecord);
    }

    public Task RevokeAsync(string token, CancellationToken cancellationToken = default)
        => cacheService.Clear(KeyFor(token));

    private Task WriteAsync(string token, RefreshTokenRecord record)
    {
        TimeSpan ttl = record.ExpiresAtUtc - DateTime.UtcNow;
        if (ttl <= TimeSpan.Zero)
        {
            ttl = TimeSpan.FromSeconds(1);
        }

        return cacheService.SetStringAsync(KeyFor(token), JsonSerializer.Serialize(record), ttl.TotalMinutes);
    }
}
