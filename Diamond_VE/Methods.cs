using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_VE
{
    class Methods
    {
        /*Country code identification*/
        static string CountryCodeIdentification(string str)
        {
            string countryCode = "";
            List<string> ccFullNames = new List<string> { "japon", "belgica", "canada", "afganistán", "åland", "albania", "argelia", "samoa americana", "andorra", "angola", "anguila", "antártida", "antigua y barbuda", "argentina", "armenia", "aruba", "isla ascensión", "australia", "austria", "azerbaiyán", "bahamas", "bahréin", "bangladesh", "barbados", "bielorrusia", "bélgica", "belice", "benín", "bermudas", "bután", "bolivia", "bonaire, san estaquio y saba", "bosnia-herzegovina", "botsuana", "brasil", "territorio británico en el océano índico", "islas vírgenes británicas", "brunei darussalam", "bulgaria", "burkina faso", "burundi", "camboya", "camerún", "canadá", "cabo verde", "islas caimán", "república centroafricana", "chad", "chile", "china", "isla de navidad", "islas cocos", "colombia", "comoras", "congo", "islas cook", "costa rica", "costa de marfil", "croacia", "cuba", "curasao", "chipre", "república checa", "república democrática del congo", "dinamarca", "yibuti", "dominica", "república dominicana", "timor oriental", "ecuador", "egipto", "el salvador", "guinea ecuatorial", "eritrea", "estonia", "etiopía", "unión europea", "islas malvinas", "islas feroe", "fiyi", "finlandia", "francia", "guayana francesa", "polinesia francesa", "territorios australes franceses", "gabón", "gambia", "georgia", "alemania", "ghana", "gibraltar", "grecia", "groenlandia", "granada", "guadalupe", "guam", "guatemala", "guernesey", "guinea", "guinea-bissau", "guyana", "haití", "islas heard y mcdonald", "holanda", "honduras", "hong kong", "hungría", "islandia", "india", "indonesia", "irán", "iraq", "irlanda", "isla de man", "israel", "italia", "costa de marfil", "jamaica", "japón", "isla de jersey", "jordania", "kazajistán", "kenia", "kiribati", "corea (del norte)", "corea (del sur)", "kuwait", "kirguistán", "laos", "letonia", "líbano", "lesotho", "liberia", "libia", "liechtenstein", "lituania", "luxemburgo", "macao", "macedonia", "madagascar", "malawi", "malasia", "maldivas", "malí", "malta", "islas marshall", "martinica", "mauritania", "mauricio", "mayotte", "méxico", "micronesia", "moldavia", "mónaco", "mongolia", "montenegro", "montserrat", "marruecos", "mozambique", "myanmar", "namibia", "nauru", "nepal", "países bajos", "antillas nederlandesas", "nueva caledonia", "nueva zelanda", "nicaragua", "níger", "nigeria", "niue", "isla norfolk", "irlanda del norte", "islas marianas del norte", "noruega", "omán", "pakistán", "palau", "palestina", "panamá", "papúa nueva guinea", "paraguay", "perú", "filipinas", "islas pitcairn", "polonia", "portugal", "puerto rico", "qatar", "reunión", "rumania", "rusia", "ruanda", "santa helena", "san cristóbal y nieves", "santa lucía", "san martín", "san vicente y las granadinas", "san pedro y miquelón", "san marino", "santo tomé y príncipe", "arabia saudita", "senegal", "serbia", "seychelles", "sierra leona", "singapur", "eslovaquia", "eslovenia", "islas salomón", "somalia", "sudáfrica", "islas georgias del sur y sandwich del sur", "sudán del sur", "unión soviética", "españa", "sri lanka", "sudán", "surinam", "islas svalbard y jan mayen", "swazilandia", "suecia", "suiza", "siria", "taiwán", "tayikistán", "tanzania", "tailandia", "togo", "tokelau", "tonga", "trinidad y tobago", "túnez", "turquía", "turkmenistán", "islas turcas y caicos", "tuvalu", "uganda", "ucrania", "emiratos árabes unidos", "reino unido", "estados unidos", "uruguay", "islas vírgenes de los estados unidos", "uzbekistán", "vanuatu", "ciudad del vaticano", "venezuela", "vietnam", "wallis y futuna", "samoa del oeste", "yemen", "yugoslavia", "zambia", "zimbabue" };
            List<string> ccShortNames = new List<string> { "JP", "BE", "CA", "AF", "AX", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR", "AM", "AW", "AC", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY", "BE", "BZ", "BJ", "BM", "BT", "BO", "BQ", "BA", "BW", "BR", "IO", "VG", "BN", "BG", "BF", "BI", "KH", "CM", "CA", "CV", "KY", "CF", "TD", "CL", "CN", "CX", "CC", "CO", "KM", "CG", "CK", "CR", "CI", "HR", "CU", "CW", "CY", "CZ", "CD", "DK", "DJ", "DM", "DO", "TL", "EC", "EG", "SV", "GQ", "ER", "EE", "ET", "EU", "FK", "FO", "FJ", "FI", "FR", "GF", "PF", "TF", "GA", "GM", "GE", "DE", "GH", "GI", "GR", "GL", "GD", "GP", "GU", "GT", "GG", "GN", "GW", "GY", "HT", "HM", "NL", "HN", "HK", "HU", "IS", "IN", "ID", "IR", "IQ", "IE", "IM", "IL", "IT", "CI", "JM", "JP", "JE", "JO", "KZ", "KE", "KI", "KP", "KO", "KW", "KG", "LA", "LV", "LB", "LS", "LR", "LY", "LI", "LT", "LU", "MO", "MK", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MQ", "MR", "MU", "YT", "MX", "FM", "MD", "MC", "MN", "ME", "MS", "MA", "MZ", "MM", "NA", "NR", "NP", "NL", "AN", "NC", "NZ", "NI", "NE", "NG", "NU", "NF", "GB", "MP", "NO", "OM", "PK", "PW", "PS", "PA", "PG", "PY", "PE", "PH", "PN", "PL", "PT", "PR", "QA", "RE", "RO", "RU", "RW", "SH", "KN", "LC", "SX", "VC", "PM", "SM", "ST", "SA", "SN", "RS", "SC", "SL", "SG", "SK", "SI", "SB", "SO", "ZA", "GS", "SS", "SU", "ES", "LK", "SD", "SR", "SJ", "SZ", "SE", "CH", "SY", "TW", "TJ", "TZ", "TH", "TG", "TK", "TO", "TT", "TN", "TR", "TM", "TC", "TV", "UG", "UA", "AE", "UK", "US", "UY", "VI", "UZ", "VU", "VA", "VE", "VN", "WF", "WS", "YE", "YU", "ZM", "ZW" };
            foreach (var country in ccFullNames)
            {
                if (str.ToLower().Contains(country))
                {
                    countryCode = ccShortNames.ElementAt(ccFullNames.IndexOf(country));
                    return countryCode;
                }
            }
            return str;
        }


        /*Date swapping*/
        public static string DateNormalize(string tmpDate)
        {
            try
            {
                string swapDate = "";
                string[] splitDate = tmpDate.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitDate.Count() == 3)
                {
                    swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
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
            public string[] Country { get; set; }

        }
        ///*Name, address, country code split in 71,72,73,74,75 fields*/
        //public static NameCountryStruct NameSpl(string tmpString)
        //{
        //    NameCountryStruct splittedValues = new NameCountryStruct();
        //    string tmpValues = tmpString.Replace("(71)", "").Replace("(72)", "").Replace("(73)", "").Replace("(74)", "").Replace("(75)", "").Replace("\n", "****").Trim();
        //    if (Regex.IsMatch(tmpValues, @"\*{4}[A-Z]{2}\*{4}"))
        //    {
        //        string[] tmpSplValues = Regex.Split(tmpValues, @"(?<=\*{4}[A-Z]{2}\*{4})").Where(d => d != "").ToArray();
        //        foreach (var rec in tmpSplValues)
        //        {
        //            if (Regex.IsMatch(rec, @"\*{4}[A-Z]{2}\*{4}"))
        //            {
        //                splittedValues.Country = (splittedValues.Country ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(rec, @"\*{4}[A-Z]{2}\*{4}$").Value.Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
        //                splittedValues.Name = (splittedValues.Name ?? Enumerable.Empty<string>()).Concat(new string[] { rec.Remove(rec.IndexOf(Regex.Match(rec, @"\*{4}[A-Z]{2}\*{4}$").Value)).Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
        //            }
        //            else splittedValues.Name = (splittedValues.Name ?? Enumerable.Empty<string>()).Concat(new string[] { rec.Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
        //        }
        //    }
        //    else splittedValues.Name = (splittedValues.Name ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValues.Replace("****", " ").Trim().Trim(',').Trim() }).ToArray();
        //    return splittedValues;
        //}

        /*73 field split*/
        public class AssigneeStruct
        {
            public static readonly string AddressKey = "Domicilio:";
            public static readonly string CountryKey = "País:";
            public string[] Name { get; set; }
            public string[] Address { get; set; }
            public string[] Country { get; set; }
        }
        public static AssigneeStruct AssigneeSplit(string str)
        {
            string[] splValues = null;
            string I73 = "(73)";
            string tmpStr = str.Replace(I73, "")/*.Replace("\n", " ")*/.Trim();
            /*Multiple (TEST VERSION, need more cases to check!)*/
            Regex addr = new Regex(@"Domicilio\:", RegexOptions.IgnoreCase);
            Regex country = new Regex(@"País\:", RegexOptions.IgnoreCase);
            MatchCollection matchesAddr = addr.Matches(tmpStr);
            MatchCollection matchesCountry = country.Matches(tmpStr);
            if (matchesAddr.Count > 1 && matchesCountry.Count > 1 && tmpStr.Contains("\n"))
            {

                string testStr = tmpStr.Replace("\n", "****");
                //string tt = Regex.Replace(testStr, @"(País\:)(.*)([A-Z])(\*{4})([A-Z](.*)(Domicilio\:))", "$1$2$3[separator]$5$6");
                string tt = Regex.Replace(testStr, @"(País\:)(.*)([A-Z]|\))(\*{4})([A-Z](.*)(Domicilio\:))", m => m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value + "[separator]" + m.Groups[5].Value);
                /*works, but not for all of cases*/
                //splValues = Regex.Split(testStr, @"(\*{4}.*)(?=)").Where(x => x != "").Select(x => x.Replace("****", " ").Trim()).ToArray();
                if (tt.Contains("[separator]"))
                {
                    splValues = tt.Split(new string[] { "[separator]" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace("****", " ")).ToArray();
                }
                else
                {
                    splValues = (splValues ?? Enumerable.Empty<string>()).Concat(new string[] { tmpStr }).ToArray();
                }
            }
            else
            {
                splValues = (splValues ?? Enumerable.Empty<string>()).Concat(new string[] { tmpStr }).ToArray();
            }
            /*End multiple identification*/

            if (splValues != null)
            {
                AssigneeStruct assignee = new AssigneeStruct();
                foreach (var record in splValues)
                {
                    string tmpRecord = record;
                    if (tmpRecord.Contains(AssigneeStruct.AddressKey) && tmpRecord.Contains(AssigneeStruct.CountryKey))
                    {

                        assignee.Country = (assignee.Country ?? Enumerable.Empty<string>()).Concat(new string[] { CountryCodeIdentification(tmpRecord.Substring(tmpRecord.IndexOf(AssigneeStruct.CountryKey)).Replace(AssigneeStruct.CountryKey, "").Trim()) }).ToArray();
                        tmpRecord = tmpRecord.Remove(tmpRecord.IndexOf(AssigneeStruct.CountryKey)).Trim();
                        assignee.Address = (assignee.Address ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecord.Substring(tmpRecord.IndexOf(AssigneeStruct.AddressKey)).Replace(AssigneeStruct.AddressKey, "").Trim().Trim(',').Trim() }).ToArray();
                        tmpRecord = tmpRecord.Remove(tmpRecord.IndexOf(AssigneeStruct.AddressKey)).Trim().Trim(',').Trim();
                        assignee.Name = (assignee.Name ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecord }).ToArray();
                    }
                }
                return assignee;
            }
            else return null;
        }

        public static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString/*.Replace("\n", " ")*/.Trim();
            string tmpDescValue = null;
            if (recString != "")
            {
                if (recString.Contains("(57)"))
                {
                    tmpDescValue = tempStrC.Substring(tempStrC.IndexOf("(57)")).Trim();
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
            //public string[] Version { get; set; } // Date
            public string[] Classification { get; set; } // Number
        }
        /*Classification Information 51 inid*/
        public static ClassificationStruct ClassificationInfoSplit(string tmpString)
        {
            ClassificationStruct priority = new ClassificationStruct();
            string[] splClass = null;
            string tmpClass = tmpString.Replace("(51)", "").Replace("\n", " ").Replace(" ", "").Trim();
            if (tmpClass.Contains(";"))
            {
                splClass = tmpClass.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            }
            else
            {
                splClass = (splClass ?? Enumerable.Empty<string>()).Concat(new string[] { tmpClass }).ToArray();
            }
            if (splClass != null)
            {
                foreach (var record in splClass)
                {
                    if (Regex.IsMatch(record, @"[A-Z]{1}\d{2}[A-Z]{1}\d+\/\d+"))
                    {
                        priority.Classification = (priority.Classification ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(record, @"[A-Z]{1}\d{2}[A-Z]{1}\d+\/\d+").Value.Insert(4, " ") }).ToArray();
                    }
                    else
                    {
                        priority.Classification = (priority.Classification ?? Enumerable.Empty<string>()).Concat(new string[] { record.Length > 3 ? record.Insert(4, " ") : record }).ToArray();
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
            string tmpPrio = prioString.Replace("(30)", "").Trim();
            if (tmpPrio == "")
            {
                return null;
            }
            string[] lineSplittedPrio = null;
            string datePattern = @"\d{2}\/\d{2}\/\d{4}";
            string countryPattern = @"[A-Z]{2}\,";
            /*If more than one priority separated with ;*/
            if (tmpPrio.Contains(";"))
            {
                lineSplittedPrio = tmpPrio.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
            else
            {
                lineSplittedPrio = (lineSplittedPrio ?? Enumerable.Empty<string>()).Concat(new string[] { tmpPrio }).ToArray();
            }
            /*Trying to split each record to date/number/country*/
            if (lineSplittedPrio != null)
            {
                foreach (var rec in lineSplittedPrio)
                {
                    /*add found value to output*/
                    PriorityStruct record = new PriorityStruct
                    {
                        Date = DateNormalize(Regex.Match(rec, datePattern).Value.Trim()),
                        Number = rec.Remove(rec.IndexOf(Regex.Match(rec, countryPattern).Value.Trim())).Trim(),
                        Country = Regex.Match(rec, countryPattern).Value.Trim().Trim(',')
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
                string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
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
