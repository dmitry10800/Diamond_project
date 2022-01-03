using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BA
{
    class Program
    {
        private static readonly string TetmlDirectory = @"C:\Work\BA\BA_20201231_04";
        private static readonly bool IsStaging = true;
        private static readonly string StagingLensLink = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
        private static readonly string ProductionLensLink = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
        private static readonly string Sub4Key = @"OBJAVA PROŠIRENIH EVROPSKIH PATENATA UPISANIH U REGISTAR PATENATA";

        public static object JsonConvert { get; private set; }

        static void Main(string[] args)
        {
            List<XElement> sub4elements;
            var files = Methods.GetTetmlFiles(TetmlDirectory);
            foreach (var file in files)
            {
                var elements = XElement.Load(file.FullName);
                if (elements?.Descendants().Where(x => x.Value.Contains(Sub4Key)).Count() > 0)
                {
                    sub4elements = elements.Descendants().Where(x => x.Name.LocalName == "Text" && !x.Value.Contains(Sub4Key) && !string.IsNullOrEmpty(x.Value)).ToList();
                    var patents = Subcodes.ProcessSubcode4(sub4elements, file.Name.Replace(".tetml", ".pdf"));
        //            SendToDiamond(patents);
                }
            }
        }

        private static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            var url = IsStaging ? StagingLensLink : ProductionLensLink;
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                HttpClient httpClient = new HttpClient{ BaseAddress = new Uri(url) };
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = httpClient.PostAsync("", new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json")).Result;
                if (!result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error sending event: {result.Content.ReadAsStringAsync().Result}");
                }
            }
        }
    }
}
