using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;

namespace OnlineAccountingApp.WebApi.Configurations;

public static partial class ApiConfig
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(setup =>
        {
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
            };

            setup.AddSecurityDefinition(jwtSecurityScheme.Scheme, jwtSecurityScheme);

            setup.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference(jwtSecurityScheme.Scheme, document),
                    new List<string>()
                }
            });
        });
        return services;
    }
}
