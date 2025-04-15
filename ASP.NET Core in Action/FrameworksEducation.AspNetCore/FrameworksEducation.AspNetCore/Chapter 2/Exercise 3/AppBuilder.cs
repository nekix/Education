using Yarp.ReverseProxy.Configuration;

namespace FrameworksEducation.AspNetCore.Chapter_2.Exercise_3;

public static class AppBuilder
{
    /// <summary>
    /// Настраивает WebApplicationBuilder для условно 'нового приложения',
    /// которое проксирует запросы по относительному пути 'legacy/*' на localhost:5006.
    /// При этом остальные запросы поступают на Endpoint в этом проекте.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static WebApplication BuildAndConfigureModernApp(string[] args)
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

        List<RouteConfig> routes = new List<RouteConfig>()
        {
            new RouteConfig
            {
                RouteId = "legacyActionRoute",
                ClusterId = "legacyCluster",
                Match = new RouteMatch
                {
                    Path = "legacy/{*catch-all}"
                }
            }
        };

        List<ClusterConfig> clusters = new List<ClusterConfig>()
        {
            new ClusterConfig
            {
                ClusterId = "legacyCluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "legacyDestination", new DestinationConfig { Address = "http://localhost:5006" }}
                }
            }
        };

        builder.Services.AddReverseProxy().LoadFromMemory(routes, clusters);

        WebApplication app = builder.Build();

        app.MapReverseProxy();

        app.Map("/{*catch-all}", () => Task.FromResult("Modern function execute!"));

        return app;
    }

    /// <summary>
    /// Настраивает WebApplicationBuilder для условно 'старого приложения',
    /// которое обрабатывает запросы, приходящие к нему как на прямую, так и проксируемые.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static WebApplication BuildAndConfigureLegacyApp(string[] args)
    {
        WebApplicationOptions options = new WebApplicationOptions
        {
            Args = args
        };

        WebApplicationBuilder builder = WebApplication.CreateEmptyBuilder(options);

        builder.WebHost.UseKestrelCore();
        builder.WebHost.UseUrls("http://localhost:5006");

        // Configure logging
        builder.Logging.AddConsole();
        builder.Services.AddHttpLogging(opts =>
            opts.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties);
        builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

        WebApplication app = builder.Build();

        app.Run(async context =>
        {
            await context.Response.WriteAsync("Legacy function execute!");
        });

        return app;
    }
}
