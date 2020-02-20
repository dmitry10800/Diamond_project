using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_MA
{
    class Methods
    {
        public static string DateNormalize(string tmpDate)
        {
            string swapDate = tmpDate;
            string[] splitDate = null;
            if (tmpDate.Contains("."))
            {
                splitDate = tmpDate.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (splitDate.Count() == 3)
                {
                    return swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
                }
            }
            return tmpDate;
        }
        public static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString.Replace("•", "").Trim();
            string tempDesc = null;
            if (tempStrC.Contains(Process.Sub1.I57))
            {
                tempDesc = tempStrC.Substring(tempStrC.IndexOf(Process.Sub1.I57)).Trim();
                tempStrC = tempStrC.Remove(tempStrC.IndexOf(Process.Sub1.I57)).Trim();
            }
            if (tempStrC != "")
            {
                if (tempStrC.Contains("\n"))
                {
                    tempStrC = tempStrC.Replace("\n", " ");
                }
                Regex regexPatOne = new Regex(@"\(\d{2}\)\s", RegexOptions.IgnoreCase);
                MatchCollection matchesClass = regexPatOne.Matches(tempStrC);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempDesc != null)
                {
                    splittedRecord = (splittedRecord ?? Enumerable.Empty<string>()).Concat(new string[] { tempDesc.Replace("\n", " ") }).ToArray();
                }
            }
            return splittedRecord;
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
    }
}
