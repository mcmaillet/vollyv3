using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace VollyV3.Services.SendGrid
{
    public static class SendGridCache
    {
        public static async Task<string> GetAllMarketingSegmentsJson(
            IMemoryCache memoryCache,
            HttpClient client
            )
        {
            return await memoryCache.GetOrCreateAsync(SendGridEndpoints.MarketingSegments, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60 * 1);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, SendGridEndpoints.MarketingSegments);

                HttpResponseMessage responseMessage = await client.SendAsync(request);
                var response = await responseMessage.Content.ReadAsStringAsync();
                return response;
            });
        }

        public static async Task<string> GetAllCustomFieldsJson(
            IMemoryCache memoryCache,
            HttpClient client
            )
        {
            return await memoryCache.GetOrCreateAsync(SendGridEndpoints.MarketingFieldDefinitions, async entry =>
             {
                 entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60 * 12);
                 HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, SendGridEndpoints.MarketingFieldDefinitions);

                 HttpResponseMessage responseMessage = await client.SendAsync(request);
                 var response = await responseMessage.Content.ReadAsStringAsync();
                 return response;
             });
        }

        public static async Task<string> GetAllContactsJson(
            IMemoryCache memoryCache,
            HttpClient client)
        {
            return await memoryCache.GetOrCreateAsync(SendGridEndpoints.MarketingContacts, async entry =>
              {
                  entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60 * 12);
                  HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, SendGridEndpoints.MarketingContacts);

                  HttpResponseMessage responseMessage = await client.SendAsync(request);
                  var response = await responseMessage.Content.ReadAsStringAsync();
                  return response;
              });
        }
    }
}