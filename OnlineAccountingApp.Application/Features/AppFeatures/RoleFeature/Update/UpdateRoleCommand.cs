using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Update;

public sealed class UpdateRoleCommand : IRequest<RoleListItemDto>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public bool Status { get; set; }
}
