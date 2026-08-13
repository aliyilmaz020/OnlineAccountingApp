namespace OnlineAccountingApp.WebApi.Configurations;

public static partial class ApiConfig
{
    internal const string DevCorsPolicy = "DevCorsPolicy";

    extension(IServiceCollection services)
    {
        public void AddConfigureCors()
        {
            services.AddCors(options =>
            {
                options.AddPolicy(DevCorsPolicy, policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
        }
    }
}
