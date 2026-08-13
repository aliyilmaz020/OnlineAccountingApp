namespace OnlineAccountingApp.WebApi.Tenancy;

/// <summary>
/// Marks a controller or action whose work targets the current company's own database,
/// so Swagger documents the required <c>X-Company-Id</c> header.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequiresCompanyHeaderAttribute : Attribute
{
}
