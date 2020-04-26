using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Data;

namespace VollyV3.Models.ViewModels.PlatformAdministrator.Newsletters
{
    public class SendViewModel
    {
        [Display(Name ="Subject line")]
        public string NewsletterSubject { get; set; }
        public List<Opportunity> Opportunities { get; set; }
        public ICollection<int> OpportunityIds { get; set; }
    }
}
