using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Update;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Update;

public sealed class UpdateCompanyCommandHandler(IUnitOfWork unitOfWork)
    : BaseUpdateCommandHandler<UpdateCompanyCommand, Company, CompanyListItemDto>(unitOfWork)
{
    protected override Expression<Func<Company, bool>>? GetConflictPredicate(UpdateCompanyCommand request, Company entity)
        => company => company.Name == request.Name && company.Id != request.Id;

    protected override string GetNotFoundErrorCode() => AppErrorCodes.Company.NotFound;

    protected override string GetNotFoundErrorMessage() => "Company not found.";

    protected override string GetConflictErrorCode() => AppErrorCodes.Company.AlreadyExists;

    protected override string GetConflictErrorMessage() => "A company with the same name already exists.";
}
