using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Create;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;

public sealed class CreateUniformChartOfAccountCommandValidator : BaseCreateCommandValidator<CreateUniformChartOfAccountCommand>
{
    protected override void ConfigureRules()
    {
        RuleFor(command => command.Code).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Type).NotEmpty().MaximumLength(50);
    }
}
