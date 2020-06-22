using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Data;

namespace VollyV3.Models.ViewModels.Volunteer.VolunteerHours
{
    public class TrackViewModel
    {
        public IEnumerable<Application> Applications { get; set; }
        public IEnumerable<Organization> Organizations { get; set; }
    }
}
