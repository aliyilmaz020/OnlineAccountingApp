using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.GetById;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetById;

public sealed class GetUniformChartOfAccountByIdQueryHandler(ICompanyUnitOfWork unitOfWork)
    : BaseGetByIdQueryHandler<GetUniformChartOfAccountByIdQuery, UniformChartOfAccount, UniformChartOfAccountListItemDto>(unitOfWork)
{
    protected override Expression<Func<UniformChartOfAccount, bool>> BuildPredicate(GetUniformChartOfAccountByIdQuery request)
        => account => account.Id == request.Id && !account.Deleted;

    protected override string GetNotFoundErrorCode() => AppErrorCodes.UniformChartOfAccount.NotFound;

    protected override string GetNotFoundErrorMessage() => "Uniform chart of account not found.";
}
