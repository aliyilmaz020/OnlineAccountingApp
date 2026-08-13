namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;

public sealed class MainRoleListItemDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public bool IsRoleCreateByAdmin { get; set; }
    public string CompanyId { get; set; }
}
