using VollyV3.Data;

namespace VollyV3.Models.ViewModels.OrganizationAdministrator.Opportunities
{
    public class OpportunityDetailsViewModel
    {
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public OpportunityType OpportunityType { get; set; }
        public string Address { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
