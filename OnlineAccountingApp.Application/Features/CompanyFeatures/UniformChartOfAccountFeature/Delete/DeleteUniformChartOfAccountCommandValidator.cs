using FluentValidation;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Delete;

public sealed class DeleteUniformChartOfAccountCommandValidator : AbstractValidator<DeleteUniformChartOfAccountCommand>
{
    public DeleteUniformChartOfAccountCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}
