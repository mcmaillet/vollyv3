﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using VollyV3.Models;
using VollyV3.Services.EmailSender;
using VollyV3.Services.Recaptcha;

namespace VollyV3.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<VollyV3User> _signInManager;
        private readonly UserManager<VollyV3User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSenderExtended _emailSender;

        public RegisterModel(
            UserManager<VollyV3User> userManager,
            SignInManager<VollyV3User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSenderExtended emailSender
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public string GoogleRecaptchaSiteKey { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "I work at a non profit and want to register an organization account to recruit volunteers")]
            public bool IsOrganizationAdministrator { get; set; }

            public bool TripCheck { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            GoogleRecaptchaSiteKey = Environment.GetEnvironmentVariable("google_recaptcha_site_key");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            GoogleRecaptchaSiteKey = Environment.GetEnvironmentVariable("google_recaptcha_site_key");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!RecaptchaValidator.IsRecaptchaValid(Request.Form["g-recaptcha-response"]) || Input.TripCheck)
            {
                ModelState.AddModelError(string.Empty, "Captcha invalid");
                return Page();
            }

            if (ModelState.IsValid)
            {
                var user = new VollyV3User
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    CreatedDateTime = DateTime.Now
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email", ComposeEmailMessage(callbackUrl));

                    await _userManager.AddToRoleAsync(user, Enum.GetName(typeof(Role), Role.Volunteer));
                    if (Input.IsOrganizationAdministrator)
                    {
                        await _userManager.AddToRoleAsync(user, Enum.GetName(typeof(Role), Role.OrganizationAdministrator));
                    }

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private string ComposeEmailMessage(string callbackUrl)
        {
            var emailMessage = "";
            if (Input.IsOrganizationAdministrator)
            {
                emailMessage += "<p>" +
                    "Your account has been created!" +
                    "<br />" +
                    "We must approve all accounts to make sure your organization falls within our community guidelines. You can make a posting now which will be published once your account is approved." +
                    "</p>";
            }
            emailMessage += $"<p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</p>";
            return emailMessage;
        }
    }
}
