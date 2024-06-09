using Contracts.Interfaces;
using EmploymentRegistry.Formatter;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using Service.Contracts;

namespace EmploymentRegistry.Extensions
{
    public static class ServiceExtensions
    {
        // Configure Logger Service IOC
        public static void ConfigureLoggerService(this IServiceCollection serviceDescriptors) =>
            serviceDescriptors.AddSingleton<ILoggerManager, LoggerManager>();

        // Configure Repository Manager Service (DAL)
        public static void ConfigureRepositoryManager(this IServiceCollection serviceDescriptors) =>
            serviceDescriptors.AddScoped<IRepositoryManager, RepositoryManager>();

        // Configure ServiceManager Service (BLL)
        public static void ConfigureServiceManager(this IServiceCollection serviceDescriptors) =>
            serviceDescriptors.AddScoped<IServiceManager, ServiceManager>();

        // Configure CSV Output Formatter for controllers
        public static IMvcBuilder AddCustomCsvFormatter(this IMvcBuilder mvcBuilder) =>
           mvcBuilder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

        // Configure DbContext
        public static void ConfigureRepositoryContext(this IServiceCollection serviceDescriptors,
                                                        IConfiguration configuration) =>
            serviceDescriptors.AddDbContext<RepositoryContext>(options =>
                                options.UseSqlServer(configuration.GetConnectionString("EmploymentRegistryDb")));

        // Configure CORS
        public static void ConfigureCors(this IServiceCollection serviceDescriptors) =>
            serviceDescriptors.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

        // Configure IIS
        public static void ConfigureIISIntegration(this IServiceCollection serviceDescriptors) =>
            serviceDescriptors.Configure<IISOptions>(options =>
            {

            });
    }
}
