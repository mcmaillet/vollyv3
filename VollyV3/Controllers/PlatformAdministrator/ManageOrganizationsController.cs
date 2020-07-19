using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VollyV3.Areas.Identity;
using VollyV3.Models;
using VollyV3.Models.ViewModels.PlatformAdministrator.Organizations;

namespace VollyV3.Controllers.PlatformAdministrator
{
    [Authorize(Roles = nameof(Role.PlatformAdministrator))]
    public class ManageOrganizationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ManageOrganizationsController(
            ApplicationDbContext context
            )
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(new ManageOrganizationsViewModel()
            {
                Organizations = _context.Organizations.ToList()
            });
        }

        public IActionResult Details(int id)
        {
            var organization = _context.Organizations
                .Where(x => x.Id == id)
                .SingleOrDefault();
            return View(organization);
        }
        [HttpGet]
        public IActionResult ToggleStatus(int id)
        {
            var organization = _context.Organizations
                .Where(x => x.Id == id)
                .SingleOrDefault();
            return View(organization);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStatusAsync(int id)
        {
            var organization = _context.Organizations
                .Where(x => x.Id == id)
                .SingleOrDefault();
            organization.Enabled = !organization.Enabled;
            await _context.SaveChangesAsync();
            TempData["Messages"] = $"{organization.Name} has been {(organization.Enabled? "enabled":"disabled")}";
            return RedirectToAction(nameof(Index));
        }
    }
}