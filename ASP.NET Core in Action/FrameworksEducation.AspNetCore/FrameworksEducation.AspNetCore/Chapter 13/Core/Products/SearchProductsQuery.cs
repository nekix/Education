namespace FrameworksEducation.AspNetCore.Chapter_13.Core.Products;

public class SearchProductsQuery
{
    public required string SearchString { get; init; }

    public int PageNumber { get; init; }

    public int PageSize { get; init; }
}