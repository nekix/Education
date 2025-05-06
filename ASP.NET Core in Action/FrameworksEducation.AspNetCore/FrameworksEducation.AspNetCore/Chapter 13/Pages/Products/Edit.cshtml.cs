using System.ComponentModel.DataAnnotations;
using FrameworksEducation.AspNetCore.Chapter_13.Core.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrameworksEducation.AspNetCore.Chapter_13.Pages.Products;

public class EditModel : PageModel
{
    private readonly ProductAppService _productService;

    public ProductDto? Product { get; set; }

    [BindProperty] 
    public EditProductBindingModel Input { get; set; } = default!;

    public EditModel(ProductAppService productService)
    {
        _productService = productService;
    }

    public IActionResult OnGet(int id)
    {
        ProductDto? product = _productService.FindById(id);

        if (product == null)
            return NotFound();

        Input = new EditProductBindingModel
        {
            Title = product.Title,
            Description = product.Description
        };

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        EditProductCommand command = new EditProductCommand
        {
            Id = Product!.Id,
            Title = Input.Title,
            Description = Input.Description,
        };

        _productService.EditProduct(command);

        return Page();
    }

    public class EditProductBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Максимальная длина {1}")]
        [Display(Name = "Наименование")]
        public string Title { get; set; } = default!;

        [Required]
        [StringLength(500, ErrorMessage = "Максимальная длина {1}")]
        [Display(Name = "Описание")]
        public string Description { get; set; } = default!;
    }
}