using EmploymentRegistry.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add Service Configuration by Extension Methods.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else 
    app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthorization();

// test middleware

// Use() method example
app.Use(async (context, next) =>
{
    Console.WriteLine("Before next() delegate in the Use()");
    await next();
    Console.WriteLine("after next() delegate in the Use()");
});

// Map(PathString, Action<IApplicationBuilder> configuration)
app.Map("/testmapbranch", builder =>
{
    builder.Use(async (context, next) =>
    {
        Console.WriteLine("Map() branch: before next.Invoke()");
        await next();
        Console.WriteLine("Map() branch: after next.Invoke()");
    });
    builder.Run(async context =>
    {
        Console.WriteLine("Map() branch last middleware Run()");
        await context.Response.WriteAsync("This is a test middleware component of Map() branch");
    });
});

// MapWhen()
app.MapWhen(context => context.Request.Query.ContainsKey("balaboba"), builder =>
{
    builder.Run(async context =>
    {
        await context.Response.WriteAsync("This is a test middleware component of MapWhen() branch");
    });
});

// Run() method - last middleware component()
app.Run(async context =>
{
    Console.WriteLine("Writing resonse to the console in the Run()");
    await context.Response.WriteAsync("This is a test middleware component");
});

app.MapControllers();

app.Run();
