using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Models.ViewModels.OrganizationAdministrator
{
    public class SetupViewModel
    {
        [Required]
        [Display(Name = "Organization name")]
        public string OrganizationName { get; set; }
    }
}
