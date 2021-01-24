using CsvHelper.Configuration;
using System;
using VollyV3.Data;

namespace VollyV3.Models.CSVModels
{
    public class VolunteerHoursCSVModel
    {
        public string Company { get; set; }
        public string CompanyCode { get; set; }
        public string User { get; set; }
        public string OrganizationName { get; set; }
        public string OpportunityName { get; set; }
        public DateTime? DateTime { get; set; }
        public double Hours { get; set; }
        public static VolunteerHoursCSVModel FromVolunteerHours(VolunteerHours volunteerHours)
        {
            var csvModel = new VolunteerHoursCSVModel()
            {
                User = volunteerHours.User.Email,
                OrganizationName = volunteerHours.OrganizationName,
                OpportunityName = volunteerHours.OpportunityName,
                Hours = volunteerHours.Hours
            };
            var dateTime = volunteerHours.DateTime;
            if (dateTime != null)
            {
                csvModel.DateTime = dateTime;
            }
            return csvModel;
        }
    }
    public class VolunteerHoursCSVModelMap : ClassMap<VolunteerHoursCSVModel>
    {
        public VolunteerHoursCSVModelMap()
        {
            Map(m => m.Company).Name("Company");
            Map(m => m.CompanyCode).Name("CompanyCode");
            Map(m => m.User).Name("User");
            Map(m => m.OpportunityName).Name("OpportunityName");
            Map(m => m.OrganizationName).Name("OrganizationName");
            Map(m => m.DateTime).Name("Date");
            Map(m => m.Hours).Name("Hours");
        }
    }
}
