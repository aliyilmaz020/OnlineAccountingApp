using OnlineAccountingApp.Framework.MedatR.GetList;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;

public sealed class GetUniformChartOfAccountsQuery : BaseGetListQuery<UniformChartOfAccountListItemDto>
{
    public string? SearchTerm { get; set; }
}
