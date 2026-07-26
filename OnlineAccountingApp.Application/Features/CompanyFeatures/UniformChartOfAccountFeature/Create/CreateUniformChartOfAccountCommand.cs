using MediatR;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;

public sealed class CreateUniformChartOfAccountCommand : IRequest<UniformChartOfAccountListItemDto>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
}
