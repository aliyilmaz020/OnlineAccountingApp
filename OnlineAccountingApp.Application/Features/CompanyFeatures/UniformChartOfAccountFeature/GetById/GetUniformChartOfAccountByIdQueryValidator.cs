using FluentValidation;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetById;

public sealed class GetUniformChartOfAccountByIdQueryValidator : AbstractValidator<GetUniformChartOfAccountByIdQuery>
{
    public GetUniformChartOfAccountByIdQueryValidator()
    {
        RuleFor(query => query.Id).NotEmpty();
    }
}
