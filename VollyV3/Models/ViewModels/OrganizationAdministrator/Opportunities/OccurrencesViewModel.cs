using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Data;

namespace VollyV3.Models.ViewModels.OrganizationAdministrator.Opportunities
{
    public class OccurrencesViewModel
    {
        public int OpportunityId { get; set; }
        public string OpportunityName { get; set; }
        public string OpportunityDescription { get; set; }
        public string OpportunityAddress { get; set; }
    }
}
