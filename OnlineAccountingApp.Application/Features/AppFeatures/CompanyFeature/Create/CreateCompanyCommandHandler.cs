using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;

public sealed class CreateCompanyCommandHandler(ICompanyService companyService) : IRequestHandler<CreateCompanyCommand, bool>
{
    public async Task<bool> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        Company? existingCompany = await companyService.GetCompanyByNameAsync(request.Name);
        if (existingCompany is not null)
        {
            throw new Exception("A company with the same name already exists.");
        }
        var company = request.Adapt<Company>();
        return await companyService.CreateAsync(company);
    }
}
