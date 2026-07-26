using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Update;

public sealed class UpdateCompanyCommandHandler(ICompanyService companyService, IUnitOfWork unitOfWork) : IRequestHandler<UpdateCompanyCommand, CompanyListItemDto>
{
    public async Task<CompanyListItemDto> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        Company? existingCompany = await companyService.GetByIdAsync(request.Id, cancellationToken);
        if (existingCompany is null)
        {
            throw new BusinessException(AppErrorCodes.Company.NotFound, "Company not found.");
        }

        Company? companyWithSameName = await companyService.GetCompanyByNameAsync(request.Name);
        if (companyWithSameName is not null && companyWithSameName.Id != request.Id)
        {
            throw new BusinessException(AppErrorCodes.Company.AlreadyExists, "A company with the same name already exists.");
        }

        request.Adapt(existingCompany);

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        Company updatedCompany = await companyService.UpdateAsync(existingCompany, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return updatedCompany.Adapt<CompanyListItemDto>();
    }
}
