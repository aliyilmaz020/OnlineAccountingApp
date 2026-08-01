using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Create;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;

public sealed class CreateUniformChartOfAccountCommandHandler(ICompanyUnitOfWork unitOfWork)
    : BaseCreateCommandHandler<CreateUniformChartOfAccountCommand, UniformChartOfAccount, UniformChartOfAccountListItemDto>(unitOfWork)
{
    protected override Expression<Func<UniformChartOfAccount, bool>>? GetExistsPredicate(CreateUniformChartOfAccountCommand request)
        => account => account.Code == request.Code && !account.Deleted;

    protected override Task BeforeCreateAsync(UniformChartOfAccount entity, CreateUniformChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        entity.CreateDate = DateTime.UtcNow;
        entity.Status = true;
        return Task.CompletedTask;
    }

    protected override string GetAlreadyExistsErrorCode() => AppErrorCodes.UniformChartOfAccount.AlreadyExists;

    protected override string GetAlreadyExistsErrorMessage() => "A uniform chart of account with the same code already exists.";
}
