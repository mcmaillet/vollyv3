using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Models.Contact
{
    public class ContactIndexViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public bool TripCheck { get; set; }
        public string GoogleRecaptchaSiteKey { get; set; }
        public string GetEmailMessage(string ipAddress)
        {
            return "<p>Contact Name: " + Name + "</p>" +
                   "<p>Contact Email: " + Email + "</p>" +
                   "<p>Message: " + Message + "</p>" +
                   "<p>From IP: " + ipAddress + "</p>";
        }
    }
}
