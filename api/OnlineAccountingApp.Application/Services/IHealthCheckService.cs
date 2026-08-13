namespace OnlineAccountingApp.Application.Services;

public interface IHealthCheckService
{
    public string ServiceName { get; }
    Task<bool> CheckHealthAsync(CancellationToken cancellationToken);
}