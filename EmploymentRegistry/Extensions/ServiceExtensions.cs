using AspNetCoreRateLimit;
using Contracts.Interfaces;
using EmploymentRegistry.Formatter;
using Entities.ConfigModels;
using Entities.Entities;
using LoggerService;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.Controllers;
using Repository;
using Service;
using Service.Contracts;
using System.Text;

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

        // Add Versioning for API (PL)
        public static void ConfigureVersioning(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;    // adds Api version header to Response
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
                // Conventions to assegn versions to different controllers
                // instead of Attribute[ApiVersion]
                options.Conventions.Controller<CompaniesController>()
                                        .HasApiVersion(new ApiVersion(1, 0));
                options.Conventions.Controller<CompaniesControllerV2>()
                                        .HasDeprecatedApiVersion(new ApiVersion(2, 0));
            });
        }

        // Add Response Cache (PL)
        public static void ConfigureResponseCaching(this IServiceCollection serviceDescriptors) =>
            serviceDescriptors.AddResponseCaching(responseCachingOptions => { });

        // Add Response Cache Headers(for Validation & Expiration) (PL)
        public static void ConfigureResponseCacheHeaders(this IServiceCollection serviceDescriptors) =>
            serviceDescriptors.AddHttpCacheHeaders(
                expirationModelOptions => 
                {
                    expirationModelOptions.MaxAge = 120;
                    expirationModelOptions.CacheLocation = CacheLocation.Private;
                }, 
                validationModelOptions =>
                {
                    validationModelOptions.MustRevalidate = true;
                });

        // Add Rate Limiting (PL)
        public static void ConfigureRateLimitingOptions(this IServiceCollection serviceDescriptors)
        {
            // List of Rate Limiting rules for specific endpoints
            var rateLimitRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Limit = 10,
                    Period = "5m"
                }
            };

            // set up rules for Rate Limit
            serviceDescriptors.Configure<IpRateLimitOptions>(rateLimitOptions =>
            {
                rateLimitOptions.GeneralRules = rateLimitRules;
            });
            // Store for Rate Limit counters
            serviceDescriptors.AddSingleton<IRateLimitCounterStore, 
                                                MemoryCacheRateLimitCounterStore>();
            // Store for Ip Rate Limit policies
            serviceDescriptors.AddSingleton<IIpPolicyStore, 
                                                MemoryCacheIpPolicyStore>();
            serviceDescriptors.AddSingleton<IRateLimitConfiguration, 
                                                RateLimitConfiguration>();
            serviceDescriptors.AddSingleton<IProcessingStrategy, 
                                                AsyncKeyLockProcessingStrategy>();
        }

        // Configure Identity (DAL)
        public static void ConfigureIdentity(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddIdentity<User, IdentityRole>(identityOptions =>
            {
                identityOptions.Password.RequiredLength = 8;
                identityOptions.Password.RequireDigit = false;
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequireNonAlphanumeric = false;
                identityOptions.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<RepositoryContext>().AddDefaultTokenProviders();
        }

        // Configure JWT
        public static void ConfigureJWT(this IServiceCollection serviceDescriptors,
                                            IConfiguration configuration)
        {
            var jwtConfig = new JwtConfiguration();
            configuration.Bind(jwtConfig);

            var secretKey = Environment.GetEnvironmentVariable("EMPREGAPP_SECRET");
            secretKey += new string(secretKey.ToCharArray().Reverse().ToArray());

            serviceDescriptors.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme =
                                JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme =
                                JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.ValidIssuer,
                    ValidAudience = jwtConfig.ValidAudience,
                    IssuerSigningKey = 
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
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
