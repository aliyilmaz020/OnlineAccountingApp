using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Create;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(100);
        RuleFor(command => command.Code).NotEmpty().MaximumLength(50);
    }
}
