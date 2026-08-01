using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Services.AppServices;

public interface ICompanyService : IRepository<Company>
{
    Task<bool> MigrateCompanyDbAsync();
}
