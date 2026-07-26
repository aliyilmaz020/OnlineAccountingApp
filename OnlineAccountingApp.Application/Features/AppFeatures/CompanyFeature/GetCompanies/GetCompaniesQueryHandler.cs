using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;

public sealed class GetCompaniesQueryHandler(ICompanyService companyService) : IRequestHandler<GetCompaniesQuery, PagedResult<CompanyListItemDto>>
{
    public async Task<PagedResult<CompanyListItemDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Company, bool>>? predicate = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? null
            : company => company.Name.Contains(request.SearchTerm);

        PagedResult<Company> paged = await companyService.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            predicate,
            cancellationToken: cancellationToken);

        return new PagedResult<CompanyListItemDto>
        {
            Items = paged.Items.Adapt<List<CompanyListItemDto>>(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }
}
