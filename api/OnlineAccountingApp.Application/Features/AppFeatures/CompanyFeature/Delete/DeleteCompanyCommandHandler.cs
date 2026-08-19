using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Domain.Roles;
using OnlineAccountingApp.Framework.MedatR.Delete;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Delete;

public sealed class DeleteCompanyCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, ICompanyContext companyContext)
    : BaseDeleteCommandHandler<DeleteCompanyCommand, Company>(unitOfWork)
{
    protected override string GetNotFoundErrorCode() => AppErrorCodes.Company.NotFound;

    protected override string GetNotFoundErrorMessage() => "Company not found.";

    /// <summary>See UpdateCompanyCommandHandler.BeforeUpdateAsync: checked against entity.Id, not the X-Company-Id header.</summary>
    protected override async Task BeforeDeleteAsync(Company entity, DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        if (companyContext.IsInRole(RoleList.SystemAdmin))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(companyContext.UserId)
            || !await permissionService.HasPermissionAsync(companyContext.UserId, entity.Id, RoleList.CompanyDeleteCode, cancellationToken))
        {
            throw new BusinessException(AppErrorCodes.Company.PermissionDenied, "You do not have permission to delete this company.");
        }
    }
}
