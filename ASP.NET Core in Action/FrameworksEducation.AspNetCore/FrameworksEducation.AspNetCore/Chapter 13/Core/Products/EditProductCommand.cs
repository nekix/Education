namespace FrameworksEducation.AspNetCore.Chapter_13.Core.Products;

public class EditProductCommand
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }
}