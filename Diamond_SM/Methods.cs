using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_SM
{
    class Methods : ProcessFirstList
    {
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
        /*Sending data into Diamond*/
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);

                /*Staging value*/
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                /*Production value*/
                string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; // PRODUCTION

                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
        /*Splitting record by inid codes/names*/
        public static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString.Trim();
            tempStrC = tempStrC.Replace("\n", " ")
                .Replace(I21, "*****" + I21)
                .Replace(I22, "*****" + I22)
                .Replace(I30, "*****" + I30)
                .Replace(I54, "*****" + I54)
                .Replace(I57, "*****" + I57)
                .Replace(I72, "*****" + I72)
                .Replace(I73, "*****" + I73)
                .Replace(I74, "*****" + I74)
                .Replace(I96Date, "*****" + I96Date)
                .Replace(I96Number, "*****" + I96Number)
                .Replace(I97, "*****" + I97);
            if (tempStrC != "")
            {
                splittedRecord = tempStrC.Split(new string[] { "*****" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            }
            return splittedRecord;
        }

        /*Date*/
        public static string DateNormalize(string tmpDate)
        {
            Regex patternDate = new Regex(@"(?<day>\d+)\s*\.*\/*\-*(?<month>\d+)\s*\.*\/*\-*(?<year>\d{4})");
            var a = patternDate.Match(tmpDate);
            if (a.Success)
            {
                return a.Groups["year"].Value + "-" + a.Groups["month"].Value + "-" + a.Groups["day"].Value;
            }
            else return tmpDate;
        }

        internal static List<ElementOut.PrioStruct> PriorityProcess(string v)
        {
            List<ElementOut.PrioStruct> prios = new List<ElementOut.PrioStruct>();
            ElementOut.PrioStruct prio;
            List<string> prioList = new List<string>();
            Regex pat = new Regex(@"(?<number>.*)\s*\b(?<date>\d+\/\d+\/\d{4})\s*(?<country>[A-Z]{2})");
            string tmpV = v.Replace("and", ";");
            if (tmpV.Trim() != "")
            {
                /*Case - multiple recrods*/
                if (tmpV.Contains(";"))
                {
                    prioList = tmpV.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                }
                /*Case - one record*/
                else
                {
                    prioList.Add(tmpV);
                }
                foreach (var item in prioList)
                {
                    if (pat.Match(item).Success)
                    {
                        var k = pat.Match(item);
                        prio = new ElementOut.PrioStruct
                        {
                            Number = k.Groups["number"].Value.Trim(),
                            Date = DateNormalize(k.Groups["date"].Value),
                            Country = k.Groups["country"].Value
                        };
                        prios.Add(prio);
                    }
                    else
                    {
                        Console.WriteLine("Priority identification error");
                    }
                }
            }
            return prios;
        }
        /*72*/
        internal static List<ElementOut.InventorsStruct> InventorsProcess(string v)
        {
            List<ElementOut.InventorsStruct> inventors = new List<ElementOut.InventorsStruct>();
            ElementOut.InventorsStruct inv;
            List<string> invList = new List<string>();
            string tmpV = v.Trim();
            if (tmpV.Contains(";"))
            {
                invList = tmpV.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            }
            else
            {
                invList.Add(tmpV);
            }
            foreach (var item in invList)
            {
                inv = new ElementOut.InventorsStruct
                {
                    Name = item
                };
                inventors.Add(inv);
            }
            return inventors;
        }
        /*73*/
        internal static List<ElementOut.ApplicantsStruct> ApplicantsProcess(string s)
        {
            var v = s;
            v = Regex.Replace(v, @"\,\s*d\.o\.o", " d.o.o", RegexOptions.IgnoreCase);
            v = Regex.Replace(v, @"\,\s*ltd", " Ltd", RegexOptions.IgnoreCase);
            v = Regex.Replace(v, @"\,\s*s\.l", " S.L.", RegexOptions.IgnoreCase);
            v = Regex.Replace(v, @"\,\s*inc\.", " Inc", RegexOptions.IgnoreCase);
            v = Regex.Replace(v, @"\,\s*s\.a\.", " S.A.", RegexOptions.IgnoreCase);
            v = Regex.Replace(v, @"\,\s*s\.p\.a\.", " S.P.A.", RegexOptions.IgnoreCase);

            List<ElementOut.ApplicantsStruct> applicants = new List<ElementOut.ApplicantsStruct>();
            ElementOut.ApplicantsStruct app;
            Regex pat = new Regex(@"(?<separator>\sand\s)(?<valueNext>[A-Z]+\b\s*([A-Z]+))");

            List<string> appList = new List<string>();
            if (pat.Match(v).Success)
            {
                int matchCount = pat.Matches(v).Count;
                int startIndex;
                for (int i = 0; i <= matchCount; i++)
                {
                    if (pat.Match(v).Success)
                    {
                        startIndex = pat.Match(v).Groups["separator"].Index;
                        appList.Add(v.Remove(startIndex));
                        v = v.Substring(startIndex + 4).Trim();
                    }
                    else
                    {
                        appList.Add(v);
                    }
                }
            }
            else
            {
                appList.Add(v);
            }
            foreach (var item in appList)
            {
                string tmp = item.Replace("\n", " ").Trim();
                Regex recPat = new Regex(@"(?<name>.*^[^,]+),(?<address>.*),\s*(?<country>([a-zA-z\.]+\s*)+)");
                var k = recPat.Match(tmp);
                app = new ElementOut.ApplicantsStruct();
                app.Name = k.Groups["name"].Value.Trim();
                app.Address = k.Groups["address"].Value.Trim();
                app.Country = CountryCodeIdentification(k.Groups["country"].Value.Trim());
                applicants.Add(app);
            }
            return applicants;
        }
        /*74*/
        internal static ElementOut.AgentStruct AgentProcess(string v)
        {
            ElementOut.AgentStruct agent = new ElementOut.AgentStruct();
            Regex patA = new Regex(@"(?<name>.*)(?<separator>\sVia\s)(?<address>.*)");
            Regex patB = new Regex(@"(?<name>.*\,\s*[A-z]+\b)\s(?<address>.*)");
            /*first case: string contains Via*/
            if (patA.Match(v).Success)
            {
                var k = patA.Match(v);
                agent.Name = k.Groups["name"].Value.Trim();
                agent.Address = k.Groups["address"].Value.Trim();
            }
            /*second case: string doesn't contains Via*/
            else if (patB.Match(v).Success)
            {
                var k = patB.Match(v);
                agent.Name = k.Groups["name"].Value.Trim();
                agent.Address = k.Groups["address"].Value.Trim();
            }
            else
                throw new System.ArgumentException("Agent identification error");

            return agent;
        }
    }
}
