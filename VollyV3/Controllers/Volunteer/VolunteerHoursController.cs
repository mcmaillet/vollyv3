using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VollyV3.Areas.Identity;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.ViewModels.Volunteer.VolunteerHours;

namespace VollyV3.Controllers.Volunteer
{
    [Authorize(Roles = nameof(Role.Volunteer))]
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
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> TrackAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var applications = _context.Applications
                .Where(x => x.User == user)
                .Include(x => x.Opportunity)
                .ThenInclude(x => x.CreatedBy)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Occurrence)
                .ToList();
            var organizations = _context.Organizations
                .Where(x => x.Enabled)
                .ToList();
            return View(new TrackViewModel()
            {
                Applications = applications,
                Organizations = organizations
            });
        }
        /// <summary>
        /// TrackApplication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> TrackApplicationAsync(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var application = _context.Applications
                .Where(x => x.Id == id
                && x.User == user)
                .Include(x => x.Opportunity)
                .ThenInclude(x => x.CreatedBy)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Occurrence)
                .SingleOrDefault();
            if (application == null)
            {
                TempData["Messages"] = "Application not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(application);
        }
        [HttpPost]
        public async Task<IActionResult> TrackApplicationAsync(int id, double hours)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var application = _context.Applications
               .Where(x => x.Id == id && x.User == user)
               .Include(x => x.Opportunity)
               .ThenInclude(x => x.CreatedBy)
               .ThenInclude(x => x.Organization)
               .Include(x => x.Occurrence)
               .SingleOrDefault();
            if (application == null)
            {
                TempData["Messages"] = "Application not found.";
                return RedirectToAction(nameof(Index));
            }
            _context.Add(new VolunteerHours()
            {
                Organization = application.Opportunity.CreatedBy.Organization,
                Opportunity = application.Opportunity,
                User = user,
                DateTime = application.Occurrence?.StartTime,
                Hours = hours
            });
            _context.SaveChanges();
            TempData["Messages"] = $"{hours} hours added.";
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// TrackOrganization
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult TrackOrganization(int id)
        {
            var organization = _context.Organizations
                .Where(x => x.Id == id && x.Enabled)
                .SingleOrDefault();
            if (organization == null)
            {
                TempData["Messages"] = "Organization not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }
        [HttpPost]
        public async Task<IActionResult> TrackOrganizationAsync(TrackOrganizationViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var organization = _context.Organizations
               .Where(x => x.Id == viewModel.Id && x.Enabled)
               .SingleOrDefault();
            if (organization == null)
            {
                TempData["Messages"] = "Organization not found.";
                return RedirectToAction(nameof(Index));
            }
            var volunteerHours = new VolunteerHours()
            {
                Organization = organization,
                User = user,
                Hours = viewModel.Hours
            };
            if (!string.IsNullOrEmpty(viewModel.StartDate) && !string.IsNullOrEmpty(viewModel.StartTime))
            {
                volunteerHours.DateTime = DateTime.Parse($"{viewModel.StartDate} {viewModel.StartTime}");
            }
            _context.Add(volunteerHours);
            _context.SaveChanges();
            TempData["Messages"] = $"{viewModel.Hours} hours added.";
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Edit
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(_context.VolunteerHours
                .Where(x => x.User == user)
                .ToList());
        }
    }
}
