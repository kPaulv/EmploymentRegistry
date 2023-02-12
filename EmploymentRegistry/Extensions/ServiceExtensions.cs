namespace EmploymentRegistry.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection serviceDescriptors) =>
            serviceDescriptors.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

        public static void ConfigureIISIntegration(this IServiceCollection serviceDescriptors) =>
            serviceDescriptors.Configure<IISOptions>(options =>
            {

            });
    }
}
