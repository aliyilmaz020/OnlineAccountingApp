using FluentValidation;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;

public sealed class UpdateUniformChartOfAccountCommandValidator : AbstractValidator<UpdateUniformChartOfAccountCommand>
{
    public UpdateUniformChartOfAccountCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Code).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Type).NotEmpty().MaximumLength(50);
    }
}
