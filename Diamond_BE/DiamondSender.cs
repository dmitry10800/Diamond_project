using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Diamond_BE
{
    class DiamondSender
    {
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            //foreach (var rec in events)
            //{
            //    string tmpValue = JsonConvert.SerializeObject(rec);
            //    string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; //Staging
            //    //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; //Production
            //    HttpClient httpClient = new HttpClient();
            //    httpClient.BaseAddress = new Uri(url);
            //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
            //    var result = httpClient.PostAsync(url, content).Result;
            //    var answer = result.Content.ReadAsStringAsync().Result;
            //}

            for (var i = 0; i < 200; i++)
            {
                if (i < events.Count)
                {
                    var tmpValue = JsonConvert.SerializeObject(events[i]);
                    var url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; //Staging
                    //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; //Production
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
