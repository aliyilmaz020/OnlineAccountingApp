using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationships;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.GetById;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationshipById;

public sealed class GetMainRoleAndUserRelationshipByIdQueryHandler(IUnitOfWork unitOfWork)
    : BaseGetByIdQueryHandler<GetMainRoleAndUserRelationshipByIdQuery, MainRoleAndUserRelationship, MainRoleAndUserRelationshipListItemDto>(unitOfWork)
{
    protected override string GetNotFoundErrorCode() => AppErrorCodes.MainRoleAndUserRelationship.NotFound;

    protected override string GetNotFoundErrorMessage() => "Main role - user relationship not found.";
}
