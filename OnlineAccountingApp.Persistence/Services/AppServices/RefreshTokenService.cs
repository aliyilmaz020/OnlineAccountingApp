using Microsoft.EntityFrameworkCore;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Persistence.Context;

namespace OnlineAccountingApp.Persistence.Services.AppServices;

public sealed class RefreshTokenService(AppDbContext context) : Repository<RefreshToken, AppDbContext>(context), IRefreshTokenService
{
    private static readonly Func<AppDbContext, string, Task<RefreshToken?>> GetByTokenAsyncCompiled = EF.CompileAsyncQuery((AppDbContext dbContext, string token) =>
        dbContext.RefreshTokens.FirstOrDefault(refreshToken => refreshToken.Token == token));

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await GetByTokenAsyncCompiled(context, token);
    }
}
