using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.AssignRoleToUser;

public sealed class AssignRoleToUserCommandValidator : AbstractValidator<AssignRoleToUserCommand>
{
    public AssignRoleToUserCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.RoleName).NotEmpty();
    }
}
