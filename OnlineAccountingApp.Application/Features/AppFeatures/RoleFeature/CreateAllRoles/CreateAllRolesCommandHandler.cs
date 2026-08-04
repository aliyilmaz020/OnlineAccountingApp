using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using RoleList = OnlineAccountingApp.Domain.Roles.RoleList;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.CreateAllRoles;

public sealed class CreateAllRolesCommandHandler(IRoleService roleService)
    : IRequestHandler<CreateAllRolesCommand, List<RoleListItemDto>>
{
    public async Task<List<RoleListItemDto>> Handle(CreateAllRolesCommand request, CancellationToken cancellationToken)
    {
        var createdRoles = new List<AppRole>();

        foreach (AppRole role in RoleList.GetStaticRoles())
        {
            AppRole? existingRole = await roleService.GetByCodeAsync(role.Code, cancellationToken);
            if (existingRole is not null)
            {
                continue;
            }

            role.Id = Guid.NewGuid().ToString();
            role.CreateDate = DateTime.UtcNow;
            role.Status = true;
            role.Deleted = false;

            createdRoles.Add(await roleService.CreateAsync(role, cancellationToken));
        }

        return createdRoles.Adapt<List<RoleListItemDto>>();
    }
}
