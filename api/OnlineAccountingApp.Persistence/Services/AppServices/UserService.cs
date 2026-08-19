using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Framework.Services;
using OnlineAccountingApp.Persistence.Context;
using DomainValidationException = OnlineAccountingApp.Domain.Exceptions.ValidationException;

namespace OnlineAccountingApp.Persistence.Services.AppServices;

/// <summary>
/// Unlike the other app services this one does not extend <c>Repository&lt;T, AppDbContext&gt;</c>:
/// <see cref="AppUser"/> is not a <c>BaseEntity</c>. Going through <see cref="UserManager{TUser}"/>
/// keeps the query consistent with how <see cref="RoleService"/> reads <c>AppRole</c>. The company
/// filter in <see cref="GetPagedAsync"/> still needs direct <see cref="AppDbContext"/> access to
/// join against UserCompanies, which UserManager doesn't expose.
/// </summary>
public sealed class UserService(UserManager<AppUser> userManager, AppDbContext context) : IUserService
{
    public async Task<PagedResult<AppUser>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? companyId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AppUser> query = ActiveUsers();

        if (!string.IsNullOrWhiteSpace(companyId))
        {
            IQueryable<string> memberUserIds = context.UserCompanies
                .Where(uc => !uc.Deleted && uc.CompanyId == companyId)
                .Select(uc => uc.AppUserId);
            query = query.Where(user => memberUserIds.Contains(user.Id));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(user =>
                (user.UserName != null && user.UserName.Contains(searchTerm)) ||
                (user.Email != null && user.Email.Contains(searchTerm)));
        }

        int totalCount = await query.CountAsync(cancellationToken);
        List<AppUser> items = await query
            .OrderBy(user => user.UserName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AppUser>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<AppUser> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await RequireUserAsync(userId);
    }

    public async Task<AppUser> UpdateProfileAsync(
        string userId, string? userName, string? email, string? phoneNumber, string? firstName, string? lastName,
        CancellationToken cancellationToken = default)
    {
        AppUser user = await RequireUserAsync(userId);

        if (!string.IsNullOrWhiteSpace(userName) && userName != user.UserName)
        {
            ThrowIfFailed(await userManager.SetUserNameAsync(user, userName));
        }

        if (!string.IsNullOrWhiteSpace(email) && email != user.Email)
        {
            ThrowIfFailed(await userManager.SetEmailAsync(user, email));
        }

        user.PhoneNumber = phoneNumber;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.EditDate = DateTime.UtcNow;
        ThrowIfFailed(await userManager.UpdateAsync(user));

        return user;
    }

    public async Task ChangePasswordAsync(
        string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        AppUser user = await RequireUserAsync(userId);
        ThrowIfFailed(await userManager.ChangePasswordAsync(user, currentPassword, newPassword));
    }

    /// <summary>Soft-deleted users are invisible to every read path.</summary>
    private IQueryable<AppUser> ActiveUsers()
    {
        return userManager.Users.AsNoTracking().Where(user => !user.Deleted);
    }

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
