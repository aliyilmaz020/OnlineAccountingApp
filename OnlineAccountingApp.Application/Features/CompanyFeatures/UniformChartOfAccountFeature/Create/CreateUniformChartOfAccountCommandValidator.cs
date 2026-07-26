using FluentValidation;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;

public sealed class CreateUniformChartOfAccountCommandValidator : AbstractValidator<CreateUniformChartOfAccountCommand>
{
    public CreateUniformChartOfAccountCommandValidator()
    {
        RuleFor(command => command.Code).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Type).NotEmpty().MaximumLength(50);
    }
}
