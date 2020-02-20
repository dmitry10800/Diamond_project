using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_MX
{
    class Methods
    {
        static string DateZeroAdd(string s)
        {
            if (s.Length == 1) return 0 + s;
            else return s;
        }
        static string DateMonthToDigit(string s)
        {
            switch (s)
            {
                case var month when new Regex(@"January", RegexOptions.IgnoreCase).IsMatch(month): return "01";
                case var month when new Regex(@"February", RegexOptions.IgnoreCase).IsMatch(month): return "02";
                case var month when new Regex(@"March", RegexOptions.IgnoreCase).IsMatch(month): return "03";
                case var month when new Regex(@"April", RegexOptions.IgnoreCase).IsMatch(month): return "04";
                case var month when new Regex(@"May", RegexOptions.IgnoreCase).IsMatch(month): return "05";
                case var month when new Regex(@"June", RegexOptions.IgnoreCase).IsMatch(month): return "06";
                case var month when new Regex(@"July", RegexOptions.IgnoreCase).IsMatch(month): return "07";
                case var month when new Regex(@"August", RegexOptions.IgnoreCase).IsMatch(month): return "08";
                case var month when new Regex(@"September", RegexOptions.IgnoreCase).IsMatch(month): return "09";
                case var month when new Regex(@"October", RegexOptions.IgnoreCase).IsMatch(month): return "10";
                case var month when new Regex(@"November", RegexOptions.IgnoreCase).IsMatch(month): return "11";
                case var month when new Regex(@"December", RegexOptions.IgnoreCase).IsMatch(month): return "12";
                default: return "00";
            }
        }
        public static string ChooseSubCode(string s)
        {
            string kind = null;
            var pat = new Regex(@"[A-Z]{2}\/(?<kind>[a-z]{1})\/\d{4}\/\d+");
            var m = pat.Match(s);
            if (m.Success)
            {
                kind = m.Groups["kind"].Value;
            }
            else
                Console.WriteLine("AppNumber pattern doesn't match!");

            switch (kind)
            {
                case "a": return "2";
                case "u": return "3";
                default: return null;
            }
        }
        public static string DateProcess(string v)
        {
            var datePat = new Regex(@"(?<day>\d+)\s(?<month>\w+)\s(?<year>\d{4})");
            var a = datePat.Match(v);
            if (a.Success)
            {
                return a.Groups["year"].Value + "-" + DateMonthToDigit(a.Groups["month"].Value) + "-" + DateZeroAdd(a.Groups["day"].Value);
            }
            else
                Console.WriteLine($"Date pattern doesn't match\t {v}");
            return v;
        }
        public static List<OutElements.NameAddressCountry> NamesProcess(string s)
        {
            List<OutElements.NameAddressCountry> values = new List<OutElements.NameAddressCountry>();
            var pattern = new Regex(@"(?<name>.*),\s*(?<address>[^,]+),\s*(?<country>[A-Z]{2})$");
            if (s != null)
            {
                List<string> tmpValues = s.Replace("\n", " ").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var item in tmpValues)
                {
                    var match = pattern.Match(item);
                    if (match.Success)
                    {
                        values.Add(new OutElements.NameAddressCountry
                        {
                            Name = match.Groups["name"].Value.Trim()
                        });
                    }
                    else
                        Console.WriteLine($"Pattern doesn't match!{item}");
                }
            }
            return values;
        }
        public static string DateNormalize(string tmpDate)
        {
            Regex patternDate = new Regex(@"(?<day>\d+)\s*\.*\/*\-*(?<month>\d+)\s*\.*\/*\-*(?<year>\d{4})");
            var a = patternDate.Match(tmpDate);
            if (a.Success)
            {
                return a.Groups["year"].Value + "-" + a.Groups["month"].Value + "-" + a.Groups["day"].Value;
            }
            else return tmpDate;
        }
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
    }
}
