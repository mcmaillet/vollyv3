using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Models.Browse
{
    public class BrowseModel
    {
        public ApplicationModel ApplicationModel { get; set; }
        public string GoogleMapsAPIKey { get; set; }
    }
}
