using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Framework.MedatR.Update;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;

public sealed class UpdateUniformChartOfAccountCommand : BaseUpdateCommand<UniformChartOfAccountListItemDto>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
}
