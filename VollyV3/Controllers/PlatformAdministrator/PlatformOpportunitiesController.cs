using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using VollyV3.Areas.Identity;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.OrganizationAdministrator.Dto;
using VollyV3.Models.ViewModels.PlatformAdministrator.Opportunities;
using VollyV3.Services.ImageManager;

namespace VollyV3.Controllers.PlatformAdministrator
{
    [Authorize(Roles = nameof(Role.PlatformAdministrator))]
    public class PlatformOpportunitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<VollyV3User> _userManager;
        private readonly IImageManager _imageManager;
        public PlatformOpportunitiesController(ApplicationDbContext context,
            UserManager<VollyV3User> userManager,
            IImageManager imageManager)
        {

            _context = context;
            _userManager = userManager;
            _imageManager = imageManager;
        }
        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            IIncludableQueryable<Opportunity, Organization> opportunitiesQueryable = _context.Opportunities
                .Include(o => o.Category)
                .Include(o => o.CreatedBy)
                .ThenInclude(u => u.Organization);

            List<Opportunity> opportunities = opportunitiesQueryable
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(opportunities
                .Select(opportunity => new OpportunityIndexViewModel()
                {
                    Id = opportunity.Id,
                    OrganizationName = opportunity.CreatedBy?.Organization?.Name,
                    Name = opportunity.Name,
                    Category = opportunity.Category?.Name,
                    OpportunityType = opportunity.OpportunityType
                })
                .ToList());
        }
        /// <summary>
        /// Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Details(int id)
        {
            var opp = _context.Opportunities
                .Where(x => x.Id == id)
                .Include(x => x.Category)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Organization)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return GetDetailsViewResult(opp);
        }
        public ViewResult GetDetailsViewResult(Opportunity opportunity)
        {
            return View(new OpportunityDetailsViewModel()
            {
                OrganizationName = opportunity.CreatedBy?.Organization?.Name,
                Name = opportunity.Name,
                OpportunityType = opportunity.OpportunityType,
                Address = opportunity.Address,
                Description = opportunity.Description,
                Category = opportunity.Category?.Name,
                ImageUrl = opportunity.ImageUrl,
                ContactEmail = opportunity.ContactEmail
            });
        }
        /// <summary>
        /// Create
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            return GetCreateViewResult(user);
        }
        public ViewResult GetCreateViewResult(VollyV3User user)
        {
            return View(new OpportunityCreateViewModel()
            {
                ContactEmail = user.Email,
                Categories = new SelectList(_context.Categories
                .OrderBy(c => c.Name)
                .ToList(), "Id", "Name"),
                Organizations = new SelectList(_context.Organizations
                .OrderBy(x => x.Name)
                .ToList(), "Id", "Name")
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OpportunityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var organization = _context.Organizations
                    .Where(x => x.Id == model.OrganizationId)
                    .FirstOrDefault();

                Opportunity opportunity = model.GetOpportunity(_imageManager);
                opportunity.CreatedBy = _context.OrganizationAdministratorUsers
                    .Where(x => x.User == user && x.Organization == organization)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(model.ContactEmail))
                {
                    opportunity.ContactEmail = user.Email;
                }

                opportunity.CreatedDateTime = DateTime.Now;

                _context.Add(opportunity);

                await _context.SaveChangesAsync();

                if (Opportunity.RequiresOccurrences(model.OpportunityType))
                {
                    return RedirectToAction(nameof(Occurrences), new { id = opportunity.Id });
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="opportunity"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var opportunity = _context.Opportunities
                .Where(x => x.Id == id)
                .Include(x => x.Category)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Organization)
                .FirstOrDefault();

            if (opportunity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new OpportunityEditViewModel()
            {
                Id = opportunity.Id,
                OrganizationName = opportunity.CreatedBy?.Organization?.Name,
                Name = opportunity.Name,
                OpportunityType = opportunity.OpportunityType,
                Description = opportunity.Description,
                Address = opportunity.Address,
                CategoryId = opportunity.Category?.Id,
                Categories = new SelectList(_context.Categories
              .OrderBy(c => c.Name)
              .ToList(), "Id", "Name"),
                ExternalSignUpUrl = opportunity.ExternalSignUpUrl,
                ImageUrl = opportunity.ImageUrl,
                ContactEmail = opportunity.ContactEmail
            });
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(OpportunityEditViewModel model)
        {
            var opp = model.GetOpportunity(_context, _imageManager);

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();

            TempData["Messages"] = $"\"{model.Name}\" successfully saved.";
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var opp = _context.Opportunities
                .Where(x => x.Id == id)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Organization)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new OpportunityDeleteViewModel()
            {
                Id = id,
                OrganizationName = opp.CreatedBy?.Organization?.Name,
                Name = opp.Name,
                Description = opp.Description
            });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(OpportunityDeleteViewModel model)
        {
            var opportunity = _context.Opportunities
                .Where(x => x.Id == model.Id)
                .SingleOrDefault();

            if (opportunity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var volunteerHours = _context.VolunteerHours
                .Where(x => x.Opportunity == opportunity)
                .Count();

            if (volunteerHours > 0)
            {
                TempData["Messages"] = $"\"{opportunity.Name}\" has volunteer hours logged against it and cannot be deleted.";
                return RedirectToAction(nameof(Index));
                //return RedirectToAction(nameof(Archive), new { opportunity.Id });
            }

            var applications = _context.Applications
                .Where(x => x.Opportunity == opportunity)
                .Count();

            if (applications > 0)
            {
                TempData["Messages"] = $"\"{opportunity.Name}\" has applications and cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            _context.Remove(opportunity);

            await _context.SaveChangesAsync();

            TempData["Messages"] = $"\"{opportunity.Name}\" has been deleted.";
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Occurrences
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Occurrences(int id)
        {
            var opp = _context.Opportunities
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new OccurrencesViewModel()
            {
                OpportunityId = opp.Id,
                OpportunityName = opp.Name,
                OpportunityDescription = opp.Description,
                OpportunityAddress = opp.Address,
            });
        }
        [HttpPost]
        public async Task<IActionResult> Occurrences([FromBody] OccurrencePost occurrencePost)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (_context.Opportunities
                .Where(x =>
                x.Id == occurrencePost.OpportunityId
                && x.CreatedBy.User == user
            ).FirstOrDefault() == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var newOccurrence = new Occurrence()
            {
                OpportunityId = occurrencePost.OpportunityId,
                StartTime = DateTime.Parse($"{occurrencePost.StartDate} {occurrencePost.StartTime}"),
                EndTime = DateTime.Parse($"{occurrencePost.EndDate} {occurrencePost.EndTime}"),
                Openings = int.Parse(occurrencePost.Openings)
            };
            if (!string.IsNullOrEmpty(occurrencePost.ApplicationDeadlineDate))
            {
                newOccurrence.ApplicationDeadline = DateTime.Parse($"{occurrencePost.ApplicationDeadlineDate} 12:00am");
            }

            _context.Occurrences.Add(newOccurrence);

            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpGet]
        public IActionResult GetOccurrences(int id)
        {
            return Ok(
                _context.Occurrences
                .Where(x => x.OpportunityId == id)
                .ToList());
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteOccurrence(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var occurrence = _context.Occurrences
                .Where(x =>
                x.Id == id
                && x.Opportunity.CreatedBy.User == user
                ).ToList();
            if (occurrence.Count == 1)
            {
                _context.Remove(occurrence[0]);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}
