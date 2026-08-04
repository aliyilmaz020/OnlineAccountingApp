using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationships;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Update;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Update;

public sealed class UpdateMainRoleAndUserRelationshipCommandHandler(IUnitOfWork unitOfWork)
    : BaseUpdateCommandHandler<UpdateMainRoleAndUserRelationshipCommand, MainRoleAndUserRelationship, MainRoleAndUserRelationshipListItemDto>(unitOfWork)
{
    protected override Expression<Func<MainRoleAndUserRelationship, bool>>? GetConflictPredicate(
        UpdateMainRoleAndUserRelationshipCommand request, MainRoleAndUserRelationship entity)
        => relationship => relationship.UserId == request.UserId
            && relationship.MainRoleId == request.MainRoleId
            && relationship.CompanyId == request.CompanyId
            && relationship.Id != request.Id;

    protected override string GetNotFoundErrorCode() => AppErrorCodes.MainRoleAndUserRelationship.NotFound;

    protected override string GetNotFoundErrorMessage() => "Main role - user relationship not found.";

    protected override string GetConflictErrorCode() => AppErrorCodes.MainRoleAndUserRelationship.AlreadyExists;

    protected override string GetConflictErrorMessage() => "This user already has this main role in this company.";
}
