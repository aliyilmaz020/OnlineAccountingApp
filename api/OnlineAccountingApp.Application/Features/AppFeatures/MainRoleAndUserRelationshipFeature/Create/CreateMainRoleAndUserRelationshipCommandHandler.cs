using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationships;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Create;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Create;

public sealed class CreateMainRoleAndUserRelationshipCommandHandler(IUnitOfWork unitOfWork)
    : BaseCreateCommandHandler<CreateMainRoleAndUserRelationshipCommand, MainRoleAndUserRelationship, MainRoleAndUserRelationshipListItemDto>(unitOfWork)
{
    protected override Expression<Func<MainRoleAndUserRelationship, bool>>? GetExistsPredicate(CreateMainRoleAndUserRelationshipCommand request)
        => relationship => relationship.UserId == request.UserId
            && relationship.MainRoleId == request.MainRoleId
            && relationship.CompanyId == request.CompanyId;

    protected override string GetAlreadyExistsErrorCode() => AppErrorCodes.MainRoleAndUserRelationship.AlreadyExists;

    protected override string GetAlreadyExistsErrorMessage() => "This user already has this main role in this company.";
}
