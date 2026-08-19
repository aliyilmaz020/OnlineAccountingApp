using Microsoft.AspNetCore.Authorization;

namespace OnlineAccountingApp.Grpc.Authorization;

/// <summary>Local copy of WebApi's PermissionRequirement - Grpc does not reference WebApi.</summary>
public sealed class PermissionRequirement(string permissionCode) : IAuthorizationRequirement
{
    public string PermissionCode { get; } = permissionCode;
}
