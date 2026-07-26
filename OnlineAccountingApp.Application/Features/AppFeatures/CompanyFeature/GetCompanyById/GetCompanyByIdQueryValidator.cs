using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanyById;

public sealed class GetCompanyByIdQueryValidator : AbstractValidator<GetCompanyByIdQuery>
{
    public GetCompanyByIdQueryValidator()
    {
        RuleFor(query => query.Id).NotEmpty();
    }
}
