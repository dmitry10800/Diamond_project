using System;
using Integration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_NG_Maksim
{

    class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = new();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                         .SkipWhile(val => !val.Value.Contains("1. APLICATION"))
                         .TakeWhile(val => !val.Value.StartsWith("DESIGNS"))
                         .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=Application|APLICATION\sNUMBER)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var note in notes)
                    {
                        // statusEvents.Add(CreateRecord(note, subCode, "AA"));
                    }
                }

                if (subCode == "4")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .ToList();

                    var text = MakeText(xElements, subCode);

                    var notes = Regex.Split(text, @"(?=Publica\s?on\s?Date)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Publica"))
                        .ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(CreateRecord(note, subCode, "BZ"));
                    }
                }
            }

            return statusEvents;
        }

        private string MakeText(List<XElement> xElements, string subCode)
        {
            var text = new StringBuilder();

            if(subCode == "1" || subCode == "4")
            {
                foreach (var xElement in xElements)
                {
                    text = text.AppendLine(xElement.Value + "\n");
                }
            }
            return text.ToString();
        }
        private string GetMonth(string month) => month switch
        {
            "January" => "01",
            "February" => "02",
            "March" => "03",
            "April" => "04",
            "May" => "05",
            "June" => "06",
            "July" => "07",
            "August" => "08",
            "September" => "09",
            "October" => "10",
            "November" => "11",
            "December" => "12",
            _ => null
        };

        private Diamond.Core.Models.LegalStatusEvent CreateRecord(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalEvent = new()
            {
                CountryCode = "NG",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new Biblio(),
                LegalEvent = new LegalEvent()
            };

            if (subCode == "4")
            {
                var match = Regex.Match(note,
                    @"on\sDate.+?,\s?(?<month>.+?)\s(?<day>\d+),?\s?(?<year>\d{4}).+Number(?<pubNum>.+?)\s.+Title.+?on(?<title>.+)Applicants(?<applicants>.+)Patent\sInventors(?<inventors>.+)Correspondence(?<agent>.+)\s\d+",
                    RegexOptions.Singleline);

                if (match.Success)
                {
                    var day = match.Groups["day"].Value.Trim().PadLeft(2, '0');
                    var month = GetMonth(match.Groups["month"].Value.Trim());
                    var year = match.Groups["year"].Value.Trim();
                    legalEvent.Biblio.Publication.Date = year + "/" + month + "/" + day;

                    legalEvent.Biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();

                    legalEvent.Biblio.Titles.Add(new Title()
                    {
                        Language = "EN",
                        Text = match.Groups["title"].Value.Replace("\r","").Replace("\n"," ").Trim()
                    });

                    var applicants = Regex.Split(match.Groups["applicants"].Value.Trim(),
                            @"(?<applicant>.*?,\s*(?:\+?\d[\d\s-]*),\s*\S+@\S+.*?(?=(?:\r?\n)?[^\r\n]+,\s*(?:\+?\d[\d\s-]*),\s*\S+@\S+|\Z))",
                                        RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val))
                        .ToList();  

                    foreach (var applicant in applicants)
                    {
                        var matchApplicant = Regex.Match(applicant.Replace("\r", "").Replace("\n", " ").Replace("  "," ").Trim(),
                            @"(?<name>.+?),\s?\+?\d[\d\s-]*,?.+\S+@\S+(?<adress>.+),(?<country>.+)");

                        if (matchApplicant.Success)
                        {
                            var country = GetCountry(matchApplicant.Groups["country"].Value.Trim());
                            if(country == null) 
                            {
                                Console.WriteLine(matchApplicant.Groups["country"].Value.Trim());
                            } 

                            legalEvent.Biblio.Applicants.Add(new PartyMember()
                            {
                                Name = matchApplicant.Groups["name"].Value.Trim(),
                                Address1 = matchApplicant.Groups["adress"].Value.Trim(),
                                Country = country
                            });
                        }
                        else
                        {
                            var matchApplicant2 = Regex.Match(applicant.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Trim(),
                                @"(?<name>.+?),\s?\+?\d[\d\s-]*,.+\.[a-z]+,\s(?<adress>.+),(?<country>.+)");

                            if (matchApplicant2.Success)
                            {
                                var country = GetCountry(matchApplicant2.Groups["country"].Value.Trim());
                                if (country == null)
                                {
                                    Console.WriteLine(matchApplicant2.Groups["country"].Value.Trim());
                                }

                                legalEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = matchApplicant2.Groups["name"].Value.Trim(),
                                    Address1 = matchApplicant2.Groups["adress"].Value.Trim(),
                                    Country = country
                                });
                            }
                            else
                            {
                                var matchApplicant3 = Regex.Match(applicant.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Trim(),
                                    @"(?<name>.+?),.+\S+@\S+(?<adress>.+),(?<country>.+)");

                                if (matchApplicant3.Success)
                                {
                                    var country = GetCountry(matchApplicant3.Groups["country"].Value.Trim());
                                    if (country == null)
                                    {
                                        Console.WriteLine(matchApplicant3.Groups["country"].Value.Trim());
                                    }

                                    legalEvent.Biblio.Applicants.Add(new PartyMember()
                                    {
                                        Name = matchApplicant3.Groups["name"].Value.Trim(),
                                        Address1 = matchApplicant3.Groups["adress"].Value.Trim(),
                                        Country = country
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"{applicant} ------------------ 71");
                                }
                                    
                            }
                        }
                    }

                    var inventors = Regex.Split(match.Groups["inventors"].Value.Trim(),
                            @"(?<inventor>.*?,\s*(?:\+?\d[\d\s-]*),\s*\S+@\S+.*?(?=(?:\r?\n)?[^\r\n]+,\s*(?:\+?\d[\d\s-]*),\s*\S+@\S+|\Z))",
                            RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val))
                        .ToList();

                    foreach (var inventor in inventors)
                    {
                        var matchInventor = Regex.Match(inventor.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Trim(),
                            @"(?<name>.+?),\s?\+?\d[\d\s-]*,.+?\S+@\S+(?<adress>.+),(?<country>.+)");

                        if (matchInventor.Success)
                        {
                            var country = GetCountry(matchInventor.Groups["country"].Value.Trim());
                            if (country == null)
                            {
                                Console.WriteLine(matchInventor.Groups["country"].Value.Trim());
                            }

                            legalEvent.Biblio.Inventors.Add(new PartyMember()
                            {
                                Name = matchInventor.Groups["name"].Value.Trim(),
                                Address1 = matchInventor.Groups["adress"].Value.Trim(),
                                Country = country
                            });
                        }
                        else
                        {
                            var matchApplicant2 = Regex.Match(inventor.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Trim(),
                                @"(?<name>.+?),\s?\+?\d[\d\s-]*,.+\.[a-z]+,\s(?<adress>.+),(?<country>.+)");

                            if (matchApplicant2.Success)
                            {
                                var country = GetCountry(matchApplicant2.Groups["country"].Value.Trim());
                                if (country == null)
                                {
                                    Console.WriteLine(matchApplicant2.Groups["country"].Value.Trim());
                                }

                                legalEvent.Biblio.Inventors.Add(new PartyMember()
                                {
                                    Name = matchApplicant2.Groups["name"].Value.Trim(),
                                    Address1 = matchApplicant2.Groups["adress"].Value.Trim(),
                                    Country = country
                                });
                            }
                            else
                            {
                                var matchApplicant3 = Regex.Match(inventor.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Trim(),
                                    @"(?<name>.+?),.+\S+@\S+(?<adress>.+),(?<country>.+)");

                                if (matchApplicant3.Success)
                                {
                                    var country = GetCountry(matchApplicant3.Groups["country"].Value.Trim());
                                    if (country == null)
                                    {
                                        Console.WriteLine(matchApplicant3.Groups["country"].Value.Trim());
                                    }

                                    legalEvent.Biblio.Inventors.Add(new PartyMember()
                                    {
                                        Name = matchApplicant3.Groups["name"].Value.Trim(),
                                        Address1 = matchApplicant3.Groups["adress"].Value.Trim(),
                                        Country = country
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"{inventor} ------------------ 72");
                                }

                            }
                        }
                    }

                    var agents = Regex.Split(match.Groups["agent"].Value.Trim(),
                            @"(?<agent>.*?,\s*(?:\+?\d[\d\s-]*),\s*\S+@\S+.*?(?=(?:\r?\n)?[^\r\n]+,\s*(?:\+?\d[\d\s-]*),\s*\S+@\S+|\Z))",
                            RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val))
                        .ToList();
                    foreach (var agent in agents)
                    {
                        var matchAgent = Regex.Match(agent.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Trim(),
                            @"(?<name>.+?),\s?\+?\d[\d\s-]*,.+?\S+@\S+(?<adress>.+)");

                        if (matchAgent.Success)
                        {
                            legalEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = matchAgent.Groups["name"].Value.Trim(),
                                Address1 = matchAgent.Groups["adress"].Value.Trim(),
                            });
                        }
                        else
                        {
                            Console.WriteLine($"{agent} ------------------ 74");
                        }
                    }
                }
                else
                {
                    Console.WriteLine(note);
                }
            }
            return legalEvent;
        }

        private string GetCountry(string country) => country switch
        {
            "Afghanistan" => "AF",
            "Albania" => "AL",
            "Algeria" => "DZ",
            "Andorra" => "AD",
            "Angola" => "AO",
            "Antigua and Barbuda" => "AG",
            "Argentina" => "AR",
            "Armenia" => "AM",
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
            "Bhutan" => "BT",
            "Bolivia" => "BO",
            "Bosnia and Herzegovina" => "BA",
            "Botswana" => "BW",
            "Brazil" => "BR",
            "Brunei" => "BN",
            "Bulgaria" => "BG",
            "Burkina Faso" => "BF",
            "Burundi" => "BI",
            "Cabo Verde" => "CV",
            "Cambodia" => "KH",
            "Cameroon" => "CM",
            "Canada" => "CA",
            "Central African Republic" => "CF",
            "Chad" => "TD",
            "Chile" => "CL",
            "China" => "CN",
            "Colombia" => "CO",
            "Comoros" => "KM",
            "Congo (Congo-Brazzaville)" => "CG",
            "Congo (Congo-Kinshasa)" => "CD",
            "Costa Rica" => "CR",
            "Croatia" => "HR",
            "Cuba" => "CU",
            "Cyprus" => "CY",
            "Czechia" => "CZ",
            "Denmark" => "DK",
            "Djibouti" => "DJ",
            "Dominica" => "DM",
            "Dominican Republic" => "DO",
            "Ecuador" => "EC",
            "Egypt" => "EG",
            "El Salvador" => "SV",
            "Equatorial Guinea" => "GQ",
            "Eritrea" => "ER",
            "Estonia" => "EE",
            "Eswatini" => "SZ",
            "Ethiopia" => "ET",
            "Fiji" => "FJ",
            "Finland" => "FI",
            "France" => "FR",
            "Gabon" => "GA",
            "Gambia" => "GM",
            "Georgia" => "GE",
            "Germany" => "DE",
            "Ghana" => "GH",
            "Greece" => "GR",
            "Grenada" => "GD",
            "Guatemala" => "GT",
            "Guinea" => "GN",
            "Guinea-Bissau" => "GW",
            "Guyana" => "GY",
            "Haiti" => "HT",
            "Honduras" => "HN",
            "Hungary" => "HU",
            "Iceland" => "IS",
            "India" => "IN",
            "Indonesia" => "ID",
            "Iran" => "IR",
            "Iraq" => "IQ",
            "Ireland" => "IE",
            "Israel" => "IL",
            "Italy" => "IT",
            "Jamaica" => "JM",
            "Japan" => "JP",
            "Jordan" => "JO",
            "Kazakhstan" => "KZ",
            "Kenya" => "KE",
            "Kiribati" => "KI",
            "Kuwait" => "KW",
            "Kyrgyzstan" => "KG",
            "Laos" => "LA",
            "Latvia" => "LV",
            "Lebanon" => "LB",
            "Lesotho" => "LS",
            "Liberia" => "LR",
            "Libya" => "LY",
            "Liechtenstein" => "LI",
            "Lithuania" => "LT",
            "Luxembourg" => "LU",
            "Madagascar" => "MG",
            "Malawi" => "MW",
            "Malaysia" => "MY",
            "Maldives" => "MV",
            "Mali" => "ML",
            "Malta" => "MT",
            "Marshall Islands" => "MH",
            "Mauritania" => "MR",
            "Mauritius" => "MU",
            "Mexico" => "MX",
            "Micronesia" => "FM",
            "Moldova" => "MD",
            "Monaco" => "MC",
            "Mongolia" => "MN",
            "Montenegro" => "ME",
            "Morocco" => "MA",
            "Mozambique" => "MZ",
            "Myanmar" => "MM",
            "Namibia" => "NA",
            "Nauru" => "NR",
            "Nepal" => "NP",
            "Netherlands" => "NL",
            "New Zealand" => "NZ",
            "Nicaragua" => "NI",
            "Niger" => "NE",
            "Nigeria" => "NG",
            "North Korea" => "KP",
            "North Macedonia" => "MK",
            "Norway" => "NO",
            "Oman" => "OM",
            "Pakistan" => "PK",
            "Palau" => "PW",
            "Panama" => "PA",
            "Papua New Guinea" => "PG",
            "Paraguay" => "PY",
            "Peru" => "PE",
            "Philippines" => "PH",
            "Poland" => "PL",
            "Portugal" => "PT",
            "Qatar" => "QA",
            "Romania" => "RO",
            "Russia" => "RU",
            "Rwanda" => "RW",
            "Saint Kitts and Nevis" => "KN",
            "Saint Lucia" => "LC",
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
            "Slovakia" => "SK",
            "Slovenia" => "SI",
            "Solomon Islands" => "SB",
            "Somalia" => "SO",
            "South Africa" => "ZA",
            "South Korea" => "KR",
            "South Sudan" => "SS",
            "Spain" => "ES",
            "Sri Lanka" => "LK",
            "Sudan" => "SD",
            "Suriname" => "SR",
            "Sweden" => "SE",
            "Switzerland" => "CH",
            "Syria" => "SY",
            "Taiwan" => "TW",
            "Tajikistan" => "TJ",
            "Tanzania" => "TZ",
            "Thailand" => "TH",
            "Timor-Leste" => "TL",
            "Togo" => "TG",
            "Tonga" => "TO",
            "Trinidad and Tobago" => "TT",
            "Tunisia" => "TN",
            "Turkey" => "TR",
            "Turkmenistan" => "TM",
            "Tuvalu" => "TV",
            "Uganda" => "UG",
            "Ukraine" => "UA",
            "United Arab Emirates" => "AE",
            "United Kingdom" => "GB",
            "United States" => "US",
            "Uruguay" => "UY",
            "Uzbekistan" => "UZ",
            "Vanuatu" => "VU",
            "Vatican City" => "VA",
            "Venezuela" => "VE",
            "Vietnam" => "VN",
            "Yemen" => "YE",
            "Zambia" => "ZM",
            "Zimbabwe" => "ZW",
            _ => null
        };
    }
}
