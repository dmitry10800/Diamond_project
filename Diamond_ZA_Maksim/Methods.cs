using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_ZA_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (FileInfo file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = new();

            foreach (string tetml in files)
            {
                CurrentFileName = tetml;

                tet = XElement.Load(tetml);

                if(subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("THE PARTICULARS APPEAR IN THE FOLLOWING SEQUENCE:"))
                            .TakeWhile(val => !val.Value.StartsWith("ASSIGNMENTS IN TERMS OF SECTION 60-REGULATIONS 58-60 AND 64 (1)"))
                            .ToList();

                    List<string> firstNotes = Regex.Split(MakeText(xElements, subCode), @"(?=\.\s?-\s?APPLIED ON\s)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith(". -")).ToList();

                    List<string> notes = new();

                    foreach (string note in firstNotes)
                    {
                        Match match = Regex.Match(note.Trim(), @"(?<date>.+)\s?-\s?(?<all>\d{4}\/.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            string date = match.Groups["date"].Value.Trim();

                            List<string> secondNotes = Regex.Split(match.Groups["all"].Value.Trim(), @"(?=[0-9]{4}\/[0-9]{5,6}\s?~\s?[A-Z])").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string item in secondNotes)
                            {
                                notes.Add(item.Trim() + " " + date);
                            }
                        }
                        else Console.WriteLine($"{note}");
                    }

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AZ"));
                    }

                }
                else
                if(subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                           .SkipWhile(val => !val.Value.StartsWith("COMPLETE SPECIFICATIONS ACCEPTED A ABRIDGEMENTS OR ABSTRACTS THEREOF"))
                           .TakeWhile(val => !val.Value.StartsWith("No records available"))
                           .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=21:\s)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("21: ")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FZ"));
                    }
                }
            }
                return statusEvents;
        }



        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "ZA",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            CultureInfo culture = new("ru-RU");

            if(subCode == "1")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<aNum>.+?)\s.+(?<title>54.+)(?<applicants>71:.+)(?<inventors>72:.+?)(?<priority>33:.+)(?<date22>\.\s?-\s?APP.+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "EN",
                        Text = match.Groups["title"].Value.Replace("54:", "").Replace("~", "").Trim()
                    });

                    List<string> applicants = Regex.Split(match.Groups["applicants"].Value.Replace("~", "").Replace("71:", ""), @";\s?[A-Z]").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string applicant in applicants)
                    {
                        Match appli = Regex.Match(applicant.Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<country>.+)");

                        if (appli.Success)
                        {
                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = appli.Groups["name"].Value.Trim(),
                                Address1 = appli.Groups["adress"].Value.Trim(),
                                Country = MakeCountryCode(appli.Groups["country"].Value.Trim())
                            });

                            if (MakeCountryCode(appli.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{appli.Groups["country"].Value.Trim()}");
                        }
                        else 
                        {
                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = applicant.Trim()
                            });
                        }
                    }

                    List<string> inventors = Regex.Split(match.Groups["inventors"].Value.Replace("~", "").Replace("72:", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string inventor in inventors)
                    {
                        Match inve = Regex.Match(inventor.Trim(), @"(?<name>.+?)\s?\((?<country>.+)\)");

                        if (inve.Success)
                        {
                            statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                            {
                                Name = inve.Groups["name"].Value.Trim(),
                                Country = MakeCountryCode(inve.Groups["country"].Value.Trim())
                            });

                            if (MakeCountryCode(inve.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{inve.Groups["country"].Value.Trim()}");
                        }
                        else
                        {
                            statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                            {
                                Name = inventor.Trim()
                            });                                
                        }
                    }

                    List<string> priorities = Regex.Split(match.Groups["priority"].Value.Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string priority in priorities)
                    {
                        Match prior = Regex.Match(priority.Trim(), @"33:(?<code>\D{2})\s?~?31:(?<num>.+)\s?~32:(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (prior.Success)
                        {
                            statusEvent.Biblio.Priorities.Add(new Integration.Priority
                            {
                                Number = prior.Groups["num"].Value.Trim(),
                                Country = prior.Groups["code"].Value.Trim(),
                                Date = DateTime.Parse(prior.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                            });
                        }
                        else Console.WriteLine($"{priority}   ------------- 30");
                    }

                    Match date22 = Regex.Match(match.Groups["date22"].Value.Trim(), @".+(?<date>\d{4}\/\d{2}\/\d{2})");
                    if (date22.Success)
                    {
                        statusEvent.Biblio.Application.Date = date22.Value.Trim();
                    }
                    else Console.WriteLine($"{match.Groups["date22"].Value.Trim()} ----------- date22");
                }
                else
                {
                    Match match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<aNum>.+?)\s.+(?<title>54.+)(?<applicants>71.+)(?<inventors>72.+)(?<date22>\.\s?-\s?APP.+)");

                    if (match1.Success)
                    {
                        statusEvent.Biblio.Application.Number = match1.Groups["aNum"].Value.Trim();

                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "EN",
                            Text = match1.Groups["title"].Value.Replace("54:", "").Replace("~", "").Trim()
                        });

                        List<string> applicants = Regex.Split(match1.Groups["applicants"].Value.Replace("~", "").Replace("71:", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match appli = Regex.Match(applicant.Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<country>.+)");

                            if (appli.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = appli.Groups["name"].Value.Trim(),
                                    Address1 = appli.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(appli.Groups["country"].Value.Trim())
                                });

                                if (MakeCountryCode(appli.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{appli.Groups["country"].Value.Trim()}");
                            }
                            else
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicant.Trim()
                                });
                            }
                        }

                        List<string> inventors = Regex.Split(match1.Groups["inventors"].Value.Replace("~", "").Replace("72:", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match inve = Regex.Match(inventor.Trim(), @"(?<name>.+?)\s?\((?<country>.+)\)");

                            if (inve.Success)
                            {
                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = inve.Groups["name"].Value.Trim(),
                                    Country = MakeCountryCode(inve.Groups["country"].Value.Trim())
                                });

                                if (MakeCountryCode(inve.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{inve.Groups["country"].Value.Trim()}");
                            }
                            else
                            {
                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = inventor.Trim()
                                });
                            }
                        }

                        Match date22 = Regex.Match(match1.Groups["date22"].Value.Trim(), @".+(?<date>\d{4}\/\d{2}\/\d{2})");
                        if (date22.Success)
                        {
                            statusEvent.Biblio.Application.Date = date22.Value.Trim();
                        }
                    }
                    else Console.WriteLine($"{note}");
                }
                  
            }
            else
            if(subCode == "3")
            {
                List<string> inids = Regex.Split(note.Trim(), @"(?=\d{2}:\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                List<string> inid31 = new();
                List<string> inid32 = new();
                List<string> inid33 = new();

                foreach (string inid in inids)
                {
                    if (inid.StartsWith("21:"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("21:", "").Trim().TrimEnd('.');
                    }
                    else
                    if (inid.StartsWith("22:"))
                    {
                        statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("22:", "").TrimEnd('.').Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith("43:"))
                    {
                        statusEvent.Biblio.Publication.Date = DateTime.Parse(inid.Replace("43:", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith("51:"))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace("51:", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                            {
                                Class = ipc.Trim()
                            });
                        }
                    }
                    else
                    if (inid.StartsWith("71:"))
                    {
                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                        {
                            Name = inid.Replace("71:", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith("72:"))
                    {
                        List<string> inventors = Regex.Split(inid.Replace("72:", "").Trim(), @"(?<=[a-z],\s)|(?<=\.,\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                            {
                                Name = inventor.Trim().TrimEnd(',')
                            });
                        }
                    }
                    else
                    if (inid.StartsWith("33:"))
                    {
                        inid33.Add(inid.Replace("33:", "").Trim());
                    }
                    else
                    if (inid.StartsWith("31:"))
                    {
                        inid31.Add(inid.Replace("31:", "").Trim());
                    }
                    else
                    if (inid.StartsWith("32:"))
                    {
                        inid32.Add(inid.Replace("32:", "").Replace("-", "/").Trim());
                    }
                    else
                    if (inid.StartsWith("54:"))
                    {
                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "EN",
                            Text = inid.Replace("54:", "").Replace("\r", "").Replace("\n", " ").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith("00:"))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                        {
                            Language = "EN",
                            Text = inid.Replace("00:", "").Replace("-\n","").Replace("\r","").Replace("\n"," ").Trim()
                        });
                    }
                    else Console.WriteLine($"{inid} - not process");
                }

                for (int i = 0; i < inid31.Count; i++)
                {
                    statusEvent.Biblio.Priorities.Add(new Integration.Priority
                    {
                        Date = inid32[i],
                        Country = inid33[i],
                        Number = inid31[i]
                    });
                }
            }
            return statusEvent;
        }

        internal string MakeCountryCode(string code) => code switch
        {
            "Afghanistan" => "AF",
            "Albania" => "AL",
            "Algeria" => "DZ",
            "American Samoa" => "AS",
            "Andorra" => "AD",
            "Angola" => "AO",
            "Anguilla" => "AI",
            "Antarctica" => "AQ",
            "Antigua and Barbuda" => "AG",
            "Argentina" => "AR",
            "Armenia" => "AM",
            "Aruba" => "AW",
            "Australia" => "AU",
            "Austria" => "AT",
            "Azerbaijan" => "AZ",
            "Bahamas" => "BS",
            "Bahrain" => "BH",
            "Bangladesh" => "BD",
            "Barbados" => "BB",
            "Belarus" => "BY",
            "Belgium" => "BE",
            "Belize" => "BZ",
            "Benin" => "BJ",
            "Bermuda" => "BM",
            "Bhutan" => "BT",
            "Bolivia" => "BO",
            "Bonaire Sint Eustatius and Saba" => "BQ",
            "Bosnia and Herzegovina" => "BA",
            "Botswana" => "BW",
            "Bouvet Island" => "BV",
            "Brazil" => "BR",
            "British Indian Ocean Territory" => "IO",
            "Brunei Darussalam" => "BN",
            "Bulgaria" => "BG",
            "Burkina Faso" => "BF",
            "Burundi" => "BI",
            "Cabo Verde" => "CV",
            "Cambodia" => "KH",
            "Cameroon" => "CM",
            "Canada" => "CA",
            "Cayman Islands" => "KY",
            "Central African Republic" => "CF",
            "Chad" => "TD",
            "Chile" => "CL",
            "People's Republic of China" => "CN",
            "China" => "CN",
            "Christmas Island" => "CX",
            "Cocos Islands" => "CC",
            "Colombia" => "CO",
            "Comoros" => "KM",
            "Congo (the Democratic Republic of the)" => "CD",
            "Congo" => "CG",
            "Cook Islands" => "CK",
            "Costa Rica" => "CR",
            "Croatia" => "HR",
            "Cuba" => "CU",
            "Curaçao" => "CW",
            "Cyprus" => "CY",
            "Czechia" => "CZ",
            "Côte d'Ivoire" => "CI",
            "Denmark" => "DK",
            "Djibouti" => "DJ",
            "Dominica" => "DM",
            "Dominican Republic " => "DO",
            "Ecuador" => "EC",
            "Egypt" => "EG",
            "El Salvador" => "SV",
            "Equatorial Guinea" => "GQ",
            "Eritrea" => "ER",
            "Estonia" => "EE",
            "Eswatini" => "SZ",
            "Ethiopia" => "ET",
            "Falkland Islands" => "FK",
            "Faroe Islands" => "FO",
            "Fiji" => "FJ",
            "Finland" => "FI",
            "France" => "FR",
            "French Guiana" => "GF",
            "French Polynesia" => "PF",
            "French Southern Territories " => "TF",
            "Gabon" => "GA",
            "Gambia (the)" => "GM",
            "Georgia" => "GE",
            "Germany" => "DE",
            "Ghana" => "GH",
            "Gibraltar" => "GI",
            "Greece" => "GR",
            "Greenland" => "GL",
            "Grenada" => "GD",
            "Guadeloupe" => "GP",
            "Guam" => "GU",
            "Guatemala" => "GT",
            "Guernsey" => "GG",
            "Guinea" => "GN",
            "Guinea-Bissau" => "GW",
            "Guyana" => "GY",
            "Haiti" => "HT",
            "Heard Island and McDonald Islands" => "HM",
            "Holy See" => "VA",
            "Honduras" => "HN",
            "Hong Kong" => "HK",
            "Hungary" => "HU",
            "Iceland" => "IS",
            "India" => "IN",
            "Indonesia" => "ID",
            "Iran" => "IR",
            "Iraq" => "IQ",
            "Ireland" => "IE",
            "Isle of Man" => "IM",
            "Israel" => "IL",
            "Italy" => "IT",
            "Jamaica" => "JM",
            "Japan" => "JP",
            "Jersey" => "JE",
            "Jordan" => "JO",
            "Kazakhstan" => "KZ",
            "Kenya" => "KE",
            "Kiribati" => "KI",
            "Korea (the Democratic People's Republic of)" => "KP",
            "Republic of Korea" => "KR",
            "Kuwait" => "KW",
            "Kyrgyzstan" => "KG",
            "Lao People's Democratic Republic" => "LA",
            "Latvia" => "LV",
            "Lebanon" => "LB",
            "Lesotho" => "LS",
            "Liberia" => "LR",
            "Libya" => "LY",
            "Liechtenstein" => "LI",
            "Lithuania" => "LT",
            "Luxembourg" => "LU",
            "Macao" => "MO",
            "Madagascar" => "MG",
            "Malawi" => "MW",
            "Malaysia" => "MY",
            "Maldives" => "MV",
            "Mali" => "ML",
            "Malta" => "MT",
            "Marshall Islands" => "MH",
            "Martinique" => "MQ",
            "Mauritania" => "MR",
            "Mauritius" => "MU",
            "Mayotte" => "YT",
            "Mexico" => "MX",
            "Micronesia " => "FM",
            "Moldova " => "MD",
            "Monaco" => "MC",
            "Mongolia" => "MN",
            "Montenegro" => "ME",
            "Montserrat" => "MS",
            "Morocco" => "MA",
            "Mozambique" => "MZ",
            "Myanmar" => "MM",
            "Namibia" => "NA",
            "Nauru" => "NR",
            "Nepal" => "NP",
            "Netherlands" => "NL",
            "New Caledonia" => "NC",
            "New Zealand" => "NZ",
            "Nicaragua" => "NI",
            "Niger" => "NE",
            "Nigeria" => "NG",
            "Niue" => "NU",
            "Norfolk Island" => "NF",
            "Northern Mariana Islands " => "MP",
            "Norway" => "NO",
            "Oman" => "OM",
            "Pakistan" => "PK",
            "Palau" => "PW",
            "Palestine" => "PS",
            "Panama" => "PA",
            "Papua New Guinea" => "PG",
            "Paraguay" => "PY",
            "Peru" => "PE",
            "Philippines" => "PH",
            "Pitcairn" => "PN",
            "Poland" => "PL",
            "Portugal" => "PT",
            "Puerto Rico" => "PR",
            "Qatar" => "QA",
            "Republic of North Macedonia" => "MK",
            "Romania" => "RO",
            "Russian Federation" => "RU",
            "Rwanda" => "RW",
            "Réunion" => "RE",
            "Saint Barthélemy" => "BL",
            "Saint Helena Ascension and Tristan da Cunha" => "SH",
            "Saint Kitts and Nevis" => "KN",
            "Saint Lucia" => "LC",
            "Saint Martin" => "MF",
            "Saint Pierre and Miquelon" => "PM",
            "Saint Vincent and the Grenadines" => "VC",
            "Samoa" => "WS",
            "San Marino" => "SM",
            "Sao Tome and Principe" => "ST",
            "Saudi Arabia" => "SA",
            "Senegal" => "SN",
            "Serbia" => "RS",
            "Seychelles" => "SC",
            "Sierra Leone" => "SL",
            "Singapore" => "SG",
            "Sint Maarten" => "SX",
            "Slovakia" => "SK",
            "Slovenia" => "SI",
            "Solomon Islands" => "SB",
            "Somalia" => "SO",
            "South Africa" => "ZA",
            "South Georgia and the South Sandwich Islands" => "GS",
            "South Sudan" => "SS",
            "Spain" => "ES",
            "Sri Lanka" => "LK",
            "Sudan" => "SD",
            "Suriname" => "SR",
            "Svalbard and Jan Mayen" => "SJ",
            "Sweden" => "SE",
            "Switzerland" => "CH",
            "Swaziland" => "SZ",
            "Syrian Arab Republic" => "SY",
            "Taiwan" => "TW",
            "Tajikistan" => "TJ",
            "Tanzania United Republic of" => "TZ",
            "Thailand" => "TH",
            "Timor-Leste" => "TL",
            "Togo" => "TG",
            "Tokelau" => "TK",
            "Tonga" => "TO",
            "Trinidad and Tobago" => "TT",
            "Tunisia" => "TN",
            "Turkey" => "TR",
            "Turkmenistan" => "TM",
            "Turks and Caicos Islands " => "TC",
            "Tuvalu" => "TV",
            "Uganda" => "UG",
            "Ukraine" => "UA",
            "United Arab Emirates" => "AE",
            "United Kingdom of Great Britain and Northern Ireland (the)" => "GB",
            "United Kingdom" => "GB",
            "United States Minor Outlying Islands" => "UM",
            "United States of America" => "US",
            "Uruguay" => "UY",
            "Uzbekistan" => "UZ",
            "Vanuatu" => "VU",
            "Venezuela" => "VE",
            "Viet Nam" => "VN",
            "Virgin Islands" => "VG",
            "Virgin Islands (U.S.)" => "VI",
            "Wallis and Futuna" => "WF",
            "Western Sahara" => "EH",
            "Yemen" => "YE",
            "Zambia" => "ZM",
            "Zimbabwe" => "ZW",
            "Åland Islands" => "AX",
            _ => null
        };
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

            if(subCode == "1")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + " ";
                }
            }
            else
            if(subCode == "3")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + " ";
                }
            }

            return text;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                if (SendToProduction == true)
                {
                    url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";  // продакшен
                }
                else
                {
                    url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";     // стейдж
                }
                HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = httpClient.PostAsync("", content).Result;
                string answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
