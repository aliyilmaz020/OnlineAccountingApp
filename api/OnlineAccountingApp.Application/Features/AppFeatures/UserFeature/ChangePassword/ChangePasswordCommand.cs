using MediatR;

namespace OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.ChangePassword;

public sealed class ChangePasswordCommand : IRequest<bool>
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}
