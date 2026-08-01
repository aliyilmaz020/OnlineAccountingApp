using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Services.AppServices;

public interface IRefreshTokenService : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
}
