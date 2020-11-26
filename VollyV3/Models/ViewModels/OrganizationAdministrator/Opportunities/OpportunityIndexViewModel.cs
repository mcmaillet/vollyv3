using System.ComponentModel.DataAnnotations;
using VollyV3.Data;

namespace VollyV3.Models.ViewModels.OrganizationAdministrator.Opportunities
{
    public class OpportunityIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }
        public OpportunityType OpportunityType { get; set; }
    }
}
