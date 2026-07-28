using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using DomainValidationException = OnlineAccountingApp.Domain.Exceptions.ValidationException;

namespace OnlineAccountingApp.Persistence.Services.AppServices;

/// <summary>
/// Unlike the other app services this one does not extend <c>Repository&lt;T, AppDbContext&gt;</c>:
/// <see cref="AppRole"/> is not a <c>BaseEntity</c>. Going through <see cref="RoleManager{TRole}"/>
/// also keeps <c>NormalizedName</c> maintained, which a raw DbSet write would silently skip.
/// </summary>
public sealed class RoleService(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager) : IRoleService
{
    /// <summary>
    /// Deliberately tracked. Update/Delete hand the returned instance straight back to
    /// <see cref="RoleManager{TRole}.UpdateAsync"/>, whose validator re-loads the same row via
    /// <c>FindByNameAsync</c>; if this instance were detached, the subsequent <c>Attach</c> would
    /// collide with that tracked copy and throw.
    /// </summary>
    public async Task<AppRole?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await roleManager.Roles
            .Where(role => !role.Deleted)
            .FirstOrDefaultAsync(role => role.Id == id, cancellationToken);
    }

    public async Task<AppRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        string normalizedName = Normalize(name);
        return await ActiveRoles().FirstOrDefaultAsync(role => role.NormalizedName == normalizedName, cancellationToken);
    }

    public async Task<AppRole?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await ActiveRoles().FirstOrDefaultAsync(role => role.Code == code, cancellationToken);
    }

    public async Task<PagedResult<AppRole>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AppRole> query = ActiveRoles();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(role => role.Name!.Contains(searchTerm) || role.Code.Contains(searchTerm));
        }

        int totalCount = await query.CountAsync(cancellationToken);
        List<AppRole> items = await query
            .OrderBy(role => role.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AppRole>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<AppRole> CreateAsync(AppRole role, CancellationToken cancellationToken = default)
    {
        ThrowIfFailed(await roleManager.CreateAsync(role));
        return role;
    }

    public async Task<AppRole> UpdateAsync(AppRole role, CancellationToken cancellationToken = default)
    {
        ThrowIfFailed(await roleManager.UpdateAsync(role));
        return role;
    }

    public async Task<bool> SoftDeleteAsync(AppRole role, CancellationToken cancellationToken = default)
    {
        role.Deleted = true;
        role.Status = false;
        role.EditDate = DateTime.UtcNow;

        ThrowIfFailed(await roleManager.UpdateAsync(role));
        return true;
    }

    public async Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await userManager.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId && !user.Deleted, cancellationToken);
    }

    public async Task<bool> AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        AppUser user = await RequireUserAsync(userId);
        if (await userManager.IsInRoleAsync(user, roleName))
        {
            return true;
        }

        ThrowIfFailed(await userManager.AddToRoleAsync(user, roleName));
        return true;
    }

    public async Task<bool> RemoveRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        AppUser user = await RequireUserAsync(userId);
        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            return false;
        }

        ThrowIfFailed(await userManager.RemoveFromRoleAsync(user, roleName));
        return true;
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        AppUser user = await RequireUserAsync(userId);
        return await userManager.GetRolesAsync(user);
    }

    /// <summary>Soft-deleted roles are invisible to every read path.</summary>
    private IQueryable<AppRole> ActiveRoles()
    {
        return roleManager.Roles.AsNoTracking().Where(role => !role.Deleted);
    }

    /// <summary>
    /// The handlers validate the user up front; this only guards against a row disappearing
    /// between that check and the write.
    /// </summary>
    private async Task<AppUser> RequireUserAsync(string userId)
    {
        AppUser? user = await userManager.FindByIdAsync(userId);
        if (user is null || user.Deleted)
        {
            throw new DomainValidationException(new Dictionary<string, string[]>
            {
                ["UserId"] = ["User not found."]
            });
        }

        return user;
    }

    private string Normalize(string value)
    {
        return roleManager.KeyNormalizer is null ? value.ToUpperInvariant() : roleManager.KeyNormalizer.NormalizeName(value);
    }

    /// <summary>Surfaces Identity's own failures through the app's validation error shape.</summary>
    private static void ThrowIfFailed(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = result.Errors
            .GroupBy(error => error.Code)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

        throw new DomainValidationException(errors);
    }
}
