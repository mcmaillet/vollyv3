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

                List<Opportunity> opportunities = await context.Opportunities
                    .Include(o => o.CreatedBy)
                    .ThenInclude(u => u.Organization)
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
                        && (oc.Openings == 0 || oc.Openings > oc.Applications.Count)
                        ).ToList();
                }
                return opportunities;
            });
        }
    }
}
