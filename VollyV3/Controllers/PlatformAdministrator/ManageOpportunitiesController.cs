using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VollyV3.Areas.Identity;
using VollyV3.Controllers.OrganizationAdministrator;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.ViewModels.PlatformAdministrator.Opportunities;
using VollyV3.Services.ImageManager;

namespace VollyV3.Controllers.PlatformAdministrator
{
    [Authorize(Roles = nameof(Role.PlatformAdministrator))]
    public class ManageOpportunitiesController : OpportunitiesController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<VollyV3User> _userManager;
        private readonly IImageManager _imageManager;
        public ManageOpportunitiesController(ApplicationDbContext context,
            UserManager<VollyV3User> userManager,
            IImageManager imageManager) : base(context, userManager, imageManager)
        {

            _context = context;
            _userManager = userManager;
            _imageManager = imageManager;
        }
        public override ViewResult GetIndexViewResult(List<Opportunity> opportunities)
        {

            return View(opportunities
                .Select(opportunity => new OpportunityIndexViewModel()
                {
                    Id = opportunity.Id,
                    OrganizationName = opportunity.CreatedBy?.Organization?.Name,
                    Name = opportunity.Name,
                    Category = opportunity.Category?.Name,
                    ImageUrl = opportunity.ImageUrl,
                    OpportunityType = opportunity.OpportunityType
                })
                .ToList());
        }
    }
}
