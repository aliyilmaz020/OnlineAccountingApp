using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;

public sealed class GetRolesQueryValidator : AbstractValidator<GetRolesQuery>
{
    public GetRolesQueryValidator()
    {
        RuleFor(query => query.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
    }
}
