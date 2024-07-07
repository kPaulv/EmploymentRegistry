using Contracts.Interfaces;
using EmploymentRegistry.Extensions;
using EmploymentRegistry.Formatter;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
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
}).AddXmlDataContractSerializerFormatters()
  .AddCustomCsvFormatter()
  .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);

// Add custom JSON, XML media types (Content-Type's)
builder.Services.AddCustomMediaTypes();

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

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
