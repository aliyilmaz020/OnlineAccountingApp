using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Delete;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Delete;

public sealed class DeleteMainRoleAndUserRelationshipCommandHandler(IUnitOfWork unitOfWork)
    : BaseDeleteCommandHandler<DeleteMainRoleAndUserRelationshipCommand, MainRoleAndUserRelationship>(unitOfWork)
{
    protected override string GetNotFoundErrorCode() => AppErrorCodes.MainRoleAndUserRelationship.NotFound;

    protected override string GetNotFoundErrorMessage() => "Main role - user relationship not found.";
}
