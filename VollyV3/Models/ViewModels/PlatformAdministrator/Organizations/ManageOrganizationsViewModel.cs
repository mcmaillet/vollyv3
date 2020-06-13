using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Data;

namespace VollyV3.Models.ViewModels.PlatformAdministrator
{
    public class ManageOrganizationsViewModel
    {
        [Display(Name = "Organizations")]
        public IEnumerable<Organization> Organizations  { get; set; }
    }
}
