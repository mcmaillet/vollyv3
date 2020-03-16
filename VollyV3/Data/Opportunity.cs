using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VollyV3.Models;
using VollyV3.Models.Users;
using VollyV3.Services.ImageManager;

namespace VollyV3.Data
{
    public enum OpportunityType
    {
        All,
        Episodic,
        Ongoing,
        Group,
        Donation
    }

    public class OpportunityName
    {
        public static Dictionary<OpportunityType, string> MapDictionary = new Dictionary<OpportunityType, string>()
        {
            { OpportunityType.All, "All" },
            { OpportunityType.Episodic, "Events" },
            { OpportunityType.Ongoing, "Ongoing" },
            { OpportunityType.Group, "Group" },
            { OpportunityType.Donation, "Donation" }
        };
    }
    public class Opportunity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public virtual Location Location { get; set; }
        public string ImageUrl { get; set; }
        public string ExternalSignUpUrl { get; set; }
        public string CreatedByUserId { get; set; }
        public OpportunityType OpportunityType { get; set; }
        public virtual OrganizationAdministratorUser CreatedByUser { get; set; }
        public virtual List<OpportunityImage> OpportunityImages { get; set; }
        public virtual List<Occurrence> Occurrences { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public Opportunity Clone()
        {
            return new Opportunity()
            {
                Name = "copy of " + Name,
                Description = Description,
                Address = Address,
                Location = new Location()
                {
                    Longitude = Location.Longitude,
                    Latitude = Location.Latitude
                },
                ImageUrl = ImageUrl,
                ExternalSignUpUrl = ExternalSignUpUrl,
                OpportunityType = OpportunityType,
                CreatedDateTime = DateTime.Now
            };
        }

        public async Task<OpportunityImage> UploadImage(IImageManager imageManager, ApplicationDbContext context, IFormFile imageFile)
        {
            string imageUrl = await imageManager.UploadImageAsync(imageFile, GetImageFileName(imageFile.FileName));
            OpportunityImage image = new OpportunityImage()
            {
                OpportunityId = Id,
                ImageUrl = imageUrl

            };
            context.OpportunityImages.Add(image);
            await context.SaveChangesAsync();

            return image;
        }

        private string GetImageFileName(string fileName)
        {
            return "oppimage" + Id + fileName;
        }
    }
}
