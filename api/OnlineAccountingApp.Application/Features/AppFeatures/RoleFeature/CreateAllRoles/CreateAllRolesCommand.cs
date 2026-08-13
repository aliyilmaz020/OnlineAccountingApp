using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.CreateAllRoles;

public sealed class CreateAllRolesCommand : IRequest<List<RoleListItemDto>>
{
}
