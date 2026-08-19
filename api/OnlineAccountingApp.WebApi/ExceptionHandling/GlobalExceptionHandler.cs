using Microsoft.AspNetCore.Diagnostics;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.WebApi.Models;

namespace OnlineAccountingApp.WebApi.ExceptionHandling;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        string? language = httpContext.Request.Headers.AcceptLanguage.FirstOrDefault();

        var (statusCode, response) = exception switch
        {
            ValidationException validationException =>
                (validationException.HttpStatusCode, ApiResponse.Fail(
                    validationException.ErrorCode,
                    ErrorMessageTranslator.Translate(validationException.Message, language),
                    TranslateErrors(validationException.Errors, language))),
            BusinessException businessException =>
                (businessException.HttpStatusCode, ApiResponse.Fail(
                    businessException.ErrorCode, ErrorMessageTranslator.Translate(businessException.Message, language))),
            _ => (StatusCodes.Status500InternalServerError, ApiResponse.Fail(null, ErrorMessageTranslator.Translate("An unexpected error occurred.", language)))
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }

    private static IReadOnlyDictionary<string, string[]>? TranslateErrors(IReadOnlyDictionary<string, string[]> errors, string? language)
    {
        return errors.ToDictionary(
            entry => entry.Key,
            entry => entry.Value.Select(message => ErrorMessageTranslator.Translate(message, language)).ToArray());
    }
}
