using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;

public sealed class CreateUniformChartOfAccountCommandHandler(IUniformChartOfAccountService uniformChartOfAccountService, ICompanyUnitOfWork unitOfWork) : IRequestHandler<CreateUniformChartOfAccountCommand, UniformChartOfAccountListItemDto>
{
    public async Task<UniformChartOfAccountListItemDto> Handle(CreateUniformChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        UniformChartOfAccount? existingAccount = await uniformChartOfAccountService.GetByCodeAsync(request.Code, cancellationToken);
        if (existingAccount is not null)
        {
            throw new BusinessException(AppErrorCodes.UniformChartOfAccount.AlreadyExists, "A uniform chart of account with the same code already exists.");
        }

        var account = request.Adapt<UniformChartOfAccount>();
        account.CreateDate = DateTime.UtcNow;
        account.Status = true;

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        UniformChartOfAccount createdAccount = await uniformChartOfAccountService.CreateAsync(account, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return createdAccount.Adapt<UniformChartOfAccountListItemDto>();
    }
}
