using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Framework.MedatR.Create;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;

public sealed class CreateUniformChartOfAccountCommand : BaseCreateCommand<UniformChartOfAccountListItemDto>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
}
