using MediatR;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Delete;

public sealed class DeleteCompanyCommand : IRequest<bool>
{
    public string Id { get; set; }
}
