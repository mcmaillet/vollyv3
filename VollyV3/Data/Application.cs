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
        public virtual Occurrence Occurrence { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public DateTime SubmittedDateTime { get; set; }
        public virtual VollyV3User User { get; set; }
    }
}
