using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;
using OnlineAccountingApp.Framework.MedatR.Create;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Create;

public sealed class CreateMainRoleCommand : BaseCreateCommand<MainRoleListItemDto>
{
    public string Title { get; set; }
    public bool IsRoleCreateByAdmin { get; set; }
    public string CompanyId { get; set; }
}
