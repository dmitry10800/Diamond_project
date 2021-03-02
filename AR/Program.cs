using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AR
{
    class Program
    {
        private static readonly string TetmlDirectory = @"C:\Work\AR\AR_20201001_201906";
        private static readonly bool IsStaging = true;
        private static readonly string StagingLensLink = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
        private static readonly string ProductionLensLink = @"https://diamond.lighthouseip.online/external-api/import/legal-event";

        static void Main(string[] args)
        {
            List<XElement> sub2sub3;
            var files = Methods.GetTetmlFiles(TetmlDirectory);
            foreach (var file in files)
            {
                var elements = XElement.Load(file.FullName);

                sub2sub3 = elements.Descendants()
                    .SkipWhile(x => !x.Value.StartsWith(@"<Primera>"))
                    .Where(x => x.Name.LocalName == "Text" && !string.IsNullOrEmpty(x.Value)).ToList();
                var patents = Subcodes.ProcessSubcode(sub2sub3, file.Name.Replace(".tetml", ".pdf"));
                Console.WriteLine();

                SendToDiamond(patents);
            }
        }

        internal static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }




        //private static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        //{
        //    var url = IsStaging ? StagingLensLink : ProductionLensLink;
        //    foreach (var rec in events)
        //    {
        //        string tmpValue = JsonConvert.SerializeObject(rec);
        //        HttpClient httpClient = new HttpClient { BaseAddress = new Uri(url) };
        //        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var result = httpClient.PostAsync("", new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json")).Result;
        //        if (!result.IsSuccessStatusCode)
        //        {
        //            Console.WriteLine($"Error sending event: {result.Content.ReadAsStringAsync().Result}");
        //        }
        //    }
        //}
    }
}
