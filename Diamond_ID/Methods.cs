using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_ID
{
    class Methods
    {
        /*Date*/
        public static string DateNormalize(string tmpDate)
        {
            var swapDate = tmpDate;
            string[] splitDate = null;
            string month = null;
            if (tmpDate.Contains(" "))
            {
                try
                {
                    splitDate = tmpDate.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitDate.Count() == 3)
                    {
                        switch (splitDate[1].ToLower())
                        {
                            case "january":
                                month = "01";
                                break;
                            case "januari":
                                month = "01";
                                break;
                            case "february":
                                month = "02";
                                break;
                            case "februari":
                                month = "02";
                                break;
                            case "march":
                                month = "03";
                                break;
                            case "maret":
                                month = "03";
                                break;
                            case "april":
                                month = "04";
                                break;
                            case "may":
                                month = "05";
                                break;
                            case "mei":
                                month = "05";
                                break;
                            case "june":
                                month = "06";
                                break;
                            case "juni":
                                month = "06";
                                break;
                            case "july":
                                month = "07";
                                break;
                            case "juli":
                                month = "07";
                                break;
                            case "august":
                                month = "08";
                                break;
                            case "agustus":
                                month = "08";
                                break;
                            case "september":
                                month = "09";
                                break;
                            case "october":
                                month = "10";
                                break;
                            case "oktober":
                                month = "10";
                                break;
                            case "november":
                                month = "11";
                                break;
                            case "december":
                                month = "12";
                                break;
                            case "desember":
                                month = "12";
                                break;
                            default:
                                month = "00";
                                break;
                        }
                    }
                    return swapDate = splitDate[2] + "-" + month + "-" + splitDate[0];
                }
                catch (Exception)
                {
                    return tmpDate;
                }
            }
            return tmpDate;
        }

        /*Splitting records by INID numbers*/
        public static string[] RecSplit(string recString)
        {
            var I57 = @"\(57\)\s*Abstrak\s*\:*";
            var I31 = "(31) Nomor";
            var I32 = "(32) Tanggal";
            var I33 = "(33) Negara";
            string[] splittedRecord = null;
            var tempStrC = recString.Replace(I31, "").Replace(I32, "").Replace(I33, "").Trim();
            string tmpDescValue = null;
            if (recString != "")
            {
                if (Regex.IsMatch(recString, I57))
                {
                    tmpDescValue = tempStrC.Substring(tempStrC.IndexOf(Regex.Match(recString, I57).Value)).Trim();
                    tempStrC = tempStrC.Remove(tempStrC.IndexOf(Regex.Match(recString, I57).Value)).Trim();
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

        public class PriorityStruct
        {
            public string[] Number { get; set; }
            public string[] Date { get; set; }
            public string[] Country { get; set; }
        }

        public class ClassificationInfoStruct
        {
            public string[] Class { get; set; }
            public string[] Date { get; set; }
            public string NotesValue { get; set; }
        }

        /*Classification Information 51 inid*/
        public static ClassificationInfoStruct ClassificationInfoSplit(string tmpString)
        {
            var classInfo = new ClassificationInfoStruct();
            var patternClass = @"[A-Z]{1}\d{2}[A-Z]{1}(\d+)\/(\d+)";
            var patternDate = @"\(\d{4}\.\d{2}\)";
            var I51 = @"(51) I.P.C : Int.Cl.";
            string[] splittedRecords = null;
            var tmpValue = tmpString.Replace(I51, "").Replace("\n", "").Trim();
            /*Value after // going to Notes*/
            if (tmpValue.Contains("//"))
            {
                classInfo.NotesValue = tmpValue.Substring(tmpValue.IndexOf("//")).Replace("//", "").Trim().Trim('(').Trim(')').Trim();
                tmpValue = tmpValue.Remove(tmpValue.IndexOf("//")).Trim();
            }
            /*Split records if more than one present and separated with comma*/
            if (tmpValue.Contains(","))
            {
                splittedRecords = tmpValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace(" ", "").Trim()).ToArray();
            }
            else
            {
                splittedRecords = (splittedRecords ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValue.Replace(" ", "").Trim() }).ToArray();
            }
            /*Split class and date info for each record*/
            if (splittedRecords != null)
            {
                foreach (var rec in splittedRecords)
                {
                    string dateValue = null;
                    string classValue = null;
                    if (Regex.IsMatch(rec, patternClass)) { classValue = Regex.Match(rec, patternClass).Value; }
                    if (Regex.IsMatch(rec, patternDate)) { dateValue = Regex.Match(rec, patternDate).Value.Replace("(", "").Replace(")", ""); }
                    if (classValue != null)
                    {
                        classInfo.Class = (classInfo.Class ?? Enumerable.Empty<string>()).Concat(new string[] { classValue.Insert(4, " ") }).ToArray();
                    }
                    if (dateValue != null)
                    {
                        classInfo.Date = (classInfo.Date ?? Enumerable.Empty<string>()).Concat(new string[] { dateValue }).ToArray();
                    }
                }
            }
            if (classInfo != null) return classInfo;
            else return null;
        }

        /*Inventor splitting method*/
        public class InventorStruct
        {
            public string[] Name { get; set; }
            public string[] Country { get; set; }
        }
        public static InventorStruct InventorSplit(string invString)
        {
            var inventors = new InventorStruct();
            var I72 = @"(72) Nama Inventor :";
            var patternCountry = @"\,\s*[A-Z]{2}$";
            string[] splInventors = null;
            var tmpValue = invString.Replace(I72, "").Trim();
            if (tmpValue.Contains("\n"))
            {
                splInventors = tmpValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
            else
            {
                splInventors = (splInventors ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValue }).ToArray();
            }
            foreach (var rec in splInventors)
            {
                string countryValue = null;
                string nameValue = null;
                if (Regex.IsMatch(rec, patternCountry))
                {
                    countryValue = Regex.Match(rec, patternCountry).Value;
                    nameValue = rec.Replace(countryValue, "").Trim();
                }
                if (countryValue != null && nameValue != null)
                {
                    inventors.Country = (inventors.Country ?? Enumerable.Empty<string>()).Concat(new string[] { countryValue.Replace(",", "").Trim() }).ToArray();
                    inventors.Name = (inventors.Name ?? Enumerable.Empty<string>()).Concat(new string[] { nameValue }).ToArray();
                }
            }

            if (inventors != null) return inventors;
            else return null;
        }

        /*Priority split method (31,32,33)*/
        public static PriorityStruct PrioritySplit(string prioString)
        {
            var I30 = "(30) Data Prioritas :";
            var tmpPrio = prioString.Replace(I30, "").Trim();
            if (tmpPrio != "")
            {
                var priorityValues = new PriorityStruct();
                string[] lineSplittedPrio = null;
                var datePattern = @"\d{2}\s*[A-Z]{1}[a-z]+\s*\d{4}";
                var countryPattern = @"[A-Z]{2}$";
                var testDate = Regex.Match(tmpPrio, datePattern).Value;
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
                        var dateValue = DateNormalize(Regex.Match(rec, datePattern).Value.Trim());
                        var numberValue = rec.Remove(rec.IndexOf(Regex.Match(rec, datePattern).Value.Trim())).Trim();
                        var countryValue = Regex.Match(rec, countryPattern).Value.Trim();
                        if (dateValue != null && numberValue != null && countryValue != null)
                        {
                            priorityValues.Date = (priorityValues.Date ?? Enumerable.Empty<string>()).Concat(new string[] { dateValue }).ToArray();
                            priorityValues.Number = (priorityValues.Number ?? Enumerable.Empty<string>()).Concat(new string[] { numberValue }).ToArray();
                            priorityValues.Country = (priorityValues.Country ?? Enumerable.Empty<string>()).Concat(new string[] { countryValue }).ToArray();
                        }
                    }
                }
                if (priorityValues != null) return priorityValues;
                else return null;
            }
            else return null;

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
    }
}
