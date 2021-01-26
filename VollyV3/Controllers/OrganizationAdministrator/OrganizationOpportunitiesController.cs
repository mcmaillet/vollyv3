using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;
using VollyV3.Areas.Identity;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.OrganizationAdministrator.Dto;
using VollyV3.Models.ViewModels.OrganizationAdministrator.Opportunities;
using VollyV3.Services.ImageManager;

namespace VollyV3.Controllers.OrganizationAdministrator
{
    [Authorize(Roles = nameof(Role.OrganizationAdministrator), Policy = "IsConfigured")]
    public class OrganizationOpportunitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<VollyV3User> _userManager;
        private readonly IImageManager _imageManager;
        public OrganizationOpportunitiesController(ApplicationDbContext context,
            UserManager<VollyV3User> userManager,
            IImageManager imageManager)
        {
            _context = context;
            _userManager = userManager;
            _imageManager = imageManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var organizationsManagedByUser = _context.OrganizationAdministratorUsers
                .Where(x => x.User == user)
                .Select(x => x.OrganizationId)
                .ToList();

            IIncludableQueryable<Opportunity, Organization> opportunitiesQueryable = _context.Opportunities
                .Include(o => o.Category)
                .Include(o => o.CreatedBy)
                .Include(o => o.Organization);

            List<Opportunity> opportunities = opportunitiesQueryable
                .Where(x => organizationsManagedByUser.Contains(x.Organization.Id))
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(opportunities
                .Select(opportunity => new OpportunityIndexViewModel()
                {
                    Id = opportunity.Id,
                    Name = opportunity.Name,
                    Category = opportunity.Category?.Name,
                    OpportunityType = opportunity.OpportunityType
                })
                .ToList());
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

                var organizationsManagedByUser = _context.OrganizationAdministratorUsers
                    .Where(x => x.UserId == user.Id)
                    .Select(x => x.Organization)
                    .ToList();

                if (organizationsManagedByUser.Count != 1)
                {
                    return RedirectToAction("Index", "Error");
                }

                Opportunity opportunity = model.GetOpportunity(_imageManager);
                opportunity.CreatedBy = user;
                opportunity.Organization = organizationsManagedByUser[0];
                opportunity.CreatedDateTime = DateTime.Now;

                if (string.IsNullOrEmpty(model.ContactEmail))
                {
                    opportunity.ContactEmail = user.Email;
                }

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
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var opp = _context.Opportunities
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new OpportunityDeleteViewModel()
            {
                Id = id,
                Name = opp.Name,
                Description = opp.Description
            });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(OpportunityDeleteViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var organizationsManagedByUser = _context.OrganizationAdministratorUsers
                .Where(x => x.UserId == user.Id)
                .Select(x => x.OrganizationId)
                .ToList();

            var opportunity = _context.Opportunities
                .Include(x => x.Organization)
                .Where(x => x.Id == model.Id && organizationsManagedByUser.Contains(x.Organization.Id))
                .SingleOrDefault();

            if (opportunity == null)
            {
                TempData["Messages"] = $"\"{opportunity.Name}\" not found for organizations you manage.";
                return RedirectToAction(nameof(Index));
            }

            var volunteerHours = _context.VolunteerHours
                .Where(x => x.OpportunityId == opportunity.Id)
                .ToList();

            if (volunteerHours.Count > 0)
            {
                TempData["Messages"] = $"\"{opportunity.Name}\" has volunteer hours logged against it and cannot be deleted.";
                return RedirectToAction(nameof(Index));
                //return RedirectToAction(nameof(Archive), new { opportunity.Id });
            }

            _context.Remove(opportunity);

            await _context.SaveChangesAsync();

            TempData["Messages"] = $"\"{opportunity.Name}\" has been deleted.";
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Archive
        /// </summary>
        /// <param name="opportunity"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Archive(int id)
        {
            var opportunity = _context.Opportunities
                .Where(x => x.Id == id)
                .SingleOrDefault();
            return View(opportunity);
        }
        [HttpPost]
        public async Task<IActionResult> ArchiveAsync(int id, IFormCollection form)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var organizationsManagedByUser = _context.OrganizationAdministratorUsers
                .Where(x => x.UserId == user.Id)
                .Select(x => x.OrganizationId)
                .ToList();

            var opportunity = _context.Opportunities
                .Include(x => x.Organization)
                .Where(x => x.Id == id && organizationsManagedByUser.Contains(x.Organization.Id))
                .SingleOrDefault();

            if (opportunity == null)
            {
                TempData["Messages"] = $"\"{opportunity.Name}\" not found for organizations you manage.";
                return RedirectToAction(nameof(Index));
            }

            opportunity.IsArchived = true;

            _context.Update(opportunity);

            TempData["Messages"] = $"Opportunity '{opportunity.Name}' has been archived.";
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
            var opp = _context.Opportunities
                .Where(x => x.Id == id)
                .Include(x => x.Category)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new OpportunityDetailsViewModel()
            {
                Name = opp.Name,
                OpportunityType = opp.OpportunityType,
                Address = opp.Address,
                Description = opp.Description,
                Category = opp.Category?.Name,
                ImageUrl = opp.ImageUrl,
                ContactEmail = opp.ContactEmail
            });
        }
        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var opp = _context.Opportunities
                .Where(x => x.Id == id)
                .Include(x => x.Category)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new OpportunityEditViewModel()
            {
                Id = id,
                Name = opp.Name,
                OpportunityType = opp.OpportunityType,
                Description = opp.Description,
                Address = opp.Address,
                CategoryId = opp.Category?.Id,
                Categories = new SelectList(_context.Categories
                .OrderBy(c => c.Name)
                .ToList(), "Id", "Name"),
                ExternalSignUpUrl = opp.ExternalSignUpUrl,
                ImageUrl = opp.ImageUrl,
                ContactEmail = opp.ContactEmail
            });
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(OpportunityEditViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var organizationsManagedByUser = _context.OrganizationAdministratorUsers
                .Where(x => x.UserId == user.Id)
                .Select(x => x.OrganizationId)
                .ToList();

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
        /// Occurrences
        /// </summary>
        /// <param name="id"></param>
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
                .Where(x => x.Id == occurrencePost.OpportunityId && x.CreatedBy == user)
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
                .Where(x => x.Id == id && x.Opportunity.CreatedBy == user)
                .SingleOrDefault();

            if (occurrence != null)
            {
                _context.Remove(occurrence);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}