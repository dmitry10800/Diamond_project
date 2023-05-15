using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Diamond_ZA
{
    class Methods
    {
        public static string CcIdentification(string ccStr)
        {
            var tmpStr = "";
            var ccFullNames = new List<string> { "afghanistan", "aland islands", "albania", "algeria", "american samoa", "andorra", "angola", "anguilla", "antarctica", "antigua and barbuda", "argentina", "armenia", "aruba", "australia", "austria", "azerbaijan", "bahamas", "bahrain", "bangladesh", "barbados", "belarus", "belgium", "belize", "benin", "bermuda", "bhutan", "bolivia", "bosnia and herzegovina", "botswana", "bouvet island", "brazil", "british virgin islands", "british indian ocean territory", "brunei darussalam", "bulgaria", "burkina faso", "burundi", "cambodia", "cameroon", "canada", "cape verde", "cayman islands", "central african republic", "chad", "chile", "china", "hong kong, sar china", "macao, sar china", "christmas island", "cocos (keeling) islands", "colombia", "comoros", "congo (brazzaville)", "congo, (kinshasa)", "cook islands", "costa rica", "côte d'ivoire", "croatia", "cuba", "cyprus", "czech republic", "denmark", "djibouti", "dominica", "dominican republic", "ecuador", "egypt", "el salvador", "equatorial guinea", "eritrea", "estonia", "ethiopia", "falkland islands (malvinas)", "faroe islands", "fiji", "finland", "france", "french guiana", "french polynesia", "french southern territories", "gabon", "gambia", "georgia", "germany", "ghana", "gibraltar", "greece", "greenland", "grenada", "guadeloupe", "guam", "guatemala", "guernsey", "guinea", "guinea-bissau", "guyana", "haiti", "heard and mcdonald islands", "holy see (vatican city state)", "honduras", "hungary", "iceland", "india", "indonesia", "iran, islamic republic of", "iraq", "ireland", "isle of man", "israel", "italy", "jamaica", "japan", "jersey", "jordan", "kazakhstan", "kenya", "kiribati", "korea (north)", "korea (south)", "kuwait", "kyrgyzstan", "lao pdr", "latvia", "lebanon", "lesotho", "liberia", "libya", "liechtenstein", "lithuania", "luxembourg", "macedonia, republic of", "madagascar", "malawi", "malaysia", "maldives", "mali", "malta", "marshall islands", "martinique", "mauritania", "mauritius", "mayotte", "mexico", "micronesia, federated states of", "moldova", "monaco", "mongolia", "montenegro", "montserrat", "morocco", "mozambique", "myanmar", "namibia", "nauru", "nepal", "netherlands", "netherlands antilles", "new caledonia", "new zealand", "nicaragua", "niger", "nigeria", "niue", "norfolk island", "northern mariana islands", "norway", "oman", "pakistan", "palau", "palestinian territory", "panama", "papua new guinea", "paraguay", "peru", "philippines", "pitcairn", "poland", "portugal", "puerto rico", "qatar", "réunion", "romania", "russian federation", "rwanda", "saint-barthélemy", "saint helena", "saint kitts and nevis", "saint lucia", "saint-martin (french part)", "saint pierre and miquelon", "saint vincent and grenadines", "samoa", "san marino", "sao tome and principe", "saudi arabia", "senegal", "serbia", "seychelles", "sierra leone", "singapore", "slovakia", "slovenia", "solomon islands", "somalia", "south africa", "south georgia and the south sandwich islands", "south sudan", "spain", "sri lanka", "sudan", "suriname", "svalbard and jan mayen islands", "swaziland", "sweden", "switzerland", "syrian arab republic (syria)", "taiwan, republic of china", "tajikistan", "tanzania, united republic of", "thailand", "timor-leste", "togo", "tokelau", "tonga", "trinidad and tobago", "tunisia", "turkey", "turkmenistan", "turks and caicos islands", "tuvalu", "uganda", "ukraine", "united arab emirates", "united kingdom", "united states of america", "us minor outlying islands", "uruguay", "uzbekistan", "vanuatu", "venezuela (bolivarian republic)", "viet nam", "virgin islands, us", "wallis and futuna islands", "western sahara", "yemen", "zambia", "zimbabwe", "p. r. china", "republic of korea", "korea", "republic of san marino", "u s a", "u.s.a", "united states", "uae", "u.a.e", "congo", "switzerland", "chile", "prc", "p.r.c.", "england", "london", "united kingdom", "hong kong", "india", "cayman", "méxico", "r.o.c.", "u.s.a", "united sates of america", "united state of america,", "vietnam", "russia", "european union", "benelux" };
            var ccShortNames = new List<string> { "AF", "AX", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR", "AM", "AW", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY", "BE", "BZ", "BJ", "BM", "BT", "BO", "BA", "BW", "BV", "BR", "VG", "IO", "BN", "BG", "BF", "BI", "KH", "CM", "CA", "CV", "KY", "CF", "TD", "CL", "CN", "HK", "MO", "CX", "CC", "CO", "KM", "CG", "CD", "CK", "CR", "CI", "HR", "CU", "CY", "CZ", "DK", "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "ET", "FK", "FO", "FJ", "FI", "FR", "GF", "PF", "TF", "GA", "GM", "GE", "DE", "GH", "GI", "GR", "GL", "GD", "GP", "GU", "GT", "GG", "GN", "GW", "GY", "HT", "HM", "VA", "HN", "HU", "IS", "IN", "ID", "IR", "IQ", "IE", "IM", "IL", "IT", "JM", "JP", "JE", "JO", "KZ", "KE", "KI", "KP", "KR", "KW", "KG", "LA", "LV", "LB", "LS", "LR", "LY", "LI", "LT", "LU", "MK", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MQ", "MR", "MU", "YT", "MX", "FM", "MD", "MC", "MN", "ME", "MS", "MA", "MZ", "MM", "NA", "NR", "NP", "NL", "AN", "NC", "NZ", "NI", "NE", "NG", "NU", "NF", "MP", "NO", "OM", "PK", "PW", "PS", "PA", "PG", "PY", "PE", "PH", "PN", "PL", "PT", "PR", "QA", "RE", "RO", "RU", "RW", "BL", "SH", "KN", "LC", "MF", "PM", "VC", "WS", "SM", "ST", "SA", "SN", "RS", "SC", "SL", "SG", "SK", "SI", "SB", "SO", "ZA", "GS", "SS", "ES", "LK", "SD", "SR", "SJ", "SZ", "SE", "CH", "SY", "TW", "TJ", "TZ", "TH", "TL", "TG", "TK", "TO", "TT", "TN", "TR", "TM", "TC", "TV", "UG", "UA", "AE", "GB", "US", "UM", "UY", "UZ", "VU", "VE", "VN", "VI", "WF", "EH", "YE", "ZM", "ZW", "CN", "KR", "KR", "SM", "US", "US", "US", "AE", "AE", "CD", "CH", "CL", "CN", "CN", "GB", "GB", "GB", "HK", "IN", "KY", "MX", "TW", "US", "US", "US", "VN", "RU", "EM", "BX" };
            foreach (var country in ccFullNames)
            {
                if (ccStr.ToLower().Contains(country))
                {
                    tmpStr = ccShortNames.ElementAt(ccFullNames.IndexOf(country));
                    return tmpStr;
                }
                else { tmpStr = ccStr; }
            }
            return tmpStr;
        }
        /*Date*/
        public static string DateSwap(string tmpDate)
        {
            string swapDate;
            var splitDate = tmpDate.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            return swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
        }

        /*71 owner split*/
        public static (string[] ownerName, string[] ownerAddress, string[] ownerCountry) OwnerSplit(string tmpStr)
        {
            var I71 = "71:";
            tmpStr = tmpStr/*.Replace(" and ", ";")*/.Replace(I71, "").Replace("\n", " ").Trim();
            string[] ownerName = null;
            string[] ownerAddress = null;
            string[] ownerCountry = null;
            string[] splitedOwner = null;
            if (tmpStr.Contains(";"))
            {
                splitedOwner = tmpStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitedOwner.Count() > 0)
                {
                    foreach (var record in splitedOwner)
                    {
                        if (record.Contains(","))
                        {
                            string tmpSplName = null;
                            string tmpSplAddr = null;
                            if (Regex.IsMatch(record.ToLower(), @"(\,.*\sllc\,)|(\,.*\sgmbh\,)|(\,.*\sinc(\.|\,)*\,)|(\,.*\sltd(\.|\,)*\,)|(\,.*\ss\.a\.\,)|(\,.*\ss\.l\.\,)|(\,.*\ss\.a\.u\.\,)"))
                            {
                                if (record.Count(f => f == ',') >= 2)
                                {
                                    var firstIndex = record.IndexOf(",");
                                    var secondIndex = record.IndexOf(",", firstIndex + 1);
                                    tmpSplName = record.Remove(secondIndex).Trim();
                                    tmpSplAddr = record.Substring(secondIndex).Trim(',').Trim();
                                }
                            }
                            else
                            {
                                tmpSplName = record.Remove(record.IndexOf(",")).Trim();
                                tmpSplAddr = record.Substring(record.IndexOf(",")).Trim(',').Trim();
                            }
                            ownerName = (ownerName ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplName }).ToArray();
                            ownerAddress = (ownerAddress ?? Enumerable.Empty<string>()).Concat(new[] { tmpSplAddr }).ToArray();
                            if (record.Contains(","))
                            {
                                ownerCountry = (ownerCountry ?? Enumerable.Empty<string>()).Concat(new[] { CcIdentification(record.Substring(record.LastIndexOf(","))) }).ToArray();
                            }
                            else
                            {
                                ownerCountry = (ownerCountry ?? Enumerable.Empty<string>()).Concat(new[] { CcIdentification(record) }).ToArray();
                            }
                        }
                    }
                }
            }
            else if (tmpStr.Contains(","))
            {
                string tmpSplName = null;
                string tmpSplAddr = null;
                if (Regex.IsMatch(tmpStr.ToLower(), @"(\,.*\sllc\,)|(\,.*\sgmbh\,)|(\,.*\sinc(\.|\,)*\,)|(\,.*\sltd(\.|\,)*\,)|(\,.*\ss\.a\.\,)|(\,.*\ss\.l\.\,)|(\,.*\ss\.a\.u\.\,)"))
                {
                    if (tmpStr.Count(f => f == ',') >= 2)
                    {
                        var firstIndex = tmpStr.IndexOf(",");
                        var secondIndex = tmpStr.IndexOf(",", firstIndex + 1);
                        tmpSplName = tmpStr.Remove(secondIndex).Trim();
                        tmpSplAddr = tmpStr.Substring(secondIndex).Trim(',').Trim();
                    }
                }
                else
                {
                    tmpSplName = tmpStr.Remove(tmpStr.IndexOf(",")).Trim();
                    tmpSplAddr = tmpStr.Substring(tmpStr.IndexOf(",")).Trim(',').Trim();
                }
                ownerName = (ownerName ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplName }).ToArray();
                ownerAddress = (ownerAddress ?? Enumerable.Empty<string>()).Concat(new[] { tmpSplAddr }).ToArray();
                if (tmpStr.Contains(","))
                {
                    ownerCountry = (ownerCountry ?? Enumerable.Empty<string>()).Concat(new[] { CcIdentification(tmpStr.Substring(tmpStr.LastIndexOf(","))) }).ToArray();
                }
                else
                {
                    ownerCountry = (ownerCountry ?? Enumerable.Empty<string>()).Concat(new[] { CcIdentification(tmpStr) }).ToArray();
                }

            }
            return (ownerName, ownerAddress, ownerCountry);
        }

        /*72 split*/
        public static string[] StSplit(string tmpStr)
        {
            var I72 = "72:";
            tmpStr = tmpStr.Replace(I72, "").Replace(",", "").Trim();
            string[] stSplitted = null;
            List<string> stSplittedTrimed = null;
            stSplittedTrimed = new List<string>();
            if (tmpStr.Contains(";"))
            {
                stSplitted = tmpStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in stSplitted)
                {
                    stSplittedTrimed.Add(item.Trim());
                }
                stSplitted = stSplittedTrimed.ToArray();
            }
            else
            {
                stSplitted = (stSplitted ?? Enumerable.Empty<string>()).Concat(new string[] { tmpStr }).ToArray();
            }
            return stSplitted;
        }

        /*Splitting record by INIDs numbers*/
        public static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            List<string> splittedRecordTrimed = null;
            splittedRecordTrimed = new List<string>();
            var tempStrC = recString.Replace("\n", " ");
            tempStrC = Regex.Replace(tempStrC, @"(\~*\s*)(\d{2}\s*\:)", m => "~" + m.Groups[2].Value);
            if (recString != "")
            {
                if (recString.Contains("\n"))
                {
                    recString = recString.Replace("\n", " ");
                }
                if (recString.Contains("~"))
                {
                    splittedRecord = tempStrC.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in splittedRecord)
                    {
                        splittedRecordTrimed.Add(HttpUtility.HtmlDecode(item.Replace("¿", " ").Trim()));
                    }
                }
            }
            return splittedRecord = splittedRecordTrimed.ToArray();
        }


        /*Splitting method for Complete Specifications chapter*/
        public static string[] RecSplitSecondList(string recString)
        {
            string[] splittedRecord = null;
            var tempStrC = recString.Replace("\n", " ").Replace("Registrar of Patents", " ").Trim();
            string tmpDescValue = null;
            if (recString != "")
            {
                if (Regex.IsMatch(recString, @"00\:\s*\-*"))
                {
                    tmpDescValue = tempStrC.Substring(tempStrC.IndexOf(Regex.Match(recString, @"00\:\s*\-*").Value)).Trim();
                    tempStrC = tempStrC.Remove(recString.IndexOf(Regex.Match(recString, @"00\:\s*\-*").Value)).Trim();
                }
                var regexPatOne = new Regex(@"\d{2}\:", RegexOptions.IgnoreCase);
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

        /*Sending data into Diamond*/
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                var url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
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
