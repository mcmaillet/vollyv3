using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VollyV3.Data;
using VollyV3.Models.Users;

namespace VollyV3.Models
{
    // Add profile data for application users by adding properties to the VollyV3User class
    public class VollyV3User : IdentityUser
    {
        public DateTime CreatedDateTime { get; set; }
        public string FullName { get; set; }
    }
}
