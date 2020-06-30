using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Data;
using VollyV3.Helpers;

namespace VollyV3.Models.ViewModels.Components
{
    public class OccurrenceViewModel
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public int Openings { get; set; }

        public static OccurrenceViewModel FromOccurrence(Occurrence occurrence)
        {
            var openingsCount = occurrence.Openings - occurrence.Applications.Count;
            return new OccurrenceViewModel
            {
                Id = occurrence.Id,
                StartTime = ChronoHelper.ConvertFromUtc(occurrence.StartTime),
                EndTime = ChronoHelper.ConvertFromUtc(occurrence.EndTime),
                ApplicationDeadline = ChronoHelper.ConvertFromUtc(occurrence.ApplicationDeadline),
                Openings = openingsCount > 0 ? openingsCount : 0
            };
        }
    }
}
