using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Infrastructure.Options;
using OnlineAccountingApp.Infrastructure.Services;

namespace OnlineAccountingApp.Grpc.DependencyInjections;

/// <summary>Local copy of WebApi's InfrastructureDependencyInjection.</summary>
public static class GrpcInfrastructureDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddGrpcInfrastructure(IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddScoped<ITokenService, JwtTokenService>();
        }
    }
}
