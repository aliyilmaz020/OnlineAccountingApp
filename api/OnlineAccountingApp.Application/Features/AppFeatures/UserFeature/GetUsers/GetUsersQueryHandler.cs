using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Domain.Roles;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetUsers;

public sealed class GetUsersQueryHandler(IUserService userService, ICompanyContext companyContext)
    : IRequestHandler<GetUsersQuery, PagedResult<UserListItemDto>>
{
    public async Task<PagedResult<UserListItemDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        bool isAdmin = companyContext.IsInRole(RoleList.SystemAdmin);

        // A system admin sees every user; anyone else only sees their own company's personnel,
        // so a company must actually be selected first.
        string? companyId = null;
        if (!isAdmin)
        {
            if (string.IsNullOrWhiteSpace(companyContext.CompanyId))
            {
                throw new BusinessException(
                    AppErrorCodes.Tenant.CompanyNotSpecified,
                    $"The '{ICompanyContext.HeaderName}' header is required for this operation.");
            }

            companyId = companyContext.CompanyId;
        }

        PagedResult<AppUser> paged = await userService.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            companyId,
            cancellationToken);

        return new PagedResult<UserListItemDto>
        {
            Items = paged.Items.Adapt<List<UserListItemDto>>(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }
}
