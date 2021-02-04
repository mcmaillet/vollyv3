using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using VollyV3.Data;

namespace VollyV3.Services.Recaptcha
{
    public class RecaptchaValidator
    {
        private static readonly string GoogleRecaptchaSiteSecret = Environment.GetEnvironmentVariable("google_recaptcha_site_secret");
        public static bool IsRecaptchaValid(string recaptchaResponse)
        {
            var result = false;
            var requestUri = string.Format(
                GlobalConstants.RecaptchaPOSTUrl,
                GoogleRecaptchaSiteSecret,
                recaptchaResponse
                );
            var request = (HttpWebRequest)WebRequest.Create(requestUri);

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    JObject jResponse = JObject.Parse(stream.ReadToEnd());
                    var isSuccess = jResponse.Value<bool>("success");
                    result = isSuccess ? true : false;
                }
            }
            return result;
        }
    }
}
