using System.Reflection;
using OnlineAccountingApp.Application.Services;

namespace OnlineAccountingApp.WebApi.DependencyInjections;

public class HealthCheckStartupService(IServiceProvider sp) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // sp is the root provider (this service is a singleton); AppDbContext and other
        // scoped dependencies can only be resolved from a scope, not directly from root.
        using IServiceScope scope = sp.CreateScope();

        var persistence = Assembly.GetAssembly(typeof(OnlineAccountingApp.Persistence.PersistenceMarker));
        var persistenceHealthCheckServices = persistence!.GetTypes()
            .Where(t => typeof(IHealthCheckService).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => ActivatorUtilities.CreateInstance(scope.ServiceProvider, t) as IHealthCheckService)
            .ToList();

        foreach (var service in persistenceHealthCheckServices)
        {
            var isHealthy = await service!.CheckHealthAsync(cancellationToken);
            if (!isHealthy)
            {
                throw new Exception($"Health check failed for {service.ServiceName}");
            }
        }

    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}