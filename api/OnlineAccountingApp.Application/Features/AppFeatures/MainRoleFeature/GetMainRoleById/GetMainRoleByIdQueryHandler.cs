using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.GetById;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoleById;

public sealed class GetMainRoleByIdQueryHandler(IUnitOfWork unitOfWork)
    : BaseGetByIdQueryHandler<GetMainRoleByIdQuery, MainRole, MainRoleListItemDto>(unitOfWork)
{
    protected override string GetNotFoundErrorCode() => AppErrorCodes.MainRole.NotFound;

    protected override string GetNotFoundErrorMessage() => "Main role not found.";
}
