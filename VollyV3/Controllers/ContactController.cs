using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VollyV3.Data;
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
            if (!IsRecaptchaValid() || model.TripCheck)
            {
                return Ok();
            }

            var ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            await _emailSender.SendEmailAsync(PlatformContactEmail, "Message From: " + model.Name, model.GetEmailMessage(ip));

            return RedirectToAction(nameof(Confirm));
        }
        private bool IsRecaptchaValid()
        {
            var result = false;
            var requestUri = string.Format(
                GlobalConstants.RecaptchaPOSTUrl,
                GoogleRecaptchaSiteSecret,
                Request.Form["g-recaptcha-response"]
                );
            var request = (HttpWebRequest)WebRequest.Create(requestUri);

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    JObject jResponse = JObject.Parse(stream.ReadToEnd());
                    var isSuccess = jResponse.Value<bool>("success");
                    result = isSuccess ? true : false;
                }
            }
            return result;
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
