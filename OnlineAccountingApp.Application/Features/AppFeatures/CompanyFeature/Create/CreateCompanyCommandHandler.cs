using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Create;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;

public class CreateCompanyCommandHandler(IUnitOfWork unitOfWork)
    : BaseCreateCommandHandler<CreateCompanyCommand, Company, CompanyListItemDto>(unitOfWork)
{
    protected override Expression<Func<Company, bool>>? GetExistsPredicate(CreateCompanyCommand request)
        => company => company.Name == request.Name;

    protected override string GetAlreadyExistsErrorCode() => AppErrorCodes.Company.AlreadyExists;

    protected override string GetAlreadyExistsErrorMessage() => "A company with the same name already exists.";
}
