using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Persistence.Context;

namespace OnlineAccountingApp.Persistence.Services.CompanyServices;

public sealed class UniformChartOfAccountService(CompanyDbContext context) : Repository<UniformChartOfAccount, CompanyDbContext>(context), IUniformChartOfAccountService
{
}
