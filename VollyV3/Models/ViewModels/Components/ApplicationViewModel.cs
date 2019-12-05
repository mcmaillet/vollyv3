using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VollyV3.Data;
using VollyV3.Helpers;

namespace VollyV3.Models.ViewModels.Components
{
    public class ApplicationViewModel
    {
        public int Id { get; set; }
        public int OpportunityId { get; set; }
        public string OpportunityName { get; set; }
        public string OpportunityImageUrl { get; set; }
        public string OpportunityDescription { get; set; }
        public string OpportunityAddress { get; set; }
        public List<OccurrenceViewModel> OccurrenceViews { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        [DisplayFormat(DataFormatString = "{0:ddd MMM d yyyy h:mm tt}")]
        public DateTime DateTime { get; set; }
        public string UserName { get; set; }

        public static ApplicationViewModel FromApplication(Application application)
        {
            List<OccurrenceViewModel> occurrenceViews = application.Occurrences
                .Select(ao => ao.Occurrence)
                .Select(OccurrenceViewModel.FromOccurrence)
                .ToList();

            return new ApplicationViewModel()
            {
                Id = application.Id,
                OpportunityId = application.Opportunity.Id,
                OpportunityName = application.Opportunity.Name,
                OpportunityImageUrl = application.Opportunity.ImageUrl,
                OpportunityAddress = application.Opportunity.Address,
                OpportunityDescription = application.Opportunity.Description,
                Name = application.Name,
                Email = application.Email,
                PhoneNumber = application.PhoneNumber,
                Message = application.Message,
                DateTime = ChronoHelper.ConvertFromUtc(application.DateTime),
                OccurrenceViews = occurrenceViews,
                UserName = application.VollyV3User?.UserName
            };
        }
        public string GetEmailMessage()
        {
            return "<p>Applicant Name: " + Name + "<p/>" +
                   "<p>Applicant Email: " + Email + "<p/>" +
                   "<p>Applicant Phone: " + PhoneNumber + "<p/>" +
                   "<p>Message: " + Message + "<p/>";
        }
    }
}
