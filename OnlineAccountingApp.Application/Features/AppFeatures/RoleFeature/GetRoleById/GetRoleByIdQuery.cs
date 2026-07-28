using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoleById;

public sealed class GetRoleByIdQuery : IRequest<RoleListItemDto>
{
    public string Id { get; set; }
}
