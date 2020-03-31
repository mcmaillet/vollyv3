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
        public BrowseController(
            ApplicationDbContext context,
            IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
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
    }
}