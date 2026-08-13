using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Update;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Update;

public sealed class UpdateMainRoleCommandHandler(IUnitOfWork unitOfWork)
    : BaseUpdateCommandHandler<UpdateMainRoleCommand, MainRole, MainRoleListItemDto>(unitOfWork)
{
    protected override Expression<Func<MainRole, bool>>? GetConflictPredicate(UpdateMainRoleCommand request, MainRole entity)
        => mainRole => mainRole.Title == request.Title && mainRole.CompanyId == request.CompanyId && mainRole.Id != request.Id;

    protected override string GetNotFoundErrorCode() => AppErrorCodes.MainRole.NotFound;

    protected override string GetNotFoundErrorMessage() => "Main role not found.";

    protected override string GetConflictErrorCode() => AppErrorCodes.MainRole.AlreadyExists;

    protected override string GetConflictErrorMessage() => "A main role with the same title already exists for this company.";
}
