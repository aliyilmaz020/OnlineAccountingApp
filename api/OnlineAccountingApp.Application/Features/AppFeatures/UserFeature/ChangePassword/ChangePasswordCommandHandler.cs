using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.ChangePassword;

public sealed class ChangePasswordCommandHandler(IUserService userService, ICompanyContext companyContext)
    : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(companyContext.UserId))
        {
            throw new BusinessException(AppErrorCodes.Auth.InvalidCredentials, "Authentication is required to access this resource.");
        }

        await userService.ChangePasswordAsync(companyContext.UserId, request.CurrentPassword, request.NewPassword, cancellationToken);
        return true;
    }
}
