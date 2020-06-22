using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VollyV3.Models;

namespace VollyV3.Controllers.Volunteer
{
    public class VolunteerHoursController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<VollyV3User> _userManager;
        public VolunteerHoursController(ApplicationDbContext context,
            UserManager<VollyV3User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var applications = _context.Applications
                .Where(x => x.User == user)
                .Include(x => x.Opportunity)
                .ThenInclude(x => x.CreatedBy)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Occurrence)
                .ToList();
            return View(applications);
        }
        public IActionResult Track(int id)
        {
            var opportunity = _context.Opportunities
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (opportunity != null)
            {
                return RedirectToAction("Manage", "Identity/Account");
            }
            return View(opportunity);
        }
    }
}
