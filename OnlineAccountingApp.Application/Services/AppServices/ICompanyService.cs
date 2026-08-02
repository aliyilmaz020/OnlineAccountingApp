using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Services.AppServices;

public interface ICompanyService : IRepository<Company>
{
    Task<bool> MigrateCompanyDbAsync();
}
