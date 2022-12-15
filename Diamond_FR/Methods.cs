using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_FR
{
    class Methods
    {
        public static string XElemToString(List<XElement> v)
        {
            return string.Join(" ", v.Select(x => x.Value));
        }
        public static string GetDateFromGazette(string v)
        {
            Regex date = new Regex(@"[A-Z]_(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})");
            var _ = date.Match(v);
            if (_.Success)
            {
                return _.Groups["year"].Value + "-" + _.Groups["month"].Value + "-" + _.Groups["day"].Value;
            }
            else return null;
        }
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; // STAGING
                string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; // PRODUCTION
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }

        public static List<string> SplitByNumberAndName(string v)
        {
            v = v.Replace("(21)", "*****(21)").Replace("\n", " ").Trim();
            return v.Split(new string[] { "*****" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        public static List<OutElements.SecondList> GetSplElements(List<string> v)
        {
            List<OutElements.SecondList> elements = new List<OutElements.SecondList>();
            var pat = new Regex(@"\(21\)\s*(?<number>.*)\(71\)(?<name>.*$)");
            foreach (var rec in v)
            {
                if (rec.StartsWith("(21) 2000060459"))
                {

                }
                var k = pat.Match(rec);
                if (k.Success)
                {
                    elements.Add(new OutElements.SecondList
                    {
                        AppNumber = k.Groups["number"].Value.Trim(),
                        RegNumber = k.Groups["number"].Value.Trim(),
                        NatureOfApplication = k.Groups["name"].Value.Trim()
                    });
                }
                else
                {
                    Console.WriteLine("Record doesn't match pattern!" + rec);
                }
            }
            return elements;
        }
    }
}
