using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Data;

namespace VollyV3.Models.Search
{
    public class OpportunitySearch
    {
        public List<int> CauseIds { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<int> OrganizationIds { get; set; }
        public List<int> CommunityIds { get; set; }
        public OpportunityType OpportunityType { get; set; }
        public List<DateTime> Dates { get; set; }
        public int Sort { get; set; }
    }
}
