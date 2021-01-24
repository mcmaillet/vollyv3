using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using VollyV3.Data;

namespace VollyV3.Services.EmailSender
{
    public class EmailSender : IEmailSenderExtended
    {
        private static readonly string FromEmail = Environment.GetEnvironmentVariable("from_email");
        private static readonly string SendgridApiKey = Environment.GetEnvironmentVariable("sendgrid_api_key");
        private static readonly string ApplicationsCCEmail = Environment.GetEnvironmentVariable("applications_cc_email");
        private static readonly bool SendApplicationsCCOrganizationEmailFeature = bool.TryParse(Environment.GetEnvironmentVariable("send_applications_cc_organization_email"), out bool result) && result;

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await SendEmailsAsync(new List<string> { email }, subject, message);
        }

        public async Task SendEmailsAsync(List<string> emailList, string subject, string message)
        {
            var client = new SendGridClient(SendgridApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(FromEmail, GlobalConstants.FromEmailName),
                Subject = subject,
                HtmlContent = message,
                PlainTextContent = message
            };

            foreach (string email in emailList)
            {
                msg.AddTo(new EmailAddress(email));
            }

            msg.SetClickTracking(false, false);

            await client.SendEmailAsync(msg);
        }
        public async Task SendAccountCreatedConfirm(string email, string password)
        {
            var client = new SendGridClient(SendgridApiKey);

            string messageText = "Welcome to volly!<br/>" +
                                 "An account has been created for you automatically!<br/>" +
                                 "Login with your Email and temporary password:<br/><br/>" +
                                 $"Email: {email}<br/>" +
                                 $"Password: {password}";

            SendGridMessage sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(FromEmail, GlobalConstants.FromEmailName),
                Subject = "Volly: Account auto created",
                HtmlContent = messageText,
                PlainTextContent = messageText
            };
            await client.SendEmailAsync(sendGridMessage);
        }

        async Task<HttpStatusCode> IEmailSenderExtended.SendEmailApplicationConfirmationAsync(
            string applicantName, string applicantEmail,
            string organizationContactEmail,
            string subject, string htmlMessage)
        {
            var client = new SendGridClient(SendgridApiKey);

            SendGridMessage sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(FromEmail, "Volly Team"),
                Subject = subject,
                HtmlContent = htmlMessage,
                PlainTextContent = htmlMessage
            };

            if (!string.IsNullOrEmpty(applicantEmail))
            {
                sendGridMessage.AddTo(new EmailAddress(applicantEmail.ToLower().Trim(), applicantName));

                var ccEmails = new HashSet<string>();

                if (SendApplicationsCCOrganizationEmailFeature)
                {
                    if (!string.IsNullOrWhiteSpace(organizationContactEmail)
                    && !organizationContactEmail.ToLower().Trim().Equals(applicantEmail.ToLower().Trim())
                    )
                    {
                        ccEmails.Add(organizationContactEmail.ToLower().Trim());
                    }
                }

                if (!string.IsNullOrWhiteSpace(ApplicationsCCEmail)
                    && !ApplicationsCCEmail.ToLower().Trim().Equals(applicantEmail.ToLower().Trim())
                    )
                {
                    ccEmails.Add(ApplicationsCCEmail.ToLower().Trim());
                }

                foreach (var email in ccEmails)
                {
                    sendGridMessage.AddCc(new EmailAddress(email));
                }
            }

            var result = await client.SendEmailAsync(sendGridMessage);

            return result.StatusCode;
        }
    }
}
