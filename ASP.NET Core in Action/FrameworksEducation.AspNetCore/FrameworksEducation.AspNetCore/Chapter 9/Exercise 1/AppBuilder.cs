using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        //builder.Services.AddScoped<IConsoleWriter, SimpleConsoleWriter>();
        //builder.Services.AddSingleton<IConsoleWriter, ColoredConsoleWriter>();

        // 2. Отработал ColoredConsoleWriter Scoped
        //builder.Services.AddSingleton<IConsoleWriter, SimpleConsoleWriter>();
        //builder.Services.AddScoped<IConsoleWriter, ColoredConsoleWriter>();

        // 3. Отработал ColoredConsoleWriter Singleton
        //builder.Services.AddSingleton<IConsoleWriter, SimpleConsoleWriter>();
        //builder.Services.AddSingleton<IConsoleWriter, SimpleConsoleWriter>();
        //builder.Services.Replace(new ServiceDescriptor(typeof(IConsoleWriter), typeof(ColoredConsoleWriter),
        //    ServiceLifetime.Singleton));

        // 4. Отработал ColoredConsoleWriter Scoped (неожиданно, получается просто добавил раз не было такого)
        //builder.Services.AddSingleton<IConsoleWriter, SimpleConsoleWriter>();
        //builder.Services.AddSingleton<IConsoleWriter, SimpleConsoleWriter>();
        //builder.Services.Replace(new ServiceDescriptor(typeof(IConsoleWriter), typeof(ColoredConsoleWriter),
        //    ServiceLifetime.Scoped));

        // 5. Отработал SimpleConsoleWriter Scoped
        //builder.Services.AddScoped<IConsoleWriter, SimpleConsoleWriter>();
        //builder.Services.AddSingleton<IConsoleWriter, SimpleConsoleWriter>();
        //builder.Services.Replace(new ServiceDescriptor(typeof(IConsoleWriter), typeof(ColoredConsoleWriter),
        //    ServiceLifetime.Scoped));

        // 6. Отработал SimpleConsoleWriter Singleton
        //builder.Services.AddSingleton<IConsoleWriter, SimpleConsoleWriter>();
        //builder.Services.TryAdd(new ServiceDescriptor(typeof(IConsoleWriter), typeof(ColoredConsoleWriter), ServiceLifetime.Singleton));

        // 7. Отработал SimpleConsoleWriter Singleton
        //builder.Services.AddSingleton<IConsoleWriter, SimpleConsoleWriter>();
        //builder.Services.TryAdd(new ServiceDescriptor(typeof(IConsoleWriter), typeof(ColoredConsoleWriter), ServiceLifetime.Scoped));

        // 8. Отработал SimpleConsoleWriter Scoped
        builder.Services.TryAdd(new ServiceDescriptor(typeof(IConsoleWriter), typeof(SimpleConsoleWriter), ServiceLifetime.Scoped));
        builder.Services.TryAdd(new ServiceDescriptor(typeof(IConsoleWriter), typeof(ColoredConsoleWriter), ServiceLifetime.Singleton));

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