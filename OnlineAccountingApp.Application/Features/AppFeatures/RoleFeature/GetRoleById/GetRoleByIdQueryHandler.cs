using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoleById;

public sealed class GetRoleByIdQueryHandler(IRoleService roleService) : IRequestHandler<GetRoleByIdQuery, RoleListItemDto>
{
    public async Task<RoleListItemDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        AppRole? role = await roleService.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
        {
            throw new BusinessException(AppErrorCodes.Role.NotFound, "Role not found.");
        }

        return role.Adapt<RoleListItemDto>();
    }
}
