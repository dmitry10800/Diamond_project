using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_BA
{
    class Methods
    {
        /*Date swapping*/
        public static string DateNormalize(string tmpDate)
        {
            try
            {
                string swapDate = "";
                string[] splitDate = tmpDate.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitDate.Count() == 3)
                {
                    swapDate = splitDate[2] + splitDate[1] + splitDate[0];
                }
                return swapDate;
            }
            catch (Exception)
            {
                return tmpDate;
            }
        }

        public class NameCountryStruct
        {
            public string[] Name { get; set; }
            public string[] Address { get; set; }
            public string[] Country { get; set; }

        }
        /*Name,country code split in 71,73,74 fields*/
        public static NameCountryStruct NameSpl(string tmpString)
        {
            NameCountryStruct splittedValues = new NameCountryStruct();
            string tmpValues = tmpString.Replace("(71)", "").Replace("(73)", "").Replace("(74)", "").Replace("\n", "****").Trim();
            if (Regex.IsMatch(tmpValues, @"\*{4}[A-Z]{2}\*{4}"))
            {
                string[] tmpSplValues = Regex.Split(tmpValues, @"(?<=\*{4}[A-Z]{2}\*{4})").Where(d => d != "").ToArray();
                foreach (var rec in tmpSplValues)
                {
                    if (Regex.IsMatch(rec, @"\*{4}[A-Z]{2}\*{4}"))
                    {
                        splittedValues.Country = (splittedValues.Country ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(rec, @"\*{4}[A-Z]{2}\*{4}$").Value.Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
                        splittedValues.Name = (splittedValues.Name ?? Enumerable.Empty<string>()).Concat(new string[] { rec.Remove(rec.IndexOf(Regex.Match(rec, @"\*{4}[A-Z]{2}\*{4}$").Value)).Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
                    }
                    else splittedValues.Name = (splittedValues.Name ?? Enumerable.Empty<string>()).Concat(new string[] { rec.Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
                }
            }
            else splittedValues.Name = (splittedValues.Name ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValues.Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
            return splittedValues;
        }

        /*Name, address and country code split in 72,75 fields*/
        public static NameCountryStruct NameSplWithAddress(string tmpString)
        {
            NameCountryStruct splittedValues = new NameCountryStruct();
            string tmpValues = tmpString.Replace("(72)", "").Replace("(75)", "").Replace("\n", "****").Trim();
            if (Regex.IsMatch(tmpValues, @"\*{4}[A-Z]{2}\*{4}"))
            {
                string[] tmpSplValues = Regex.Split(tmpValues, @"(?<=\*{4}[A-Z]{2}\*{4})").Where(d => d != "").ToArray();
                foreach (var rec in tmpSplValues)
                {
                    if (Regex.IsMatch(rec, @"\*{4}[A-Z]{2}\*{4}"))
                    {
                        splittedValues.Country = (splittedValues.Country ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(rec, @"\*{4}[A-Z]{2}\*{4}$").Value.Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
                        string tmpName = rec.Remove(rec.IndexOf(Regex.Match(rec, @"\*{4}[A-Z]{2}\*{4}$").Value)).Trim().Trim(',').Trim();
                        if (tmpName.Contains("****"))
                        {
                            splittedValues.Name = (splittedValues.Name ?? Enumerable.Empty<string>()).Concat(new string[] { tmpName.Remove(tmpName.IndexOf("****")).Trim() }).ToArray();
                            splittedValues.Address = (splittedValues.Address ?? Enumerable.Empty<string>()).Concat(new string[] { tmpName.Substring(tmpName.IndexOf("****")).Replace("****", " ").Trim() }).ToArray();
                        }
                        else
                        {
                            splittedValues.Name = (splittedValues.Name ?? Enumerable.Empty<string>()).Concat(new string[] { tmpName.Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
                            splittedValues.Address = (splittedValues.Address ?? Enumerable.Empty<string>()).Concat(new string[] { "" }).ToArray();
                        }
                    }
                    else
                    {
                        splittedValues.Name = (splittedValues.Name ?? Enumerable.Empty<string>()).Concat(new string[] { rec.Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
                        splittedValues.Address = (splittedValues.Address ?? Enumerable.Empty<string>()).Concat(new string[] { "" }).ToArray();
                    }
                }
            }
            else
            {
                splittedValues.Name = (splittedValues.Name ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValues.Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
                splittedValues.Address = (splittedValues.Address ?? Enumerable.Empty<string>()).Concat(new string[] { "" }).ToArray();
                splittedValues.Country = (splittedValues.Country ?? Enumerable.Empty<string>()).Concat(new string[] { "" }).ToArray();
            }
            return splittedValues;
        }

        public static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString
                .Replace("(32)", "")
                .Replace("(33)", "")
                .Replace("Broj ostalih patentnih zahtjeva:", "(I99)")
                .Trim();
            string tmpDescValue = null;
            if (recString != "")
            {
                if (recString.Contains("(57)"))
                {
                    tmpDescValue = tempStrC.Substring(tempStrC.IndexOf("(57)")).Trim().Replace("�", "ti");
                    tempStrC = tempStrC.Remove(recString.IndexOf("(57)")).Trim();
                }
                Regex regexPatOne = new Regex(@"\(\d{2}\)", RegexOptions.IgnoreCase);
                MatchCollection matchesClass = regexPatOne.Matches(recString);
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

        public class ClassificationStruct
        {
            public string[] Version { get; set; } // Date
            public string[] Classification { get; set; } // Number
        }
        /*Classification Information 51 inid*/
        public static ClassificationStruct ClassificationInfoSplit(string tmpString)
        {
            ClassificationStruct priority = new ClassificationStruct();
            string[] splClass = null;
            string tmpClass = tmpString.Replace("(51)", "").Replace(" ", "").Trim();
            if (tmpClass.Contains(","))
            {
                splClass = tmpClass.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            }
            else
            {
                splClass = (splClass ?? Enumerable.Empty<string>()).Concat(new string[] { tmpClass }).ToArray();
            }
            if (splClass != null)
            {
                foreach (var record in splClass)
                {
                    if (Regex.IsMatch(record, @"\(\d{4}\.\d{2}\)"))
                    {
                        priority.Classification = (priority.Classification ?? Enumerable.Empty<string>()).Concat(new string[] { record.Remove(record.IndexOf(Regex.Match(record, @"\(\d{4}\.\d{2}\)").Value)).Insert(4, " ") }).ToArray();
                        priority.Version = (priority.Version ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(record, @"\(\d{4}\.\d{2}\)").Value.Replace("(", "").Replace(")", "").Trim() }).ToArray();
                    }
                    else
                    {
                        priority.Classification = (priority.Classification ?? Enumerable.Empty<string>()).Concat(new string[] { record.Length > 3 ? record.Insert(4, " ") : record }).ToArray();
                        priority.Version = (priority.Version ?? Enumerable.Empty<string>()).Concat(new string[] { null }).ToArray();
                    }
                }
            }
            if (splClass != null) return priority;
            else return null;
        }
        /*Priority split method (31,32,33)*/
        public class PriorityStruct
        {
            public string Number { get; set; }
            public string Date { get; set; }
            public string Country { get; set; }
        }
        public static List<PriorityStruct> PrioritySplit(string prioString)
        {
            List<PriorityStruct> priorityStruct = new List<PriorityStruct>();
            string tmpPrio = prioString.Replace("(31)", "").Trim();
            string[] lineSplittedPrio = null;
            string datePattern = @"\d{4}\-\d{2}\-\d{2}";
            string countryPattern = @"[A-Z]{2}$";
            /*If more than one priority separated with new line*/
            if (tmpPrio.Contains("\n"))
            {
                lineSplittedPrio = tmpPrio.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
            else { lineSplittedPrio = (lineSplittedPrio ?? Enumerable.Empty<string>()).Concat(new string[] { tmpPrio }).ToArray(); }
            /*Trying to split each record to date/number/country*/
            if (lineSplittedPrio != null)
            {
                foreach (var rec in lineSplittedPrio)
                {
                    /*add found value to output*/
                    PriorityStruct record = new PriorityStruct
                    {
                        Date = Regex.Match(rec, datePattern).Value.Trim(),
                        Number = rec.Remove(rec.IndexOf(Regex.Match(rec, datePattern).Value.Trim())).Trim(),
                        Country = Regex.Match(rec, countryPattern).Value.Trim()
                    };
                    priorityStruct.Add(record);
                }
            }
            if (priorityStruct != null) return priorityStruct;
            else return null;
        }
        /*Sending data into Diamond*/
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
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
