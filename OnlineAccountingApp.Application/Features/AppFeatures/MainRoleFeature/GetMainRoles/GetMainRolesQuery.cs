using OnlineAccountingApp.Framework.MedatR.GetList;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;

public sealed class GetMainRolesQuery : BaseGetListQuery<MainRoleListItemDto>
{
    public string? SearchTerm { get; set; }
}
