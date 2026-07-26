using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;

public sealed class UpdateUniformChartOfAccountCommandHandler(IUniformChartOfAccountService uniformChartOfAccountService, ICompanyUnitOfWork unitOfWork) : IRequestHandler<UpdateUniformChartOfAccountCommand, UniformChartOfAccountListItemDto>
{
    public async Task<UniformChartOfAccountListItemDto> Handle(UpdateUniformChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        UniformChartOfAccount? existingAccount = await uniformChartOfAccountService.GetByIdAsync(request.Id, cancellationToken);
        if (existingAccount is null || existingAccount.Deleted)
        {
            throw new BusinessException(AppErrorCodes.UniformChartOfAccount.NotFound, "Uniform chart of account not found.");
        }

        UniformChartOfAccount? accountWithSameCode = await uniformChartOfAccountService.GetByCodeAsync(request.Code, cancellationToken);
        if (accountWithSameCode is not null && accountWithSameCode.Id != request.Id)
        {
            throw new BusinessException(AppErrorCodes.UniformChartOfAccount.AlreadyExists, "A uniform chart of account with the same code already exists.");
        }

        request.Adapt(existingAccount);
        existingAccount.EditDate = DateTime.UtcNow;

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        UniformChartOfAccount updatedAccount = await uniformChartOfAccountService.UpdateAsync(existingAccount, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return updatedAccount.Adapt<UniformChartOfAccountListItemDto>();
    }
}
