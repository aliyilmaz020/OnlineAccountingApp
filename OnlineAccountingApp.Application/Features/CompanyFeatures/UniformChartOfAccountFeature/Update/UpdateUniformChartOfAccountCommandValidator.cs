using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Update;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;

public sealed class UpdateUniformChartOfAccountCommandValidator : BaseUpdateCommandValidator<UpdateUniformChartOfAccountCommand>
{
    protected override void ConfigureRules()
    {
        RuleFor(command => command.Code).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Type).NotEmpty().MaximumLength(50);
    }
}
