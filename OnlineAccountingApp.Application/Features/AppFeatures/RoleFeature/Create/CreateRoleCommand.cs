using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Create;

public sealed class CreateRoleCommand : IRequest<RoleListItemDto>
{
    public string Name { get; set; }
    public string Code { get; set; }
}
