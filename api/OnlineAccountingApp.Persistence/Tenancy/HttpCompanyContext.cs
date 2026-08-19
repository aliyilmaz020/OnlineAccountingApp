using Microsoft.AspNetCore.Http;
using OnlineAccountingApp.Application.Services.CompanyServices;
using System.Security.Claims;

namespace OnlineAccountingApp.Persistence.Tenancy;

/// <summary>
/// Resolves the current company from the <c>X-Company-Id</c> request header, and the current
/// user from the bearer token, so company membership can be verified.
/// </summary>
/// <remarks>
/// Shared by every ASP.NET Core host (WebApi, Grpc): Grpc.AspNetCore surfaces incoming gRPC
/// metadata (e.g. <c>x-company-id</c>) on <see cref="HttpContext.Request"/>.Headers just like a
/// regular HTTP header, so this implementation works unmodified for gRPC services too.
/// </remarks>
public sealed class HttpCompanyContext(IHttpContextAccessor httpContextAccessor) : ICompanyContext
{
    public string? CompanyId
    {
        get
        {
            string? companyId = httpContextAccessor.HttpContext?.Request.Headers[ICompanyContext.HeaderName];
            return string.IsNullOrWhiteSpace(companyId) ? null : companyId;
        }
    }

    public string? UserId
    {
        get
        {
            string? userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrWhiteSpace(userId) ? null : userId;
        }
    }

    public bool IsInRole(string roleName) => httpContextAccessor.HttpContext?.User.IsInRole(roleName) ?? false;
}
