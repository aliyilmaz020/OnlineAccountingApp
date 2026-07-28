using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Delete;

public sealed class DeleteRoleCommandHandler(IRoleService roleService) : IRequestHandler<DeleteRoleCommand, bool>
{
    public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        AppRole? existingRole = await roleService.GetByIdAsync(request.Id, cancellationToken);
        if (existingRole is null)
        {
            throw new BusinessException(AppErrorCodes.Role.NotFound, "Role not found.");
        }

        return await roleService.SoftDeleteAsync(existingRole, cancellationToken);
    }
}
