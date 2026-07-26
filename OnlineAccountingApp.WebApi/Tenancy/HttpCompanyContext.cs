using OnlineAccountingApp.Application.Services.CompanyServices;

namespace OnlineAccountingApp.WebApi.Tenancy;

/// <summary>
/// Resolves the current company from the <c>X-Company-Id</c> request header.
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
}
