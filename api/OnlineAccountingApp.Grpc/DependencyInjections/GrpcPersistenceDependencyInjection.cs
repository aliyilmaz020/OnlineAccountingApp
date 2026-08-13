using Microsoft.EntityFrameworkCore;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using OnlineAccountingApp.Persistence.Context;
using OnlineAccountingApp.Persistence.Services;
using OnlineAccountingApp.Persistence.Services.CompanyServices;
using OnlineAccountingApp.Persistence.Tenancy;

namespace OnlineAccountingApp.Grpc.DependencyInjections;

/// <summary>
/// Local copy of WebApi's PersistenceDependencyInjection, kept in sync by hand: the Grpc host
/// registers its own AppDbContext/Identity/tenancy pipeline rather than referencing WebApi.
/// </summary>
public static class GrpcPersistenceDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddGrpcPersistence(IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            AddCompanyTenancy(services);
        }
    }

    private static void AddCompanyTenancy(IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddScoped<ICompanyContext, HttpCompanyContext>();

        serviceCollection.AddScoped(serviceProvider =>
        {
            ICompanyContext companyContext = serviceProvider.GetRequiredService<ICompanyContext>();
            if (string.IsNullOrWhiteSpace(companyContext.CompanyId))
            {
                throw new BusinessException(
                    AppErrorCodes.Tenant.CompanyNotSpecified,
                    $"The '{ICompanyContext.HeaderName}' header is required for this operation.");
            }

            AppDbContext appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
            Company? company = appDbContext.Companies
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == companyContext.CompanyId && !c.Deleted);

            if (company is null)
            {
                throw new BusinessException(
                    AppErrorCodes.Tenant.CompanyNotFound,
                    $"Company '{companyContext.CompanyId}' was not found.");
            }

            bool isMember = companyContext.UserId is not null && appDbContext.UserCompanies
                .AsNoTracking()
                .Any(uc => uc.AppUserId == companyContext.UserId
                        && uc.CompanyId == company.Id
                        && !uc.Deleted);

            if (!isMember)
            {
                throw new BusinessException(
                    AppErrorCodes.Auth.CompanyAccessDenied,
                    "You do not have access to this company.");
            }

            return new CompanyDbContext(company);
        });

        serviceCollection.AddScoped<ICompanyUnitOfWork, CompanyUnitOfWork>();
    }
}
