using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VollyV3.Models;
using VollyV3.Services.ImageManager;

namespace VollyV3.Data
{
    public enum OpportunityType
    {
        Events,
        Ongoing,
        Group,
        Donation
    }

    public class OpportunityName
    {
        public static Dictionary<OpportunityType, string> MapDictionary = new Dictionary<OpportunityType, string>()
        {
            { OpportunityType.Events, "Events" },
            { OpportunityType.Ongoing, "Ongoing" },
            { OpportunityType.Group, "Group" },
            { OpportunityType.Donation, "Donation" }
        };
    }
    public class Opportunity : IComparable
    {
        private static readonly List<OpportunityType> OpportunityTypesRequiringOccurrences = new List<OpportunityType>()
    {
        OpportunityType.Events,
        OpportunityType.Group
    };
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public virtual Location Location { get; set; }
        public virtual int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public string ImageUrl { get; set; }
        public string ExternalSignUpUrl { get; set; }
        public OpportunityType OpportunityType { get; set; }
        public string CreatedById { get; set; }
        public virtual VollyV3User CreatedBy { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual List<OpportunityImage> OpportunityImages { get; set; }
        public virtual List<Occurrence> Occurrences { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string ContactEmail { get; set; }
        public bool IsArchived { get; set; }
        public DateTime UpdatedDateTime { get; set; }

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
                IsArchived = IsArchived,
                UpdatedDateTime = DateTime.Now
            };
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
