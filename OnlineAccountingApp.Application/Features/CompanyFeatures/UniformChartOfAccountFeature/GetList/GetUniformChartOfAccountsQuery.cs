using MediatR;
using OnlineAccountingApp.Application.Services;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;

public sealed class GetUniformChartOfAccountsQuery : IRequest<PagedResult<UniformChartOfAccountListItemDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
}
