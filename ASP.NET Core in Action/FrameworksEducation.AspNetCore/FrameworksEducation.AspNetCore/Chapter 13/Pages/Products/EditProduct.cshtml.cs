using FrameworksEducation.AspNetCore.Chapter_13.Core.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrameworksEducation.AspNetCore.Chapter_13.Pages.Products
{
    public class EditProductModel : PageModel
    {
        private readonly ProductAppService _productService;

        public void OnGet()
        {
        }
    }
}
