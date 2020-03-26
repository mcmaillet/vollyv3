using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Models.OrganizationAdministrator.Dto
{
    public class OccurrencePost
    {
        public int OpportunityId { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string ApplicationDeadlineDate { get; set; }
        public string Openings { get; set; }
    }
}
