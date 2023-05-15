using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_AR
{
    class Methods
    {
        public static string DateNormalize(string tmpDate)
        {
            var swapDate = tmpDate;
            string[] splitDate = null;
            if (tmpDate.Contains("/"))
            {
                splitDate = tmpDate.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitDate.Count() == 3)
                {
                    return swapDate = splitDate[2] + "/" + splitDate[1] + "/" + splitDate[0];
                }
            }
            return tmpDate;
        }

        public static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            var tempStrC = recString.Trim();
            string tmpDescValue = null;
            string tmpTitle = null;
            if (recString != "")
            {
                if (Regex.IsMatch(recString, @"\n\(62\)\s") || Regex.IsMatch(recString, @"\n\(71\)\s"))
                {
                    if (Regex.IsMatch(recString, @"\n\(57\)\s"))
                    {
                        string tmpEndString = null;
                        var tmpWholeString = recString.Remove(recString.IndexOf("(57) "));
                        if (Regex.IsMatch(recString, @"\n\(62\)\s"))
                        {
                            tmpEndString = recString.Substring(recString.LastIndexOf("\n(62)"));
                            tmpDescValue = recString.Remove(recString.LastIndexOf("\n(62)")).Substring(recString.IndexOf("\n(57)"));
                            tmpWholeString += tmpEndString;
                        }
                        else if (Regex.IsMatch(recString, @"\n\(71\)\s"))
                        {
                            tmpEndString = recString.Substring(recString.LastIndexOf("\n(71)"));
                            tmpDescValue = recString.Remove(recString.LastIndexOf("\n(71)")).Substring(recString.IndexOf("\n(57)"));
                            tmpWholeString += tmpEndString;
                        }
                        tempStrC = tmpWholeString;
                    }
                }
                var regexPatOne = new Regex(@"\n\(\d{2}\)\s");
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
                if (tmpDescValue != null) splittedRecord = splittedRecord.Concat(new string[] { tmpDescValue.Trim() }).ToArray();
            }
            return splittedRecord.Select(x => x.Trim()).ToArray();
        }

        /*Sending data into Diamond*/
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; // STAGING
                var url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; // PRODUCTION
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }

        public class ApplicantStruct
        {
            public List<string> Name = new List<string>();
            public List<string> Address = new List<string>();
            public List<string> CountryCode = new List<string>();

        }
        public static ApplicantStruct ApplicantIdentification(string tmpRecValue)
        {
            tmpRecValue = Regex.Replace(tmpRecValue, @"(\,)(\s*\bllc)", " $2", RegexOptions.IgnoreCase);
            tmpRecValue = Regex.Replace(tmpRecValue, @"(\,)(\s*\binc\b\.*)", " $2", RegexOptions.IgnoreCase);
            tmpRecValue = Regex.Replace(tmpRecValue, @"(\,)(\s*\bgmbh\b)", " $2", RegexOptions.IgnoreCase);
            tmpRecValue = Regex.Replace(tmpRecValue, @"(\,)(\s*\bsarl\b)", " $2", RegexOptions.IgnoreCase);
            tmpRecValue = Regex.Replace(tmpRecValue, @"(\,)(\s*\bltd\b)", " $2", RegexOptions.IgnoreCase);
            tmpRecValue = Regex.Replace(tmpRecValue, @"(\,)(\s*\blp\b)", " $2", RegexOptions.IgnoreCase);
            tmpRecValue = tmpRecValue.Replace("-\n", "");

            var applicant = new ApplicantStruct();
            string[] splittedString = null;
            var pattern = @"(?<=\,\s*[A-Z]{2}\n|$)";
            //string wholeRecPattern = @"(?<Name>^.*)\n(?<Address>.*)(?<Country>\,\s*[A-Z]{2}$)";
            if (Regex.IsMatch(tmpRecValue, pattern))
            {
                splittedString = Regex.Split(tmpRecValue, pattern).Where(x => x != "" && x != "\n" && x != " ").Select(a => a.Trim()).ToArray();
            }
            if (splittedString != null)
            {
                foreach (var record in splittedString)
                {
                    //if (Regex.IsMatch(record, wholeRecPattern))
                    //{
                    //    applicant.Name.Add(Regex.Match(record, wholeRecPattern).Groups["Name"].Value.Trim());
                    //    applicant.Address.Add(Regex.Match(record, wholeRecPattern).Groups["Address"].Value.Trim());
                    //    applicant.CountryCode.Add(Regex.Match(record, wholeRecPattern).Groups["Country"].Value.Trim().Trim(',').Trim());
                    //}
                    if (record.Contains("\n"))
                    {
                        var tmp = record;
                        try
                        {
                            applicant.CountryCode.Add(tmp.Substring(tmp.LastIndexOf(",")).Trim(',').Trim());
                            tmp = tmp.Remove(tmp.LastIndexOf(",")).Trim(',').Trim();
                            applicant.Name.Add(tmp.Remove(tmp.IndexOf("\n")).Trim());
                            applicant.Address.Add(tmp.Substring(tmp.IndexOf("\n")).Trim(',').Trim().Replace("\n", " "));
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Error in 71 field identification");
                        }
                    }
                    else
                    {
                        //applicant.Address.Add("HEGENHEIMERMATTWEG 91, CH-4123 ALLSCHWIL");
                        //applicant.Name.Add(tmpRecValue);
                        //applicant.CountryCode.Add("CH");
                    }
                }
            }
            if (applicant != null) return applicant;
            return null;
        }
    }
}
