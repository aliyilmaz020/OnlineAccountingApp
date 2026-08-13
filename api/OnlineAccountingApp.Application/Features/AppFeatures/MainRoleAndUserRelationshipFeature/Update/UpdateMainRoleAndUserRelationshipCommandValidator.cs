using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Update;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Update;

public sealed class UpdateMainRoleAndUserRelationshipCommandValidator : BaseUpdateCommandValidator<UpdateMainRoleAndUserRelationshipCommand>
{
    protected override void ConfigureRules()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.MainRoleId).NotEmpty();
        RuleFor(command => command.CompanyId).NotEmpty();
    }
}
