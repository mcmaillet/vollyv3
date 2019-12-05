using Microsoft.AspNetCore.Mvc;

namespace VollyV3.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}