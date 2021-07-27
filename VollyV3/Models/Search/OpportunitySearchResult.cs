using System.Collections.Generic;
using VollyV3.Models.ViewModels.Components;

namespace VollyV3.Models.Search
{
    public class OpportunitySearchResult
    {
        public int TotalCount { get; set; }
        public List<OpportunityViewModel> Opportunities { get; set; }
    }
}
