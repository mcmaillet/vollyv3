using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.Search;
using VollyV3.Models.ViewModels.Components;
using VollyV3.Services;

namespace VollyV3.Controllers.API
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public SearchController(ApplicationDbContext context,
            IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        [Route("/api/Search/Opportunities")]
        public async Task<IActionResult> Search([FromBody] OpportunitySearch model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var opportunities = await MemoryCacheImpl.GetOpportunitiesAcceptingApplications(_memoryCache, _context);

            var eligibleOpportunities = opportunities
                .Where(GetEligibleOpportunityPredicate(model));

            var totalCount = eligibleOpportunities.Count();

            return Ok(new OpportunitySearchResult()
            {
                TotalCount = totalCount,
                Opportunities = eligibleOpportunities
                .Select(OpportunityViewModel.FromOpportunity)
                .OrderByDescending(x => x.Id)
                .Skip((model.Page - 1) * model.Limit)
                .Take(model.Limit)
                .ToList()
            });
        }
        private static Func<Opportunity, bool> GetEligibleOpportunityPredicate(OpportunitySearch opportunitySearch)
        {
            return o => opportunitySearch.OpportunityType == -1 || (int)o.OpportunityType == opportunitySearch.OpportunityType;
        }
    }
}
