using MediatR;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Delete;

public sealed class DeleteCompanyCommandHandler(ICompanyService companyService, IUnitOfWork unitOfWork) : IRequestHandler<DeleteCompanyCommand, bool>
{
    public async Task<bool> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        Company? existingCompany = await companyService.GetByIdAsync(request.Id, cancellationToken);
        if (existingCompany is null)
        {
            throw new BusinessException(AppErrorCodes.Company.NotFound, "Company not found.");
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        bool result = await companyService.SoftDeleteAsync(existingCompany, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return result;
    }
}
