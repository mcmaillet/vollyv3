using System;
using VollyV3.Models;

namespace VollyV3.Data
{
    public class VolunteerHours
    {
        public int Id { get; set; }
        public virtual VollyV3User User { get; set; }
        public int? OpportunityId { get; set; }
        public string OpportunityName { get; set; }
        public int? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public DateTime? DateTime { get; set; }
        public double Hours { get; set; }

    }
}
