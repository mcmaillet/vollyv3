using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Services.SendGrid.Results
{
    public class CustomFields
    {
        public IEnumerable<Result> custom_fields { get; set; }
        public class Result
        {
            public string id { get; set; }
            public string name { get; set; }
            public string field_type { get; set; }
        }
    }
}
