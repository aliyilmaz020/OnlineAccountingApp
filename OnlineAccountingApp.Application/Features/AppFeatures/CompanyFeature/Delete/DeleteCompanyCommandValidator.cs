using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Delete;

public sealed class DeleteCompanyCommandValidator : AbstractValidator<DeleteCompanyCommand>
{
    public DeleteCompanyCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}
