using Microsoft.EntityFrameworkCore;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Persistence.Context;

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
        }

    }
}

