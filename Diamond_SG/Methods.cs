using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_SG
{
    class Methods
    {
        public static string XElemToString(List<XElement> v)
        {
            return string.Join(" ", v.Select(x => x.Value))
                .Replace("PATENTS CEASED THROUGH NON PAYMENT OF RENEWAL FEES - cont’d", "")
                .Replace("PATENTS CEASED THROUGH NON PAYMENT OF RENEWAL FEES", "")
                .Replace("PATENTS CEASED AFTER THE TERM OF THE PATENT - cont’d", "")
                .Replace("PATENTS CEASED AFTER THE TERM OF THE PATENT", "");
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
                string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; // STAGING
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; // PRODUCTION
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
            var splittedRecords = new List<string>();
            v = v.Replace("\n", " ");
            var yearPattern = new Regex(@"(\d+th\sYear)");
            var recordPattern = new Regex(@"(?<AppNumber>\(21\).*)(?<Owner>\(71\).*)");
            var tmpList = yearPattern.Split(v);
            if (tmpList.Count() > 1)
            {
                foreach (var rec in tmpList)
                {
                    if (yearPattern.Match(rec).Success)
                    {
                        splittedRecords.Add(rec);
                    }
                    else if (rec.Trim().StartsWith("(21)"))
                    {
                        var a = rec.Trim().Replace("(21)", "*****(21)").Replace("\n", " ").Trim();
                        var b = a.Split(new string[] { "*****" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        splittedRecords.AddRange(b);
                    }
                }
            }
            else
            {
                var a = v.Trim().Replace("(21)", "*****(21)").Replace("\n", " ").Trim();
                var b = a.Split(new string[] { "*****" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                splittedRecords.AddRange(b.Where(x => x.StartsWith("(21)")));
            }
            return splittedRecords;
        }
        public static List<OutElements.SecondList> GetSplElements(List<string> v)
        {
            List<OutElements.SecondList> elements = new List<OutElements.SecondList>();
            var yearPattern = new Regex(@"(\d+th\sYear)");
            var pat = new Regex(@"\(21\)\s*(?<number>.*)\(71\)(?<name>.*$)");
            string tmpYear = null;
            foreach (var rec in v)
            {
                var k = pat.Match(rec);
                var m = yearPattern.Match(rec);
                if (k.Success)
                {
                    elements.Add(new OutElements.SecondList
                    {
                        AppNumber = k.Groups["number"].Value.Trim(),
                        OwnerName = k.Groups["name"].Value.Trim(),
                        Year = tmpYear
                    });
                }
                else if (m.Success)
                {
                    tmpYear = m.Value.Trim();
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
