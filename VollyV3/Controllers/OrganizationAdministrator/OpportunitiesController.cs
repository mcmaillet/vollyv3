using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.OrganizationAdministrator.Dto;
using VollyV3.Models.ViewModels.OrganizationAdministrator.Opportunities;
using VollyV3.Models.Volly;
using VollyV3.Services.ImageManager;

namespace VollyV3.Controllers.OrganizationAdministrator
{
    [Authorize(Roles = "OrganizationAdministrator", Policy = "IsConfigured")]
    public class OpportunitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<VollyV3User> _userManager;
        private readonly IImageManager _imageManager;
        public OpportunitiesController(ApplicationDbContext context,
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

            var organizationAdministratorUser = _context.OrganizationAdministratorUsers
                .Include(x => x.Organization)
                .Where(x => x.UserId == user.Id)
                .Single();

            IIncludableQueryable<Opportunity, Organization> opportunitiesQueryable = _context.Opportunities
                .Include(o => o.CreatedByUser)
                .ThenInclude(u => u.Organization);

            List<Opportunity> opportunities = await opportunitiesQueryable
                .Where(x => x.CreatedByUser.Organization.Id == organizationAdministratorUser.Organization.Id)
                .ToListAsync();

            return View(opportunities);
        }
        public IActionResult Create()
        {
            OpportunityModel model = new OpportunityModel
            {

            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(
            "Id," +
            "Name," +
            "Description," +
            "Address," +
            "DateTime," +
            "EndDateTime," +
            "ApplicationDeadline," +
            "Openings," +
            "ImageFile," +
            "ExternalSignUpUrl," +
            "OpportunityType"
            )] OpportunityModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                Opportunity opportunity = model.GetOpportunity(_context, _imageManager);
                opportunity.CreatedByUser = _context.OrganizationAdministratorUsers.
                    Where(x => x.UserId == user.Id)
                    .FirstOrDefault();
                opportunity.CreatedDateTime = DateTime.Now;
                _context.Add(opportunity);
                await _context.SaveChangesAsync();
                if (Opportunity.RequiresOccurences(model.OpportunityType))
                {
                    return RedirectToAction(nameof(Occurrences), new { id = opportunity.Id });
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        /*
         * Delete
         */
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

            return View(new OpportunityModel()
            {
                Id = id,
                Name = opp.Name,
                Description = opp.Description
            });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(OpportunityModel model)
        {
            var opp = _context.Opportunities
                .Where(x => x.Id == model.Id)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _context.Remove(opp);

            await _context.SaveChangesAsync();

            TempData["Messages"] = $"\"{opp.Name}\" has been deleted.";
            return RedirectToAction(nameof(Index));
        }
        /*
         * Details
         */
        [HttpGet]
        public IActionResult Details(int id)
        {
            var opp = _context.Opportunities
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new OpportunityModel()
            {
                Name = opp.Name,
                OpportunityType = opp.OpportunityType,
                Address = opp.Address,
                Description = opp.Description,
                ImageUrl = opp.ImageUrl
            });
        }
        /*
         * Edit
         */
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var opp = _context.Opportunities
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new OpportunityModel()
            {
                Id = id,
                Name = opp.Name,
                OpportunityType = opp.OpportunityType,
                Description = opp.Description,
                Address = opp.Address,
                ExternalSignUpUrl = opp.ExternalSignUpUrl,
                ImageUrl = opp.ImageUrl
            });
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(OpportunityModel model)
        {
            var opp = _context.Opportunities
                .Where(x => x.Id == model.Id)
                .FirstOrDefault();

            if (opp == null)
            {
                return RedirectToAction(nameof(Index));
            }

            opp.Name = model.Name;
            opp.OpportunityType = model.OpportunityType;
            opp.Description = model.Description;
            opp.Address = model.Address;

            await _context.SaveChangesAsync();

            TempData["Messages"] = $"\"{model.Name}\" successfully saved.";
            return RedirectToAction(nameof(Index));
        }
        /*
         * Occurrences
         */
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
            if (!string.IsNullOrEmpty(occurrencePost.ApplicationDeadlineDate))
            {
                _context.Occurrences.Add(
               new Occurrence()
               {
                   OpportunityId = occurrencePost.OpportunityId,
                   StartTime = DateTime.Parse($"{occurrencePost.StartDate} {occurrencePost.StartTime}"),
                   EndTime = DateTime.Parse($"{occurrencePost.EndDate} {occurrencePost.EndTime}"),
                   ApplicationDeadline = DateTime.Parse($"{occurrencePost.ApplicationDeadlineDate} 12:00am"),
                   Openings = int.Parse(occurrencePost.Openings)
               });
            }
            else
            {
                _context.Occurrences.Add(
               new Occurrence()
               {
                   OpportunityId = occurrencePost.OpportunityId,
                   StartTime = DateTime.Parse($"{occurrencePost.StartDate} {occurrencePost.StartTime}"),
                   EndTime = DateTime.Parse($"{occurrencePost.EndDate} {occurrencePost.EndTime}"),
                   Openings = int.Parse(occurrencePost.Openings)
               });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpGet]
        public IActionResult OccurrencesGet(int id)
        {
            return Ok(
                _context.Occurrences
                .Where(x => x.OpportunityId == id)
                .ToList());
        }
    }
}