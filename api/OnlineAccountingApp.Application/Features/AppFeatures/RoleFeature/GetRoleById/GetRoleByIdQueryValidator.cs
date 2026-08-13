using FluentValidation;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoleById;

public sealed class GetRoleByIdQueryValidator : AbstractValidator<GetRoleByIdQuery>
{
    public GetRoleByIdQueryValidator()
    {
        RuleFor(query => query.Id).NotEmpty();
    }
}
