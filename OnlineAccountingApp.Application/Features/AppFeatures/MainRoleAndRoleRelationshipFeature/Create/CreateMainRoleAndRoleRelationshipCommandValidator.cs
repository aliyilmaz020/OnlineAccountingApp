using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Create;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Create;

public sealed class CreateMainRoleAndRoleRelationshipCommandValidator : BaseCreateCommandValidator<CreateMainRoleAndRoleRelationshipCommand>
{
    protected override void ConfigureRules()
    {
        RuleFor(command => command.RoleId).NotEmpty();
        RuleFor(command => command.MainRoleId).NotEmpty();
    }
}
