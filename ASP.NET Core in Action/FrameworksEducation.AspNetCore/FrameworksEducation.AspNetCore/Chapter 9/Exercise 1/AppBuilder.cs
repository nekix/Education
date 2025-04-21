using System.Security.Cryptography;

namespace FrameworksEducation.AspNetCore.Chapter_9.Exercise_1;

public class AppBuilder
{
    public static WebApplication Configure(string[] args)
    {
        WebApplicationOptions options = new WebApplicationOptions
        {
            Args = args
        };

        WebApplicationBuilder builder = WebApplication.CreateEmptyBuilder(options);

        builder.WebHost.UseKestrelCore();
        builder.WebHost.UseUrls("http://localhost:5005");

        builder.Services.AddRoutingCore();
        builder.Services.AddProblemDetails();

        // 1. Отработал ColoredConsoleWriter Singleton
        builder.Services.AddScoped<IConsoleWriter, SimpleConsoleWriter>();
        builder.Services.AddSingleton<IConsoleWriter, ColoredConsoleWriter>();

        WebApplication app = builder.Build();

        app.UseRouting();

        app.MapGet("di-test/", (IConsoleWriter writer) =>
        {
            writer.WriteToConsole("Invoked 'di-test' handler!");

            return Results.Ok();
        });

        return app;
    }
}

public interface IConsoleWriter
{
    void WriteToConsole(string message);
}

public class SimpleConsoleWriter : IConsoleWriter
{
    private readonly int _id;

    public SimpleConsoleWriter()
    {
        _id = new Random().Next();
    }

    public void WriteToConsole(string message)
    {
        Console.WriteLine($"{_id}:{message}");
    }
}

public class ColoredConsoleWriter : IConsoleWriter
{
    private readonly int _id;

    public ColoredConsoleWriter()
    {
        _id = new Random().Next();
    }

    public void WriteToConsole(string message)
    {
        Console.WriteLine($"{_id}:colored message: {message}");
    }
}