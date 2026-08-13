using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Update;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Update;

public sealed class UpdateCompanyCommandValidator : BaseUpdateCommandValidator<UpdateCompanyCommand>
{
    protected override void ConfigureRules()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Address).NotEmpty();
        RuleFor(command => command.IdentityNumber).NotEmpty();
        RuleFor(command => command.TaxDepartment).NotEmpty();
        RuleFor(command => command.PhoneNumber).NotEmpty();
        RuleFor(command => command.Email).NotEmpty().EmailAddress();
        RuleFor(command => command.ServerName).NotEmpty();
        RuleFor(command => command.DatabaseName).NotEmpty();
        RuleFor(command => command.ServerUserId).NotEmpty();
        RuleFor(command => command.ServerPassword).NotEmpty();
    }
}
