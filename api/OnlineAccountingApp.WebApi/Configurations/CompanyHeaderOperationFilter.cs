using Microsoft.OpenApi;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.WebApi.Tenancy;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OnlineAccountingApp.WebApi.Configurations;

/// <summary>
/// Documents the <c>X-Company-Id</c> header on every endpoint marked with
/// <see cref="RequiresCompanyHeaderAttribute"/> so it can be supplied from Swagger UI.
/// </summary>
public sealed class CompanyHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        bool requiresCompanyHeader =
            context.MethodInfo.GetCustomAttributes(inherit: true).OfType<RequiresCompanyHeaderAttribute>().Any() ||
            (context.MethodInfo.DeclaringType?.GetCustomAttributes(inherit: true).OfType<RequiresCompanyHeaderAttribute>().Any() ?? false);

        if (!requiresCompanyHeader)
        {
            return;
        }

        operation.Parameters ??= [];
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = ICompanyContext.HeaderName,
            In = ParameterLocation.Header,
            Required = true,
            Description = "Identifier of the company whose own database this request targets.",
            Schema = new OpenApiSchema { Type = JsonSchemaType.String }
        });
    }
}
