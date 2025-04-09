using Microsoft.Extensions.Logging;

namespace FrameworksEducation.AspNetCore;

public class Program
{
    public static void Main(string[] args)
    {
        //WebApplication app = ConfigureFromDefaultBuilder(args);
        //WebApplication app = ConfigureFromSlimBuilder(args);
        WebApplication app = ConfigureFromEmptyBuilder(args);

        app.Run();
    }

    private static WebApplication ConfigureFromDefaultBuilder(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        WebApplication app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        return app;
    }

    private static WebApplication ConfigureFromSlimBuilder(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
        WebApplication app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        return app;
    }

    private static WebApplication ConfigureFromEmptyBuilder(string[] args)
    {
        WebApplicationOptions options = new WebApplicationOptions
        {
            Args = args
        };

        WebApplicationBuilder builder = WebApplication.CreateEmptyBuilder(options);
        
        builder.WebHost.UseKestrelCore();
        builder.WebHost.UseUrls("http://localhost:5005");

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
            await context.Response.WriteAsync("Hello World!");
            await next(context);
        });

        return app;
    }
}
