using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Areas.Identity;

namespace VollyV3.Models.ViewModels.PlatformAdministrator.Users
{
    public class UserDetailsViewModel
    {
        public VollyV3User User { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
