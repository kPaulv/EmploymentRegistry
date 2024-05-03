﻿using Contracts.Interfaces;
using LoggerService;
using Repository;

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
