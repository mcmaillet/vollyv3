using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VollyV3.Models;

namespace VollyV3.Controllers.OrganizationAdministrator
{
    [Authorize(Roles = "OrganizationAdministrator")]
    public class OpportunitiesController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<VollyV3User> _userManager;
        public OpportunitiesController(ApplicationDbContext context,
            UserManager<VollyV3User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var organizationUser = _context.OrganizationAdministratorUsers
                .Where(x => x.UserId == user.Id)
                .Single();
            var organization = _context.Organizations
                .Where(x => x.Id == organizationUser.OrganizationId)
                .Single();

            return View();
        }
    }
}