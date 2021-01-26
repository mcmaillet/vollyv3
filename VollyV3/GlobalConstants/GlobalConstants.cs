using System;

namespace VollyV3.Data
{
    public static class GlobalConstants
    {
        public static readonly string FromEmailName = "Volly Team";
        public static readonly string TimeZoneId = "Mountain Standard Time";
        public static readonly string OpportunityCacheKey = "OpportunityCache";
        public static readonly string OrganizationCacheKey = "OrganizationCache";
        public const string RecaptchaPOSTUrl = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";

        private static readonly Random Random = new Random();
        public static int GetRandom()
        {
            return Random.Next(999999);
        }
    }
}
