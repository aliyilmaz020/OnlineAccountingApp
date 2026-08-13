using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationships;
using OnlineAccountingApp.Framework.MedatR.Create;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Create;

public sealed class CreateMainRoleAndUserRelationshipCommand : BaseCreateCommand<MainRoleAndUserRelationshipListItemDto>
{
    public string UserId { get; set; }
    public string MainRoleId { get; set; }
    public string CompanyId { get; set; }
}
