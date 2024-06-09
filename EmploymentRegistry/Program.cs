using Contracts.Interfaces;
using EmploymentRegistry.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;

var builder = WebApplication.CreateBuilder(args);

// Add Logger Configuration
LogManager.Setup()
          .LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), 
                                        "/NLog.config"));

// Add Service Configuration by Extension Methods.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
// Contracts.Logger
builder.Services.ConfigureLoggerService();
// Repository.RepositoryManager (DAL)
builder.Services.ConfigureRepositoryManager();
// Service.ServiceManager (BLL)
builder.Services.ConfigureServiceManager();
// DbContext
builder.Services.ConfigureRepositoryContext(builder.Configuration);
// Auto-Mapper
builder.Services.AddAutoMapper(typeof(Program));

// Add services to the container.

// Add controllers (PL)
builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;   // for content negotiation
    config.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters()
  .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);

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
