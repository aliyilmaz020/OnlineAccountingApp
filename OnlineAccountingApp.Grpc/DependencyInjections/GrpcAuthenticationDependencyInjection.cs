using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OnlineAccountingApp.Infrastructure.Options;
using System.Text;

namespace OnlineAccountingApp.Grpc.DependencyInjections;

/// <summary>
/// Local copy of WebApi's ApiConfig.AddConfigureAuthentication, minus the JwtBearerEvents that
/// write an HTTP JSON body on 401/403 - gRPC has no response-body concept, and the default
/// JwtBearer/authorization middleware already surfaces 401/403 as the correct gRPC status.
/// Must be called *after* AddGrpcPersistence(): AddIdentity registers cookie handlers as the
/// default scheme, and this later registration wins.
/// </summary>
public static class GrpcAuthenticationDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddGrpcAuthentication(IConfiguration configuration)
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
            });
        }
    }
}
