using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Roles;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;

public sealed class GetCompaniesQueryHandler(IUnitOfWork unitOfWork, ICompanyContext companyContext)
    : IRequestHandler<GetCompaniesQuery, PagedResult<CompanyListItemDto>>
{
    public async Task<PagedResult<CompanyListItemDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        // A system admin can CRUD every company, so they need to see every company; anyone else
        // only sees the companies they actually belong to.
        bool isAdmin = companyContext.IsInRole(RoleList.SystemAdmin);
        List<string>? companyIds = null;
        if (!isAdmin)
        {
            IEnumerable<UserCompany> userCompanies = await unitOfWork.Repository<UserCompany>()
                .GetAllAsync(uc => uc.AppUserId == companyContext.UserId, cancellationToken: cancellationToken);
            companyIds = userCompanies.Select(uc => uc.CompanyId).ToList();
        }

        PagedResult<Company> paged = await unitOfWork.Repository<Company>().GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            predicate: c => (companyIds == null || companyIds.Contains(c.Id)) &&
                (string.IsNullOrWhiteSpace(request.SearchTerm) || c.Name.Contains(request.SearchTerm)),
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
