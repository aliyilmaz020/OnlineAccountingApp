using OnlineAccountingApp.Domain.Entities;

namespace OnlineAccountingApp.Application.Services.AppServices;

public interface ICompanyService : IRepository<Company>
{
    Task<Company?> GetCompanyByNameAsync(string name);
    Task<bool> MigrateCompanyDbAsync();
}
