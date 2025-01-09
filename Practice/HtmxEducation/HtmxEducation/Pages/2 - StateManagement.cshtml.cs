using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HtmxEducation.Pages;

[IgnoreAntiforgeryToken]
public class StateManagement : PageModel
{
    private static int _count = 0;

    public void OnGet()
    {
        _count = 0;
    }

    public IActionResult OnPost()
    {
        return Content($"{++_count}", "text/html");
    }
}