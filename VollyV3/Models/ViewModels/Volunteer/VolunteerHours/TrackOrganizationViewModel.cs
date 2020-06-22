using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Models.ViewModels.Volunteer.VolunteerHours
{
    public class TrackOrganizationViewModel
    {
        public int Id { get; set; }
        public double Hours { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
    }
}
