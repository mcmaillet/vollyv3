using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VollyV3.Helpers;

namespace VollyV3.Data
{
    public class Occurrence
    {
        public int Id { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:ddd MMM d yyyy h:mm tt}")]
        public DateTime StartTime { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:ddd MMM d yyyy h:mm tt}")]
        public DateTime EndTime { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:ddd MMM d yyyy h:mm tt}")]
        public DateTime ApplicationDeadline { get; set; }
        [Required]
        public int Openings { get; set; }
        [JsonIgnore]
        [Required]
        public int OpportunityId { get; set; }
        [JsonIgnore]
        public virtual Opportunity Opportunity { get; set; }
        [JsonIgnore]
        public virtual List<Application> Applications { get; set; }

        public override string ToString()
        {
            return "Start: " + StartTime + " End: " + EndTime;
        }
    }

    public static class OccurrenceTimeZoneConverter
    {
        public static Func<Occurrence, Occurrence> ConvertFromUtc()
        {
            return delegate (Occurrence occurrence)
            {
                occurrence.StartTime = ChronoHelper.ConvertFromUtc(occurrence.StartTime);
                occurrence.EndTime = ChronoHelper.ConvertFromUtc(occurrence.EndTime);
                occurrence.ApplicationDeadline = ChronoHelper.ConvertFromUtc(occurrence.ApplicationDeadline);
                return occurrence;
            };
        }

        public static Func<Occurrence, Occurrence> ConvertToUtc()
        {
            return delegate (Occurrence occurrence)
            {
                occurrence.StartTime = ChronoHelper.ConvertToUtc(occurrence.StartTime);
                occurrence.EndTime = ChronoHelper.ConvertToUtc(occurrence.EndTime);
                occurrence.ApplicationDeadline = ChronoHelper.ConvertToUtc(occurrence.ApplicationDeadline);
                return occurrence;
            };
        }
    }
}
