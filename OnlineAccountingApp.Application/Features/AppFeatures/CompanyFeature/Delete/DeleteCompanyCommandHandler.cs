using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Delete;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Delete;

public sealed class DeleteCompanyCommandHandler(IUnitOfWork unitOfWork)
    : BaseDeleteCommandHandler<DeleteCompanyCommand, Company>(unitOfWork)
{
    protected override string GetNotFoundErrorCode() => AppErrorCodes.Company.NotFound;

    protected override string GetNotFoundErrorMessage() => "Company not found.";
}
