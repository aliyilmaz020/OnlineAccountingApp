using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanyById;

public sealed class GetCompanyByIdQuery : IRequest<CompanyListItemDto>
{
    public string Id { get; set; }
}
