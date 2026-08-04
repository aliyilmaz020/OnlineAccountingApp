using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Create;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Create;

public sealed class CreateMainRoleCommandValidator : BaseCreateCommandValidator<CreateMainRoleCommand>
{
    protected override void ConfigureRules()
    {
        RuleFor(command => command.Title).NotEmpty().MaximumLength(200);
        RuleFor(command => command.CompanyId).NotEmpty();
    }
}
