using System.ComponentModel.DataAnnotations;
using VollyV3.Data;

namespace VollyV3.Models.ViewModels.PlatformAdministrator.Opportunities
{
    public class OpportunityIndexViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Organization")]
        public string OrganizationName { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        [Display(Name = "Type")]
        public OpportunityType OpportunityType { get; set; }
    }
}
