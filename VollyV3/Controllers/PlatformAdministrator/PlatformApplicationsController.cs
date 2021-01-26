using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Areas.Identity;
using VollyV3.Models;

namespace VollyV3.Controllers.PlatformAdministrator
{
    [Authorize(Roles = nameof(Role.PlatformAdministrator))]
    public class PlatformApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlatformApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var applications = _context.Applications
                .Include(x => x.Opportunity)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Occurrence)
                .OrderByDescending(x => x.Opportunity.Id)
                .ToList();

            return View(applications);
        }
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Delete(int id)
        {

            var application = _context.Applications
                .Include(x => x.Opportunity)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Occurrence)
                .Where(x => x.Id == id)
                .SingleOrDefault();

            if (application == null)
            {
                TempData["Messages"] = "Application not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(application);
        }
        [HttpPost]
        public IActionResult Delete(int id, IFormCollection form)
        {
            var application = _context.Applications
                .Where(x => x.Id == id)
                .SingleOrDefault();

            if (application == null)
            {
                TempData["Messages"] = "Application not found.";
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
        public IActionResult Details(int id)
        {

            var application = _context.Applications
                .Include(x => x.Opportunity)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Occurrence)
                .Where(x => x.Id == id)
                .SingleOrDefault();

            if (application == null)
            {
                TempData["Messages"] = "Application not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(application);
        }
    }
}
