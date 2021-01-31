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
        public static async Task<List<Opportunity>> GetOpportunitiesAcceptingApplications(IMemoryCache memoryCache,
            ApplicationDbContext context)
        {
            return await memoryCache.GetOrCreateAsync(GlobalConstants.OpportunityCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

                List<Opportunity> opportunities = await context.Opportunities
                .Where(x => !x.IsArchived)

                .Include(o => o.CreatedBy)
                .Include(o => o.Organization)

                .Where(x => x.Organization.Enabled)

                .Include(o => o.Location)
                .Include(o => o.Occurrences)
                .ThenInclude(o => o.Applications)

                .AsNoTracking()
                .ToListAsync();

                foreach (Opportunity opportunity in opportunities)
                {
                    opportunity.Occurrences = opportunity.Occurrences
                        .Where(oc =>
                        (oc.ApplicationDeadline == DateTime.MinValue || oc.ApplicationDeadline > DateTime.Now)
                        && (oc.Openings == 0 && Opportunity.RequiresOccurrences(opportunity.OpportunityType) || oc.Openings > oc.Applications.Count)
                        && (oc.EndTime > DateTime.Now)
                        ).ToList();
                }
                return opportunities
                .Where(x => !Opportunity.RequiresOccurrences(x.OpportunityType) || x.Occurrences.Count > 0)
                .ToList();
            });
        }
    }
}
