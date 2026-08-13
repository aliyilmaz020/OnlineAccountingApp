using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Update;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Update;

public sealed class UpdateMainRoleCommandValidator : BaseUpdateCommandValidator<UpdateMainRoleCommand>
{
    protected override void ConfigureRules()
    {
        RuleFor(command => command.Title).NotEmpty().MaximumLength(200);
        RuleFor(command => command.CompanyId).NotEmpty();
    }
}
