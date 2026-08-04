using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Delete;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Delete;

public sealed class DeleteMainRoleAndRoleRelationshipCommandHandler(IUnitOfWork unitOfWork)
    : BaseDeleteCommandHandler<DeleteMainRoleAndRoleRelationshipCommand, MainRoleAndRoleRelationship>(unitOfWork)
{
    protected override string GetNotFoundErrorCode() => AppErrorCodes.MainRoleAndRoleRelationship.NotFound;

    protected override string GetNotFoundErrorMessage() => "Main role - role relationship not found.";
}
