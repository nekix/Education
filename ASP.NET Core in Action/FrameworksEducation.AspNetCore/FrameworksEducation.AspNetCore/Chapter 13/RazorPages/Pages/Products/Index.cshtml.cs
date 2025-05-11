using System.ComponentModel.DataAnnotations;
using FrameworksEducation.AspNetCore.Chapter_13.Core.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrameworksEducation.AspNetCore.Chapter_13.RazorPages.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ProductAppService _productService;

        public IndexModel(ProductAppService productService)
        {
            _productService = productService;

            Products = new List<ProductDto>(10);
        }

        [BindProperty(SupportsGet = true)]
        public required InputModel Input { get; set; }

        public List<ProductDto> Products { get; set; }
                
        public IActionResult OnGet()
        {
            if (ModelState.IsValid)
            {
                Products.AddRange(_productService.GetList());

                return Page();
            }

            return RedirectToPage("./Index");
        }

        public class InputModel
        {
            [FromQuery]
            [StringLength(maximumLength: 20, MinimumLength = 3)]
            public string? SearchString { get; set; }

            [FromQuery]
            [Range(5, int.MaxValue)]
            public int? PageSize { get; set; } = 20;

            [FromQuery] [Range(1, int.MaxValue)] 
            public int? PageNumber { get; set; } = 1;
        }
    }
}
