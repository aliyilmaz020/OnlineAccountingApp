using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Update;

public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Name).NotEmpty().MaximumLength(100);
        RuleFor(command => command.Code).NotEmpty().MaximumLength(50);
    }
}
