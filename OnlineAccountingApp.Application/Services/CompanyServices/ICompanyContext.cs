namespace OnlineAccountingApp.Application.Services.CompanyServices;

/// <summary>
/// Identifies the company whose own database the current request operates against.
/// </summary>
public interface ICompanyContext
{
    /// <summary>
    /// Name of the request header carrying the company identifier.
    /// </summary>
    const string HeaderName = "X-Company-Id";

    string? CompanyId { get; }

    /// <summary>
    /// Identifier of the authenticated user making the request, used to verify that the
    /// user actually belongs to the requested company.
    /// </summary>
    string? UserId { get; }
}
