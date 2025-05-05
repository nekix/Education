namespace FrameworksEducation.AspNetCore.Chapter_13.Core.Products;

public class Product
{
    public int Id { get; init; }

    public string Title { get; set; }

    public string Description { get; set; }

    public Product(int id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
    }
}