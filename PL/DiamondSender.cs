using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    class DiamondSender
    {
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool isStaging)
        {
            string url;
            if (isStaging)
                url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
            else
                url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
            foreach (var iEvent in events)
            {
                var tmpEvent = iEvent;
                tmpEvent.Biblio.Publication.LanguageDesignation = "TestValueLangDesignation";
                string tmpValue = JsonConvert.SerializeObject(tmpEvent);
                //string tmpValue = JsonConvert.SerializeObject(iEvent);
                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync(url, content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
