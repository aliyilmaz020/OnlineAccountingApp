using Microsoft.EntityFrameworkCore;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Persistence.Context;

namespace OnlineAccountingApp.Persistence.Services;

public sealed class CompanyService(AppDbContext context) : Repository<Company>(context), ICompanyService
{
    private static readonly Func<AppDbContext, string, Task<Company?>> GetCompanyByNameAsyncCompiled = EF.CompileAsyncQuery((AppDbContext dbContext, string name) =>
        dbContext.Companies.FirstOrDefault(c => c.Name == name));

    public async Task<Company?> GetCompanyByNameAsync(string name)
    {
        return await GetCompanyByNameAsyncCompiled(context, name);
    }

    public async Task<bool> MigrateCompanyDbAsync()
    {
        var companies = await context.Set<Company>().ToListAsync();
        foreach (var company in companies)
        {
            var db = new CompanyDbContext(company);
            try
            {
                await db.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        return true;
    }
}
