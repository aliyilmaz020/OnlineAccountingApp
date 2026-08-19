using MediatR;

namespace OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetMyProfile;

/// <summary>Returns the currently authenticated user's own profile. No parameters: the user comes from ICompanyContext.</summary>
public sealed class GetMyProfileQuery : IRequest<MyProfileDto>
{
}
