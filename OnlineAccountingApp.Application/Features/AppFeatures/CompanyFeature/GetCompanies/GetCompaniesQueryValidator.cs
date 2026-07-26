using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;

public sealed class GetCompaniesQueryValidator : AbstractValidator<GetCompaniesQuery>
{
    public GetCompaniesQueryValidator()
    {
        RuleFor(query => query.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
    }
}
