namespace OnlineAccountingApp.Application.Services.AppServices;

/// <summary>
/// Resolves a user's effective permission codes for a given company by walking the
/// MainRoleAndUserRelationship -> MainRoleAndRoleRelationship -> AppRole chain, since
/// permission roles are assigned per company via MainRole rather than through Identity's
/// own (company-agnostic) user-role assignment.
/// </summary>
public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string userId, string companyId, string permissionCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetPermissionCodesAsync(string userId, string companyId, CancellationToken cancellationToken = default);
}
