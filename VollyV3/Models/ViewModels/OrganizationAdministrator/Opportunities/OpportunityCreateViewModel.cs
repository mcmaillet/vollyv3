using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.IO;
using VollyV3.Data;
using VollyV3.Services;
using VollyV3.Services.ImageManager;

namespace VollyV3.Models.ViewModels.OrganizationAdministrator.Opportunities
{
    public class OpportunityCreateViewModel
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
        [Display(Name = "Choose a category")]
        public int? CategoryId { get; set; }
        public SelectList Categories { get; set; }
        public string ImageUrl { get; set; }
        [Display(Name = "Upload an image for this event (supported formats: png, jpg, bmp)")]
        public IFormFile ImageFile { get; set; }
        [Display(Name = "Enter the URL of the application form or sign up sheet. Ex: volunteersignup.org or signupgenius")]
        public string ExternalSignUpUrl { get; set; }
        [Display(Name = "Opportunity Type")]
        public OpportunityType OpportunityType { get; set; }
        [Display(Name = "Contact Email")]
        [EmailAddress]
        public string ContactEmail { get; set; }
        public Opportunity GetOpportunity(IWebHostEnvironment environment, IImageManager imageManager)
        {
            var imageStream = ImageFile == null ? new FileStream(environment.WebRootPath + "/images/assets/logo-dark.png", FileMode.Open) : ImageFile.OpenReadStream();
            var imageFileName = ImageFilenameProducer.Create();
            string imageUrl = imageManager.UploadOpportunityImageAsync(imageStream, imageFileName).Result;

            return new Opportunity
            {
                Name = Name,
                Description = Description,
                Address = Address,
                CategoryId = CategoryId,
                ImageUrl = imageUrl,
                ExternalSignUpUrl = ExternalSignUpUrl,
                Location = GoogleLocator.GetLocationFromAddress(Address),
                OpportunityType = OpportunityType,
                ContactEmail = ContactEmail
            };
        }
    }
}
