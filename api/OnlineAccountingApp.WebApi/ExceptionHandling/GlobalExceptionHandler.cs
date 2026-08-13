using Microsoft.AspNetCore.Diagnostics;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.WebApi.Models;

namespace OnlineAccountingApp.WebApi.ExceptionHandling;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationException =>
                (validationException.HttpStatusCode, ApiResponse.Fail(validationException.ErrorCode, validationException.Message, validationException.Errors)),
            BusinessException businessException =>
                (businessException.HttpStatusCode, ApiResponse.Fail(businessException.ErrorCode, businessException.Message)),
            _ => (StatusCodes.Status500InternalServerError, ApiResponse.Fail(null, "An unexpected error occurred."))
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
