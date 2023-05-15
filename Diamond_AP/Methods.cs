using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_AP
{
    class Methods
    {
        public static string DateNormalize(string tmpDate)
        {
            var swapDate = tmpDate;
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
            var tempStrC = recString.Replace("\n", " ").Replace("●●", "").Trim();
            if (recString != "")
            {
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
            return splittedRecord;
        }

        public static string PctSplit(string tmpPct, out string pctNumber)
        {
            pctNumber = null;
            string pctDate = null;
            var pattern = new Regex(@"\d{2}\.\d{2}\.\d{4}", RegexOptions.IgnoreCase);
            var matchesClass = pattern.Matches(tmpPct);
            if (matchesClass.Count == 1)
            {
                foreach (Match matchC in matchesClass)
                {
                    pctDate = DateNormalize(matchC.Value.Trim());
                    pctNumber = tmpPct.Replace(matchC.Value, "").Trim();
                }
            }
            return pctDate;
        }

        /*IPC split*/
        public static string[] IPCSplit(string recString, out string[] ipcYear)
        {
            string[] splittedRecord = null;
            ipcYear = null;
            var tempStrC = recString.Replace("\n", " ").Replace("●●", "").Trim();
            if (recString != "")
            {
                var regexPatOne = new Regex(@"\(\d{4}\.\d{2}\)?", RegexOptions.IgnoreCase);
                var matchesClass = regexPatOne.Matches(recString);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        tempStrC = tempStrC.Replace(matchC.Value, "***");
                        ipcYear = (ipcYear ?? Enumerable.Empty<string>()).Concat(new string[] { matchC.Value.Replace("(", "").Replace(")", "") }).ToArray();
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
            }
            return splittedRecord;
        }

        public static string[] OwnerSplit(string tmpStr)
        {
            string[] splittedOwner = null;
            string[] splittedOwnerTrimed = null;
            tmpStr = tmpStr.Replace(", et al", "").Replace(" and ", ",").Trim();
            if (tmpStr.Contains(","))
            {
                splittedOwner = tmpStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            }
            else
            {
                splittedOwner = (splittedOwner ?? Enumerable.Empty<string>()).Concat(new string[] { tmpStr }).ToArray();
            }
            //else if (tmpStr.Contains(" and ") && !tmpStr.Contains(","))
            //{
            //    splittedOwner = tmpStr.Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries);
            //}
            //else
            //{
            //    splittedOwner = (splittedOwner ?? Enumerable.Empty<string>()).Concat(new string[] { tmpStr }).ToArray();
            //}
            //foreach (var item in splittedOwner)
            //{
            //    splittedOwnerTrimed = (splittedOwnerTrimed ?? Enumerable.Empty<string>()).Concat(new string[] { item.Trim() }).ToArray();
            //}
            //return splittedOwnerTrimed;
            return splittedOwner;
        }

        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                var url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; // Staging
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; // Production
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
