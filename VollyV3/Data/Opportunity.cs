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
    public class Opportunity : IComparable
    {
        private static readonly List<OpportunityType> OpportunityTypesRequiringOccurrences = new List<OpportunityType>()
    {
        OpportunityType.Episodic
    };
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public virtual Location Location { get; set; }
        public virtual Category Category { get; set; }
        public string ImageUrl { get; set; }
        public string ExternalSignUpUrl { get; set; }
        public OpportunityType OpportunityType { get; set; }
        public string CreatedByUserId { get; set; }
        public int CreatedByOrganizationId { get; set; }
        public virtual OrganizationAdministratorUser CreatedBy { get; set; }
        public virtual List<OpportunityImage> OpportunityImages { get; set; }
        public virtual List<Occurrence> Occurrences { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string ContactEmail { get; set; }
        public bool IsArchived { get; set; }

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
                Category = Category,
                ImageUrl = ImageUrl,
                ExternalSignUpUrl = ExternalSignUpUrl,
                OpportunityType = OpportunityType,
                CreatedDateTime = DateTime.Now,
                ContactEmail = ContactEmail,
                IsArchived = IsArchived
            };
        }

        public async Task<OpportunityImage> UploadImage(IImageManager imageManager, ApplicationDbContext context, IFormFile imageFile)
        {
            string imageUrl = await imageManager.UploadOpportunityImageAsync(
                imageFile,
                GetImageFileName(imageFile.FileName));
            OpportunityImage opportunityImage = new OpportunityImage()
            {
                OpportunityId = Id,
                ImageUrl = imageUrl

            };
            context.OpportunityImages.Add(opportunityImage);
            await context.SaveChangesAsync();

            return opportunityImage;
        }

        private string GetImageFileName(string fileName)
        {
            return "oppimage" + Id + fileName;
        }

        public static bool RequiresOccurrences(OpportunityType opportunityType)
        {
            return OpportunityTypesRequiringOccurrences.Contains(opportunityType);
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(((Opportunity)obj).Id);
        }
    }
}
