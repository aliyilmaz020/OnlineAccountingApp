namespace OnlineAccountingApp.Domain.Exceptions;

public sealed class ValidationException(IReadOnlyDictionary<string, string[]> errors)
    : BusinessException(AppErrorCodes.Common.ValidationFailed, "One or more validation errors occurred.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
