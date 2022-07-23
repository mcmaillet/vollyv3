using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IWebHostEnvironment _environment;
        public PlatformOpportunitiesController(ApplicationDbContext context,
            UserManager<VollyV3User> userManager,
            IImageManager imageManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _imageManager = imageManager;
            _environment = environment;
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
                .Include(o => o.Organization);

            List<Opportunity> opportunities = opportunitiesQueryable
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(opportunities
                .Select(opportunity => new OpportunityIndexViewModel()
                {
                    Id = opportunity.Id,
                    CreatedDateTime = opportunity.CreatedDateTime,
                    OrganizationName = opportunity.Organization.Name,
                    Name = opportunity.Name,
                    Category = opportunity.Category?.Name,
                    OpportunityType = opportunity.OpportunityType,
                    IsArchived = opportunity.IsArchived
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
            var opportunity = _context.Opportunities
                .Where(x => x.Id == id)
                .Include(x => x.Category)
                .Include(x => x.CreatedBy)
                .Include(x => x.Organization)
                .FirstOrDefault();

            if (opportunity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new OpportunityDetailsViewModel()
            {
                Id = id,
                OrganizationName = opportunity.Organization.Name,
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

                if (organization == null)
                {
                    TempData["Messages"] = "Organization not found in request.";
                    return RedirectToAction(nameof(Index));
                }

                var now = DateTime.Now;

                Opportunity opportunity = model.GetOpportunity(_context, _environment, _imageManager);
                opportunity.CreatedBy = user;
                opportunity.Organization = organization;
                opportunity.CreatedDateTime = now;
                opportunity.UpdatedDateTime = now;

                if (string.IsNullOrEmpty(model.ContactEmail))
                {
                    opportunity.ContactEmail = user.Email;
                }

                _context.Add(opportunity);

                await _context.SaveChangesAsync();

                TempData["Messages"] = $"\"{opportunity.Name}\" has been created.";

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
                .Include(x => x.Organization)
                .FirstOrDefault();

            if (opportunity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new OpportunityEditViewModel()
            {
                Id = opportunity.Id,
                OrganizationName = opportunity.Organization.Name,
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
            var opp = model.GetOpportunity(_context, _environment, _imageManager);

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
                .Include(x => x.Organization)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new OpportunityDeleteViewModel()
            {
                Id = id,
                OrganizationName = opp.Organization.Name,
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
                .Where(x => x.OpportunityId == opportunity.Id)
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
                .Include(x => x.Organization)
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
                OrganizationName = opp.Organization.Name,
                OpportunityDescription = opp.Description,
                OpportunityAddress = opp.Address,
            });
        }
        [HttpPost]
        public async Task<IActionResult> Occurrences([FromBody] OccurrencePost occurrencePost)
        {
            if (_context.Opportunities
                .Where(x => x.Id == occurrencePost.OpportunityId)
                .FirstOrDefault() == null)
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
            var occurrences = _context.Occurrences
                .Where(x => x.OpportunityId == id)
                .ToList();
            return Ok(occurrences);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteOccurrence(int id)
        {
            var occurrence = _context.Occurrences
                .Where(x => x.Id == id)
                .SingleOrDefault();

            if (occurrence != null)
            {
                _context.Remove(occurrence);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
        /// <summary>
        /// Archive
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Archive(int id)
        {
            var result = _context.Opportunities
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (result == null)
            {
                return RedirectToAction("Index", "Error");
            }

            return View(result);
        }
        [HttpPost]
        public IActionResult Archive(int id, IFormCollection form)
        {
            var opp = _context.Opportunities
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction("Index", "Error");
            }

            opp.IsArchived = !opp.IsArchived;
            opp.UpdatedDateTime = DateTime.Now;

            _context.SaveChanges();

            TempData["Messages"] = $"'{opp.Name}' was {(opp.IsArchived ? "" : "un")}archived";

            return RedirectToAction(nameof(Index));
        }
    }
}
