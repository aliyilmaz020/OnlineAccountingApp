namespace OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetUsers;

public sealed class UserListItemDto
{
    public string Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool Status { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? EditDate { get; set; }
}
