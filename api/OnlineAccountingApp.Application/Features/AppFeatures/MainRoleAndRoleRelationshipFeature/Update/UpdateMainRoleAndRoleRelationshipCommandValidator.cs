using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Update;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Update;

public sealed class UpdateMainRoleAndRoleRelationshipCommandValidator : BaseUpdateCommandValidator<UpdateMainRoleAndRoleRelationshipCommand>
{
    protected override void ConfigureRules()
    {
        RuleFor(command => command.RoleId).NotEmpty();
        RuleFor(command => command.MainRoleId).NotEmpty();
    }
}
