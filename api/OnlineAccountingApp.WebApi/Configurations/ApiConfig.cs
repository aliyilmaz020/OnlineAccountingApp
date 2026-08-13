namespace OnlineAccountingApp.WebApi.Configurations;

public static partial class ApiConfig
{
    extension(IServiceCollection services)
    {
        public void ConfigureApi()
        {
            services.AddConfigureSwagger();
            services.AddConfigureErrorHandling();
            services.AddConfigureCors();
        }
    }
}
