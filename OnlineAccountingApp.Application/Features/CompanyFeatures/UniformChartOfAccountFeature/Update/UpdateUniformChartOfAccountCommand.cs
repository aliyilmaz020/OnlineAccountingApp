using MediatR;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;

public sealed class UpdateUniformChartOfAccountCommand : IRequest<UniformChartOfAccountListItemDto>
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
}
