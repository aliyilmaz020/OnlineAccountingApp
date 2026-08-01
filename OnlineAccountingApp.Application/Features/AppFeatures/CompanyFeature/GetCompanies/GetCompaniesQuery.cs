using OnlineAccountingApp.Framework.MedatR.GetList;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;

public sealed class GetCompaniesQuery : BaseGetListQuery<CompanyListItemDto>
{
    public string? SearchTerm { get; set; }
}
