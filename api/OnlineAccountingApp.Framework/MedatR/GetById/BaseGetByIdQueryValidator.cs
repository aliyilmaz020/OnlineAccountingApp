using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Common;

namespace OnlineAccountingApp.Framework.MedatR.GetById;

public abstract class BaseGetByIdQueryValidator<TQuery> : BaseCommandValidator<TQuery>
    where TQuery : IHasId
{
    protected override void ConfigureCommonRules()
    {
        RuleFor(query => query.Id).NotEmpty();
    }
}
