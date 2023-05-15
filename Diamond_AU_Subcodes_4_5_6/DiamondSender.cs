using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Diamond_AU_Subcodes_4_5_6
{
    class DiamondSender
    {
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            for (var i = 0; i < 1000; i++)
            {
                if (i < events.Count)
                {
                    var tmpValue = JsonConvert.SerializeObject(events[i]);
                    //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; //Staging
                    var url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; //Production
                    var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri(url);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                    var result = httpClient.PostAsync(url, content).Result;
                    var answer = result.Content.ReadAsStringAsync().Result;
                }
            }
        }
    }
}
