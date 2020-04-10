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

        public IActionResult Index(int? id)
        {
            BrowseModel browseModel = new BrowseModel
            {
                ApplicationModel = new ApplicationModel()
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
                .FirstOrDefault();

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
            var baseApplication = new Application()
            {
                Opportunity = opportunity,
                Name = application.Name,
                Email = application.Email,
                PhoneNumber = application.PhoneNumber,
                Message = application.Message,
                User = user,
                SubmittedDateTime = DateTime.Now
            };
            if (occurrences.Count == 0)
            {
                _context.Applications.Add(baseApplication);
            }
            else
            {
                foreach (var occurrence in application.Occurrences)
                {
                    baseApplication.Occurrence = occurrences.Where(x => x.Id == int.Parse(occurrence)).FirstOrDefault();
                    _context.Applications.Add(baseApplication);
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}