using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using VollyV3.Areas.Identity;
using VollyV3.Models;
using VollyV3.Models.ViewModels.PlatformAdministrator.Users;

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
        /// <summary>
        /// Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == id);
            return View(new UserDetailsViewModel()
            {
                User = user,
                Roles = await _userManager.GetRolesAsync(user)
            });
        }
        /// <summary>
        /// PasswordReset
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult PasswordReset(string id)
        {
            return View(
                _userManager.Users.SingleOrDefault(u => u.Id == id));
        }
        [HttpPost]
        public async Task<IActionResult> PasswordResetAsync(string id, IFormCollection collect)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == id);
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);
            return RedirectToAction(nameof(PasswordResetConfirm), new PasswordResetConfirmViewModel()
            {
                PasswordResetUrl = HtmlEncoder.Default.Encode(callbackUrl)
            });
        }
        [HttpGet]
        public IActionResult PasswordResetConfirm(PasswordResetConfirmViewModel model)
        {
            return View(model);
        }
        /// <summary>
        /// Unlock
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Unlock(string id)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == id);
            if (user.LockoutEnd == null)
            {
                TempData["Messages"] = $"User {user.Email} is not locked.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> UnlockAsync(string id, IFormCollection collection)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == id);

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (result.Succeeded)
            {
                TempData["Messages"] = $"User {user.Email} unlocked";
            }

            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Delete(string id)
        {
            return View(
                _userManager.Users.SingleOrDefault(u => u.Id == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, IFormCollection collection)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == id);

            var email = user.Email;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            TempData["Messages"] = $"User {email} deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}