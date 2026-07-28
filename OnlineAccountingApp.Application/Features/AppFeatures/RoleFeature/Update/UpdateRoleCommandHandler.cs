using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Update;

public sealed class UpdateRoleCommandHandler(IRoleService roleService) : IRequestHandler<UpdateRoleCommand, RoleListItemDto>
{
    public async Task<RoleListItemDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        AppRole? existingRole = await roleService.GetByIdAsync(request.Id, cancellationToken);
        if (existingRole is null)
        {
            throw new BusinessException(AppErrorCodes.Role.NotFound, "Role not found.");
        }

        AppRole? roleWithSameName = await roleService.GetByNameAsync(request.Name, cancellationToken);
        if (roleWithSameName is not null && roleWithSameName.Id != request.Id)
        {
            throw new BusinessException(AppErrorCodes.Role.AlreadyExists, "A role with the same name already exists.");
        }

        AppRole? roleWithSameCode = await roleService.GetByCodeAsync(request.Code, cancellationToken);
        if (roleWithSameCode is not null && roleWithSameCode.Id != request.Id)
        {
            throw new BusinessException(AppErrorCodes.Role.AlreadyExists, "A role with the same code already exists.");
        }

        request.Adapt(existingRole);
        existingRole.EditDate = DateTime.UtcNow;

        AppRole updatedRole = await roleService.UpdateAsync(existingRole, cancellationToken);

        return updatedRole.Adapt<RoleListItemDto>();
    }
}
