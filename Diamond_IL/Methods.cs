using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_IL
{
    class Methods
    {
        public static string CountryCodeIdentification(string tmpString)
        {
            string tmpStr = tmpString;
            List<string> ccFullNames = new List<string> { "afghanistan", "aland islands", "albania", "algeria", "american samoa", "andorra", "angola", "anguilla", "antarctica", "antigua and barbuda", "argentina", "armenia", "aruba", "australia", "austria", "azerbaijan", "bahamas", "bahrain", "bangladesh", "barbados", "belarus", "belgium", "belize", "benin", "bermuda", "bhutan", "bolivia", "bosnia and herzegovina", "botswana", "bouvet island", "brazil", "british virgin islands", "british indian ocean territory", "brunei darussalam", "bulgaria", "burkina faso", "burundi", "cambodia", "cameroon", "canada", "cape verde", "cayman islands", "central african republic", "chad", "chile", "china", "hong kong, sar china", "macao, sar china", "christmas island", "cocos (keeling) islands", "colombia", "comoros", "congo (brazzaville)", "congo, (kinshasa)", "cook islands", "costa rica", "côte d'ivoire", "croatia", "cuba", "cyprus", "czech republic", "denmark", "djibouti", "dominica", "dominican republic", "ecuador", "egypt", "el salvador", "equatorial guinea", "eritrea", "estonia", "ethiopia", "falkland islands", "faroe islands", "fiji", "finland", "france", "french guiana", "french polynesia", "french southern territories", "gabon", "gambia", "georgia", "germany", "ghana", "gibraltar", "greece", "greenland", "grenada", "guadeloupe", "guam", "guatemala", "guernsey", "guinea", "guinea-bissau", "guyana", "haiti", "heard and mcdonald islands", "holy see (vatican city state)", "honduras", "hungary", "iceland", "india", "indonesia", "iran, islamic republic of", "iraq", "ireland", "isle of man", "israel", "italy", "jamaica", "japan", "jersey", "jordan", "kazakhstan", "kenya", "kiribati", "korea (north)", "korea (south)", "kuwait", "kyrgyzstan", "lao pdr", "latvia", "lebanon", "lesotho", "liberia", "libya", "liechtenstein", "lithuania", "luxembourg", "macedonia, republic of", "madagascar", "malawi", "malaysia", "maldives", "mali", "malta", "marshall islands", "martinique", "mauritania", "mauritius", "mayotte", "mexico", "micronesia, federated states of", "moldova", "monaco", "mongolia", "montenegro", "montserrat", "morocco", "mozambique", "myanmar", "namibia", "nauru", "nepal", "netherlands", "netherlands antilles", "new caledonia", "new zealand", "nicaragua", "niger", "nigeria", "niue", "norfolk island", "northern mariana islands", "norway", "oman", "pakistan", "palau", "palestinian territory", "panama", "papua new guinea", "paraguay", "peru", "philippines", "pitcairn", "poland", "portugal", "puerto rico", "qatar", "réunion", "romania", "russian federation", "rwanda", "saint-barthélemy", "saint helena", "saint kitts and nevis", "saint lucia", "saint-martin (french part)", "saint pierre and miquelon", "saint vincent and grenadines", "samoa", "san marino", "sao tome and principe", "saudi arabia", "senegal", "serbia", "seychelles", "sierra leone", "singapore", "slovakia", "slovenia", "solomon islands", "somalia", "south africa", "south georgia and the south sandwich islands", "south sudan", "spain", "sri lanka", "sudan", "suriname", "svalbard and jan mayen islands", "swaziland", "sweden", "switzerland", "syria", "taiwan, republic of china", "tajikistan", "tanzania, united republic of", "thailand", "timor-leste", "togo", "tokelau", "tonga", "trinidad and tobago", "tunisia", "turkey", "turkmenistan", "turks and caicos islands", "tuvalu", "uganda", "ukraine", "united arab emirates", "united kingdom", "united states of america", "us minor outlying islands", "uruguay", "uzbekistan", "vanuatu", "venezuela (bolivarian republic)", "viet nam", "virgin islands, us", "wallis and futuna islands", "western sahara", "yemen", "zambia", "zimbabwe", "p. r. china", "republic of korea", "korea", "republic of san marino", "u s a", "u.s.a", "united states", "uae", "u.a.e", "congo", "switzerland", "chile", "prc", "p.r.c.", "england", "london", "united kingdom", "hong kong", "india", "cayman", "méxico", "r.o.c.", "u.s.a", "united sates of america", "united state of america,", "vietnam", "russia", "european union", "benelux" };
            List<string> ccShortNames = new List<string> { "AF", "AX", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR", "AM", "AW", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY", "BE", "BZ", "BJ", "BM", "BT", "BO", "BA", "BW", "BV", "BR", "VG", "IO", "BN", "BG", "BF", "BI", "KH", "CM", "CA", "CV", "KY", "CF", "TD", "CL", "CN", "HK", "MO", "CX", "CC", "CO", "KM", "CG", "CD", "CK", "CR", "CI", "HR", "CU", "CY", "CZ", "DK", "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "ET", "FK", "FO", "FJ", "FI", "FR", "GF", "PF", "TF", "GA", "GM", "GE", "DE", "GH", "GI", "GR", "GL", "GD", "GP", "GU", "GT", "GG", "GN", "GW", "GY", "HT", "HM", "VA", "HN", "HU", "IS", "IN", "ID", "IR", "IQ", "IE", "IM", "IL", "IT", "JM", "JP", "JE", "JO", "KZ", "KE", "KI", "KP", "KR", "KW", "KG", "LA", "LV", "LB", "LS", "LR", "LY", "LI", "LT", "LU", "MK", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MQ", "MR", "MU", "YT", "MX", "FM", "MD", "MC", "MN", "ME", "MS", "MA", "MZ", "MM", "NA", "NR", "NP", "NL", "AN", "NC", "NZ", "NI", "NE", "NG", "NU", "NF", "MP", "NO", "OM", "PK", "PW", "PS", "PA", "PG", "PY", "PE", "PH", "PN", "PL", "PT", "PR", "QA", "RE", "RO", "RU", "RW", "BL", "SH", "KN", "LC", "MF", "PM", "VC", "WS", "SM", "ST", "SA", "SN", "RS", "SC", "SL", "SG", "SK", "SI", "SB", "SO", "ZA", "GS", "SS", "ES", "LK", "SD", "SR", "SJ", "SZ", "SE", "CH", "SY", "TW", "TJ", "TZ", "TH", "TL", "TG", "TK", "TO", "TT", "TN", "TR", "TM", "TC", "TV", "UG", "UA", "AE", "GB", "US", "UM", "UY", "UZ", "VU", "VE", "VN", "VI", "WF", "EH", "YE", "ZM", "ZW", "CN", "KR", "KR", "SM", "US", "US", "US", "AE", "AE", "CD", "CH", "CL", "CN", "CN", "GB", "GB", "GB", "HK", "IN", "KY", "MX", "TW", "US", "US", "US", "VN", "RU", "EM", "BX" };
            foreach (var country in ccFullNames)
            {
                if (tmpString.ToLower().Contains(country))
                {
                    tmpStr = ccShortNames.ElementAt(ccFullNames.IndexOf(country));
                    return tmpStr;
                }
            }
            return tmpStr;
        }

        public static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tmpDescValue = null;
            string tempStrC = recString.Trim();
            if (recString != "")
            {
                Regex regexPatOne = new Regex(@"\[\s*\d{2}\s*\]", RegexOptions.IgnoreCase);
                MatchCollection matchesClass = regexPatOne.Matches(recString);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        //if (!matchC.Value.StartsWith(I32) && !matchC.Value.StartsWith(I33))
                        //{
                        //    tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                        //}
                        tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
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

        public static string NormalizeDate(string tmpDate)
        {
            string swapDate;
            string[] splitDate = null;
            if (Regex.IsMatch(tmpDate, @"\d{2}\.\d{2}\.\d{4}"))
            {
                splitDate = Regex.Match(tmpDate, @"\d{2}\.\d{2}\.\d{4}").Value.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (Regex.IsMatch(tmpDate, @"\d{2}\/\d{2}\/\d{4}"))
            {
                splitDate = Regex.Match(tmpDate, @"\d{2}\/\d{2}\/\d{4}").Value.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            }
            return splitDate != null ? swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0] : null;
        }

        public class IntClassStruct
        {
            public string IVersion { get; set; }
            public string[] INumber { get; set; }
        }

        public static IntClassStruct IntClassSplit(string tmpStr)
        {
            IntClassStruct intClass = new IntClassStruct();
            string classVersion = null;
            string intClassValue = tmpStr;
            List<string> numbersList = new List<string>();
            string versionPattern = @"Cl\.\s*\(\d{4}\.\d{2}\)";
            string numberPattern = @"[A-Z]{1}\d{2}[A-Z]{1}";
            if (Regex.IsMatch(intClassValue, versionPattern)) { classVersion = Regex.Match(Regex.Match(intClassValue, versionPattern).Value, @"\d{4}\.\d{2}").Value; }
            if (Regex.IsMatch(intClassValue, versionPattern))
            {
                intClassValue = intClassValue.Substring(intClassValue.IndexOf(Regex.Match(intClassValue, versionPattern).Value));
                if (Regex.IsMatch(intClassValue, numberPattern))
                {
                    //@"(?<=.*\[\d{2}\])"
                    var splittedNumber = Regex.Split(intClassValue, @"(?=[A-Z]{1}\d{2}[A-Z])").Where(x => Regex.IsMatch(x, numberPattern)).Select(x => x.Trim().Trim(','));
                    if (splittedNumber != null && splittedNumber.Count() > 0)
                    {
                        foreach (var item in splittedNumber)
                        {
                            string firstPartOfNumber = Regex.Match(item, numberPattern).Value;
                            string tmpItem = item.Replace(firstPartOfNumber, "").Trim();
                            if (tmpItem.Contains(","))
                            {
                                var tmpSpl = tmpItem.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
                                foreach (var secondPartOfNumber in tmpSpl)
                                {
                                    if (Regex.IsMatch(secondPartOfNumber, @"\d+\/\d+\/\d+"))
                                    {
                                        var pat = new Regex(Regex.Escape("/"));
                                        numbersList.Add(firstPartOfNumber + " " + pat.Replace(secondPartOfNumber, "", 1));
                                    }
                                    else numbersList.Add(firstPartOfNumber + " " + secondPartOfNumber);
                                }
                            }
                            else
                            {
                                numbersList.Add(item);
                            }
                        }
                    }
                }
            }
            if (numbersList != null && numbersList.Count() > 0 && classVersion != null)
            {
                intClass.INumber = numbersList.Select(x => x.Replace("  ", " ").Trim()).ToArray();
                intClass.IVersion = classVersion;
                return intClass;
            }
            else return null;
        }

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
