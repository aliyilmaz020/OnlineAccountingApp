using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationships;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.GetById;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationshipById;

public sealed class GetMainRoleAndRoleRelationshipByIdQueryHandler(IUnitOfWork unitOfWork)
    : BaseGetByIdQueryHandler<GetMainRoleAndRoleRelationshipByIdQuery, MainRoleAndRoleRelationship, MainRoleAndRoleRelationshipListItemDto>(unitOfWork)
{
    protected override string GetNotFoundErrorCode() => AppErrorCodes.MainRoleAndRoleRelationship.NotFound;

    protected override string GetNotFoundErrorMessage() => "Main role - role relationship not found.";
}
