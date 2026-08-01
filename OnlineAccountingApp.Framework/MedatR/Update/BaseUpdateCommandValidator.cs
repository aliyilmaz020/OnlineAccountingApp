using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Common;

namespace OnlineAccountingApp.Framework.MedatR.Update;

public abstract class BaseUpdateCommandValidator<TCommand> : BaseCommandValidator<TCommand>
    where TCommand : IHasId
{
    protected override void ConfigureCommonRules()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}
