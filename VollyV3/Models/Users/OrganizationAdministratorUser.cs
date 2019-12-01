using System.Text.Json.Serialization;
using VollyV3.Data;
using VollyV3.GlobalConstants;

namespace VollyV3.Models.Users
{
    public class OrganizationAdministratorUser
    {
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual VollyV3User User { get; set; }
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
