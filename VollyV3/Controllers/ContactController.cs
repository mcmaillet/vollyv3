using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Models.Contact;
using VollyV3.Services.EmailSender;

namespace VollyV3.Controllers
{
    public class ContactController : Controller
    {
        private static readonly string GoogleRecaptchaSiteKey = Environment.GetEnvironmentVariable("google_recaptcha_site_key");
        private static readonly string GoogleRecaptchaSiteSecret = Environment.GetEnvironmentVariable("google_recaptcha_site_secret");
        private static readonly string PlatformContactEmail = Environment.GetEnvironmentVariable("platform_contact_email");

        private readonly IEmailSenderExtended _emailSender;

        public ContactController(IEmailSenderExtended emailSender)
        {
            _emailSender = emailSender;
        }
        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactIndexViewModel()
            {
                GoogleRecaptchaSiteKey = GoogleRecaptchaSiteKey
            });
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync(ContactIndexViewModel model)
        {
            if (model.TripCheck)
            {
                return Ok();
            }

            var ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            await _emailSender.SendEmailAsync(PlatformContactEmail, "Message From: " + model.Name, model.GetEmailMessage(ip));

            return RedirectToAction(nameof(Confirm));
        }
        /// <summary>
        /// Confirm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Confirm()
        {
            return View();
        }
    }
}
