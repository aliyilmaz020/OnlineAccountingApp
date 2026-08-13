using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationships;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Create;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Create;

public sealed class CreateMainRoleAndRoleRelationshipCommandHandler(IUnitOfWork unitOfWork)
    : BaseCreateCommandHandler<CreateMainRoleAndRoleRelationshipCommand, MainRoleAndRoleRelationship, MainRoleAndRoleRelationshipListItemDto>(unitOfWork)
{
    protected override Expression<Func<MainRoleAndRoleRelationship, bool>>? GetExistsPredicate(CreateMainRoleAndRoleRelationshipCommand request)
        => relationship => relationship.RoleId == request.RoleId && relationship.MainRoleId == request.MainRoleId;

    protected override string GetAlreadyExistsErrorCode() => AppErrorCodes.MainRoleAndRoleRelationship.AlreadyExists;

    protected override string GetAlreadyExistsErrorMessage() => "This role is already assigned to this main role.";
}
