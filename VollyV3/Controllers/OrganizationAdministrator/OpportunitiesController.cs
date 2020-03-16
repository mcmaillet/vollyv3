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
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.Volly;
using VollyV3.Services.ImageManager;

namespace VollyV3.Controllers.OrganizationAdministrator
{
    [Authorize(Roles = "OrganizationAdministrator")]
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
            VollyV3User user = await _userManager.GetUserAsync(HttpContext.User);

            var organizationAdministratorUser = _context.OrganizationAdministratorUsers.ToList();
                
            IIncludableQueryable<Opportunity, Organization> opportunitiesQueryable = _context.Opportunities
                .Include(o => o.CreatedByUser)
                .ThenInclude(u => u.Organization);

            List<Opportunity> opportunities = await opportunitiesQueryable
                //.Where(x => x.CreatedByUser.Organization == organizationAdministratorUser.Organization)
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
                return RedirectToAction(nameof(Index));
                //return RedirectToAction(nameof(Create), "Occurrences", new { opportunityId = opportunity.Id });
            }
            return View(model);
        }
        //public async Task<IActionResult> Index()
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);
        //    var organizationUser = _context.OrganizationAdministratorUsers
        //        .Where(x => x.UserId == user.Id)
        //        .Single();
        //    var organization = _context.Organizations
        //        .Where(x => x.Id == organizationUser.OrganizationId)
        //        .Single();

        //    return View();
        //}
    }
}