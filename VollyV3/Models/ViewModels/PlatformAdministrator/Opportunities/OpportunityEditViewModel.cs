﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using VollyV3.Data;
using VollyV3.Services;
using VollyV3.Services.ImageManager;

namespace VollyV3.Models.ViewModels.PlatformAdministrator.Opportunities
{
    public class OpportunityEditViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Organization (cannot be updated)")]
        public string OrganizationName { get; set; }
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
        public Opportunity GetOpportunity(ApplicationDbContext context, IWebHostEnvironment environment, IImageManager imageManager)
        {
            string imageUrl = null;
            try
            {
                using (var imageStream = ImageFile == null ? new FileStream(environment.WebRootPath + "/images/assets/logo-dark.png", FileMode.Open) : ImageFile.OpenReadStream())
                {
                    var imageFileName = ImageFilenameProducer.Create();
                    imageUrl = imageManager.UploadOpportunityImageAsync(imageStream, imageFileName).Result;
                }
            }
            catch (Exception e)
            {
                var error = new LoggedError()
                {
                    Id = Guid.NewGuid().ToString(),
                    ExceptionType = e?.GetType().FullName,
                    ExceptionMessage = e?.Message,
                    ExceptionStackTrace = e?.StackTrace,
                    Path = "/PlatformOpportunities/Edit",
                    CreatedDateTime = DateTime.Now
                };

                context.LoggedErrors.Add(error);
                context.SaveChangesAsync();
            }

            var opportunity = context.Opportunities.Find(Id);
            opportunity.Name = Name;
            opportunity.Description = Description;
            opportunity.Address = Address;
            opportunity.CategoryId = CategoryId;
            if (imageUrl != null)
            {
                opportunity.ImageUrl = imageUrl;
            }
            opportunity.ExternalSignUpUrl = ExternalSignUpUrl;
            opportunity.Location = GoogleLocator.GetLocationFromAddress(Address);
            opportunity.ContactEmail = ContactEmail;
            opportunity.UpdatedDateTime = DateTime.Now;
            return opportunity;
        }
    }
}
