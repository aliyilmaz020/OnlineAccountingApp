using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Delete;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Delete;

public sealed class DeleteUniformChartOfAccountCommandHandler(ICompanyUnitOfWork unitOfWork)
    : BaseDeleteCommandHandler<DeleteUniformChartOfAccountCommand, UniformChartOfAccount>(unitOfWork)
{
    protected override async Task<UniformChartOfAccount> GetExistingEntityAsync(DeleteUniformChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        UniformChartOfAccount? entity = await Repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null || entity.Deleted)
        {
            throw new BusinessException(GetNotFoundErrorCode(), GetNotFoundErrorMessage());
        }

        return entity;
    }

    protected override string GetNotFoundErrorCode() => AppErrorCodes.UniformChartOfAccount.NotFound;

    protected override string GetNotFoundErrorMessage() => "Uniform chart of account not found.";
}
