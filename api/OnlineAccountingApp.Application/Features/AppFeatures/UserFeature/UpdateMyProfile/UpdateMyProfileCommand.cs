using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetMyProfile;

namespace OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.UpdateMyProfile;

public sealed class UpdateMyProfileCommand : IRequest<MyProfileDto>
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
