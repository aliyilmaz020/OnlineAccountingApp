using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationships;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Update;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Update;

public sealed class UpdateMainRoleAndRoleRelationshipCommandHandler(IUnitOfWork unitOfWork)
    : BaseUpdateCommandHandler<UpdateMainRoleAndRoleRelationshipCommand, MainRoleAndRoleRelationship, MainRoleAndRoleRelationshipListItemDto>(unitOfWork)
{
    protected override Expression<Func<MainRoleAndRoleRelationship, bool>>? GetConflictPredicate(
        UpdateMainRoleAndRoleRelationshipCommand request, MainRoleAndRoleRelationship entity)
        => relationship => relationship.RoleId == request.RoleId
            && relationship.MainRoleId == request.MainRoleId
            && relationship.Id != request.Id;

    protected override string GetNotFoundErrorCode() => AppErrorCodes.MainRoleAndRoleRelationship.NotFound;

    protected override string GetNotFoundErrorMessage() => "Main role - role relationship not found.";

    protected override string GetConflictErrorCode() => AppErrorCodes.MainRoleAndRoleRelationship.AlreadyExists;

    protected override string GetConflictErrorMessage() => "This role is already assigned to this main role.";
}
