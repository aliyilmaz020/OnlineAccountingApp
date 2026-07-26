using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;

public sealed class GetUniformChartOfAccountsQueryHandler(IUniformChartOfAccountService uniformChartOfAccountService) : IRequestHandler<GetUniformChartOfAccountsQuery, PagedResult<UniformChartOfAccountListItemDto>>
{
    public async Task<PagedResult<UniformChartOfAccountListItemDto>> Handle(GetUniformChartOfAccountsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<UniformChartOfAccount, bool>> predicate = account => !account.Deleted;
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            string searchTerm = request.SearchTerm;
            predicate = account => !account.Deleted && (account.Code.Contains(searchTerm) || account.Name.Contains(searchTerm));
        }

        PagedResult<UniformChartOfAccount> paged = await uniformChartOfAccountService.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            predicate,
            cancellationToken: cancellationToken);

        return new PagedResult<UniformChartOfAccountListItemDto>
        {
            Items = paged.Items.Adapt<List<UniformChartOfAccountListItemDto>>(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }
}
