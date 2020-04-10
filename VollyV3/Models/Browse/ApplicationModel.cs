using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Models.Browse
{
    public class ApplicationModel
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
    }
}
