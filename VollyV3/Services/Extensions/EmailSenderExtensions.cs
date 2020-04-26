using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Services.Extensions
{
    public static class EmailSenderExtensions
    {
        public static async Task<Response> SendNewsletterAsync(
            this IEmailSender emailSender,
            string email,
            string fullName,
            string opportunityHtml)
        {
            var apikey = Environment.GetEnvironmentVariable("sendgrid_api_key");
            var infoFromEmail = Environment.GetEnvironmentVariable("info_from_email");
            var infoFromName = Environment.GetEnvironmentVariable("info_from_name");

            var client = new SendGridClient(apikey);

            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress(infoFromEmail, infoFromName));
            msg.AddTo(new EmailAddress(email, fullName));
            msg.AddTo(new EmailAddress("maillet.mark.test@gmail.com", fullName + "asdfasdf"));
            msg.SetTemplateId(Environment.GetEnvironmentVariable("newsletter_template_id"));
            msg.SetTemplateData(new TemplateData
            {
                OpportunityHtml = opportunityHtml,
            });

            return await client.SendEmailAsync(msg);
        }
        private class TemplateData
        {
            [JsonProperty("opportunity_html")]
            public string OpportunityHtml { get; set; }
        }
    }
}
