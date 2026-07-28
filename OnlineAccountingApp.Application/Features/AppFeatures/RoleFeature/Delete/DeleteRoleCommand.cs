using MediatR;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Delete;

public sealed class DeleteRoleCommand : IRequest<bool>
{
    public string Id { get; set; }
}
