using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.WebApi.ExceptionHandling;
using OnlineAccountingApp.WebApi.Filters;

namespace OnlineAccountingApp.WebApi.Configurations;

public static partial class ApiConfig
{
    extension(IServiceCollection services)
    {
        public void AddConfigureErrorHandling()
        {
            services.Configure<MvcOptions>(options => options.Filters.Add<ApiResultFilter>());
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
        }
    }
}
