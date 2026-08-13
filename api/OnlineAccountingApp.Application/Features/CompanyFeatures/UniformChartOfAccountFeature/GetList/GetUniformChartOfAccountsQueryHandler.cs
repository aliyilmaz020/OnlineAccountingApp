using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Framework.MedatR.GetList;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;

public sealed class GetUniformChartOfAccountsQueryHandler(ICompanyUnitOfWork unitOfWork)
    : BaseGetListQueryHandler<GetUniformChartOfAccountsQuery, UniformChartOfAccount, UniformChartOfAccountListItemDto>(unitOfWork)
{
    protected override Expression<Func<UniformChartOfAccount, bool>>? BuildPredicate(GetUniformChartOfAccountsQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return account => !account.Deleted;
        }

        string searchTerm = request.SearchTerm;
        return account => !account.Deleted && (account.Code.Contains(searchTerm) || account.Name.Contains(searchTerm));
    }
}
