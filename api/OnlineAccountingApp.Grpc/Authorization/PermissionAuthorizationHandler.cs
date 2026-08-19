using Microsoft.AspNetCore.Authorization;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;

namespace OnlineAccountingApp.Grpc.Authorization;

/// <summary>Local copy of WebApi's PermissionAuthorizationHandler - Grpc does not reference WebApi.</summary>
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
