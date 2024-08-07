using AspNetCoreRateLimit;
using Contracts.Interfaces;
using EmploymentRegistry.Extensions;
using EmploymentRegistry.Formatter;
using EmploymentRegistry.Utility;
using FluentValidation;
using MediatorService.Behaviours;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Presentation.ActionFilters;
using Service;
using Shared.DataTransferObjects;

var builder = WebApplication.CreateBuilder(args);

// Add Logger Configuration
LogManager.Setup()
          .LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), 
                                        "/NLog.config"));

// Add Service Configuration by Extension Methods.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
// Repository.RepositoryManager (DAL)
builder.Services.ConfigureRepositoryManager();
// DbContext (DAL)
builder.Services.ConfigureRepositoryContext(builder.Configuration);
// Service.ServiceManager (BLL)
builder.Services.ConfigureServiceManager();
// Auto-Mapper (BLL)
builder.Services.AddAutoMapper(typeof(Program));
// Data Shapers (BLL)
builder.Services.AddScoped<IDataShaper<EmployeeOutputDto>, 
                                DataShaper<EmployeeOutputDto>>();
builder.Services.AddScoped<IDataShaper<CompanyOutputDto>,
                                DataShaper<CompanyOutputDto>>();
// Validation Action Filter (PL)
builder.Services.AddScoped<ValidationFilterAttribute>();

// Media type (Accept header) Action Filter (PL)
builder.Services.AddScoped<ValidationMediaTypeAttribute>();

// Links for Employee HATEOAS responses (UTILITY)
builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>();

// Add services to the container.

// Configure custom validation in [ApiController]-marked classes
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // disable default validation in [ApiController] attribute
    options.SuppressModelStateInvalidFilter = true;
});

// Add controllers (PL)
builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;   // for content negotiation
    config.ReturnHttpNotAcceptable = true;
    config.InputFormatters.Insert(0, 
                                  new JsonPatchInputFormatter()
                                        .GetJsonPatchInputFormatter()); // for PATCH req
    config.CacheProfiles.Add("ControllerConfigCacheProfile", 
                                new CacheProfile { Duration = 90});    // cache config in startup
}).AddXmlDataContractSerializerFormatters()
  .AddCustomCsvFormatter()
  .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);

// Add custom JSON, XML media types (Content-Type's) (PL)
builder.Services.AddCustomMediaTypes();

// Add API Versioning mechanism (PL)
builder.Services.ConfigureVersioning();

// Add Response Cache (PL)
builder.Services.ConfigureResponseCaching();

// Add Response Cache HTTP Headers (Validation, Expiration) (PL)
builder.Services.ConfigureResponseCacheHeaders();

// Add Memory Cache for Rate Limiting Requests (Main + PL)
builder.Services.AddMemoryCache();

// Configure Rate Limiting (PL)
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();

// Add Authentication
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
// Add custom JwtConfiguration (Configuration service only for JWT section)
builder.Services.AddJwtConfiguration(builder.Configuration);
// Add Swagger generator for OpenAPI
builder.Services.ConfigureSwagger();
// Add MediatR (Mediator Service - API v2 BLL)
builder.Services.AddMediatR(typeof(MediatorService.AssemblyReference).Assembly);
// Add Behaviour for Fluent Validation (Mediator Service - API v2 BLL)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
// Add Fluent Validation (Mediator Service - API v2 BLL)
builder.Services.AddValidatorsFromAssembly(typeof(MediatorService.AssemblyReference).Assembly);

var app = builder.Build();

// configure Exception handler
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
    app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

// Swagger OpenAPI
app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Employment Registry V1");
    s.SwaggerEndpoint("/swagger/v2/swagger.json", "Employment Registry V2");
});

// Rate Limiting 
app.UseIpRateLimiting();

app.UseCors("CorsPolicy");

// Caching Middleware execution should be after CORS according to MSDN
app.UseResponseCaching();

app.UseHttpCacheHeaders();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
