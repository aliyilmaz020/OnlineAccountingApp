using Microsoft.AspNetCore.Authorization;

namespace OnlineAccountingApp.WebApi.Authorization;

public sealed class PermissionRequirement(string permissionCode) : IAuthorizationRequirement
{
    public string PermissionCode { get; } = permissionCode;
}
