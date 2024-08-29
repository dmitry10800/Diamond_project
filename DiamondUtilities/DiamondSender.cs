using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace DiamondUtilities
{
    public class DiamondSender
    {
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool sendToProduction)
        {
            var count = 1; 
            foreach (var rec in events)
            {
                Console.WriteLine(count++);
                var tmpValue = JsonConvert.SerializeObject(rec);
                var url = sendToProduction ? @"https://lens.lighthouseip.online/external-api/import/legal-event" : // продакшен
                    @"https://lens-staging.lighthouseip.online/external-api/import/legal-event"; // стейдж
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue, Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                _ = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
