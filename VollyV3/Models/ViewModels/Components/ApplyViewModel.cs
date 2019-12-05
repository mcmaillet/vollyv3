using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Data;

namespace VollyV3.Models.ViewModels.Components
{
    public class ApplyViewModel
    {
        public Opportunity Opportunity { get; set; }
        [Required]
        public int OpportunityId { get; set; }
        [Required]
        [MinLength(1)]
        [DisplayName("Occurrence")]
        public List<int> OccurrenceIds { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public SelectList Occurrences { get; set; }

        public async Task<ApplicationViewModel> GetApplication(ApplicationDbContext context, VollyV3User user)
        {
            Application application = new Application()
            {
                Name = Name,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Message = Message,
                DateTime = DateTime.UtcNow,
                Opportunity = context.Opportunities.Find(OpportunityId)
            };
            if (user != null)
            {
                application.UserId = user.Id;
            }

            context.Applications.Add(application);
            await context.SaveChangesAsync();
            application.Occurrences = UpdateOccurences(context, application);
            return ApplicationViewModel.FromApplication(application);
        }

        private List<ApplicationOccurrence> UpdateOccurences(ApplicationDbContext context, Application application)
        {
            List<ApplicationOccurrence> occurrenceApplications = OccurrenceIds.Select(o => new ApplicationOccurrence()
            {
                Application = application,
                Occurrence = context.Occurrences.Find(o)
            }).ToList();

            context.ApplicationsOccurrence.AddRange(occurrenceApplications);
            context.SaveChangesAsync();
            return occurrenceApplications;
        }
    }
}
