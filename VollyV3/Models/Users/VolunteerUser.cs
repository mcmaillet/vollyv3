using System.Text.Json.Serialization;

namespace VollyV3.Models.Users
{
    public class VolunteerUser
    {
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual VollyV3User User { get; set; }
    }
}
