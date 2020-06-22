using System;
using VollyV3.Models;

namespace VollyV3.Data
{
    public class VolunteerHours
    {
        public int Id { get; set; }
        public virtual VollyV3User User { get; set; }
        public virtual Opportunity Opportunity { get; set; }
        public DateTime DateTime { get; set; }
        public double Hours { get; set; }

    }
}
