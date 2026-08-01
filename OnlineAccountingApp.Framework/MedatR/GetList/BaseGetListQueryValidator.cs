using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Common;

namespace OnlineAccountingApp.Framework.MedatR.GetList;

public abstract class BaseGetListQueryValidator<TQuery> : BaseCommandValidator<TQuery>
    where TQuery : IPagedQuery
{
    protected override void ConfigureCommonRules()
    {
        RuleFor(query => query.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
    }
}
