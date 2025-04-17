using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FrameworksEducation.AspNetCore.Chapter_5.Exercise_2;

public static class AppBuilder
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

        WebApplication app = builder.Build();

        ConcurrentDictionary<int, string> vegetables = GetVegetablesData();
        ConcurrentDictionary<int, string> fruits = GetFruitsData();

        app.UseRouting();

        RouteGroupBuilder protectedGroup = app.MapGroup("/protected-api")
            .AddEndpointFilter(async (context, next) =>
            {
                Console.WriteLine("Вход в фильтр защищенного API (тут могла быть авторизация)");

                object? result = await next(context);

                Console.WriteLine("Выход из фильтра защищенного API");

                return result;
            });

        RouteGroupBuilder vegetablesGroup = protectedGroup.MapGroup("/vegetable");

        vegetablesGroup.MapGet("/{id}", (int id) =>
            vegetables.TryGetValue(id, out string? title)
                ? TypedResults.Ok(title)
                : Results.Problem(statusCode: StatusCodes.Status404NotFound));

        RouteGroupBuilder fruitGroup = protectedGroup.MapGroup("/fruit");

        fruitGroup.MapGet("/{id}", (int id) =>
            fruits.TryGetValue(id, out string? title)
                ? TypedResults.Ok(title)
                : Results.Problem(statusCode: StatusCodes.Status404NotFound));

        return app;
    }

    private static ConcurrentDictionary<int, string> GetVegetablesData()
    {
        ConcurrentDictionary<int, string> vegetables = new ConcurrentDictionary<int, string>
        (
            new Dictionary<int, string>()
            {
                { 1, "Огурец" },
                { 2, "Картофель" },
                { 3, "Морковь" }
            }
        );
        return vegetables;
    }

    private static ConcurrentDictionary<int, string> GetFruitsData()
    {
        ConcurrentDictionary<int, string> fruits = new ConcurrentDictionary<int, string>
        (
            new Dictionary<int, string>()
            {
                { 1, "Яблоко" },
                { 2, "Апельсин" },
                { 3, "Персик" }
            }
        );
        return fruits;
    }
}