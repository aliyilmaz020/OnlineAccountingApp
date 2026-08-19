using MediatR;

namespace OnlineAccountingApp.Application.Features.AppFeatures.PermissionFeature.GetMyPermissions;

/// <summary>
/// Returns the current user's permission codes (e.g. "UCAF.Read") for the company identified
/// by the request's X-Company-Id header. No parameters: user and company come from ICompanyContext.
/// </summary>
public sealed class GetMyPermissionsQuery : IRequest<List<string>>
{
}
