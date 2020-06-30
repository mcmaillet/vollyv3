using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.Browse;
using VollyV3.Models.ViewModels.Components;
using VollyV3.Services;

namespace VollyV3.Controllers
{
    public class BrowseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly UserManager<VollyV3User> _userManager;
        public BrowseController(
            ApplicationDbContext context,
            IMemoryCache memoryCache,
            UserManager<VollyV3User> userManager)
        {
            _context = context;
            _memoryCache = memoryCache;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var application = new ApplicationModel();
            if (user != null)
            {
                application.Email = user.Email;
                application.Name = user.FullName;
                application.PhoneNumber = user.PhoneNumber;
            }
            BrowseModel browseModel = new BrowseModel
            {
                ApplicationModel = application
            };
            ViewData["OpportunityId"] = id;
            return View(browseModel);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            List<Opportunity> opportunities = await MemoryCacheImpl.GetAllOpportunities(_memoryCache, _context);
            OpportunityViewModel opportunityView = opportunities
                .Where(x => x.Id == id)
                .Select(OpportunityViewModel.FromOpportunity)
                .SingleOrDefault();

            if (opportunityView == null)
            {
                return View(new OpportunityViewModel());
            }
            return View(opportunityView);
        }
        [HttpPost]
        public async Task<IActionResult> ApplyAsync([FromBody] ApplicationModel application)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var opportunity = _context
                .Opportunities
                .Where(x => x.Id == int.Parse(application.OpportunityId))
                .FirstOrDefault();
            var occurrences = _context.Occurrences.Where(x => x.Opportunity == opportunity).ToList();

            if (occurrences.Count == 0)
            {
                _context.Applications.Add(GetBaseApplication(application,
                    opportunity,
                    user));
            }
            else
            {

                var applications = application.Occurrences
                    .Select(occurrence => occurrences.Where(x => x.Id == int.Parse(occurrence)).FirstOrDefault())
                    .Select(x =>
                    {
                        var baseApplication = GetBaseApplication(application,
                            opportunity,
                            user);
                        baseApplication.Occurrence = x;
                        return baseApplication;
                    })
                    .ToList();
                _context.Applications.AddRange(applications);
            }
            _context.SaveChanges();
            return Ok();
        }

        private Application GetBaseApplication(ApplicationModel application,
            Opportunity opportunity,
            VollyV3User user)
        {
            return new Application()
            {
                Opportunity = opportunity,
                Name = application.Name,
                Email = application.Email,
                PhoneNumber = application.PhoneNumber,
                Message = application.Message,
                User = user,
                SubmittedDateTime = DateTime.Now
            };
        }
    }
}