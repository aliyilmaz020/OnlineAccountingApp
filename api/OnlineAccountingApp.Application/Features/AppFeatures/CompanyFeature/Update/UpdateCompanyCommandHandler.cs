using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Domain.Roles;
using OnlineAccountingApp.Framework.MedatR.Update;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Update;

public sealed class UpdateCompanyCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, ICompanyContext companyContext)
    : BaseUpdateCommandHandler<UpdateCompanyCommand, Company, CompanyListItemDto>(unitOfWork)
{
    protected override Expression<Func<Company, bool>>? GetConflictPredicate(UpdateCompanyCommand request, Company entity)
        => company => company.Name == request.Name && company.Id != request.Id;

    protected override string GetNotFoundErrorCode() => AppErrorCodes.Company.NotFound;

    protected override string GetNotFoundErrorMessage() => "Company not found.";

    protected override string GetConflictErrorCode() => AppErrorCodes.Company.AlreadyExists;

    protected override string GetConflictErrorMessage() => "A company with the same name already exists.";

    /// <summary>
    /// Company.Update lets a non-admin (e.g. a company's own "Yönetici") edit everything except
    /// the DB connection fields - those stay system-admin-only regardless of Company.Update,
    /// since they control which physical database the tenant's data lives in.
    /// </summary>
    protected override Task MapToEntityAsync(UpdateCompanyCommand request, Company entity, CancellationToken cancellationToken)
    {
        entity.Name = request.Name;
        entity.Address = request.Address;
        entity.IdentityNumber = request.IdentityNumber;
        entity.TaxDepartment = request.TaxDepartment;
        entity.PhoneNumber = request.PhoneNumber;
        entity.Email = request.Email;

        if (companyContext.IsInRole(RoleList.SystemAdmin))
        {
            entity.ServerName = request.ServerName;
            entity.DatabaseName = request.DatabaseName;
            entity.ServerUserId = request.ServerUserId;
            entity.ServerPassword = request.ServerPassword;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Company isn't tenant-scoped by X-Company-Id like UCAF - the company being edited is the
    /// one in the route, so the permission check targets entity.Id, not companyContext.CompanyId.
    /// </summary>
    protected override async Task BeforeUpdateAsync(Company entity, UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        if (companyContext.IsInRole(RoleList.SystemAdmin))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(companyContext.UserId)
            || !await permissionService.HasPermissionAsync(companyContext.UserId, entity.Id, RoleList.CompanyUpdateCode, cancellationToken))
        {
            throw new BusinessException(AppErrorCodes.Company.PermissionDenied, "You do not have permission to update this company.");
        }
    }
}
