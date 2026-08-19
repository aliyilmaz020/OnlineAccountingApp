using FluentValidation;
using OnlineAccountingApp.Application.Behaviors;
using OnlineAccountingApp.Application.Mapper;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Persistence.Services.AppServices;
using OnlineAccountingApp.Persistence.Services.CompanyServices;

namespace OnlineAccountingApp.WebApi.DependencyInjections.Application;

public static partial class ApplicationDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
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
            MapsterConfig.RegisterUserMappings();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IUniformChartOfAccountService, UniformChartOfAccountService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISeedService, SeedService>();
            services.AddScoped<IPermissionService, PermissionService>();
        }
    }
}
