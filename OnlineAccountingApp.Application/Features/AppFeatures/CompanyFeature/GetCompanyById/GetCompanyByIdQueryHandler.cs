using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.GetById;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanyById;

public sealed class GetCompanyByIdQueryHandler(IUnitOfWork unitOfWork)
    : BaseGetByIdQueryHandler<GetCompanyByIdQuery, Company, CompanyListItemDto>(unitOfWork)
{
    protected override string GetNotFoundErrorCode() => AppErrorCodes.Company.NotFound;

    protected override string GetNotFoundErrorMessage() => "Company not found.";
}
