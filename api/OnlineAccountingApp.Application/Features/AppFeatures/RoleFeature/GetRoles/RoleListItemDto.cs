namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;

public sealed class RoleListItemDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public bool Status { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? EditDate { get; set; }
}
