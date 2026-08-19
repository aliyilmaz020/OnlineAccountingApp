using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(command => command.CurrentPassword).NotEmpty();
        // The password policy itself (digit/lower/upper/non-alphanumeric/length) is enforced by
        // UserManager.ChangePasswordAsync via IUserService.ChangePasswordAsync - not duplicated here.
        RuleFor(command => command.NewPassword).NotEmpty();
        RuleFor(command => command.ConfirmNewPassword)
            .NotEmpty()
            .Equal(command => command.NewPassword)
            .WithMessage("The new password and confirmation do not match.");
    }
}
