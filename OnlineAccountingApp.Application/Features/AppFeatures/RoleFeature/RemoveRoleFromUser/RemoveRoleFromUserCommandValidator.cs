using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.RemoveRoleFromUser;

public sealed class RemoveRoleFromUserCommandValidator : AbstractValidator<RemoveRoleFromUserCommand>
{
    public RemoveRoleFromUserCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.RoleName).NotEmpty();
    }
}
