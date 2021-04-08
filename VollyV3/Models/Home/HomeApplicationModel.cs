using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VollyV3.Models.Home
{
    public class HomeApplicationModel
    {
        public string OpportunityId { get; set; }
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public ICollection<string> Occurrences { get; set; }
        public string Message { get; set; }
        public string GetEmailMessage()
        {
            return "<p>Applicant Name: " + Name + "<p/>" +
                   "<p>Applicant Email: " + Email + "<p/>" +
                   "<p>Applicant Phone: " + PhoneNumber + "<p/>" +
                   "<p>Message: " + Message + "<p/>";
        }
    }
}
