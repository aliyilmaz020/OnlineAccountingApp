using FluentValidation;
using OnlineAccountingApp.Framework.MedatR.Common;

namespace OnlineAccountingApp.Framework.MedatR.Delete;

public abstract class BaseDeleteCommandValidator<TCommand> : BaseCommandValidator<TCommand>
    where TCommand : BaseDeleteCommand
{
    protected override void ConfigureCommonRules()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}
