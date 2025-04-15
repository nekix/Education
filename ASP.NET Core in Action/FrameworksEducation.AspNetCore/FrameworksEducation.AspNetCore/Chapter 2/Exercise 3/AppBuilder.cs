using Yarp.ReverseProxy.Configuration;

namespace FrameworksEducation.AspNetCore.Chapter_2.Exercise_3;

public static class AppBuilder
{
    public static WebApplication BuildAndConfigureLegacyApp()
    {

    }

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
                RouteId = "route1",
                ClusterId = "cluster1",
                Match = new RouteMatch
                {
                    Path = "{**catch-all}"
                }
            }
        };
        List<ClusterConfig> clusters = new List<ClusterConfig>()
        {
            new ClusterConfig
            {
                ClusterId = "cluster1",
                Destinations = new Dictionary<string, DestinationConfig>
                {

                }
            }
        };

        builder.Services.AddReverseProxy().LoadFromMemory(routes, clusters);

        WebApplication app = builder.Build();

        app.Use(async (context, next) =>
        {
            await context.Response.WriteAsync("Hello World!");
            await next(context);
        });
    }
}
