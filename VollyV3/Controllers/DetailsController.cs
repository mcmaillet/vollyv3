using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.ViewModels.Components;
using VollyV3.Services;

namespace VollyV3.Controllers
{
    public class DetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        public DetailsController(
            ApplicationDbContext context,
            IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        [HttpGet]
        [Route("/details/{id}")]
        public async Task<IActionResult> IndexAsync(int id)
        {
            List<Opportunity> opportunities = await MemoryCacheImpl.GetOpportunitiesAcceptingApplications(_memoryCache, _context);
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
    }
}
