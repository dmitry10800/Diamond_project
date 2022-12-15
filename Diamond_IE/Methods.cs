using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_IE
{
    class Methods
    {
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                if (rec.SubCode == "6")
                {

                }
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
        public static string XElemToString(List<XElement> v)
        {
            return string.Join(" ", v.Select(x => x.Value));
        }
        public static List<string> SplitByNumberAndName(string v)
        {
            v = v.Replace("\n", " ").Trim();
            return Regex.Split(v, @"(?=\bS*\d{4}\d+\b)").ToList();
        }
        public static List<OutElements.FirstList> GetSplElements(List<string> v)
        {
            List<OutElements.FirstList> elements = new List<OutElements.FirstList>();
            //var pat = new Regex(@"(?<number>S*\d+)\b\s*(?<title>[^.]+)\.(?<name>.*$)");
            //var pat = new Regex(@"(?<number>S*\d+)\b\s*(?<intClass>Int\.\s*Cl\.\(\d{4}\.\d{2}\)([A-Z]\d{2}[A-Z]\s*\d+\/\d{2}\;*)+\s*[A-Z]\d{2}[A-Z]\s*\d+\/\d{2}\.)*(?<title>[^.]+)\.(?<name>.*$)");
            var pat = new Regex(@"(?<number>S*\d+)\b\s*(?<intClass>Int\.\s*Cl\..*\d{2}\.)*\s*(?<title>[^.]+)\.(?<name>.*$)");
            var versionPat = new Regex(@"\((?<version>\d{4}\.\d{2})\)");
            var intClassPat = new Regex(@"[A-Z]\d{2}[A-Z]\s*\d+\/\d{2}");
            foreach (var rec in v)
            {
                var k = pat.Match(rec);
                if (k.Success)
                {
                    OutElements.FirstList record = new OutElements.FirstList();
                    record.PatNumber = k.Groups["number"].Value.Trim();
                    record.Title = k.Groups["title"].Value.Trim();
                    record.AppName = k.Groups["name"].Value.Trim();
                    if (k.Groups["intClass"].Value.Trim() != "")
                    {
                        record.IpcVersion = versionPat.Match(k.Groups["intClass"].Value.Trim()).Groups["version"].Value + ".01";

                        var matches = intClassPat.Matches(k.Groups["intClass"].Value.Trim());
                        record.IpcClass = new List<string>();
                        foreach (Match item in matches)
                        {
                            record.IpcClass.Add(item.Value.Trim());
                        }
                    }
                    elements.Add(record);
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
