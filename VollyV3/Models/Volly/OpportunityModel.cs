using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VollyV3.Data;
using VollyV3.Services;
using VollyV3.Services.ImageManager;

namespace VollyV3.Models.Volly
{
    public class OpportunityModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Title of Event")]
        public string Name { get; set; }
        [Display(Name = "Describe the volunteer opportunity. Please include minimum age to volunteer and perks of volunteering.")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Where does the volunteering occur? Enter Address or simply write the city name if there are multiple locations.")]
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        [Display(Name = "Upload an image for this event")]
        public IFormFile ImageFile { get; set; }
        [Display(Name = "Enter the URL of the application form or sign up sheet. Ex: volunteersignup.org or signupgenius")]
        public string ExternalSignUpUrl { get; set; }
        [Display(Name = "Opportunity Type")]
        public OpportunityType OpportunityType { get; set; }

        public static OpportunityModel FromOpportunity(ApplicationDbContext dbContext, Opportunity opportunity)
        {
            return new OpportunityModel()
            {
                Id = opportunity.Id,
                Name = opportunity.Name,
                Description = opportunity.Description,
                Address = opportunity.Address,
                ImageUrl = opportunity.ImageUrl,
                ExternalSignUpUrl = opportunity.ExternalSignUpUrl,
                OpportunityType = opportunity.OpportunityType,
            };
        }

        public Opportunity GetOpportunity(ApplicationDbContext context, IImageManager imageManager)
        {
            string imageUrl = ImageFile == null ? null : imageManager.UploadImageAsync(
                ImageFile,
                "opp" + Id + ImageFile.FileName
                ).Result;

            Opportunity opportunity = context.Opportunities.Find(Id) ?? new Opportunity();
            opportunity.Name = Name;
            opportunity.Description = Description;
            opportunity.Address = Address;
            if (imageUrl != null)
            {
                opportunity.ImageUrl = imageUrl;
            }
            opportunity.ExternalSignUpUrl = ExternalSignUpUrl;
            opportunity.Location = GoogleLocator.GetLocationFromAddress(Address);
            opportunity.OpportunityType = OpportunityType;
            return opportunity;
        }
    }
}
