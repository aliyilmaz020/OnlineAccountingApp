using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(command => command.RefreshToken).NotEmpty();
    }
}
