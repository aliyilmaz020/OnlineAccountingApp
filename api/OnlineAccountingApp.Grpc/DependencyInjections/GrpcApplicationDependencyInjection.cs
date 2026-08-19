using FluentValidation;
using OnlineAccountingApp.Application.Behaviors;
using OnlineAccountingApp.Application.Mapper;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Persistence.Services.AppServices;
using OnlineAccountingApp.Persistence.Services.CompanyServices;

namespace OnlineAccountingApp.Grpc.DependencyInjections;

/// <summary>Local copy of WebApi's ApplicationDependencyInjection.</summary>
public static class GrpcApplicationDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddGrpcApplication()
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(MapsterConfig).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddValidatorsFromAssembly(typeof(MapsterConfig).Assembly);
            MapsterConfig.RegisterCompanyMappings();
            MapsterConfig.RegisterUniformChartOfAccountMappings();
            MapsterConfig.RegisterRoleMappings();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IUniformChartOfAccountService, UniformChartOfAccountService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
        }
    }
}
