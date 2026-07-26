using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Infrastructure.Options;
using OnlineAccountingApp.WebApi.Models;
using System.Text;

namespace OnlineAccountingApp.WebApi.Configurations;

public static partial class ApiConfig
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Must be called *after* AddPersistence(): AddIdentity registers cookie handlers as the
        /// default scheme, and the later registration wins.
        /// </summary>
        public void AddConfigureAuthentication(IConfiguration configuration)
        {
            JwtOptions jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException($"The '{JwtOptions.SectionName}' configuration section is missing.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };

                // Keep 401/403 responses in the same ApiResponse envelope as every other error.
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        await WriteApiResponseAsync(
                            context.Response,
                            StatusCodes.Status401Unauthorized,
                            AppErrorCodes.Auth.InvalidCredentials,
                            "Authentication is required to access this resource.");
                    },
                    OnForbidden = async context =>
                    {
                        await WriteApiResponseAsync(
                            context.Response,
                            StatusCodes.Status403Forbidden,
                            AppErrorCodes.Auth.CompanyAccessDenied,
                            "You do not have permission to access this resource.");
                    }
                };
            });
        }
    }

    private static async Task WriteApiResponseAsync(HttpResponse response, int statusCode, string errorCode, string message)
    {
        if (response.HasStarted)
        {
            return;
        }

        response.StatusCode = statusCode;
        response.ContentType = "application/json";
        await response.WriteAsJsonAsync(ApiResponse.Fail(errorCode, message));
    }
}
