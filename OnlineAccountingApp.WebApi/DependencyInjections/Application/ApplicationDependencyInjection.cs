using OnlineAccountingApp.Application.Mapper;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Persistence.Services;

namespace OnlineAccountingApp.WebApi.DependencyInjections.Application;

public static partial class ApplicationDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MapsterConfig).Assembly));
            MapsterConfig.RegisterCompanyMappings();
            services.AddScoped<ICompanyService, CompanyService>();
        }
    }
}
