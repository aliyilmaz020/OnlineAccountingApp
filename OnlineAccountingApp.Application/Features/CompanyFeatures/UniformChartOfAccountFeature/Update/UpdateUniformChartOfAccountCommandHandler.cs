using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Update;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;

public sealed class UpdateUniformChartOfAccountCommandHandler(ICompanyUnitOfWork unitOfWork)
    : BaseUpdateCommandHandler<UpdateUniformChartOfAccountCommand, UniformChartOfAccount, UniformChartOfAccountListItemDto>(unitOfWork)
{
    protected override async Task<UniformChartOfAccount> GetExistingEntityAsync(UpdateUniformChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        UniformChartOfAccount? entity = await Repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null || entity.Deleted)
        {
            throw new BusinessException(GetNotFoundErrorCode(), GetNotFoundErrorMessage());
        }

        return entity;
    }

    protected override Expression<Func<UniformChartOfAccount, bool>>? GetConflictPredicate(UpdateUniformChartOfAccountCommand request, UniformChartOfAccount entity)
        => account => account.Code == request.Code && account.Id != request.Id && !account.Deleted;

    protected override Task BeforeUpdateAsync(UniformChartOfAccount entity, UpdateUniformChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        entity.EditDate = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    protected override string GetNotFoundErrorCode() => AppErrorCodes.UniformChartOfAccount.NotFound;

    protected override string GetNotFoundErrorMessage() => "Uniform chart of account not found.";

    protected override string GetConflictErrorCode() => AppErrorCodes.UniformChartOfAccount.AlreadyExists;

    protected override string GetConflictErrorMessage() => "A uniform chart of account with the same code already exists.";
}
