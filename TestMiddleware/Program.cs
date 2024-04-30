var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

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

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}