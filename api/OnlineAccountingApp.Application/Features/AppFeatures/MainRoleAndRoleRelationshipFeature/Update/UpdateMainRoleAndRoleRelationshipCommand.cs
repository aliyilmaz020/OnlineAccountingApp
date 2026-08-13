using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationships;
using OnlineAccountingApp.Framework.MedatR.Update;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Update;

public sealed class UpdateMainRoleAndRoleRelationshipCommand : BaseUpdateCommand<MainRoleAndRoleRelationshipListItemDto>
{
    public string RoleId { get; set; }
    public string MainRoleId { get; set; }
}
