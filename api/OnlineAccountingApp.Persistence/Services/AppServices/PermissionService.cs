using Microsoft.EntityFrameworkCore;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Persistence.Context;

namespace OnlineAccountingApp.Persistence.Services.AppServices;

/// <summary>
/// Walks MainRoleAndUserRelationship (user+company -> MainRole) joined with
/// MainRoleAndRoleRelationship (MainRole -> AppRole) to resolve the permission codes a user
/// effectively has in a given company. See <see cref="IPermissionService"/> for why.
/// </summary>
public sealed class PermissionService(AppDbContext context) : IPermissionService
{
    public async Task<bool> HasPermissionAsync(string userId, string companyId, string permissionCode, CancellationToken cancellationToken = default)
    {
        return await PermissionCodesQuery(userId, companyId)
            .AnyAsync(code => code == permissionCode, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetPermissionCodesAsync(string userId, string companyId, CancellationToken cancellationToken = default)
    {
        return await PermissionCodesQuery(userId, companyId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private IQueryable<string> PermissionCodesQuery(string userId, string companyId)
    {
        IQueryable<string> mainRoleIds = context.MainRoleAndUserRelationships
            .Where(x => !x.Deleted && x.UserId == userId && x.CompanyId == companyId)
            .Select(x => x.MainRoleId);

        return context.MainRoleAndRoleRelationships
            .Where(x => !x.Deleted && mainRoleIds.Contains(x.MainRoleId))
            .Join(context.Roles.Where(role => !role.Deleted), x => x.RoleId, role => role.Id, (x, role) => role.Code);
    }
}
