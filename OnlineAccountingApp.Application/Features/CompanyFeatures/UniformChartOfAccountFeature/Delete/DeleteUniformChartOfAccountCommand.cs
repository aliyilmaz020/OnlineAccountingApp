using MediatR;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Delete;

public sealed class DeleteUniformChartOfAccountCommand : IRequest<bool>
{
    public string Id { get; set; }
}
