using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Infrasructure.Options;
using OnlineAccountingApp.Infrastructure.Options;
using OnlineAccountingApp.Infrastructure.Services;
using StackExchange.Redis;

namespace OnlineAccountingApp.WebApi.DependencyInjections.Infrastructure;

public static partial class InfrastructureDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructure(IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddOptions<RedisOptions>()
                    .BindConfiguration("Redis")
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConf = configuration.GetSection("Redis").Get<RedisOptions>();
                var connection = ConnectionMultiplexer.Connect(redisConf!.ConnectionString);
                return connection;
            });
        }
    }
}
