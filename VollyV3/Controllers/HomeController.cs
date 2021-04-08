using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VollyV3.Models;
using VollyV3.Models.Home;

namespace VollyV3.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string GoogleMapsAPIKey = Environment.GetEnvironmentVariable("google_maps_api_key");

        private readonly UserManager<VollyV3User> _userManager;
        public HomeController(
            UserManager<VollyV3User> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var application = new HomeApplicationModel();
            if (user != null)
            {
                application.Email = user.Email;
                application.Name = user.FullName;
                application.PhoneNumber = user.PhoneNumber;
            }
            HomeModel model = new HomeModel
            {
                ApplicationModel = application,
                GoogleMapsAPIKey = GoogleMapsAPIKey

            };
            ViewData["OpportunityId"] = id;
            return View(model);
        }
    }
}