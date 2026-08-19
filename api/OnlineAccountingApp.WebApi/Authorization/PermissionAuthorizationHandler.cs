using Microsoft.AspNetCore.Authorization;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;

namespace OnlineAccountingApp.WebApi.Authorization;

/// <summary>
/// Backs [Authorize(Policy = RoleList.UCAF*)]: resolves the caller's permission codes for the
/// company in the current request's X-Company-Id header via the MainRole chain (see
/// IPermissionService), since permission roles are not carried in the JWT's role claims.
/// </summary>
/// <remarks>
/// If the company header or the authenticated user id is missing, the requirement is simply
/// left unsatisfied (fails closed) rather than throwing - the caller gets the same 403 as a
/// genuine permission denial, though the response won't call out the missing header the way
/// AddCompanyTenancy's BusinessException does further down the pipeline.
/// </remarks>
public sealed class PermissionAuthorizationHandler(IPermissionService permissionService, ICompanyContext companyContext)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (string.IsNullOrWhiteSpace(companyContext.CompanyId) || string.IsNullOrWhiteSpace(companyContext.UserId))
        {
            return;
        }

        bool hasPermission = await permissionService.HasPermissionAsync(
            companyContext.UserId, companyContext.CompanyId, requirement.PermissionCode);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
