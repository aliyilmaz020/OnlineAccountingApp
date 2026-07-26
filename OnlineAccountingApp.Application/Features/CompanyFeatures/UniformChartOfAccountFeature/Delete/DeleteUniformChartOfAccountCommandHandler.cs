using MediatR;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Delete;

public sealed class DeleteUniformChartOfAccountCommandHandler(IUniformChartOfAccountService uniformChartOfAccountService, ICompanyUnitOfWork unitOfWork) : IRequestHandler<DeleteUniformChartOfAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteUniformChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        UniformChartOfAccount? existingAccount = await uniformChartOfAccountService.GetByIdAsync(request.Id, cancellationToken);
        if (existingAccount is null || existingAccount.Deleted)
        {
            throw new BusinessException(AppErrorCodes.UniformChartOfAccount.NotFound, "Uniform chart of account not found.");
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        bool result = await uniformChartOfAccountService.SoftDeleteAsync(existingAccount, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return result;
    }
}
