using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VollyV3.Areas.Identity;
using VollyV3.Models;

namespace VollyV3.Controllers.PlatformAdministrator
{
    [Authorize(Roles = nameof(Role.PlatformAdministrator))]
    public class UserController : Controller
    {
        private readonly UserManager<VollyV3User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            UserManager<VollyV3User> userManager,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View(
                _userManager.Users
                .ToList()
                );
        }

        public ActionResult Delete(string id)
        {
            return View(
                _userManager.Users.SingleOrDefault(u => u.Id == id)
                );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            VollyV3User user = _context.Users.SingleOrDefault(u => u.Id == id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}