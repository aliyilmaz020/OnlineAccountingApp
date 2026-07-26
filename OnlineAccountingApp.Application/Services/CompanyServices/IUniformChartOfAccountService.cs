using OnlineAccountingApp.Domain.CompanyEntities;

namespace OnlineAccountingApp.Application.Services.CompanyServices;

public interface IUniformChartOfAccountService : IRepository<UniformChartOfAccount>
{
    Task<UniformChartOfAccount?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
