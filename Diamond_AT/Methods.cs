using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_AT
{
    class Methods
    {
        /*Date*/
        public static string DateNormalize(string tmpDate)
        {
            var swapDate = tmpDate;
            string[] splitDate = null;
            if (tmpDate.Contains("/"))
            {
                splitDate = tmpDate.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitDate.Count() == 3)
                {
                    return swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
                }
            }
            return tmpDate;
        }

        public static I73Struct Sub1OwnerSplit(string s)
        {
            var pat = new Regex(@"(?<Name>[^,]+),(?<Address>.*)\s\((?<Country>[A-Z]{2})\)\.*");
            var a = pat.Match(s);
            if (a.Success)
            {
                return new I73Struct
                {
                    Name = a.Groups["Name"].Value.Trim(),
                    Address = a.Groups["Address"].Value.Trim(),
                    Country = a.Groups["Country"].Value.Trim()
                };
            }
            else
            {
                Console.WriteLine("Pattern doesn't match");
                return new I73Struct();
            }
        }

        public static string GetDate(string tmpFileName)
        {
            var fName = new FileInfo(tmpFileName);
            var name = fName.Name;
            var datePatternBig = @"^[A-Z]{2}_\d{8}_";
            var datePatternSmall = @"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})";
            if (Regex.IsMatch(name, datePatternBig))
                return
                    Regex.Match(name, datePatternSmall).Groups["year"].Value + "-" +
                    Regex.Match(name, datePatternSmall).Groups["month"].Value + "-" +
                    Regex.Match(name, datePatternSmall).Groups["day"].Value;
            else
                return null;
        }
        public static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            var tempStrC = recString.Trim();
            string tmpDescValue = null;
            if (recString != "")
            {
                if (recString.Contains("(57)"))
                {
                    tmpDescValue = tempStrC.Substring(tempStrC.IndexOf("(57)")).Trim();
                    tempStrC = tempStrC.Remove(recString.IndexOf("(57)")).Trim();
                }
                var regexPatOne = new Regex(@"\(\d{2}\)", RegexOptions.IgnoreCase);
                var matchesClass = regexPatOne.Matches(recString);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (tmpDescValue != null)
            {
                splittedRecord = (splittedRecord ?? Enumerable.Empty<string>()).Concat(new string[] { tmpDescValue }).ToArray();
            }
            return splittedRecord;
        }

        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; //Staging
                var url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; //Production
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
