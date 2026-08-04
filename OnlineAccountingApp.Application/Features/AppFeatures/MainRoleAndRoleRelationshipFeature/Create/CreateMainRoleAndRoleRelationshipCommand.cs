using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationships;
using OnlineAccountingApp.Framework.MedatR.Create;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Create;

public sealed class CreateMainRoleAndRoleRelationshipCommand : BaseCreateCommand<MainRoleAndRoleRelationshipListItemDto>
{
    public string RoleId { get; set; }
    public string MainRoleId { get; set; }
}
