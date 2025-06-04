using Microsoft.AspNetCore.Mvc;

namespace CarPark.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}
