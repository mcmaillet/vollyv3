using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VollyV3.Areas.Identity;
using VollyV3.Data;
using VollyV3.Models;

namespace VollyV3.Controllers.OrganizationAdministrator
{
    [Authorize(Roles = nameof(Role.OrganizationAdministrator), Policy = "IsConfigured")]
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
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var application = _context.Applications
                .Include(x => x.Opportunity)
                .ThenInclude(x => x.CreatedBy)
                .Include(x => x.Occurrence)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if(application == null)
            {
                TempData["Messages"] = "Application not found.";
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if(application.Opportunity.CreatedBy.User != user)
            {
                TempData["Messages"] = "You are not the owner of that opportunity. You cannot modify applications.";
                return RedirectToAction(nameof(Index));
            }
            return View(application);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id, IFormCollection form)
        {
            var application = _context.Applications
                .Include(x => x.Opportunity)
                .ThenInclude(x => x.CreatedBy)
                .Include(x => x.Occurrence)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (application == null)
            {
                TempData["Messages"] = "Application not found.";
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (application.Opportunity.CreatedBy.User != user)
            {
                TempData["Messages"] = "You are not the owner of that opportunity. You cannot modify applications.";
                return RedirectToAction(nameof(Index));
            }
            _context.Remove(application);
            _context.SaveChanges();
            TempData["Messages"] = $"Application deleted.";
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var application = _context.Applications
                .Include(x => x.Opportunity)
                .ThenInclude(x => x.CreatedBy)
                .Include(x => x.Occurrence)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (application == null)
            {
                TempData["Messages"] = "Application not found.";
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (application.Opportunity.CreatedBy.User != user)
            {
                TempData["Messages"] = "You are not the owner of that opportunity. You cannot modify applications.";
                return RedirectToAction(nameof(Index));
            }
            return View(application);
        }
    }
}