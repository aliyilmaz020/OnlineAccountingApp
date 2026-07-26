using MediatR;
using OnlineAccountingApp.Application.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;

public sealed class GetCompaniesQuery : IRequest<PagedResult<CompanyListItemDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
}
