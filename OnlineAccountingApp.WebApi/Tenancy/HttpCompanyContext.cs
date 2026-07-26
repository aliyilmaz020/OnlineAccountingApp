using OnlineAccountingApp.Application.Services.CompanyServices;
using System.Security.Claims;

namespace OnlineAccountingApp.WebApi.Tenancy;

/// <summary>
/// Resolves the current company from the <c>X-Company-Id</c> request header, and the current
/// user from the bearer token, so company membership can be verified.
/// </summary>
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
}
