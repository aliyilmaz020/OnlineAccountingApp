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

namespace OnlineAccountingApp.WebApi.DependencyInjections.Persistence;

public static partial class PersistenceDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddPersistence(IConfiguration configuration)
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

    /// <summary>
    /// Registers the per-request resolution of the current company's own database.
    /// Unlike <see cref="AppDbContext"/>, <see cref="CompanyDbContext"/> has no fixed
    /// connection string: it is built from the <see cref="Company"/> identified by the
    /// current request, and is only resolved when a company-scoped service asks for it.
    /// </summary>
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

            // The header alone is not proof of access: the caller must actually belong to
            // this company, otherwise any authenticated user could read any tenant's data.
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

