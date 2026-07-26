using Microsoft.EntityFrameworkCore;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Persistence.Context;

namespace OnlineAccountingApp.Persistence.Services.CompanyServices;

public sealed class UniformChartOfAccountService(CompanyDbContext context) : Repository<UniformChartOfAccount, CompanyDbContext>(context), IUniformChartOfAccountService
{
    private static readonly Func<CompanyDbContext, string, Task<UniformChartOfAccount?>> GetByCodeAsyncCompiled = EF.CompileAsyncQuery((CompanyDbContext dbContext, string code) =>
        dbContext.Set<UniformChartOfAccount>().FirstOrDefault(account => account.Code == code && !account.Deleted));

    public async Task<UniformChartOfAccount?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await GetByCodeAsyncCompiled(context, code);
    }
}
