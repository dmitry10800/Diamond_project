using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_BG
{
    class Methods
    {
        /*Date*/
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
            string tmpDescValue = null;
            var tempStrC = recString.Trim();
            var I32 = "(32) ";
            var I33 = "(33) ";
            if (tempStrC.Contains("(57) "))
            {
                tmpDescValue = tempStrC.Substring(tempStrC.IndexOf("(57) ")).Trim();
                tempStrC = tempStrC.Remove(tempStrC.IndexOf("(57) ")).Trim();
            }
            if (tempStrC != "")
            {
                var regexPatOne = new Regex(@"\(\d{2}\)\s", RegexOptions.IgnoreCase);
                var matchesClass = regexPatOne.Matches(tempStrC);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        if (!matchC.Value.StartsWith(I32) && !matchC.Value.StartsWith(I33))
                        {
                            tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                        }
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmpDescValue != null)
                {
                    splittedRecord = splittedRecord.Concat(new string[] { tmpDescValue }).ToArray();
                }
            }
            return splittedRecord;
        }
        /*splitting method for UM*/
        public static string[] RecSplitUM(string recString)
        {
            string[] splittedRecord = null;
            string tmpDescValue = null;
            var tempStrC = recString.Trim();
            var I32 = "(32) ";
            var I33 = "(33) ";
            if (tempStrC.Contains("(57) "))
            {
                tmpDescValue = tempStrC.Substring(tempStrC.IndexOf("(57) ")).Trim();
                tempStrC = tempStrC.Remove(tempStrC.IndexOf("(57) ")).Trim();
            }
            if (recString != "")
            {
                var regexPatOne = new Regex(@"\(\d{2}\)\s", RegexOptions.IgnoreCase);
                var matchesClass = regexPatOne.Matches(recString);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        if (!matchC.Value.StartsWith(I32) && !matchC.Value.StartsWith(I33))
                        {
                            tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                        }
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmpDescValue != null)
                {
                    splittedRecord = splittedRecord.Concat(new string[] { tmpDescValue }).ToArray();
                }
            }
            return splittedRecord;
        }

        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; //STAGING
                var url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; //PRODUCTION
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
