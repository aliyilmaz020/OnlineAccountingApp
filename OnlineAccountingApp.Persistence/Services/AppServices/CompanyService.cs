using Microsoft.EntityFrameworkCore;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Persistence.Context;

namespace OnlineAccountingApp.Persistence.Services.AppServices;

public sealed class CompanyService(AppDbContext context) : Repository<Company, AppDbContext>(context), ICompanyService
{
    public async Task<bool> MigrateCompanyDbAsync()
    {
        var companies = await context.Set<Company>().Where(c => c.Deleted == false).ToListAsync();
        foreach (var company in companies)
        {
            var db = new CompanyDbContext(company);
            await db.Database.MigrateAsync();
        }
        return true;
    }
}
