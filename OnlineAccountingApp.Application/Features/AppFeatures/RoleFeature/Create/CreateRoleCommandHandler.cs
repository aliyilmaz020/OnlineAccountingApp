using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Create;

public sealed class CreateRoleCommandHandler(IRoleService roleService) : IRequestHandler<CreateRoleCommand, RoleListItemDto>
{
    public async Task<RoleListItemDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        AppRole? roleWithSameName = await roleService.GetByNameAsync(request.Name, cancellationToken);
        if (roleWithSameName is not null)
        {
            throw new BusinessException(AppErrorCodes.Role.AlreadyExists, "A role with the same name already exists.");
        }

        AppRole? roleWithSameCode = await roleService.GetByCodeAsync(request.Code, cancellationToken);
        if (roleWithSameCode is not null)
        {
            throw new BusinessException(AppErrorCodes.Role.AlreadyExists, "A role with the same code already exists.");
        }

        var role = request.Adapt<AppRole>();
        // AppRole is not a BaseEntity, so AppDbContext.SaveChangesAsync does not stamp these,
        // and IdentityRole<string> does not generate an Id of its own.
        role.Id = Guid.NewGuid().ToString();
        role.CreateDate = DateTime.UtcNow;
        role.Status = true;
        role.Deleted = false;

        AppRole createdRole = await roleService.CreateAsync(role, cancellationToken);

        return createdRole.Adapt<RoleListItemDto>();
    }
}
