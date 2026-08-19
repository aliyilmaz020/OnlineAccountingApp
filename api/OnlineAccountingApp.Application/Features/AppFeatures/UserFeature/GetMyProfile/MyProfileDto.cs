namespace OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetMyProfile;

public sealed class MyProfileDto
{
    public string Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsAdmin { get; set; }
}
