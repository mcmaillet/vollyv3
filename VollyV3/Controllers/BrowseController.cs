using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VollyV3.Models;
using VollyV3.Models.ViewModels.Components;

namespace VollyV3.Controllers
{
    public class BrowseController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public BrowseController(ApplicationDbContext dbContext,
            UserManager<VollyV3User> userManager,
            SignInManager<VollyV3User> signInManager)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index(int Id = -1)
        {
            MapViewModel mapModel = new MapViewModel
            {
                CategoriesList = new SelectList(_dbContext.Categories
                    .OrderBy(c => c.Name)
                    .ToList(), "Id", "Name"),

                CausesList = new SelectList(_dbContext.Causes
                    .OrderBy(c => c.Name)
                    .ToList(), "Id", "Name"),

                OrganizationList = new SelectList(_dbContext.Organizations
                    .Where(o => o.Opportunities.Count > 0)
                    .OrderBy(c => c.Name)
                    .AsNoTracking()
                    .ToList(), "Id", "Name"),

                ApplyViewModel = new ApplyViewModel()
            };

            ViewData["OpportunityId"] = Id;
            return View(mapModel);
        }
    }
}