using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_EA
{
    class Methods
    {
        public static string XElemToString(List<XElement> v)
        {
            return string.Join(" ", v.Select(x => x.Value));
        }
        public static string GetDateFromGazette(string v)
        {
            var date = new Regex(@"[A-Z]_(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})");
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
                var tmpValue = JsonConvert.SerializeObject(rec);
                var url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; // STAGING
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; // PRODUCTION
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
        //public static List<OutElements.Subcode12> GetSplElements(List<XElement> v)
        //{
        //    var values = v.Select(x => x.Value.Replace("\n", " ").Trim()).ToList();
        //    List<OutElements.Subcode12> elements = new List<OutElements.Subcode12>();
        //    var pat = new Regex(@"(?<number>\b\d+\b)\s*(?<nameOld>.*)The patent has been assigned to(?<nameNew>.*)");
        //    foreach (var rec in values)
        //    {
        //        if (!rec.Contains("The patent has been assigned to"))
        //            Console.WriteLine("Separation words was not found!\t" + rec);
        //        var k = pat.Match(rec);
        //        if (k.Success)
        //        {
        //            elements.Add(new OutElements.Subcode12
        //            {
        //                AppNumber = k.Groups["number"].Value.Trim(),
        //                LePatNumber = k.Groups["number"].Value.Trim(),
        //                NameOld = k.Groups["nameOld"].Value.Trim(),
        //                NameNew = k.Groups["nameNew"].Value.Trim()
        //            });
        //        }
        //        else
        //        {
        //            Console.WriteLine("Record doesn't match pattern!" + rec);
        //        }
        //    }
        //    return elements;
        //}
    }
}
