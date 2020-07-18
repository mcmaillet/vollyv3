using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Services.SendGrid.Results
{
    public class Contacts
    {
        public IEnumerable<Result> result { get; set; }
        public class Result
        {
            public string id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public ContactCustomFields custom_fields { get; set; }
        }
        public class ContactCustomFields
        {
            public string volly_newsletter { get; set; }
        }
    }
}
