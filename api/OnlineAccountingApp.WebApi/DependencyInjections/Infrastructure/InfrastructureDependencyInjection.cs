using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Infrastructure.Options;
using OnlineAccountingApp.Infrastructure.Services;

namespace OnlineAccountingApp.WebApi.DependencyInjections.Infrastructure;

public static partial class InfrastructureDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructure(IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddScoped<ITokenService, JwtTokenService>();
        }
    }
}
