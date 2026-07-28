using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;

public sealed class GetRolesQueryHandler(IRoleService roleService) : IRequestHandler<GetRolesQuery, PagedResult<RoleListItemDto>>
{
    public async Task<PagedResult<RoleListItemDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        PagedResult<AppRole> paged = await roleService.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            cancellationToken);

        return new PagedResult<RoleListItemDto>
        {
            Items = paged.Items.Adapt<List<RoleListItemDto>>(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }
}
