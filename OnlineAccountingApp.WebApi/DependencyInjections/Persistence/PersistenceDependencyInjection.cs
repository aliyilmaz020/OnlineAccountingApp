using Microsoft.EntityFrameworkCore;
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
        }

    }
}

