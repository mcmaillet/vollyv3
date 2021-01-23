using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.Browse;
using VollyV3.Models.ViewModels.Components;
using VollyV3.Services;
using VollyV3.Services.EmailSender;

namespace VollyV3.Controllers
{
    public class BrowseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly UserManager<VollyV3User> _userManager;
        private readonly IEmailSenderExtended _emailSender;
        public BrowseController(
            ApplicationDbContext context,
            IMemoryCache memoryCache,
            UserManager<VollyV3User> userManager,
            IEmailSenderExtended emailSender)
        {
            _context = context;
            _memoryCache = memoryCache;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        /// <summary>
        /// Index
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> IndexAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var application = new ApplicationModel();
            if (user != null)
            {
                application.Email = user.Email;
                application.Name = user.FullName;
                application.PhoneNumber = user.PhoneNumber;
            }
            BrowseModel browseModel = new BrowseModel
            {
                ApplicationModel = application
            };
            ViewData["OpportunityId"] = id;
            return View(browseModel);
        }
        /// <summary>
        /// Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            List<Opportunity> opportunities = await MemoryCacheImpl.GetOpportunitiesAcceptingApplications(_memoryCache, _context);
            OpportunityViewModel opportunityView = opportunities
                .Where(x => x.Id == id)
                .Select(OpportunityViewModel.FromOpportunity)
                .SingleOrDefault();

            if (opportunityView == null)
            {
                return View(new OpportunityViewModel());
            }
            return View(opportunityView);
        }
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpStatusCode> ApplyAsync([FromBody] ApplicationModel application)
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

            return await _emailSender.SendEmailApplicationConfirmationAsync(
                    application.Name, application.Email,
                    opportunity.ContactEmail,
                    $"Application submitted for '{opportunity.Name}'",
                    ComposeEmailMessageForApplications(application, opportunity, occurrences)
                    );
        }

        private static string ComposeEmailMessageForApplications(ApplicationModel application,
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

        private static Application GetBaseApplication(ApplicationModel application,
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