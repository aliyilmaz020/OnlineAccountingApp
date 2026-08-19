using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Domain.Roles;
using OnlineAccountingApp.Framework.MedatR.Create;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;

public class CreateCompanyCommandHandler(IUnitOfWork unitOfWork, ICompanyContext companyContext)
    : BaseCreateCommandHandler<CreateCompanyCommand, Company, CompanyListItemDto>(unitOfWork)
{
    protected override Expression<Func<Company, bool>>? GetExistsPredicate(CreateCompanyCommand request)
        => company => company.Name == request.Name;

    protected override string GetAlreadyExistsErrorCode() => AppErrorCodes.Company.AlreadyExists;

    protected override string GetAlreadyExistsErrorMessage() => "A company with the same name already exists.";

    /// <summary>Only a system admin may create new companies - everyone else is limited to the companies they already belong to.</summary>
    protected override Task BeforeCreateAsync(Company entity, CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        if (!companyContext.IsInRole(RoleList.SystemAdmin))
        {
            throw new BusinessException(AppErrorCodes.Company.PermissionDenied, "You do not have permission to create a company.");
        }

        return Task.CompletedTask;
    }

    protected override async Task AfterCreateAsync(Company entity, CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(companyContext.UserId))
        {
            await UnitOfWork.Repository<UserCompany>().CreateAsync(new UserCompany
            {
                AppUserId = companyContext.UserId,
                CompanyId = entity.Id
            }, cancellationToken);
        }
    }
}
