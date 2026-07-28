using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.AssignRoleToUser;

public sealed class AssignRoleToUserCommandHandler(IRoleService roleService) : IRequestHandler<AssignRoleToUserCommand, bool>
{
    public async Task<bool> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        AppRole? role = await roleService.GetByCodeAsync(request.RoleCode, cancellationToken);
        if (role is null)
        {
            throw new BusinessException(AppErrorCodes.Role.NotFound, "Role not found.");
        }

        if (!await roleService.UserExistsAsync(request.UserId, cancellationToken))
        {
            throw new BusinessException(AppErrorCodes.Role.NotFound, "User not found.");
        }

        return await roleService.AssignRoleToUserAsync(request.UserId, role.Name!, cancellationToken);
    }
}
