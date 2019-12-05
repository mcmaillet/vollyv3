using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VollyV3.Data;

namespace VollyV3.Services
{
    public class EmailSender : IEmailSender
    {
        private static readonly string FromEmail = Environment.GetEnvironmentVariable("from_email");
        private static readonly string SendgridApiKey = Environment.GetEnvironmentVariable("sendgrid_api_key");

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
    }
}
