using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using VollyV3.Data;
using VollyV3.Models;

namespace VollyV3.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<VollyV3User> _userManager;
        private readonly SignInManager<VollyV3User> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<VollyV3User> userManager,
            SignInManager<VollyV3User> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Id { get; set; }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Full name")]
            public string FullName { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }
        public IEnumerable<Application> Applications { get; set; }
        public IEnumerable<VolunteerHours> VolunteerHours { get; set; }

        private async Task LoadAsync(VollyV3User user)
        {
            Id = user.Id;

            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            Input = new InputModel
            {
                FullName = user.FullName,
                PhoneNumber = phoneNumber
            };

            Applications = _context.Applications
                .Include(x => x.Opportunity)
                .ThenInclude(x => x.CreatedBy)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Occurrence)
                .Where(x => x.User == user)
                .OrderBy(x => x.Opportunity.CreatedByOrganizationId)
                .ThenBy(x => x.Opportunity.Id)
                .ThenBy(x => x.SubmittedDateTime)
                .ToList();

            VolunteerHours = _context.VolunteerHours
                 .Include(x => x.Opportunity)
                 .Include(x => x.Organization)
                 .Where(x => x.User == user)
                 .OrderBy(x => x.Organization.Id)
                 .ThenBy(x => x.Opportunity.Id)
                 .ThenBy(x => x.DateTime)
                 .ToList();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Input.FullName != user.FullName)
            {
                user.FullName = Input.FullName;
                await _userManager.UpdateAsync(user);
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
