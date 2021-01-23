using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Threading.Tasks;

namespace VollyV3.Services.EmailSender
{
    public interface IEmailSenderExtended : IEmailSender
    {
        Task<HttpStatusCode> SendEmailApplicationConfirmationAsync(
            string applicantName, string applicantEmail,
            string organizationContactEmail,
            string subject, string htmlMessage);
    }
}
