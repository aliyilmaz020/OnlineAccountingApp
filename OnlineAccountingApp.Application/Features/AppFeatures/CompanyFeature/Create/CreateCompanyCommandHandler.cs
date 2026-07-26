using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;

public sealed class CreateCompanyCommandHandler(ICompanyService companyService, IUnitOfWork unitOfWork) : IRequestHandler<CreateCompanyCommand, CompanyListItemDto>
{
    public async Task<CompanyListItemDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        Company? existingCompany = await companyService.GetCompanyByNameAsync(request.Name);
        if (existingCompany is not null)
        {
            throw new BusinessException(AppErrorCodes.Company.AlreadyExists, "A company with the same name already exists.");
        }
        var company = request.Adapt<Company>();

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        Company createdCompany = await companyService.CreateAsync(company, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return createdCompany.Adapt<CompanyListItemDto>();
    }
}
