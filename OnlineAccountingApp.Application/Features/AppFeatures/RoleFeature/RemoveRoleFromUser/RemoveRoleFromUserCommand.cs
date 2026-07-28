using MediatR;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.RemoveRoleFromUser;

public sealed class RemoveRoleFromUserCommand : IRequest<bool>
{
    public string UserId { get; set; }
    public string RoleName { get; set; }
}
