using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Data;

namespace VollyV3.Models.Search
{
    public class OpportunitySearch
    {
        public int OpportunityType { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Sort { get; set; }
    }
}
