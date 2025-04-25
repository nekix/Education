using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing.Constraints;

namespace FrameworksEducation.AspNetCore.Chapter_11.Exercise_1;

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

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.Configure<RouteOptions>(opt => opt.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

        builder.Services.AddLogging(b => b.AddConsole());

        WebApplication app = builder.Build();

        List<Product> products = new List<Product>
        {
            new Product("Нихромовая нить ГОСТ 8803-89 0.012 мм ХН20Н80-ВИ"),
            new Product("Нихромовая нить ГОСТ 8803-89 0.09 мм ХН20Н80-ВИ"),
            new Product("Нихромовая нить 0.12 мм Х20Н80"),
            new Product("Нихромовая нить 0.09 мм Х20Н60И")
        };

        app.UseSwagger();
        app.UseSwaggerUI();

        Random random = new Random();

        app.MapGet("products/{category}/search", ([AsParameters] ProductsSearchQuery query) 
            => random.Next(1, 2) % 2 == 0 
                ? TypedResults.Ok(products)
                : Results.NotFound())
            .WithName("SearchProducts")
            .WithTags("products")
            .WithSummary("Поиск продукта")
            .WithDescription("Поиск продукта по ... и возвращает 404, если продукты не найдены.")
            .Produces<List<Product>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .WithParameterValidation()
            .ProducesValidationProblem();

        app.MapPost("products/{category}/search", (Product product) =>
            {
                products.Add(product);

                return TypedResults.Ok(product);
            })
            .WithName("AddProduct")
            .WithTags("products")
            .WithSummary("Добавление нового продукта")
            .WithDescription("Добавляет новый продукт.")
            .Produces<Product>()
            .WithOpenApi(o =>
            {
                o.RequestBody.Description = "Добавляемый продукт";
                return o;
            })
            .WithParameterValidation()
            .ProducesValidationProblem();

        return app;
    }
}


public class ProductsSearchQuery : IValidatableObject
{
    [Required]
    [Display(Name = "Search string")]
    [FromQuery(Name = "query")]
    [MinLength(3)]
    public required string SearchString { get; set; }

    [Required]
    [Display(Name = "Category")]
    [FromRoute(Name = "category")]
    [MinLength(5)]
    public required string Category { get; set; }

    [FromQuery(Name = "min-price")]
    [Display(Name = "Minimal price")]
    [Range(0, int.MaxValue)]
    public decimal? MinPrice { get; set; }

    [FromQuery(Name = "max-price")]
    [Display(Name = "Maximal price")]
    [Range(0, int.MaxValue)]
    public decimal? MaxPrice { get; set; }

    [FromQuery(Name = "av-quantity")]
    [Display(Name = "Available quantity")]
    [Range(0, int.MaxValue)]
    public int? AvailableQuantity { get; set; }

    [FromQuery(Name = "tag")]
    [Display(Name = "Tags")]
    public string[]? Tags { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(SearchString))
            yield return new ValidationResult("Search string must not be empty!",
                new[] { nameof(MinPrice), nameof(MaxPrice) });

        if (MinPrice > MaxPrice)
            yield return new ValidationResult("Min price must be less or equal max price!",
                new[] { nameof(MinPrice), nameof(MaxPrice) });
    }
}

public class Product
{
    [Required]
    [Display(Name = "Product name")]
    [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)]
    [MinLength(3)]
    public string Title { get; set; }

    // and others fields

    public Product(string title)
    {
        Title = title;
    }
}