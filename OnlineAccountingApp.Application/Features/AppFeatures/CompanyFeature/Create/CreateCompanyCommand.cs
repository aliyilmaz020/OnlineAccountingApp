using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Framework.MedatR.Create;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;

public sealed class CreateCompanyCommand : BaseCreateCommand<CompanyListItemDto>
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string IdentityNumber { get; set; }
    public string TaxDepartment { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string ServerName { get; set; }
    public string DatabaseName { get; set; }
    public string ServerUserId { get; set; }
    public string ServerPassword { get; set; }
}
