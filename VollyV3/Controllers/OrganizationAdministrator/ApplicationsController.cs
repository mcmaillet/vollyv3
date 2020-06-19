using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VollyV3.Models;

namespace VollyV3.Controllers.OrganizationAdministrator
{
    [Authorize(Roles = "OrganizationAdministrator", Policy = "IsConfigured")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<VollyV3User> _userManager;

        public ApplicationsController(
           ApplicationDbContext context,
           UserManager<VollyV3User> userManager
           )
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var applications = _context.Applications
                .Include(x => x.Opportunity)
                .ThenInclude(x => x.CreatedBy)
                .Include(x => x.Occurrence)
                .Where(x => x.Opportunity.CreatedBy.User == user)
                .OrderBy(x => x.Opportunity.Id)
                .ToList();

            return View(applications);
        }
    }
}