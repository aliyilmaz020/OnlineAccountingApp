using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Delete;

public sealed class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}
