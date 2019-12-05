using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Data;
using VollyV3.Models;

namespace VollyV3.Services
{
    public class MemoryCacheImpl
    {
        public static async Task<List<Opportunity>> GetAllOpportunities(IMemoryCache memoryCache,
            ApplicationDbContext context)
        {
            return await memoryCache.GetOrCreateAsync(GlobalConstants.OpportunityCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

                List<int> opportunityIds = await context.Occurrences
                    .Where(o => o.ApplicationDeadline > DateTime.Now && o.Applications.Count < o.Openings)
                    .Select(o => o.OpportunityId)
                    .ToListAsync();

                List<Opportunity> opportunities = await context.Opportunities
                    .Where(o => opportunityIds.Contains(o.Id) && o.Approved)
                    .Include(o => o.Category)
                    .Include(o => o.Community)
                    .Include(o => o.Organization)
                    .ThenInclude(o => o.Cause)
                    .Include(o => o.Location)
                    .Include(o => o.Occurrences)
                    .ThenInclude(o => o.Applications)
                    .AsNoTracking()
                    .ToListAsync();

                foreach (Opportunity opportunity in opportunities)
                {
                    opportunity.Occurrences = opportunity.Occurrences
                        .Where(oc => oc.ApplicationDeadline > DateTime.Now && oc.Openings > oc.Applications.Count)
                        .ToList();
                }
                return opportunities;
            });
        }
    }
}
