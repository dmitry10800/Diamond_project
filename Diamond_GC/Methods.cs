using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_GC
{
    class Methods
    {
        public static string CurrentAPNR { get; set; }
        /*Splitting records by INID numbers*/
        public static string[] RecSplit(string recString)
        {
            string I30 = "[30] Priority:";
            string I31 = "[31] Priority No.";
            string I32 = "[32] Priority date";
            string I33 = "[33] State";
            string[] splittedRecord = null;
            string[] splittedRecordPrio = null;
            string tmpPrioValues = "";
            List<string> splittedList = new List<string>();
            string tempStrC = recString.Trim();
            if (recString != "")
            {
                Regex regexPatOne = new Regex(@"\[\d{2}\]", RegexOptions.IgnoreCase);
                MatchCollection matchesClass = regexPatOne.Matches(recString);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                    .Where(x => !x.StartsWith(I30) && !x.StartsWith(I31) && !x.StartsWith(I32) && !x.StartsWith(I33))
                    .ToArray();
                splittedList = splittedRecord.ToList();
                splittedRecordPrio = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => x.StartsWith(I30) || x.StartsWith(I31) || x.StartsWith(I32) || x.StartsWith(I33))
                    .Select(x => x.Replace(I31, "").Replace(I32, "").Replace(I33, "").Trim())
                    .ToArray();
                if (splittedRecordPrio != null)
                {
                    foreach (var item in splittedRecordPrio)
                    {
                        tmpPrioValues += item + "\n";
                    }
                    splittedList.Add(tmpPrioValues);
                }
                //splittedList.Concat(splittedRecord);
            }
            return splittedList.ToArray();
        }
        public static string DateNoteProcess(string s)
        {
            Regex pat = new Regex(@"\d{2}\/[a-zA-Z]+\/\d{4}");
            if (pat.Match(s).Success)
            {
                var splDate = pat.Match(s).Value.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var nDate = splDate[2] + "-" + DateStringToInt(splDate[1]) + "-" + splDate[0];
                return Regex.Replace(s, pat.Match(s).Value, nDate);
            }
            else
                return s;
        }
        /*Date Processing*/
        public static string DateProcess(string recString)
        {
            if (recString == "") return null;
            string day = null;
            string month = null;
            string year = null;
            string datePattern = @"(?<day>\d+)(\/|\.)(?<month>\d+)(\/|\.)(?<year>\d{4})";
            string datePatternTwo = @"(?<year>\d{4})-*(?<month>\d+)-*(?<day>\d+)";
            //if (Regex.IsMatch(recString, datePattern))
            //{
            //    day = Regex.Match(recString, datePattern).Groups["day"].Value;
            //    month = Regex.Match(recString, datePattern).Groups["month"].Value;
            //    year = Regex.Match(recString, datePattern).Groups["year"].Value;
            //}
            //else
            if (Regex.IsMatch(recString, datePatternTwo))
            {
                day = Regex.Match(recString, datePatternTwo).Groups["day"].Value;
                month = Regex.Match(recString, datePatternTwo).Groups["month"].Value;
                year = Regex.Match(recString, datePatternTwo).Groups["year"].Value;
            }
            if (day.Length == 1) day = "0" + day;
            if (month.Length == 1) month = "0" + month;
            if (day != null && month != null && year != null)
            {
                return year + "-" + month + "-" + day;
            }
            else return recString;
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
            public string Date { get; set; }
        }

        /*Classification Information 51 inid*/
        public static ClassificationInfoStruct ClassificationInfoSplit(string tmpString)
        {
            ClassificationInfoStruct classInfo = new ClassificationInfoStruct();
            string patternClass = @"[A-Z]{1}\d{2}[A-Z]{1}(\d+)\/(\d+)";
            string patternDate = @"\(\d{4}\.\d{2}\)";
            string[] splittedRecords = null;
            string tmpValue = tmpString.Replace("\n", "").Replace(" ", "").Trim();
            /*Value after // going to Notes*/
            if (Regex.IsMatch(tmpValue, patternDate))
            {
                classInfo.Date = Regex.Match(tmpValue, patternDate).Value.Replace("(", "").Replace(")", "").Trim();
                tmpValue = tmpValue.Replace(Regex.Match(tmpValue, patternDate).Value, "").Trim();
            }
            /*Split records if more than one present and separated with comma*/
            if (tmpValue.Contains(";"))
            {
                splittedRecords = tmpValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                splittedRecords = (splittedRecords ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValue.Trim() }).ToArray();
            }
            /*Split class and date info for each record*/
            if (splittedRecords != null)
            {
                foreach (var rec in splittedRecords)
                {
                    string dateValue = null;
                    string classValue = null;
                    string tmpFirstPart = null;
                    if (rec.Contains(","))
                    {
                        string[] tmpStringValue = rec.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in tmpStringValue)
                        {
                            if (Regex.IsMatch(item, patternClass))
                            {
                                classValue = Regex.Match(item, patternClass).Value;
                                classInfo.Class = (classInfo.Class ?? Enumerable.Empty<string>()).Concat(new string[] { classValue.Insert(4, " ") }).ToArray();
                            }
                            if (Regex.IsMatch(item, @"[A-Z]{1}\d{2}[A-Z]{1}")) tmpFirstPart = Regex.Match(item, @"[A-Z]{1}\d{2}[A-Z]{1}").Value;
                            else if (tmpFirstPart != null)
                            {
                                classValue = tmpFirstPart + item;
                                classInfo.Class = (classInfo.Class ?? Enumerable.Empty<string>()).Concat(new string[] { classValue.Insert(4, " ") }).ToArray();
                            }
                        }
                    }
                    else if (Regex.IsMatch(rec, patternClass))
                    {
                        classValue = Regex.Match(rec, patternClass).Value;
                        classInfo.Class = (classInfo.Class ?? Enumerable.Empty<string>()).Concat(new string[] { classValue.Insert(4, " ") }).ToArray();
                    }
                    else Console.WriteLine("Error with 51 field:\t" + CurrentAPNR);
                }
            }
            if (classInfo != null) return classInfo;
            else return null;
        }

        public static string DateStringToInt(string tmpStr)
        {
            switch (tmpStr)
            {
                case "Jan": return "01";
                case "Feb": return "02";
                case "Mar": return "03";
                case "Apr": return "04";
                case "May": return "05";
                case "Jun": return "06";
                case "Jyl": return "07";
                case "Jul": return "07";
                case "Aug": return "08";
                case "Sep": return "09";
                case "Ocr": return "10";
                case "Oct": return "10";
                case "Nov": return "11";
                case "Dec": return "12";
                default: return "00";
            }
        }

        /*Inventor splitting method*/
        public class InventorStruct
        {
            public string[] Name { get; set; }
        }
        public static InventorStruct InventorSplit(string invString)
        {
            InventorStruct inventors = new InventorStruct();
            string[] splInventors = null;
            string tmpValue = invString.Replace("Inventor", "").Trim();
            if (Regex.IsMatch(tmpValue, @"\d+\-"))
            {
                inventors.Name = Regex.Split(tmpValue, @"\d+\-").Select(x => x.Trim()).Where(x => x != "").ToArray();
            }
            else
            {
                inventors.Name = (splInventors ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValue }).ToArray();
            }
            if (inventors != null && inventors.Name.Count() > 0) return inventors;
            else return null;
        }
        public class OwnerStruct
        {
            public List<string> Name = new List<string>();
            public List<string> Address = new List<string>();
            public List<string> Country = new List<string>();
        }
        public static OwnerStruct OwnerSplit(string tmpValue)
        {
            OwnerStruct owners = new OwnerStruct();
            string ownerValue = tmpValue.Replace("\n", " ");
            List<string> splittedOwners = new List<string>();
            //Regex ownersPattern = new Regex(@"(?<ownerName>^.*[^,]+)(?<ownerAddress>.*)(\,)(?<ownerCountry>.*$)");
            Regex separator = new Regex(@"\،\d+\-");
            if (Regex.IsMatch(ownerValue, @"\،\d+\-"))
            {
                splittedOwners = separator.Split(ownerValue).ToList();
            }
            else splittedOwners.Add(ownerValue);
            if (splittedOwners != null && splittedOwners.Count > 0)
            {
                foreach (var owner in splittedOwners)
                {
                    string repOwner = owner;
                    repOwner = Regex.Replace(repOwner, @"(\,)(\s*\bllc)", " $2", RegexOptions.IgnoreCase);
                    repOwner = Regex.Replace(repOwner, @"(\,)(\s*\binc\b\.*)", " $2", RegexOptions.IgnoreCase);
                    repOwner = Regex.Replace(repOwner, @"(\,)(\s*\bgmbh\b)", " $2", RegexOptions.IgnoreCase);
                    repOwner = Regex.Replace(repOwner, @"(\,)(\s*\bsarl\b)", " $2", RegexOptions.IgnoreCase);
                    repOwner = Regex.Replace(repOwner, @"(\,)(\s*\bltd\b)", " $2", RegexOptions.IgnoreCase);
                    repOwner = Regex.Replace(repOwner, @"\s+", " ");
                    if (repOwner.Contains(",") && repOwner.Count(x => x == ',') > 2)
                    {
                        owners.Name.Add(repOwner.Remove(repOwner.IndexOf(",")).Trim());
                        owners.Address.Add(repOwner.Remove(repOwner.LastIndexOf(",")).Substring(repOwner.IndexOf(",")).Trim(',').Trim());
                        owners.Country.Add(CountryCodeIdentification(repOwner.Substring(repOwner.LastIndexOf(",")).Trim(',').Trim()));
                    }
                    else
                    {
                        owners.Name.Add(repOwner);
                        owners.Address.Add("");
                        owners.Country.Add("");

                        //owners.Name.Add("Amgen Research (Munich) GmbH");
                        //owners.Address.Add("Staffelseestrasse 2, 81477 Munich, Germany, Munich, Germany AMGEN INC");
                        //owners.Country.Add("US");
                    }
                }
            }
            if (owners != null && owners.Name.Count > 0) return owners;
            else return null;
        }
        public static string CountryCodeIdentification(string tmpString)
        {
            string tmpStr = tmpString;
            List<string> ccFullNames = new List<string> { "virginislands", "virgin islands", "xizhimen north", "unitedkingdom", "saudiarabia", "afghanistan", "aland islands", "albania", "algeria", "american samoa", "andorra", "angola", "anguilla", "antarctica", "antigua and barbuda", "argentina", "armenia", "aruba", "australia", "austria", "azerbaijan", "bahamas", "bahrain", "bangladesh", "barbados", "belarus", "belgium", "belize", "benin", "bermuda", "bhutan", "bolivia", "bosnia and herzegovina", "botswana", "bouvet island", "brazil", "british virgin islands", "british indian ocean territory", "brunei darussalam", "bulgaria", "burkina faso", "burundi", "cambodia", "cameroon", "canada", "cape verde", "cayman islands", "central african republic", "chad", "chile", "china", "hong kong, sar china", "macao, sar china", "christmas island", "cocos (keeling) islands", "colombia", "comoros", "congo (brazzaville)", "congo, (kinshasa)", "cook islands", "costa rica", "côte d'ivoire", "croatia", "cuba", "cyprus", "czech republic", "denmark", "djibouti", "dominica", "dominican republic", "ecuador", "egypt", "el salvador", "equatorial guinea", "eritrea", "estonia", "ethiopia", "falkland islands", "faroe islands", "fiji", "finland", "france", "french guiana", "french polynesia", "french southern territories", "gabon", "gambia", "georgia", "germany", "ghana", "gibraltar", "greece", "greenland", "grenada", "guadeloupe", "guam", "guatemala", "guernsey", "guinea", "guinea-bissau", "guyana", "haiti", "heard and mcdonald islands", "holy see (vatican city state)", "honduras", "hungary", "iceland", "india", "indonesia", "iran, islamic republic of", "iraq", "ireland", "isle of man", "israel", "italy", "jamaica", "japan", "jersey", "jordan", "kazakhstan", "kenya", "kiribati", "korea (north)", "korea (south)", "kuwait", "kyrgyzstan", "lao pdr", "latvia", "lebanon", "lesotho", "liberia", "libya", "liechtenstein", "lithuania", "luxembourg", "macedonia, republic of", "madagascar", "malawi", "malaysia", "maldives", "mali", "malta", "marshall islands", "martinique", "mauritania", "mauritius", "mayotte", "mexico", "micronesia, federated states of", "moldova", "monaco", "mongolia", "montenegro", "montserrat", "morocco", "mozambique", "myanmar", "namibia", "nauru", "nepal", "netherlands", "netherlands antilles", "new caledonia", "new zealand", "nicaragua", "niger", "nigeria", "niue", "norfolk island", "northern mariana islands", "norway", "oman", "pakistan", "palau", "palestinian territory", "panama", "papua new guinea", "paraguay", "peru", "philippines", "pitcairn", "poland", "portugal", "puerto rico", "qatar", "réunion", "romania", "russian federation", "rwanda", "saint-barthélemy", "saint helena", "saint kitts and nevis", "saint lucia", "saint-martin (french part)", "saint pierre and miquelon", "saint vincent and grenadines", "samoa", "san marino", "sao tome and principe", "saudi arabia", "senegal", "serbia", "seychelles", "sierra leone", "singapore", "slovakia", "slovenia", "solomon islands", "somalia", "south africa", "south georgia and the south sandwich islands", "south sudan", "spain", "sri lanka", "sudan", "suriname", "svalbard and jan mayen islands", "swaziland", "sweden", "switzerland", "syria", "taiwan, republic of china", "tajikistan", "tanzania, united republic of", "thailand", "timor-leste", "togo", "tokelau", "tonga", "trinidad and tobago", "tunisia", "turkey", "turkmenistan", "turks and caicos islands", "tuvalu", "uganda", "ukraine", "united arab emirates", "united kingdom", "united states of america", "us minor outlying islands", "uruguay", "uzbekistan", "vanuatu", "venezuela (bolivarian republic)", "viet nam", "virgin islands, us", "wallis and futuna islands", "western sahara", "yemen", "zambia", "zimbabwe", "p. r. china", "republic of korea", "korea", "republic of san marino", "u s a", "u.s.a", "united states", "uae", "u.a.e", "congo", "switzerland", "chile", "prc", "p.r.c.", "england", "london", "united kingdom", "hong kong", "india", "cayman", "méxico", "r.o.c.", "u.s.a", "united sates of america", "united state of america,", "vietnam", "russia", "european union", "benelux", "usa" };
            List<string> ccShortNames = new List<string> { "VG", "VG", "CN", "UK", "SA", "AF", "AX", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR", "AM", "AW", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY", "BE", "BZ", "BJ", "BM", "BT", "BO", "BA", "BW", "BV", "BR", "VG", "IO", "BN", "BG", "BF", "BI", "KH", "CM", "CA", "CV", "KY", "CF", "TD", "CL", "CN", "HK", "MO", "CX", "CC", "CO", "KM", "CG", "CD", "CK", "CR", "CI", "HR", "CU", "CY", "CZ", "DK", "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "ET", "FK", "FO", "FJ", "FI", "FR", "GF", "PF", "TF", "GA", "GM", "GE", "DE", "GH", "GI", "GR", "GL", "GD", "GP", "GU", "GT", "GG", "GN", "GW", "GY", "HT", "HM", "VA", "HN", "HU", "IS", "IN", "ID", "IR", "IQ", "IE", "IM", "IL", "IT", "JM", "JP", "JE", "JO", "KZ", "KE", "KI", "KP", "KR", "KW", "KG", "LA", "LV", "LB", "LS", "LR", "LY", "LI", "LT", "LU", "MK", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MQ", "MR", "MU", "YT", "MX", "FM", "MD", "MC", "MN", "ME", "MS", "MA", "MZ", "MM", "NA", "NR", "NP", "NL", "AN", "NC", "NZ", "NI", "NE", "NG", "NU", "NF", "MP", "NO", "OM", "PK", "PW", "PS", "PA", "PG", "PY", "PE", "PH", "PN", "PL", "PT", "PR", "QA", "RE", "RO", "RU", "RW", "BL", "SH", "KN", "LC", "MF", "PM", "VC", "WS", "SM", "ST", "SA", "SN", "RS", "SC", "SL", "SG", "SK", "SI", "SB", "SO", "ZA", "GS", "SS", "ES", "LK", "SD", "SR", "SJ", "SZ", "SE", "CH", "SY", "TW", "TJ", "TZ", "TH", "TL", "TG", "TK", "TO", "TT", "TN", "TR", "TM", "TC", "TV", "UG", "UA", "AE", "GB", "US", "UM", "UY", "UZ", "VU", "VE", "VN", "VI", "WF", "EH", "YE", "ZM", "ZW", "CN", "KR", "KR", "SM", "US", "US", "US", "AE", "AE", "CD", "CH", "CL", "CN", "CN", "GB", "GB", "GB", "HK", "IN", "KY", "MX", "TW", "US", "US", "US", "VN", "RU", "EM", "BX", "US" };
            foreach (var country in ccFullNames)
            {
                if (tmpString.ToLower().Contains(country))
                {
                    tmpStr = ccShortNames.ElementAt(ccFullNames.IndexOf(country));
                    return tmpStr;
                }
            }
            Console.WriteLine("CountryCode is undefined:\t" + tmpString + "\tin" + CurrentAPNR);
            return tmpStr;
        }

        /*Priority split method (31,32,33)*/
        public static PriorityStruct PrioritySplit(string prioString)
        {
            string tmpPrio = prioString;
            try
            {
                if (tmpPrio != "")
                {
                    PriorityStruct priorityValuesTmp = new PriorityStruct();
                    string[] lineSplittedPrio = null;
                    string datePattern = @"\d+(\/|\.)\d+(\/|\.)\d{4}";
                    string countryPattern = @"[A-Z]{2}$";
                    tmpPrio = tmpPrio.Replace("\n", " ");
                    tmpPrio = Regex.Replace(tmpPrio, @"\s+", " ");
                    if (tmpPrio.Contains(" "))
                    {
                        lineSplittedPrio = tmpPrio.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    }
                    foreach (var item in lineSplittedPrio)
                    {
                        if (!Regex.IsMatch(item, datePattern) && !Regex.IsMatch(item, countryPattern))
                            priorityValuesTmp.Number = (priorityValuesTmp.Number ?? Enumerable.Empty<string>()).Concat(new string[] { item }).ToArray();
                        if (Regex.IsMatch(item, datePattern))
                            priorityValuesTmp.Date = (priorityValuesTmp.Date ?? Enumerable.Empty<string>()).Concat(new string[] { DateProcess(item) }).ToArray();
                        if (Regex.IsMatch(item, countryPattern))
                            priorityValuesTmp.Country = (priorityValuesTmp.Country ?? Enumerable.Empty<string>()).Concat(new string[] { item }).ToArray();
                    }
                    if (priorityValuesTmp != null && priorityValuesTmp.Number.Count() > 0)
                    {
                        if (priorityValuesTmp.Number.Count() == priorityValuesTmp.Date.Count() && priorityValuesTmp.Number.Count() == priorityValuesTmp.Country.Count())
                            return priorityValuesTmp;
                        else
                        {
                            Console.WriteLine("Error in PrioritySplit with:\t" + CurrentAPNR);
                            return null;
                        }
                    }
                    else return null;
                }
                else return null;
            }
            catch (Exception)
            {
                Console.WriteLine("Error in PrioritySplit with:\t" + CurrentAPNR);
                return null;
            }

        }
        /*Sending data into Diamond*/
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                /*Staging URL*/
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                /*Production URL*/
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
