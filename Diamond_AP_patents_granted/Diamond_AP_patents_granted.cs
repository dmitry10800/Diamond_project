using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_AP_patents_granted
{
    class Diamond_AP_patents_granted
    {
        public static string I22 = "(22)";
        public static string I24 = "(24)";
        public static string I45 = "(22)";
        public static string I24D = "Date of Grant";
        public static string I24P = "Publication";
        public static string I51 = "(51)";
        public static string I51C = "International Classification";
        public static string I54 = "(54)";
        public static string I54T = "Title";
        public static string I57 = "(57)";
        public static string I57A = "Abstract";
        public static string I56 = "(56)";
        public static string I56D = "Documents Cited";
        public static string I72 = "(72)";
        public static string I73 = "(73)";
        public static string I74 = "(74)";
        public static string I84 = "(84)";
        public static string I84D = "Designated States";
        //public static string I22 = "(22)";
        //public static string I22 = "(22)";

        class ElementOut
        {
            public string I11 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I24I45 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
            public string[] I84 { get; set; }
            public string[] I51D { get; set; }
            public string[] I51V { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string[] I73 { get; set; }
            public string[] I72N { get; set; }
            public string[] I72C { get; set; }
            public string I74N { get; set; }
            public string I74C { get; set; }
            public string[] I56 { get; set; }
        }

        /*Date*/
        static string DateNormalize(string tmpDate)
        {
            string swapDate;
            string[] splitDate = null;
            if (tmpDate.Contains("/"))
            {
                splitDate = tmpDate.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (tmpDate.Contains("."))
            {
                splitDate = tmpDate.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            }
            return swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
        }
        static string CcIdentification(string ccStr)
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
                else { tmpStr = ccStr; }
            }
            return tmpStr;
        }
        static List<ElementOut> ElementsOut = new List<ElementOut>();
        static void Main(string[] args)
        {
            /*Folder with tetml files to process*/
            var dir = new DirectoryInfo(@"D:\_DFA_main\_Patents\AP\Reg\");
            /*list of tetml files*/
            var files = new List<string>();
            foreach (FileInfo file in dir.GetFiles("*.tetml", SearchOption.AllDirectories)) { files.Add(file.FullName); }
            string fileName; //имя файла tetml
            string folderPath; //путь к папке с файлом tetml
            string path; //путь к файлу tetml
            XElement tet;
            DirectoryInfo root;
            StreamWriter sf;
            ElementOut currentElement;
            foreach (var tetFile in files)
            {
                ElementsOut.Clear();
                fileName = tetFile;
                root = Directory.GetParent(fileName);
                folderPath = Path.Combine(root.FullName);
                path = Path.Combine(folderPath, fileName.Substring(0, fileName.IndexOf(".")) + ".txt"); //Output Filename
                tet = XElement.Load(fileName);
                Directory.CreateDirectory(folderPath);
                sf = new StreamWriter(path);
                //currentElement = null;

                /*TETML elements*/
                var elements = tet.Descendants().Where(d => d.Name.LocalName == "Text" && d.Value != "PATENTS" && d.Value != "Patent\nApplications\nFiled(Contd.)")
                    .SkipWhile(e => !e.Value.StartsWith("FORM 25"))
                    .ToList();

                for (int i = 0; i < elements.Count; ++i)
                {
                    var element = elements[i];
                    string value = element.Value;
                    int tmpCounter = i;
                    if (value.ToUpper().StartsWith("FORM 25"))
                    {
                        List<string> recordsList = new List<string>();
                        do
                        {
                            recordsList.Add(elements[tmpCounter].Value.Trim());
                            ++tmpCounter;
                        } while (tmpCounter < elements.Count()
                        && !elements[tmpCounter].Value.StartsWith("FORM 25")
                        && !elements[tmpCounter].Value.StartsWith("Page")
                        && !elements[tmpCounter].Value.StartsWith("AP; ARIPO Journal"));
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        for (int j = 0; j < recordsList.Count(); ++j)
                        {
                            if (recordsList != null)
                            {
                                /*Reg number*/
                                if (Regex.IsMatch(recordsList[j], @"AP\s*\d{4}"))
                                {
                                    currentElement.I11 = Regex.Match(recordsList[j], @"AP\s*\d{4}").Value;
                                }
                                /*App number*/
                                if (Regex.IsMatch(recordsList[j], @"AP\/P\/\d{4}\/\d{6}"))
                                {
                                    currentElement.I21 = Regex.Match(recordsList[j], @"AP\/P\/\d{4}\/\d{6}").Value;
                                }
                                /*22 date*/
                                if (recordsList[j].StartsWith(I22))
                                {
                                    string datePattern = @"\d{2}\.\d{2}\.\d{4}";
                                    if (Regex.IsMatch(recordsList[j], datePattern))
                                    {
                                        currentElement.I22 = DateNormalize(Regex.Match(recordsList[j], datePattern).Value);
                                    }
                                }
                                /*24/45 date*/
                                if (recordsList[j].StartsWith(I24) || recordsList[j].StartsWith(I24D) || recordsList[j].StartsWith(I24P) || recordsList[j].Contains(I45))
                                {
                                    string datePattern = @"\d{2}\.\d{2}\.\d{4}";
                                    int tmpRecCounter = j;
                                    string tmpRecValue = "";
                                    do
                                    {
                                        tmpRecValue += recordsList[tmpRecCounter] + " ";
                                        ++tmpRecCounter;
                                    } while (tmpRecCounter < recordsList.Count()
                                    && !Regex.IsMatch(recordsList[tmpRecCounter], @"^\(\d{2}\)"));
                                    if (Regex.IsMatch(recordsList[j], datePattern))
                                    {
                                        currentElement.I24I45 = DateNormalize(Regex.Match(recordsList[j], datePattern).Value);
                                    }
                                }
                                /*73 inid*/
                                if (recordsList[j].StartsWith(I73) || recordsList[j].Contains(I73))
                                {
                                    int tmpRecCounter = j;
                                    string tmpRecValue = "";
                                    do
                                    {
                                        tmpRecValue += recordsList[tmpRecCounter] + " ";
                                        ++tmpRecCounter;
                                    } while (tmpRecCounter < recordsList.Count()
                                    && !Regex.IsMatch(recordsList[tmpRecCounter], @"^\(\d{2}\)"));
                                    tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I73)).Replace(I73, "").Replace("Applicant(s)", "").Trim();
                                    currentElement.I73 = (currentElement.I73 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Replace("\n", " ").Trim() }).ToArray();
                                }
                                /*72 inid*/
                                if (recordsList[j].StartsWith(I72) || recordsList[j].Contains(I72))
                                {
                                    int tmpRecCounter = j;
                                    string tmpRecValue = "";
                                    string[] tmpSplittedRecValue = null;
                                    do
                                    {
                                        tmpRecValue += recordsList[tmpRecCounter] + "\n";
                                        ++tmpRecCounter;
                                    } while (tmpRecCounter < recordsList.Count()
                                    && !Regex.IsMatch(recordsList[tmpRecCounter], @"^\(\d{2}\)"));
                                    tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I72)).Replace(I72, "").Replace("Inventors", "").Trim();
                                    if (tmpRecValue.Contains("\n"))
                                    {
                                        tmpSplittedRecValue = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    }
                                    else
                                    {
                                        tmpSplittedRecValue = (tmpSplittedRecValue ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Trim() }).ToArray();
                                    }
                                    if (tmpSplittedRecValue != null)
                                    {
                                        foreach (var inventor in tmpSplittedRecValue)
                                        {
                                            if (inventor.Contains(","))
                                            {
                                                currentElement.I72N = (currentElement.I72N ?? Enumerable.Empty<string>()).Concat(new string[] { inventor.Remove(inventor.IndexOf(",")).Trim() }).ToArray();
                                                currentElement.I72C = (currentElement.I72C ?? Enumerable.Empty<string>()).Concat(new string[] { CcIdentification(inventor.Trim()) }).ToArray();
                                            }
                                            if (inventor.Contains(".") && !inventor.Contains(","))
                                            {
                                                currentElement.I72N = (currentElement.I72N ?? Enumerable.Empty<string>()).Concat(new string[] { inventor.Remove(inventor.IndexOf(".")).Trim() }).ToArray();
                                                currentElement.I72C = (currentElement.I72C ?? Enumerable.Empty<string>()).Concat(new string[] { CcIdentification(inventor.Trim()) }).ToArray();
                                            }
                                        }
                                    }

                                }
                                /*73 inid*/
                                if (recordsList[j].StartsWith(I74) || recordsList[j].Contains(I74))
                                {
                                    int tmpRecCounter = j;
                                    string tmpRecValue = "";
                                    do
                                    {
                                        tmpRecValue += recordsList[tmpRecCounter] + " ";
                                        ++tmpRecCounter;
                                    } while (tmpRecCounter < recordsList.Count()
                                    && !Regex.IsMatch(recordsList[tmpRecCounter], @"^\(\d{2}\)"));
                                    tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I74)).Replace(I74, "").Replace("\n", " ").Replace("Representative", "").Trim();
                                    if (tmpRecValue.Contains(","))
                                    {
                                        currentElement.I74N = tmpRecValue.Remove(tmpRecValue.LastIndexOf(",")).Trim();
                                        currentElement.I74C = CcIdentification(tmpRecValue);
                                    }
                                    else
                                    {
                                        currentElement.I74N = tmpRecValue.Trim();
                                        currentElement.I74C = CcIdentification(tmpRecValue);
                                    }
                                }
                                /*51 International classification*/
                                if (recordsList[j].StartsWith(I51) || recordsList[j].Contains(I51) || recordsList[j].Contains(I51C))
                                {
                                    int tmpRecCounter = j;
                                    string tmpRecValue = "";
                                    string[] splittedIPC = null;
                                    string[] splittedDateIPC = null;
                                    do
                                    {
                                        tmpRecValue += recordsList[tmpRecCounter] + " ";
                                        ++tmpRecCounter;
                                    } while (tmpRecCounter < recordsList.Count()
                                    && !Regex.IsMatch(recordsList[tmpRecCounter], @"^\(\d{2}\)"));
                                    if (recordsList[j].StartsWith(I51) && !recordsList[j].Contains(I51C))
                                    {
                                        tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I51)).Replace(I51, "").Replace(I51C, "").Replace(":", "").Replace("\n", "").Trim();
                                    }
                                    if (recordsList[j].Contains(I51C))
                                    {
                                        tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I51C)).Replace(I51, "").Replace(I51C, "").Replace(":", "").Replace("\n", "").Trim();
                                    }
                                    if (Regex.IsMatch(tmpRecValue, @"\(\d{4}\.\d{2}\)"))
                                    {
                                        splittedIPC = Regex.Split(tmpRecValue, @"\(\d{4}\.\d{2}\)").Where(x => x != "" && x != string.Empty).ToArray();
                                        splittedDateIPC = Regex.Split(tmpRecValue, @"(\(\d{4}\.\d{2}\))").Where(x => x != "" && x != string.Empty && Regex.IsMatch(x, @"\(\d{4}\.\d{2}\)")).ToArray();
                                    }
                                    if (splittedIPC != null)
                                    {
                                        currentElement.I51V = splittedIPC;
                                        currentElement.I51D = splittedDateIPC;
                                    }
                                }
                                /*54 Title*/
                                if (recordsList[j].StartsWith(I54) || recordsList[j].Contains(I54T))
                                {
                                    int tmpRecCounter = j;
                                    string tmpRecValue = "";
                                    do
                                    {
                                        tmpRecValue += recordsList[tmpRecCounter] + " ";
                                        ++tmpRecCounter;
                                    } while (tmpRecCounter < recordsList.Count()
                                    && !Regex.IsMatch(recordsList[tmpRecCounter], @"^\(\d{2}\)"));
                                    if (recordsList[j].StartsWith(I54) && !recordsList[j].Contains(I54T))
                                    {
                                        tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I54)).Replace(I54, "").Replace("\n", " ").Replace(I54T, "").Trim();
                                    }
                                    if (recordsList[j].Contains(I54T))
                                    {
                                        tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I54T)).Replace(I54, "").Replace("\n", " ").Replace(I54T, "").Trim();
                                    }
                                    currentElement.I54 = tmpRecValue;
                                }
                                /*57 Title*/
                                if (recordsList[j].StartsWith(I57) || recordsList[j].Contains(I57A))
                                {
                                    int tmpRecCounter = j;
                                    string tmpRecValue = "";
                                    do
                                    {
                                        tmpRecValue += recordsList[tmpRecCounter] + " ";
                                        ++tmpRecCounter;
                                    } while (tmpRecCounter < recordsList.Count()
                                    && !Regex.IsMatch(recordsList[tmpRecCounter], @"^\(\d{2}\)"));
                                    if (recordsList[j].StartsWith(I57) && !recordsList[j].Contains(I57A))
                                    {
                                        tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I57)).Replace(I57, "").Replace("\n", " ").Replace(I57A, "").Trim();
                                    }
                                    if (recordsList[j].Contains(I57A))
                                    {
                                        tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I57A)).Replace(I57, "").Replace("\n", " ").Replace(I57A, "").Trim();
                                    }
                                    currentElement.I57 = tmpRecValue;
                                }
                                /*56 Document Cited*/
                                if (recordsList[j].StartsWith(I56) || recordsList[j].Contains(I56D))
                                {
                                    int tmpRecCounter = j;
                                    string tmpRecValue = "";
                                    do
                                    {
                                        tmpRecValue += recordsList[tmpRecCounter] + "\n";
                                        ++tmpRecCounter;
                                    } while (tmpRecCounter < recordsList.Count()
                                    && !Regex.IsMatch(recordsList[tmpRecCounter], @"^\(\d{2}\)"));
                                    if (recordsList[j].StartsWith(I56) && !recordsList[j].Contains(I56D))
                                    {
                                        tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I56)).Replace(I56, "").Replace(I56D, "").Trim();
                                    }
                                    if (recordsList[j].Contains(I56D))
                                    {
                                        tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I56D)).Replace(I56, "").Replace(I56D, "").Trim();
                                    }
                                    if (tmpRecValue.Contains("\n"))
                                    {
                                        currentElement.I56 = tmpRecValue.Replace(":", "").Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    }
                                    else
                                    {
                                        currentElement.I56 = (currentElement.I56 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Trim() }).ToArray();
                                    }
                                }
                                /*84 Document Cited*/
                                if (recordsList[j].StartsWith(I84) || recordsList[j].Contains(I84D))
                                {
                                    int tmpRecCounter = j;
                                    string tmpRecValue = "";
                                    do
                                    {
                                        tmpRecValue += recordsList[tmpRecCounter] + "\n";
                                        ++tmpRecCounter;
                                    } while (tmpRecCounter < recordsList.Count()
                                    && !Regex.IsMatch(recordsList[tmpRecCounter], @"^\(\d{2}\)"));
                                    if (recordsList[j].StartsWith(I84) && !recordsList[j].Contains(I84D))
                                    {
                                        tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I84)).Replace(I84, "").Replace(I84D, "").Trim();
                                    }
                                    if (recordsList[j].Contains(I84D))
                                    {
                                        tmpRecValue = tmpRecValue.Substring(tmpRecValue.IndexOf(I84D)).Replace(I84, "").Replace(I84D, "").Trim();
                                    }
                                    if (tmpRecValue.Contains("\n"))
                                    {
                                        currentElement.I84 = tmpRecValue.Replace(":", "").Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    }
                                    else if (!tmpRecValue.Contains("\n") && tmpRecValue.Length > 2)
                                    {
                                        currentElement.I84 = (currentElement.I84 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Replace(":", "").Replace(";", "").Trim() }).ToArray();
                                    }
                                }
                            }
                        }
                    }
                }
                /*Output*/
                if (ElementsOut != null)
                {
                    foreach (var elemOut in ElementsOut)
                    {
                        if (elemOut.I21 != null)
                        {
                            sf.WriteLine("****");
                            sf.WriteLine("11:\t" + elemOut.I11);
                            sf.WriteLine("21:\t" + elemOut.I21);
                            sf.WriteLine("22:\t" + elemOut.I22);
                            sf.WriteLine("24:\t" + elemOut.I24I45);
                            if (elemOut.I51V != null && elemOut.I51D != null && elemOut.I51V.Count() == elemOut.I51D.Count())
                            {
                                for (int i = 0; i < elemOut.I51V.Count(); i++)
                                {
                                    sf.WriteLine("51:\t" + elemOut.I51V[i].Trim() + "\t" + elemOut.I51D[i]);
                                }
                            }
                            sf.WriteLine("54:\t" + elemOut.I54);
                            if (elemOut.I56 != null)
                            {
                                foreach (var item in elemOut.I56)
                                {
                                    sf.WriteLine("56:\t" + item);
                                }
                            }
                            sf.WriteLine("57:\t" + elemOut.I57);
                            if (elemOut.I72N != null && elemOut.I72C != null && elemOut.I72C.Count() == elemOut.I72N.Count())
                            {
                                for (int i = 0; i < elemOut.I72N.Count(); i++)
                                {
                                    sf.WriteLine("72N:\t" + elemOut.I72N[i]);
                                    sf.WriteLine("72C:\t" + elemOut.I72C[i]);
                                }
                            }
                            if (elemOut.I73 != null)
                            {
                                foreach (var item in elemOut.I73)
                                {
                                    sf.WriteLine("73:\t" + item);
                                }
                            }
                            if (elemOut.I74N != null && elemOut.I74C != null && elemOut.I74C.Count() == elemOut.I74N.Count())
                            {
                                for (int i = 0; i < elemOut.I74N.Count(); i++)
                                {
                                    sf.WriteLine("74N:\t" + elemOut.I74N[i]);
                                    sf.WriteLine("74C:\t" + elemOut.I74C[i]);
                                }
                            }
                            if (elemOut.I84 != null)
                            {
                                foreach (var item in elemOut.I84)
                                {
                                    sf.WriteLine("84:\t" + item);
                                }
                            }
                            ///*31,32,33 Priority*/
                            //if (elemOut.I31 != null && elemOut.I31.Count() == elemOut.I32.Count() && elemOut.I31.Count() == elemOut.I33.Count())
                            //{
                            //    for (int i = 0; i < elemOut.I31.Count(); i++)
                            //    {
                            //        sf.WriteLine("31:\t" + elemOut.I31[i]);
                            //        sf.WriteLine("32:\t" + elemOut.I32[i]);
                            //        sf.WriteLine("33:\t" + elemOut.I33[i]);
                            //    }
                            //}
                        }
                    }
                }
                sf.Flush();
                sf.Close();
            }
        }
    }
}
