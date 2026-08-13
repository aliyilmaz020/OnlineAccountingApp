using FluentValidation;

namespace OnlineAccountingApp.Framework.MedatR.Common;

public abstract class BaseCommandValidator<TCommand> : AbstractValidator<TCommand>
{
    protected BaseCommandValidator()
    {
        ConfigureCommonRules();
        ConfigureRules();
    }

    /// <summary>Injection point for rules shared by every command in a given base hierarchy (e.g. Id, paging). Override in intermediate base validators, not in leaf validators.</summary>
    protected virtual void ConfigureCommonRules()
    {
    }

    /// <summary>Injection point for command-specific validation rules. Override in the concrete leaf validator.</summary>
    protected virtual void ConfigureRules()
    {
    }
}
