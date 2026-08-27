namespace OnlineAccountingApp.Application.Services.AppServices;

public sealed class RefreshTokenRecord
{
    public required string UserId { get; init; }
    public required DateTime IssuedAtUtc { get; init; }
    public required DateTime ExpiresAtUtc { get; init; }
}
