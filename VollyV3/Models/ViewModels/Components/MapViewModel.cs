using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Models.ViewModels.Components
{
    public class MapViewModel
    {
        public SelectList CausesList { get; set; }
        public SelectList CategoriesList { get; set; }
        public SelectList OrganizationList { get; set; }
        public SelectList CommunityList { get; set; }
        public List<int> Causes { get; set; }
        public List<int> Categories { get; set; }
        public List<int> Organizations { get; set; }
        public List<int> Communities { get; set; }
        public ApplyViewModel ApplyModel { get; set; }
    }
}
