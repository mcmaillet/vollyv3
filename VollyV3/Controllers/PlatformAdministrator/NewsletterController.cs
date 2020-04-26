using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using VollyV3.Areas.Identity;
using VollyV3.Controllers.Extensions;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.ViewModels.PlatformAdministrator.Newsletters;
using VollyV3.Services;
using VollyV3.Services.Extensions;

namespace VollyV3.Controllers.PlatformAdministrator
{
    [Authorize(Roles = nameof(Role.PlatformAdministrator))]
    public class NewsletterController : Controller
    {
        private static readonly string opportunityUrl = "https://vollyv3.azurewebsites.net/Browse/Details/";
        private static readonly int TakeFromTopCount = 20;
        private static readonly int NumberOfOpportunitiesToInclude = 4;

        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memory;
        private readonly IEmailSender _emailSender;


        public NewsletterController(
            ApplicationDbContext context,
            IMemoryCache memory,
            IEmailSender emailSender
            )
        {
            _context = context;
            _memory = memory;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View(new IndexViewModel());
        }
        //
        // Send
        //
        [HttpGet]
        public IActionResult Send()
        {
            return View(new SendViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> SendAsync()
        {
            var opportunities = await GetRandomRecentOpportunities(TakeFromTopCount, NumberOfOpportunitiesToInclude);
            await CreateAndSendSendGridNewsletterAsync(opportunities);
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> CreateAndSendSendGridNewsletterAsync(List<Opportunity> opportunities)
        {
            var html = await GenerateSendGridHtmlFromOpportunitiesAsync(opportunities);

            await _emailSender.SendNewsletterAsync("maillet.mark@gmail.com", "test subject", html);

            return Ok();
        }
        private async Task<string> GenerateSendGridHtmlFromOpportunitiesAsync(List<Opportunity> opportunities)
        {
            ViewData["OpportunitiesHtml"] = GetOpportunitiesHtml(opportunities);
            return await this.RenderViewAsync("NewsletterSendGrid");
        }

        private string GetOpportunitiesHtml(List<Opportunity> opportunities)
        {
            string html = "<div style='background-color:#271f41;'>";
            for (var i = 0; i < opportunities.Count; i++)
            {
                html += GetTemplateForOpportunity(opportunities[i], i % 2 == 0);
            }
            html += "</div>";
            return html;
        }

        private string GetTemplateForOpportunity(Opportunity opportunity, bool IsTextLeft)
        {
            return "<table border='0' cellpadding='0' cellspacing='0' width='100%'>" +
                "<tbody>" +
                "<tr>" +
                "<td valign='top' style='padding:9px;'>" +
                "<table border='0' cellpadding='0' cellspacing='0' width='100%'>" +
                "<tbody>" +
                "<tr>" +
                "<td valign ='top' style='padding:0 9px ;'>" +
                 GetTextForTile(opportunity, IsTextLeft) +
                 GetImageForTile(opportunity, !IsTextLeft) +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>";
        }

        private string GetTextForTile(Opportunity opportunity, bool IsLeft)
        {
            return "<table align='" + (IsLeft ? "left" : "right") + "' border='0' cellpadding='0' cellspacing='0' width='264'>" +
            "<tbody>" +
            "<tr>" +
            "<td valign='top'>" +
            "<h3>" +
            "<font color='#ffffff'>" + opportunity.Name + "<br/>" + "</font>" +
            "</h3>" +
            "<p><span style='color:#FFFFFF'>" + opportunity.Description + "</span><br />" +
            "<a href='"+ opportunityUrl+"" + opportunity.Id + "' target='_blank'>" +
            "<span style='color:#EE82EE'>Sign up</span>" +
            "</a>" +
            "</p>" +
            "</td>" +
            "</tr>" +
            "</tbody>" +
            "</table>";

        }

        private string GetImageForTile(Opportunity opportunity, bool IsLeft)
        {
            return "<table align='" + (IsLeft ? "left" : "right") + "' border='0' cellpadding='0' cellspacing='0' width='264'>" +
                "<tbody>" +
                "<tr>" +
                "<td align='center' valign='top'>" +
                "<img alt='' src='" + opportunity.ImageUrl + "' width='264' style='max-width:564px;'>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>";
        }

        private async Task<List<Opportunity>> GetRandomRecentOpportunities(
            int takeFromTopCount,
            int numberOfOpportunitiesToInclude)
        {
            var opportunities = await MemoryCacheImpl.GetAllOpportunities(_memory, _context);

            opportunities.Sort();
            opportunities.Reverse();

            if (opportunities.Count > takeFromTopCount)
            {
                opportunities = opportunities.Take(takeFromTopCount).ToList();
            }

            var filteredOpportunities = opportunities;

            if (filteredOpportunities.Count > numberOfOpportunitiesToInclude)
            {
                filteredOpportunities = new List<Opportunity>();
                var filteredOpportunityIds = new List<int>();
                Random random = new Random();
                while (filteredOpportunities.Count < numberOfOpportunitiesToInclude)
                {
                    var opportunity = opportunities[random.Next(opportunities.Count)];
                    if (!filteredOpportunityIds.Contains(opportunity.Id))
                    {
                        filteredOpportunities.Add(opportunity);
                        filteredOpportunityIds.Add(opportunity.Id);
                    }
                }
            }

            return filteredOpportunities;
        }

    }
}