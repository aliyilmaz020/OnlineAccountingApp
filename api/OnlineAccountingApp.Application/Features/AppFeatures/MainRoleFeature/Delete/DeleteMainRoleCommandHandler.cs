using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.MedatR.Delete;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Delete;

public sealed class DeleteMainRoleCommandHandler(IUnitOfWork unitOfWork)
    : BaseDeleteCommandHandler<DeleteMainRoleCommand, MainRole>(unitOfWork)
{
    protected override string GetNotFoundErrorCode() => AppErrorCodes.MainRole.NotFound;

    protected override string GetNotFoundErrorMessage() => "Main role not found.";
}
