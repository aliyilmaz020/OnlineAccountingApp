using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty().EmailAddress();

        // Mirrors the Identity password policy configured in AddPersistence().
        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Matches("[a-z]").WithMessage("'Password' must contain a lowercase letter.")
            .Matches("[A-Z]").WithMessage("'Password' must contain an uppercase letter.")
            .Matches("[0-9]").WithMessage("'Password' must contain a digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("'Password' must contain a non-alphanumeric character.");
    }
}
