using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Models.ViewModels.OrganizationAdministrator
{
    public class IndexViewModel
    {
        public string OrganizationName { get; set; }
        public string OrganizationUrl { get; set; }
        public string OrganizationDescription { get; set; }
        public DateTime OrganizationCreatedDateTime { get; set; }
    }
}
