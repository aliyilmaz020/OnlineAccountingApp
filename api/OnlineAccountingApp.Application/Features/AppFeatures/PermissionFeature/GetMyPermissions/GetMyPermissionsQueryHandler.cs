using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.PermissionFeature.GetMyPermissions;

public sealed class GetMyPermissionsQueryHandler(IPermissionService permissionService, ICompanyContext companyContext)
    : IRequestHandler<GetMyPermissionsQuery, List<string>>
{
    public async Task<List<string>> Handle(GetMyPermissionsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(companyContext.CompanyId))
        {
            throw new BusinessException(
                AppErrorCodes.Tenant.CompanyNotSpecified,
                $"The '{ICompanyContext.HeaderName}' header is required for this operation.");
        }

        if (string.IsNullOrWhiteSpace(companyContext.UserId))
        {
            throw new BusinessException(AppErrorCodes.Auth.InvalidCredentials, "Authentication is required to access this resource.");
        }

        IReadOnlyList<string> codes = await permissionService.GetPermissionCodesAsync(
            companyContext.UserId, companyContext.CompanyId, cancellationToken);

        return codes.ToList();
    }
}
