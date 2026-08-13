using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationships;
using OnlineAccountingApp.Framework.MedatR.Update;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Update;

public sealed class UpdateMainRoleAndUserRelationshipCommand : BaseUpdateCommand<MainRoleAndUserRelationshipListItemDto>
{
    public string UserId { get; set; }
    public string MainRoleId { get; set; }
    public string CompanyId { get; set; }
}
