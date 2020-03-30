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
        public async Task<IActionResult> Search([FromBody] OpportunitySearch opportunitySearch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Opportunity> opportunities = await MemoryCacheImpl.GetAllOpportunities(_memoryCache, _context);
            List<OpportunityViewModel> opportunityViews = opportunities
                .Where(GetEligibleOpportunityPredicate(opportunitySearch))
                .Select(OpportunityViewModel.FromOpportunity)
                .Where(o =>
                opportunitySearch.OpportunityType == OpportunityType.All
                || o.OpportunityType == opportunitySearch.OpportunityType)
                .ToList();
            return Ok(opportunityViews);
        }
        private static Func<Opportunity, bool> GetEligibleOpportunityPredicate(OpportunitySearch opportunitySearch)
        {
            return o => true;
            //return o =>
            //    (opportunitySearch.CauseIds == null || o.Organization.Cause != null &&
            //     opportunitySearch.CauseIds.Contains(o.Organization.Cause.Id)) &&
            //    (opportunitySearch.CategoryIds == null ||
            //     opportunitySearch.CategoryIds.Contains(o.Category.Id)) &&
            //    (opportunitySearch.OrganizationIds == null ||
            //     opportunitySearch.OrganizationIds.Contains(o.Organization.Id)) &&
            //    (opportunitySearch.OpportunityType == OpportunityType.All || opportunitySearch.OpportunityType == o.OpportunityType);
        }
        private List<OpportunityViewModel> Sort(List<OpportunityViewModel> opportunityViews, int sort)
        {
            switch (sort)
            {
                case 1:
                    return opportunityViews.OrderBy(o =>
                    {
                        OccurrenceViewModel firstOcc = o.OccurrenceViews[0];
                        return firstOcc.StartTime;
                    }).ToList();
                case 2:
                    return opportunityViews.OrderBy(o => o.OrganizationName).ToList();
                case 3:
                    return opportunityViews.OrderBy(o => o.OccurrenceViews.Sum(occ => occ.Openings)).ToList();
                case 4:
                    return opportunityViews.OrderBy(o =>
                    {
                        OccurrenceViewModel firstOcc = o.OccurrenceViews[0];
                        return firstOcc.EndTime - firstOcc.StartTime;
                    }).ToList();
                default:
                    return opportunityViews;
            }
        }
    }
}
