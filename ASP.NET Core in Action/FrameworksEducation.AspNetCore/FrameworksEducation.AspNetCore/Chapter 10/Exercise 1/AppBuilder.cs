using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FrameworksEducation.AspNetCore.Chapter_10.Exercise_1;

public static class AppBuilder
{
    public static WebApplication Configure(string[] args)
    {
        WebApplicationOptions options = new WebApplicationOptions
        {
            Args = args
        };

        WebApplicationBuilder builder = WebApplication.CreateEmptyBuilder(options);

        builder.Services.AddOptions<ApiOptions>()
            .BindConfiguration("ApiOptions")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.WebHost.UseKestrelCore();
        builder.WebHost.UseUrls("http://localhost:5005");

        builder.Services.AddRoutingCore();
        builder.Services.AddProblemDetails();
        builder.Services.AddSingleton<LongTimeService>();
        builder.Services.AddSingleton<Random>();

        builder.Services.AddLogging(b => b.AddConsole());

        builder.Configuration.AddJsonFile(
            @"Chapter 10\Exercise 1\special-appsettings.json", 
            optional: false, 
             reloadOnChange: true);

        WebApplication app = builder.Build();

        app.UseRouting();

        app.UseDeveloperExceptionPage();

        app.MapGet("api-options/default", ([FromServices] IOptions<ApiOptions> opt) =>
            TypedResults.Ok(opt.Value));

        app.MapGet("api-options/snapshot", ([FromServices] IOptionsSnapshot<ApiOptions> opt) => 
            TypedResults.Ok(opt.Value));

        app.MapGet("api-options/dynamic", ([FromServices] IOptionsMonitor<ApiOptions> opt) =>
            TypedResults.Ok(opt.CurrentValue));

        app.MapGet("api-options/service", ([FromServices] LongTimeService service) =>
        {
            return Results.Ok(string.Join(Environment.NewLine,
                service.GetDefault(), 
                service.GetSnapshot(), 
                service.GetMonitor()));
        });

        // Для простоты проверки Get
        app.MapGet("api-options/update", ([FromServices] Random random) =>
        {
            string path = @"Chapter 10\Exercise 1\special-appsettings.json";

            string configJson = File.ReadAllText(path);

            JsonNode rootNode = JsonNode.Parse(configJson)!;

            rootNode["ApiOptions"]!["MaxAllowedClients"] = random.Next(0, 100);

            rootNode["ApiOptions"]!["RedirectedRootNode"] = "kvm" + random.Next(1, 200);

            configJson = JsonSerializer.Serialize(rootNode);

            File.WriteAllText(path, configJson);

            return TypedResults.Ok(rootNode);
        });

        return app;
    }
}

public class LongTimeService
{
    private readonly IOptions<ApiOptions> _optionsDefault;
    private readonly IOptionsSnapshot<ApiOptions> _optionsSnapshot;
    private readonly IOptionsMonitor<ApiOptions> _optionsMonitor;

    public LongTimeService(
        IOptions<ApiOptions> optionsDefault,
        IOptionsSnapshot<ApiOptions> optionsSnapshot,
        IOptionsMonitor<ApiOptions> optionsMonitor)
    {
        _optionsDefault = optionsDefault;
        _optionsSnapshot = optionsSnapshot;
        _optionsMonitor = optionsMonitor;

        _optionsMonitor.OnChange((opt, str) =>
        {
            Console.WriteLine($"Changed: {opt.MaxAllowedClients}:{opt.RedirectedRootNode}:{str}");
        });
    }

    public ApiOptions GetDefault()
        => _optionsDefault.Value;

    public ApiOptions GetSnapshot()
        => _optionsSnapshot.Value;

    public ApiOptions GetMonitor()
        => _optionsMonitor.CurrentValue;
}

public class ApiOptions
{
    [Required, Range(0, int.MaxValue)]
    public required int MaxAllowedClients { get; set; }

    [Required, MinLength(3)]
    public required string RedirectedRootNode { get; set; }

    public override string ToString()
    {
        return $"{MaxAllowedClients}:{RedirectedRootNode}";
    }
}