using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VollyV3.Areas.Identity;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.Users;
using VollyV3.Models.ViewModels.OrganizationAdministrator;
using VollyV3.Models.ViewModels.PlatformAdministrator;

namespace VollyV3.Controllers
{
    [Authorize(Roles = nameof(Role.OrganizationAdministrator))]
    public class OrganizationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<VollyV3User> _userManager;
        public OrganizationController(
            ApplicationDbContext context,
            UserManager<VollyV3User> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var organizationsAdministrating = _context.OrganizationAdministratorUsers
                .Where(x => x.User.Id == user.Id)
                .ToList();

            if (organizationsAdministrating.Count == 0)
            {
                return RedirectToAction(nameof(Setup));
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = nameof(Role.OrganizationAdministrator))]
        public IActionResult Setup()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = nameof(Role.OrganizationAdministrator))]
        public async Task<IActionResult> Setup(SetupViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var organization = new Organization()
            {
                Name = model.OrganizationName,
                ContactEmail = user.NormalizedEmail,
                PhoneNumber = user.PhoneNumber,
                CreatedDateTime = DateTime.Now,
            };
            await _context.Organizations.AddAsync(organization);
            await _context.SaveChangesAsync();

            _context.OrganizationAdministratorUsers.Add(new OrganizationAdministratorUser()
            {
                User = user,
                Organization = organization
            });
            await _context.SaveChangesAsync();

            await _userManager.AddToRoleAsync(user, Enum.GetName(typeof(Role), Role.IsConfigured));

            return RedirectToAction(nameof(SetupConfirm));
        }
        [HttpGet]
        [Authorize(Roles = nameof(Role.OrganizationAdministrator))]
        public IActionResult SetupConfirm()
        {
            return View();
        }
    }
}