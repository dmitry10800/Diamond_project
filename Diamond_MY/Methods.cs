using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_MY
{
    class Methods : ProcessGrants
    {
        public static string DateStringToInt(string tmpStr)
        {
            switch (tmpStr)
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
        public static string CountryCodeIdentification(string ccStr)
        {
            string tmpStr = "";
            List<string> ccFullNames = new List<string> { "afghanistan", "aland islands", "albania", "algeria", "american samoa", "andorra", "angola", "anguilla", "antarctica", "antigua and barbuda", "argentina", "armenia", "aruba", "australia", "austria", "azerbaijan", "bahamas", "bahrain", "bangladesh", "barbados", "belarus", "belgium", "belize", "benin", "bermuda", "bhutan", "bolivia", "bosnia and herzegovina", "botswana", "bouvet island", "brazil", "british virgin islands", "british indian ocean territory", "brunei darussalam", "bulgaria", "burkina faso", "burundi", "cambodia", "cameroon", "canada", "cape verde", "cayman islands", "central african republic", "chad", "chile", "china", "hong kong, sar china", "macao, sar china", "christmas island", "cocos (keeling) islands", "colombia", "comoros", "congo (brazzaville)", "congo, (kinshasa)", "cook islands", "costa rica", "côte d'ivoire", "croatia", "cuba", "cyprus", "czech republic", "denmark", "djibouti", "dominica", "dominican republic", "ecuador", "egypt", "el salvador", "equatorial guinea", "eritrea", "estonia", "ethiopia", "falkland islands (malvinas)", "faroe islands", "fiji", "finland", "france", "french guiana", "french polynesia", "french southern territories", "gabon", "gambia", "georgia", "germany", "ghana", "gibraltar", "greece", "greenland", "grenada", "guadeloupe", "guam", "guatemala", "guernsey", "guinea", "guinea-bissau", "guyana", "haiti", "heard and mcdonald islands", "holy see (vatican city state)", "honduras", "hungary", "iceland", "india", "indonesia", "iran, islamic republic of", "iraq", "ireland", "isle of man", "israel", "italy", "jamaica", "japan", "jersey", "jordan", "kazakhstan", "kenya", "kiribati", "korea (north)", "korea (south)", "kuwait", "kyrgyzstan", "lao pdr", "latvia", "lebanon", "lesotho", "liberia", "libya", "liechtenstein", "lithuania", "luxembourg", "macedonia, republic of", "madagascar", "malawi", "malaysia", "maldives", "mali", "malta", "marshall islands", "martinique", "mauritania", "mauritius", "mayotte", "mexico", "micronesia, federated states of", "moldova", "monaco", "mongolia", "montenegro", "montserrat", "morocco", "mozambique", "myanmar", "namibia", "nauru", "nepal", "netherlands", "netherlands antilles", "new caledonia", "new zealand", "nicaragua", "niger", "nigeria", "niue", "norfolk island", "northern mariana islands", "norway", "oman", "pakistan", "palau", "palestinian territory", "panama", "papua new guinea", "paraguay", "peru", "philippines", "pitcairn", "poland", "portugal", "puerto rico", "qatar", "réunion", "romania", "russian federation", "rwanda", "saint-barthélemy", "saint helena", "saint kitts and nevis", "saint lucia", "saint-martin (french part)", "saint pierre and miquelon", "saint vincent and grenadines", "samoa", "san marino", "sao tome and principe", "saudi arabia", "senegal", "serbia", "seychelles", "sierra leone", "singapore", "slovakia", "slovenia", "solomon islands", "somalia", "south africa", "south georgia and the south sandwich islands", "south sudan", "spain", "sri lanka", "sudan", "suriname", "svalbard and jan mayen islands", "swaziland", "sweden", "switzerland", "syrian arab republic (syria)", "taiwan, republic of china", "tajikistan", "tanzania, united republic of", "thailand", "timor-leste", "togo", "tokelau", "tonga", "trinidad and tobago", "tunisia", "turkey", "turkmenistan", "turks and caicos islands", "tuvalu", "uganda", "ukraine", "united arab emirates", "united kingdom", "united states of america", "us minor outlying islands", "uruguay", "uzbekistan", "vanuatu", "venezuela (bolivarian republic)", "viet nam", "virgin islands, us", "wallis and futuna islands", "western sahara", "yemen", "zambia", "zimbabwe", "p. r. china", "republic of korea", "korea", "republic of san marino", "u s a", "u.s.a", "united states", "uae", "u.a.e", "congo", "switzerland", "chile", "prc", "p.r.c.", "england", "london", "united kingdom", "hong kong", "india", "cayman", "méxico", "r.o.c.", "u.s.a", "united sates of america", "united state of america,", "vietnam", "russia", "european union", "benelux" };
            List<string> ccShortNames = new List<string> { "AF", "AX", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR", "AM", "AW", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY", "BE", "BZ", "BJ", "BM", "BT", "BO", "BA", "BW", "BV", "BR", "VG", "IO", "BN", "BG", "BF", "BI", "KH", "CM", "CA", "CV", "KY", "CF", "TD", "CL", "CN", "HK", "MO", "CX", "CC", "CO", "KM", "CG", "CD", "CK", "CR", "CI", "HR", "CU", "CY", "CZ", "DK", "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "ET", "FK", "FO", "FJ", "FI", "FR", "GF", "PF", "TF", "GA", "GM", "GE", "DE", "GH", "GI", "GR", "GL", "GD", "GP", "GU", "GT", "GG", "GN", "GW", "GY", "HT", "HM", "VA", "HN", "HU", "IS", "IN", "ID", "IR", "IQ", "IE", "IM", "IL", "IT", "JM", "JP", "JE", "JO", "KZ", "KE", "KI", "KP", "KR", "KW", "KG", "LA", "LV", "LB", "LS", "LR", "LY", "LI", "LT", "LU", "MK", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MQ", "MR", "MU", "YT", "MX", "FM", "MD", "MC", "MN", "ME", "MS", "MA", "MZ", "MM", "NA", "NR", "NP", "NL", "AN", "NC", "NZ", "NI", "NE", "NG", "NU", "NF", "MP", "NO", "OM", "PK", "PW", "PS", "PA", "PG", "PY", "PE", "PH", "PN", "PL", "PT", "PR", "QA", "RE", "RO", "RU", "RW", "BL", "SH", "KN", "LC", "MF", "PM", "VC", "WS", "SM", "ST", "SA", "SN", "RS", "SC", "SL", "SG", "SK", "SI", "SB", "SO", "ZA", "GS", "SS", "ES", "LK", "SD", "SR", "SJ", "SZ", "SE", "CH", "SY", "TW", "TJ", "TZ", "TH", "TL", "TG", "TK", "TO", "TT", "TN", "TR", "TM", "TC", "TV", "UG", "UA", "AE", "GB", "US", "UM", "UY", "UZ", "VU", "VE", "VN", "VI", "WF", "EH", "YE", "ZM", "ZW", "CN", "KR", "KR", "SM", "US", "US", "US", "AE", "AE", "CD", "CH", "CL", "CN", "CN", "GB", "GB", "GB", "HK", "IN", "KY", "MX", "TW", "US", "US", "US", "VN", "RU", "EM", "BX" };
            foreach (var country in ccFullNames)
            {
                if (ccStr.ToLower().Contains(country))
                {
                    tmpStr = ccShortNames.ElementAt(ccFullNames.IndexOf(country));
                    return tmpStr;
                }
            }
            return tmpStr;
        }
        /*Date*/
        public static string DateNormalize(string tmpDate)
        {
            Regex patternDate = new Regex(@"(?<day>\d+)\s*\.*\/*\-*(?<month>[A-z]+)\s*\.*\/*\-*(?<year>\d{4})");
            var a = patternDate.Match(tmpDate);
            if (a.Success)
            {
                return a.Groups["year"].Value + "-" + DateStringToInt(a.Groups["month"].Value) + "-" + a.Groups["day"].Value;
            }
            else return tmpDate;
        }

        public static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString.Replace("(54) Title :", I54).Replace("(57) Abstract :", I57).Trim();
            tempStrC = tempStrC/*.Replace(I11, "*****" + I11)*/
                .Replace(I21, "*****" + I21)
                .Replace(I22, "*****" + I22)
                .Replace(I30, "*****" + I30)
                .Replace(I47, "*****" + I47)
                .Replace(I51, "*****" + I51)
                .Replace(I54, "*****" + I54)
                .Replace(I56, "*****" + I56)
                .Replace(I57, "*****" + I57)
                .Replace(I72, "*****" + I72)
                .Replace(I73, "*****" + I73)
                .Replace(I74, "*****" + I74);
            if (tempStrC != "")
            {
                splittedRecord = tempStrC.Split(new string[] { "*****" }, StringSplitOptions.RemoveEmptyEntries);
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

        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; //STAGING
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; //PRODUCTION
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }

        internal static ElementOut.I11Struct I11Process(string v)
        {
            ElementOut.I11Struct a = new ElementOut.I11Struct();
            //Regex pattern = new Regex(@"(?<country>[A-Z]+\s*\-\s*)(?<number>\d+)\s*\-\s*(?<kind>[A-Z]+.*$)");
            //var b = pattern.Match(v);
            //if (b.Success)
            //{
            //    //a.Number = b.Groups["country"].Value + b.Groups["number"].Value;
            //    //a.Kind = b.Groups["kind"].Value.Trim();
            //}
            //else
            //{
            //    Console.WriteLine("I11 identification error");
            //}
            var number = Regex.Match(v, @"(?<ContryCode>MY)\s*-(?<Number>\d+)\s*-(?<Kind>[A-Z]{1}\d{0,2})");
            if (number.Success)
                a.Number = number.Groups["Number"].Value;
                a.Kind = number.Groups["Kind"].Value;
            return a;
        }
       /* internal static List<ElementOut.PrioStruct> PriorityProcess(string v)
        {
            List<ElementOut.PrioStruct> prioList = new List<ElementOut.PrioStruct>();
            ElementOut.PrioStruct a;
            List<string> tmp = new List<string>();
            Regex pat = new Regex(@"(?<number>.*)\s*;\s*(?<date>(?<day>\d+)\/(?<month>\d+)\/(?<year>\d+))\s*;\s*(?<country>[A-Z]{2})");
            if (v.Contains("\n"))
            {
                tmp = v.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else tmp.Add(v.Trim());
            foreach (var item in tmp)
            {
                if (pat.Match(item).Success)
                {
                    a = new ElementOut.PrioStruct();
                    var k = pat.Match(item);
                    a.Number = k.Groups["number"].Value;
                    a.Date = "20" + k.Groups["year"].Value + "." + k.Groups["month"].Value + "." + k.Groups["day"].Value;
                    a.Country = k.Groups["country"].Value;
                    prioList.Add(a);
                }
            }
            return prioList;
        }*/
        internal static List<ElementOut.IntClassStruct> IntClasProcess(string v)
        {
            List<ElementOut.IntClassStruct> intList = new List<ElementOut.IntClassStruct>();
            ElementOut.IntClassStruct a;
            List<string> tmp = new List<string>();
            Regex pat = new Regex(@"(?<value>[A-Z]\d{2}[A-Z]\s*\d+(\/\d+)+)\s*\(\s*(?<date>\d+\.\d+)\s*\)");
            string tmpV = v;
            /*case first - Classification, INT CL(2006.01) :*/
            Regex patOne = new Regex(@"\((?<date>\d{4}\.\d{2})\)");

            /*Split value on two by symbol ":"*/
            var values = v.Split(new string[] { ":" }, StringSplitOptions.None).Select(x => x.Trim()).ToList();
            string tmpDate = null;
            string tmpCl = null;
            /*case - value contains date*/
            if (patOne.Match(values[0]).Success)
            {
                tmpDate = patOne.Match(v).Groups["date"].Value;
                tmpV = values[1].Trim();
            }
            /*case - value contains class number*/
            else if (Regex.Match(values[0], @"^\d+$").Success)
            {
                tmpCl = values[0].Trim();
                tmpV = values[1].Trim();
            }
            /*case - value is empty*/
            else if (values[0].Trim() == "")
            {
                tmpV = values[1].Trim();
            }

            if (tmpV.Contains("\n"))
            {
                tmp = tmpV.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else tmp.Add(tmpV.Trim());

            foreach (var item in tmp)
            {
                a = new ElementOut.IntClassStruct();
                if (pat.Match(item).Success)
                {
                    var k = pat.Match(item);
                    a.Number = k.Groups["value"].Value;
                    a.Date = k.Groups["date"].Value;
                    //intList.Add(a);
                }
                else
                {
                    a.Number = item;
                    if (tmpCl != null) a.DigitClass = tmpCl;
                    if (tmpDate != null) a.Date = tmpDate;
                }
                intList.Add(a);
            }
            return intList;
        }
        /*internal static List<ElementOut.InventorsStruct> InventorsProcess(string v)
        {
            List<ElementOut.InventorsStruct> invList = new List<ElementOut.InventorsStruct>();
            List<string> tmp = new List<string>();
            if (v.Contains("\n"))
            {
                tmp = v.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else tmp.Add(v.Trim());
            foreach (var item in tmp)
            {
                invList.Add(new ElementOut.InventorsStruct { Name = item });
            }
            return invList;
        }
        internal static List<ElementOut.OwnersStruct> OwnersProcess(string v)
        {
            List<ElementOut.OwnersStruct> ownList = new List<ElementOut.OwnersStruct>();
            ElementOut.OwnersStruct a;
            List<string> tmpOwners = new List<string>();
            Regex pat = new Regex(@"(?<name>^.*)\n(?<address>(.*\n.*)+)\n(?<country>.*$)", RegexOptions.Multiline);
            if (Regex.Match(v, @"\d+\)").Success)
            {
                tmpOwners = Regex.Split(v, @"\d+\)").Where(x => x != "").Select(x => x.Trim()).ToList();
            }
            else tmpOwners.Add(v);
            foreach (var item in tmpOwners)
            {
                if (pat.Match(item).Success)
                {
                    var k = pat.Match(item);
                    a = new ElementOut.OwnersStruct
                    {
                        Name = k.Groups["name"].Value,
                        Address = k.Groups["address"].Value.Replace("\n", " "),
                        Country = CountryCodeIdentification(k.Groups["country"].Value)
                    };
                    ownList.Add(a);
                }
            }
            return ownList;
        }
        internal static ElementOut.AgentStruct AgentProcess(string v)
        {
            ElementOut.AgentStruct agent = new ElementOut.AgentStruct();
            if (v.Contains("\n"))
            {
                agent.Name = v.Remove(v.IndexOf("\n")).Trim();
                agent.Address = v.Substring(v.IndexOf("\n")).Replace("\n", " ").Trim();
            }
            else
            {
                agent.Name = v;
                agent.Address = "";
            }
            return agent;
        }*/
    }
}
