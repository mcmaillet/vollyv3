using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.Apply;
using VollyV3.Services.EmailSender;

namespace VollyV3.Controllers
{
    public class ApplyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<VollyV3User> _userManager;
        private readonly IEmailSenderExtended _emailSender;
        public ApplyController(
            ApplicationDbContext context,
            UserManager<VollyV3User> userManager,
            IEmailSenderExtended emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        [HttpPost]
        [Route("/apply")]
        public async Task<HttpStatusCode> ApplyAsync([FromBody] ApplyApplicationModel application)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var opportunity = _context
                .Opportunities
                .Where(x => x.Id == int.Parse(application.OpportunityId))
                .SingleOrDefault();
            var occurrences = _context.Occurrences.Where(x => x.Opportunity == opportunity).ToList();

            if (occurrences.Count == 0)
            {
                _context.Applications.Add(GetBaseApplication(application,
                    opportunity,
                    user));
            }
            else
            {
                var applications = application.Occurrences
                    .Select(occurrence => occurrences.Where(x => x.Id == int.Parse(occurrence)).FirstOrDefault())
                    .Select(x =>
                    {
                        var baseApplication = GetBaseApplication(application,
                            opportunity,
                            user);
                        baseApplication.Occurrence = x;
                        return baseApplication;
                    })
                    .ToList();
                _context.Applications.AddRange(applications);
            }

            var result = _context.SaveChanges();

            if (result <= 0)
            {
                return HttpStatusCode.BadRequest;
            }

            var responseCode = await _emailSender.SendEmailApplicationConfirmationAsync(
                    application.Name, application.Email,
                    opportunity.ContactEmail,
                    $"Application submitted for '{opportunity.Name}'",
                    ComposeEmailMessageForApplications(application, opportunity, occurrences)
                    );

            return responseCode;
        }

        private static string ComposeEmailMessageForApplications(ApplyApplicationModel application,
            Opportunity opportunity,
            IEnumerable<Occurrence> occurrences)
        {
            List<string> occurrenceStrings = occurrences
                .Select(o =>
                o.StartTime.ToShortDateString() + " " + o.StartTime.ToShortTimeString()
                + " --> "
                + o.EndTime.ToShortDateString() + " " + o.EndTime.ToShortTimeString()
                ).ToList();

            return
                $"<p>Received application: {application.GetEmailMessage()}</p>" +
                $"<p>{opportunity.Description}</p>" +
                $"<p><b>Address: {opportunity.Address}</b></p>" +
                $"<p><b>Times: <p>{string.Join("</p><p>", occurrenceStrings)}</p></b></p>" +
                $"";

        }

        private static Application GetBaseApplication(ApplyApplicationModel application,
                Opportunity opportunity,
                VollyV3User user)
        {
            return new Application()
            {
                Opportunity = opportunity,
                Name = application.Name,
                Email = application.Email,
                PhoneNumber = application.PhoneNumber,
                Message = application.Message,
                User = user,
                SubmittedDateTime = DateTime.Now
            };
        }
    }
}
