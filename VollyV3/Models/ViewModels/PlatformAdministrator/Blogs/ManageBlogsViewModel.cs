using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VollyV3.Data;

namespace VollyV3.Models.ViewModels.PlatformAdministrator.Blogs
{
    public class ManageBlogsViewModel
    {
        [Display(Name = "Blogs")]
        public IEnumerable<Blog> Blogs { get; set; }
    }
}
