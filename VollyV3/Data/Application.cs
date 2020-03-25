using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using VollyV3.Models;
using VollyV3.Models.Users;

namespace VollyV3.Data
{
    public class Application
    {
        public int Id { get; set; }
        public virtual Opportunity Opportunity { get; set; }
        [JsonIgnore]
        public virtual List<OccurrenceApplication> Occurrences { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }
        public virtual VollyV3User VollyV3User { get; set; }
    }
}
