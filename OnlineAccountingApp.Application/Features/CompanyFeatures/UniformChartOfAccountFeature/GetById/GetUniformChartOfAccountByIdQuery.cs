using MediatR;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetById;

public sealed class GetUniformChartOfAccountByIdQuery : IRequest<UniformChartOfAccountListItemDto>
{
    public string Id { get; set; }
}
