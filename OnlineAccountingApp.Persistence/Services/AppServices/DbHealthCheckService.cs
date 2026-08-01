
using Microsoft.Extensions.Logging;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Persistence.Context;

namespace OnlineAccountingApp.Persistence.Services.AppServices;

public class DbHealthCheckService(AppDbContext dbContext, ILogger<DbHealthCheckService> logger) : IHealthCheckService
{
    public string ServiceName => "Database";

    public async Task<bool> CheckHealthAsync(CancellationToken cancellationToken)
    {
        bool isHealthy = await dbContext.Database.CanConnectAsync(cancellationToken);

        if (isHealthy)
        {
            logger.LogInformation("Health check succeeded for {ServiceName}", ServiceName);
        }
        else
        {
            logger.LogError("Health check failed for {ServiceName}: could not connect to the database", ServiceName);
        }

        return isHealthy;
    }
}