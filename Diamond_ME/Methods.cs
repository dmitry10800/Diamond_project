using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_ME
{
    class Methods
    {
        /*Date*/
        public static string DateNormalize(string tmpDate)
        {
            Regex patternDate = new Regex(@"(?<day>\d+)\s*\.*\/*\-*(?<month>\d+)\s*\.*\/*\-*(?<year>\d{4})");
            var a = patternDate.Match(tmpDate);
            if (a.Success)
            {
                return a.Groups["year"].Value + "-" + a.Groups["month"].Value + "-" + a.Groups["day"].Value;
            }
            else
                return tmpDate;
        }

        public static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString.Replace("(32)", ".").Replace("(33)", ".").Trim();
            tempStrC = Regex.Replace(tempStrC, @"\(97\)\s+[A-Z]+.*\d+\n", "\n");
            if (tempStrC != "")
            {
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
            }
            return splittedRecord;
        }
        /*splitting method for UM*/
        public static string[] RecSplitUM(string recString)
        {
            string[] splittedRecord = null;
            string tmpDescValue = null;
            string tempStrC = recString.Trim();
            string I32 = "(32) ";
            string I33 = "(33) ";
            if (tempStrC.Contains("(57) "))
            {
                tmpDescValue = tempStrC.Substring(tempStrC.IndexOf("(57) ")).Trim();
                tempStrC = tempStrC.Remove(tempStrC.IndexOf("(57) ")).Trim();
            }
            if (recString != "")
            {
                Regex regexPatOne = new Regex(@"\(\d{2}\)\s", RegexOptions.IgnoreCase);
                MatchCollection matchesClass = regexPatOne.Matches(recString);
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

        internal static List<ProcessPublicationsOfEuroApp.ElementOut.IPCStruct> IPCProcess(string v)
        {
            List<ProcessPublicationsOfEuroApp.ElementOut.IPCStruct> ipcList = new List<ProcessPublicationsOfEuroApp.ElementOut.IPCStruct>();
            ProcessPublicationsOfEuroApp.ElementOut.IPCStruct ipc;
            List<string> tmpStrings = new List<string>();
            string pattern = @"(?<value>[A-Z]{1}\d{2}[A-Z]{1}\s*\d+\/\d+)\s*(?<date>\d{4})$";
            if (v.Contains("\n"))
            {
                tmpStrings = v.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            }
            else
            {
                tmpStrings.Add(v.Trim());
            }
            foreach (var item in tmpStrings)
            {
                ipc = new ProcessPublicationsOfEuroApp.ElementOut.IPCStruct();
                var k = Regex.Match(item, pattern);
                if (k != null)
                {
                    ipc.Number = k.Groups["value"].Value;
                    ipc.Version = k.Groups["date"].Value + "/01/01";
                    ipcList.Add(ipc);
                }
            }
            return ipcList;
        }
        /*96*/
        internal static ProcessPublicationsOfEuroApp.ElementOut.I96Struct I96Process(string s)
        {
            ProcessPublicationsOfEuroApp.ElementOut.I96Struct tmpValues = new ProcessPublicationsOfEuroApp.ElementOut.I96Struct();
            var pattern = @"(?<number>[A-Z]+.*)\s*\,*\b(?<month>\d+)\/(?<day>\d+)\/(?<year>\d{4})";
            if (Regex.IsMatch(s, pattern))
            {
                var k = Regex.Match(s, pattern);
                string tmpDay = k.Groups["day"].Value;
                string tmpMonth = k.Groups["month"].Value;
                if (tmpDay.Length == 1) tmpDay = 0 + k.Groups["day"].Value;
                if (tmpMonth.Length == 1) tmpMonth = 0 + k.Groups["month"].Value;

                tmpValues.Number = k.Groups["number"].Value.Trim().Trim(',');
                tmpValues.Date = k.Groups["year"].Value + "-" + tmpMonth + "-" + tmpDay;
            }
            else
                Console.WriteLine("96 pattern doesn't mutch!\t" + s);
            return tmpValues;
        }
        /*97*/
        internal static ProcessPublicationsOfEuroApp.ElementOut.I97Struct I97Process(string s)
        {
            ProcessPublicationsOfEuroApp.ElementOut.I97Struct tmpValues = new ProcessPublicationsOfEuroApp.ElementOut.I97Struct();
            var pattern = @"(?<number>[A-Z]+.*)\s*\,*(?<month>\d+)\/(?<day>\d+)\/(?<year>\d{4})";
            if (Regex.IsMatch(s, pattern))
            {
                var k = Regex.Match(s, pattern);
                string tmpDay = k.Groups["day"].Value;
                string tmpMonth = k.Groups["month"].Value;
                if (tmpDay.Length == 1) tmpDay = 0 + k.Groups["day"].Value;
                if (tmpMonth.Length == 1) tmpMonth = 0 + k.Groups["month"].Value;

                tmpValues.Number = k.Groups["number"].Value.Trim().Trim(',');
                tmpValues.Date = k.Groups["year"].Value + "-" + tmpMonth + "-" + tmpDay;
            }
            else
                Console.WriteLine("96 pattern doesn't match!\t" + s);
            return tmpValues;
        }
        /*30x fields*/
        internal static List<ProcessPublicationsOfEuroApp.ElementOut.PriorityStruct> PriorityProcess(string s)
        {
            List<ProcessPublicationsOfEuroApp.ElementOut.PriorityStruct> prioList = new List<ProcessPublicationsOfEuroApp.ElementOut.PriorityStruct>();
            ProcessPublicationsOfEuroApp.ElementOut.PriorityStruct prio;
            List<string> prioLines = new List<string>();
            string pattern = @"(?<number>.*)\s*\.*\s*(?<date>\d{2}\.\d{2}\.\d{4})\s*\.*\s*(?<country>[A-Z]{2}$)";
            if (s.Contains("\n"))
            {
                prioLines = s.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Where(x => Regex.IsMatch(x, pattern)).ToList();
            }
            else if (Regex.IsMatch(s, pattern))
            {
                prioLines.Add(s);
            }
            else
            {
                Console.WriteLine("Error:\tpriority identification\t" + s);
            }
            foreach (var item in prioLines)
            {
                var a = Regex.Match(item, pattern);
                prio = new ProcessPublicationsOfEuroApp.ElementOut.PriorityStruct();
                prio.Number = a.Groups["number"].Value.Trim().Trim('.').Trim();
                prio.Date = DateNormalize(a.Groups["date"].Value.Trim());
                prio.Country = a.Groups["country"].Value.Trim();
                prioList.Add(prio);
            }

            return prioList;
        }
        /*71*/
        internal static List<ProcessPublicationsOfEuroApp.ElementOut.OwnerStruct> ApplicantsProcess(string s)
        {
            s = Regex.Replace(s, @"\,\s*d\.o\.o", " d.o.o", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\,\s*ltd", " Ltd", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\,\s*s\.l", " S.L.", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\,\s*inc\.", " Inc", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\,\s*s\.a\.", " S.A.", RegexOptions.IgnoreCase);
            List<ProcessPublicationsOfEuroApp.ElementOut.OwnerStruct> applicants = new List<ProcessPublicationsOfEuroApp.ElementOut.OwnerStruct>();
            string pattern = @"(?<=\,\s*[A-Z]{2}\n|$)";
            string recPat = @"(?<name>.*^[^,]+),(?<address>.*),\s*(?<country>[A-Z]{2}$)";
            ProcessPublicationsOfEuroApp.ElementOut.OwnerStruct applicant;
            List<string> applList = new List<string>();
            if (Regex.IsMatch(s, pattern))
            {
                applList = Regex.Split(s, pattern).Where(x => x != "").ToList();
            }
            foreach (var item in applList)
            {
                string tmp = item.Replace("\n", " ").Trim();
                var k = Regex.Match(tmp, recPat);
                applicant = new ProcessPublicationsOfEuroApp.ElementOut.OwnerStruct();
                applicant.Name = k.Groups["name"].Value.Trim();
                applicant.Address = k.Groups["address"].Value.Trim();
                applicant.Country = k.Groups["country"].Value.Trim();
                applicants.Add(applicant);
            }
            return applicants;
        }
        /*72*/
        internal static List<ProcessPublicationsOfEuroApp.ElementOut.OwnerStruct> InventorsProcess(string s)
        {
            List<ProcessPublicationsOfEuroApp.ElementOut.OwnerStruct> inventors = new List<ProcessPublicationsOfEuroApp.ElementOut.OwnerStruct>();
            string pattern = @"(?<=\,\s*[A-Z]{2}\n|$)";
            string recPat = @"(?<name>^[^,]+,[^,]+),(?<address>.*),\s*(?<country>[A-Z]{2}$)";
            ProcessPublicationsOfEuroApp.ElementOut.OwnerStruct inventor;
            List<string> invList = new List<string>();
            if (Regex.IsMatch(s, pattern))
            {
                invList = Regex.Split(s, pattern).Where(x => x != "").ToList();
            }
            foreach (var item in invList)
            {
                string tmp = item.Replace("\n", " ").Trim();
                var k = Regex.Match(tmp, recPat);
                inventor = new ProcessPublicationsOfEuroApp.ElementOut.OwnerStruct();
                inventor.Name = k.Groups["name"].Value.Trim()/* + " " + k.Groups["nameB"].Value.Trim()*/;
                inventor.Address = k.Groups["address"].Value.Trim();
                inventor.Country = k.Groups["country"].Value.Trim();
                inventors.Add(inventor);
            }
            return inventors;
        }
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; //STAGING
                string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; //PRODUCTION
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
