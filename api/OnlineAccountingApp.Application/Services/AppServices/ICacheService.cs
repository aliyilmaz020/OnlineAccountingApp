namespace OnlineAccountingApp.Application.Services.AppServices;

public interface ICacheService
{
    Task<bool> SetStringAsync(string key, string value, double? expiresIn = 500);
    Task<string> GetValueAsync(string key);
    Task Clear(string key);
}