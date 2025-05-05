namespace FrameworksEducation.AspNetCore.Chapter_13.Core.Products;

public class CreateProductCommand
{
    public required string Title { get; init; }

    public required string Description { get; init; }
}