using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnlineAccountingApp.WebApi.Models;

namespace OnlineAccountingApp.WebApi.Filters;

public sealed class ApiResultFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        switch (context.Result)
        {
            case ObjectResult { Value: ApiResponse }:
                break;
            case ObjectResult objectResult:
                objectResult.Value = ApiResponse.Ok(objectResult.Value);
                break;
            case StatusCodeResult statusCodeResult:
                context.Result = new ObjectResult(ApiResponse.Ok(null)) { StatusCode = statusCodeResult.StatusCode };
                break;
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}
