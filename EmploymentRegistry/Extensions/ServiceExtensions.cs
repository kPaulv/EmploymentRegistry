using Contracts.Interfaces;
using EmploymentRegistry.Formatter;
using LoggerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
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

        // Configure custom Media types for responses (PL)
        public static void AddCustomMediaTypes(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.Configure<MvcOptions>(config =>
            {
                var jsonOutputFormatter = config.OutputFormatters
                                                    .OfType<SystemTextJsonOutputFormatter>()?
                                                    .FirstOrDefault();
                
                if(jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.
                                            Add("application/vnd.kpaulv.hateoas+json");
                }

                var xmlOutputFormatter = config.OutputFormatters
                                                    .OfType<XmlDataContractSerializerOutputFormatter>()?
                                                    .FirstOrDefault();

                if(xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes.
                                            Add("application/vnd.kpaulv.hateoas+json");
                }
            });
        }

        public static void ConfigureVersioning(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;    // adds Api version header to Response
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
        }

        // Configure DbContext (DAL)
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
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination"));
            });

        // Configure IIS
        public static void ConfigureIISIntegration(this IServiceCollection serviceDescriptors) =>
            serviceDescriptors.Configure<IISOptions>(options =>
            {

            });
    }
}
