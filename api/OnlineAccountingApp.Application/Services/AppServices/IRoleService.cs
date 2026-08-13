using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Services.AppServices;

/// <summary>
/// Role management over ASP.NET Identity. <see cref="AppRole"/> derives from
/// <c>IdentityRole&lt;string&gt;</c> rather than <c>BaseEntity</c>, so it cannot be served by
/// <see cref="IRepository{T}"/>; this contract is backed by <c>RoleManager&lt;AppRole&gt;</c> instead.
/// </summary>
public interface IRoleService
{
    Task<AppRole?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<AppRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<AppRole?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<PagedResult<AppRole>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<AppRole> CreateAsync(AppRole role, CancellationToken cancellationToken = default);
    Task<AppRole> UpdateAsync(AppRole role, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(AppRole role, CancellationToken cancellationToken = default);

    Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
    Task<bool> RemoveRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
    Task<IList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    Task<IList<AppRole>> GetRolesByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
