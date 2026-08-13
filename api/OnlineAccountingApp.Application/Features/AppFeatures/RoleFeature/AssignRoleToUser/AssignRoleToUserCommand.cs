using MediatR;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.AssignRoleToUser;

public sealed class AssignRoleToUserCommand : IRequest<bool>
{
    public string UserId { get; set; }
    public string RoleCode { get; set; }
}
