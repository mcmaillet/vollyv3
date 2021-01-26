using System.Collections.Generic;
using System.Text.Json.Serialization;
using VollyV3.Data;

namespace VollyV3.Models.Users
{
    public class OrganizationAdministratorUser
    {
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual VollyV3User User { get; set; }
        public int OrganizationId { get; set; }
        [JsonIgnore]
        public virtual Organization Organization { get; set; }
    }
}
