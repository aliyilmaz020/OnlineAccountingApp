using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Framework.MedatR.GetList;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;

public sealed class GetCompaniesQueryHandler(IUnitOfWork unitOfWork)
    : BaseGetListQueryHandler<GetCompaniesQuery, Company, CompanyListItemDto>(unitOfWork)
{
    protected override Expression<Func<Company, bool>>? BuildPredicate(GetCompaniesQuery request)
        => string.IsNullOrWhiteSpace(request.SearchTerm)
            ? null
            : company => company.Name.Contains(request.SearchTerm);
}
