using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Create;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Create;

public sealed class CreateMainRoleCommandHandler(IUnitOfWork unitOfWork)
    : BaseCreateCommandHandler<CreateMainRoleCommand, MainRole, MainRoleListItemDto>(unitOfWork)
{
    protected override Expression<Func<MainRole, bool>>? GetExistsPredicate(CreateMainRoleCommand request)
        => mainRole => mainRole.Title == request.Title && mainRole.CompanyId == request.CompanyId;

    protected override string GetAlreadyExistsErrorCode() => AppErrorCodes.MainRole.AlreadyExists;

    protected override string GetAlreadyExistsErrorMessage() => "A main role with the same title already exists for this company.";
}
