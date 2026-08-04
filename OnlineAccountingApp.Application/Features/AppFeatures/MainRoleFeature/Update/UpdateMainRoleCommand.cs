using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;
using OnlineAccountingApp.Framework.MedatR.Update;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Update;

public sealed class UpdateMainRoleCommand : BaseUpdateCommand<MainRoleListItemDto>
{
    public string Title { get; set; }
    public bool IsRoleCreateByAdmin { get; set; }
    public string CompanyId { get; set; }
}
