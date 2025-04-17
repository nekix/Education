namespace FrameworksEducation.AspNetCore.Chapter_6.Exercise_1;

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

        app.UseRouting();

        app.MapGet("/{currency}/convert/{newCurrency}", (string currency, string newCurrency, double sum) =>
            sum != default
                ? TypedResults.Ok($"{sum:F} {currency} = {sum * 13.25:F} {newCurrency}")
                : Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        { "sum", new string[] { "Sum must be defined and must be grater then zero!" } }
                    }));

        app.MapGet("/{currency}/rate/{newCurrency}", (LinkGenerator generator, string currency, string newCurrency) =>
        {
            return Results.Redirect(
                $"http://sberbank.ru/ru/quotes/currencies?tab=sbol&currency={currency}&currency={newCurrency}");
        });

        return app;
    }
}