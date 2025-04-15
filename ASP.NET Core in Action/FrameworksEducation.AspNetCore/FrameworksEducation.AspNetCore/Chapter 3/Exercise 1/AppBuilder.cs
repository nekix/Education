namespace FrameworksEducation.AspNetCore.Chapter_3.Exercise_1;

public static class AppBuilder
{
    public static WebApplication ConfigureFromDefaultBuilder(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseUrls("http://localhost:5005");

        WebApplication app = builder.Build();

        app.MapGet("/", () => "Hello Default World!");

        return app;
    }

    public static WebApplication ConfigureFromSlimBuilder(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);

        builder.WebHost.UseUrls("http://localhost:5006");

        WebApplication app = builder.Build();

        app.MapGet("/", () => "Hello Slim World!");

        return app;
    }

    public static WebApplication ConfigureFromEmptyBuilder(string[] args)
    {
        WebApplicationOptions options = new WebApplicationOptions
        {
            Args = args
        };

        WebApplicationBuilder builder = WebApplication.CreateEmptyBuilder(options);

        builder.WebHost.UseKestrelCore();
        builder.WebHost.UseUrls("http://localhost:5007");

        // Configure logging
        builder.Logging.AddConsole();
        builder.Services.AddHttpLogging(opts =>
            opts.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties);
        builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

        WebApplication app = builder.Build();

        // Configure logging middleware
        // app.UseHttpLogging();

        app.Use(async (context, next) =>
        {
            await context.Response.WriteAsync("Hello Empty World!");
            await next(context);
        });

        return app;
    }
}