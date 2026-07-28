using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetUserRoles;

public sealed class GetUserRolesQueryHandler(IRoleService roleService) : IRequestHandler<GetUserRolesQuery, List<RoleListItemDto>>
{
    public async Task<List<RoleListItemDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        if (!await roleService.UserExistsAsync(request.UserId, cancellationToken))
        {
            throw new BusinessException(AppErrorCodes.Role.NotFound, "User not found.");
        }

        IList<AppRole> roles = await roleService.GetRolesByUserIdAsync(request.UserId, cancellationToken);
        return roles.Adapt<List<RoleListItemDto>>();
    }
}
