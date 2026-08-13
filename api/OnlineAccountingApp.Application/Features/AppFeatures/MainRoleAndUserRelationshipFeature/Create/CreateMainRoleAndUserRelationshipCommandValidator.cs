using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Create;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Create;

public sealed class CreateMainRoleAndUserRelationshipCommandValidator : BaseCreateCommandValidator<CreateMainRoleAndUserRelationshipCommand>
{
    protected override void ConfigureRules()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.MainRoleId).NotEmpty();
        RuleFor(command => command.CompanyId).NotEmpty();
    }
}
