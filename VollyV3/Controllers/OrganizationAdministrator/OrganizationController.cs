using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VollyV3.Areas.Identity;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.Users;
using VollyV3.Models.ViewModels.OrganizationAdministrator;
using VollyV3.Services.EmailSender;

namespace VollyV3.Controllers.OrganizationAdministrator
{
    [Authorize(Roles = nameof(Role.OrganizationAdministrator))]
    public class OrganizationController : Controller
    {
        private const int MAX_NUMBER_OF_ORGANIZATIONS_PER_ADMIN = 1;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<VollyV3User> _userManager;
        private readonly IEmailSenderExtended _emailSender;
        public OrganizationController(
            ApplicationDbContext context,
            UserManager<VollyV3User> userManager,
            IEmailSenderExtended emailSender
            )
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var organizationsAdministrating = _context.OrganizationAdministratorUsers
                .Where(x => x.User.Id == user.Id)
                .Include(x => x.Organization)
                .ToList();

            if (organizationsAdministrating.Count == 0)
            {
                return RedirectToAction(nameof(Setup));
            }

            if (organizationsAdministrating.Count != MAX_NUMBER_OF_ORGANIZATIONS_PER_ADMIN)
            {
                TempData["Messages"] = $"You're managing {organizationsAdministrating.Count} organizations." +
                    $" We currently support only {MAX_NUMBER_OF_ORGANIZATIONS_PER_ADMIN}." +
                    $" Contact the platform administrator for resolution.";
                return RedirectToAction("Index", "Error");
            }

            var organization = organizationsAdministrating[MAX_NUMBER_OF_ORGANIZATIONS_PER_ADMIN - 1].Organization;
            return View(new IndexViewModel()
            {
                Id = organization.Id,
                Organization = organization
            });
        }
        /// <summary>
        /// Edit
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditAsync(IndexViewModel newVal)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var organizationAdministratorUsers = _context.OrganizationAdministratorUsers
                .Where(x => x.OrganizationId == newVal.Id && x.UserId == user.Id)
                .ToList();

            if (organizationAdministratorUsers.Count != MAX_NUMBER_OF_ORGANIZATIONS_PER_ADMIN)
            {
                TempData["Messages"] = $"You're only allowed to manage {MAX_NUMBER_OF_ORGANIZATIONS_PER_ADMIN} organizations." +
                    $" It looks like you're managing {organizationAdministratorUsers.Count}." +
                    $" Contact the platform administrator.";
                return RedirectToAction("Index", "Error");
            }

            var oldVal = _context.Organizations
                .Where(x =>
                x.Id == newVal.Id &&
                x.Id == organizationAdministratorUsers[MAX_NUMBER_OF_ORGANIZATIONS_PER_ADMIN - 1].OrganizationId)
                .SingleOrDefault();

            if (oldVal == null)
            {
                TempData["Messages"] = $"Failed to update organization. Id provided: {newVal.Id}";
                return RedirectToAction("Index", "Error");
            }

            var organization = newVal.Organization;
            oldVal.Name = organization.Name;
            oldVal.ContactEmail = organization.ContactEmail;
            oldVal.PhoneNumber = organization.PhoneNumber;
            oldVal.Address = organization.Address;
            oldVal.WebsiteLink = organization.WebsiteLink;
            oldVal.MissionStatement = organization.MissionStatement;
            oldVal.FullDescription = organization.FullDescription;

            _context.SaveChanges();
            
            TempData["Messages"] = $"{newVal.Organization.Name} has been updated";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Setup
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Setup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Setup(SetupViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var organization = new Organization()
            {
                Name = model.OrganizationName,
                ContactEmail = user.NormalizedEmail,
                PhoneNumber = user.PhoneNumber,
                CreatedDateTime = DateTime.Now,
            };
            await _context.Organizations.AddAsync(organization);
            await _context.SaveChangesAsync();

            _context.OrganizationAdministratorUsers.Add(new OrganizationAdministratorUser()
            {
                User = user,
                Organization = organization
            });
            await _context.SaveChangesAsync();

            await _userManager.AddToRoleAsync(user, Enum.GetName(typeof(Role), Role.IsConfigured));

            await _emailSender.SendEmailOrganizationConfiguredAsync(user.NormalizedEmail, model.OrganizationName);

            return RedirectToAction(nameof(SetupConfirm));
        }
        [HttpGet]
        public IActionResult SetupConfirm()
        {
            return View();
        }
    }
}