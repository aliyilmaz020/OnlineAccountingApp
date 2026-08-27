using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(command => command.RefreshToken).NotEmpty();
    }
}
