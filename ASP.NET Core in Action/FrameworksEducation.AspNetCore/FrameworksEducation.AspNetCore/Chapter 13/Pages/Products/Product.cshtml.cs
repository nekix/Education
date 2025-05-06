using FrameworksEducation.AspNetCore.Chapter_13.Core.Products;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrameworksEducation.AspNetCore.Chapter_13.Pages.Products;

public class ProductModel : PageModel
{
    private readonly ProductAppService _productService;

    public ProductModel(ProductAppService productService)
    {
        _productService = productService;
    }

    public ProductDto? Product { get; set; }

    public void OnGet(int id)
    {
        Product = _productService.FindById(id);
    }
}