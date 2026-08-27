using OnlineAccountingApp.Application.Services.AppServices;
using StackExchange.Redis;

namespace OnlineAccountingApp.Infrastructure.Services;

public class RedisCacheService(IConnectionMultiplexer connectionMultiplexer) : ICacheService
{
    private readonly IDatabase Database = connectionMultiplexer.GetDatabase();

    public Task<bool> SetStringAsync(string key, string value, double? expiresIn = 500)
    {
        var result = Database.StringSetAsync(key, value, TimeSpan.FromMinutes(expiresIn.Value));
        return result;
    }
    public async Task<string> GetValueAsync(string key) => await Database.StringGetAsync(key);
    public async Task Clear(string key) => await Database.KeyDeleteAsync(key);
}