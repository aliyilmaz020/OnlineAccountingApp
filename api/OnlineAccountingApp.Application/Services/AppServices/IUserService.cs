using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Services.AppServices;

/// <summary>
/// Read-only user lookup over ASP.NET Identity. <see cref="AppUser"/> derives from
/// <c>IdentityUser&lt;string&gt;</c> rather than <c>BaseEntity</c>, so it cannot be served by
/// <see cref="IRepository{T}"/>; this contract is backed by <c>UserManager&lt;AppUser&gt;</c> instead.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Lists users. When <paramref name="companyId"/> is given, results are restricted to users
    /// who belong to that company (via UserCompany) - used for a non-admin caller who may only
    /// see their own company's personnel. Pass null (system admins only) to list every user.
    /// </summary>
    Task<PagedResult<AppUser>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? companyId = null,
        CancellationToken cancellationToken = default);

    Task<AppUser> GetByIdAsync(string userId, CancellationToken cancellationToken = default);

    Task<AppUser> UpdateProfileAsync(
        string userId,
        string? userName,
        string? email,
        string? phoneNumber,
        string? firstName,
        string? lastName,
        CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}
