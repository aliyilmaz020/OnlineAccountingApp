using FluentValidation;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;

public sealed class GetUniformChartOfAccountsQueryValidator : AbstractValidator<GetUniformChartOfAccountsQuery>
{
    public GetUniformChartOfAccountsQueryValidator()
    {
        RuleFor(query => query.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
    }
}
