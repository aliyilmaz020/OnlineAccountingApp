using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetUserRoles;

public sealed class GetUserRolesQuery : IRequest<List<RoleListItemDto>>
{
    public string UserId { get; set; }
}
