using Integration;
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
using System.Xml.Linq;

namespace Diamond_IN_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            var statusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directory = new DirectoryInfo(path);

            List<string> files = new();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            var xElements = new List<XElement>();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            //.SkipWhile(val => !val.Value.StartsWith("Early Publication:"))
                            .TakeWhile(val => !val.Value.StartsWith("Publication After 18 Months:"))
                            .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(12\)\sPA)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(12) PA")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BZ"));
                    }
                }
                else if(subCode == "10")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("WEEKLY ISSUED FER (DELHI)"))
                            .TakeWhile(val => !val.Value.StartsWith("PUBLICATION UNDER SECTION 57 AND UNDER RULE 81(3)"))
                            .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?>(\d{1,3}\sKOLKATA)|(\d{1,3}\sDELHI)|(\d{1,3}\sMUMBAI)|(\d{1,3}\sCHENNAI))")
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d").Match(val).Success).ToList();

                    List<string> works = new();
                    for (var i = 0; i < notes.Count; i+=2)
                    {
                        works.Add(notes[i] + " " + notes[i+1]);
                    }

                    foreach (var note in works)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "EC"));
                    }

                }
            }

            return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "IN",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            var culture = new CultureInfo("ru-RU");
            if(subCode == "1")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(12)"))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(12)", "").Trim();
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"(?<num>\d+)\s(?<kind>\D)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Number = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid}  --- 21");
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 22");
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 43");
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"invention :\s?(?<title>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "EN",
                                Text = match.Groups["title"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 54");
                    }
                    else if (inid.StartsWith("(71)"))
                    {
                       var applicants = Regex.Split(inid.Replace("(71)Name of Applicant :","").Trim(),
                            @"(?>\d+\))", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var match = Regex.Match(applicant.Trim(), @"(?<name>.+)\sAddr.+cant\s?:(?<adress>.+),(?<code>.+?)[\s--]", RegexOptions.Singleline);

                            if (match.Success)
                            {
                                var countryCode = MakeCountryCode(match.Groups["code"].Value.Trim());

                                if (countryCode == null)
                                {
                                    Console.WriteLine($"<=================== {match.Groups["code"].Value}");
                                }
                                else
                                {
                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                    {
                                        Name = match.Groups["name"].Value.Trim(),
                                        Address1 = match.Groups["adress"].Value.Trim(),
                                        Country = countryCode
                                    });
                                }
                            }
                            else
                            {
                                var match1 = Regex.Match(applicant.Trim(), @"(?<name>.+)\sName\sof\sApplicant.+", RegexOptions.Singleline);

                                if (match1.Success)
                                {
                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                    {
                                        Name = match1.Groups["name"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                    {
                                        Name = applicant.Trim()
                                    });
                                }
                            }
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        var inventors = Regex.Split(inid.Replace("(72)Name of Inventor :", "").Trim(),
                            @"(?>\d+\))", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            var match = Regex.Match(inventor.Trim(), @"(?<name>.+)\sAddr.+cant\s?:(?<adress>.+),(?<code>.+?)[\s--]", RegexOptions.Singleline);

                            if (match.Success)
                            {
                                var countryCode = MakeCountryCode(match.Groups["code"].Value.Trim());

                                if (countryCode == null)
                                {
                                    Console.WriteLine($"<=================== {match.Groups["code"].Value}");
                                }

                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = countryCode
                                });
                            }
                            else
                            {
                                var match1 = Regex.Match(inventor.Trim(), @"(?<name>.+)\sName\sof\sApplicant.+", RegexOptions.Singleline);

                                if (match1.Success)
                                {
                                    statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = match1.Groups["name"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = inventor.Trim()
                                    });
                                }
                            }
                        }
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"act\s?:(?<text>.+?)\s(?<note>No\..+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "EN",
                                Text = match.Groups["text"].Value.Trim()
                            });

                            var match1 = Regex.Match(match.Groups["note"].Value.Trim(), @"(?<a1>No\..+?)\s?:\s?(?<p>\d+)\s(?<a2>No\..+)\s?:\s?(?<c>\d+)");

                            if (match1.Success)
                            {
                                statusEvent.LegalEvent.Note = "|| " + match1.Groups["a1"].Value.Trim() + " | " + match1.Groups["p"].Value.Trim() + " || " + match1.Groups["a2"].Value.Trim()
                                    + " | " + match1.Groups["c"].Value.Trim();
                            }
                            else Console.WriteLine($"{match.Groups["note"].Value.Trim()} --- note57");
                        }
                        else {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "EN",
                                Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(57)","").Replace("Abstract :","").Trim()
                            });

                        } 
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        var ipcsData = Regex.Match(inid.Trim(), @".+:(?<ipcs>.+)", RegexOptions.Singleline).Groups["ipcs"].Value.Replace("classification","").Trim();
                        var ipcs = Regex.Split(ipcsData, @",", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var typeOne = Regex.Match(ipc.Trim(), @"(?<fPart>\D\d+\D)00(?<sPart>\d{2})(?<tPart>.+)");
                            
                            if (typeOne.Success)
                            {
                                var fPart = typeOne.Groups["fPart"].Value.Trim();
                                var sPart = typeOne.Groups["sPart"].Value.Trim().TrimStart('0');
                                var tPart = Regex.Match(typeOne.Groups["tPart"].Value.Trim(), @"(?<part>\d.+?)0").Groups["part"].Value.Trim();

                                statusEvent.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = fPart + " " + sPart + "/" + tPart 
                                });
                            }
                            else
                            {
                                var typeTwo = Regex.Match(ipc.Trim(), @"(?<fPart>\D\d+\D)\s(?<sPart>\d{2})(?<tPart>\d+)");

                                if (typeTwo.Success)
                                {
                                    var tPart = Regex.Match(typeTwo.Groups["tPart"].Value.Trim(), @"(?<part>\d.+?)0").Groups["part"].Value.Trim();
                                    
                                    statusEvent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Class = typeTwo.Groups["fPart"].Value.Trim()+ " " + typeTwo.Groups["sPart"].Value.Trim().TrimStart('0') + "/" + tPart
                                    });
                                }
                                else
                                {
                                    var typeThree = Regex.Match(ipc.Trim(), @"(?<fPart>\D\d+\D)0(?<sPart>\d{2})(?<tPart>.+)");
                                    if (typeThree.Success)
                                    {
                                        var fPart = typeThree.Groups["fPart"].Value.Trim();
                                        var sPart = typeThree.Groups["sPart"].Value.Trim().TrimStart('0');
                                        var tPart = Regex.Match(typeThree.Groups["tPart"].Value.Trim(), @"(?<part>\d.+?)0").Groups["part"].Value.Trim();

                                        statusEvent.Biblio.Ipcs.Add(new Ipc()
                                        {
                                            Class = fPart + " " + sPart + "/" + tPart
                                        });
                                    }
                                    else
                                    {
                                        statusEvent.Biblio.Ipcs.Add(new Ipc()
                                        {
                                            Class = ipc
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if(subCode == "10")
            {
                var match = Regex.Match(note.Trim(), @"\d{1,3}?\s(?<city>\D.+?)\s\s?(?<aNum>\d.+)\s(?<eDate>\d{2}\/\d{2}\/\d{4}).+?(?<f74>[A-Z].+)\s(?<note>.+@.+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                    statusEvent.LegalEvent.Note = "|| LOCATION | " + match.Groups["city"].Value.Trim() + " || EMAIL | " + match.Groups["note"].Value.Trim();

                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["eDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                    {
                        Address1 = match.Groups["f74"].Value.Trim()
                    });
                }
                else
                {
                    Console.WriteLine($"{note}");
                }
            }
            return statusEvent;
        }

        private string MakeCountryCode(string code) => code switch
        {
            "Tamilnadu" => "IN",
            "India." => "IN",
            "India" => "IN",
            "Haryana" => "IN",
            "Gurugram" => "IN",
            "Tirunelveli." => "IN",
            "INDIA" => "IN",
            "UTTAR" => "IN",
            "China" => "CN",
            "DELHI" => "IN",
            "INDIA." => "IN",
            "MAHARASHTRA." => "IN",
            "Dehradun-" => "IN",
            "CHHATTISGARH-" => "IN",
            "Uttarakhand." => "IN",
            "Gajraula" => "IN",
            "Amroha(U.P)" => "IN",
            "Maharashtra" => "IN",
            "Thiruvallur" => "IN",
            "Mathura" => "IN",
            "Jharkhand" => "IN",
            "TAMILNADU" => "IN",
            "Tuljapur" => "IN",
            "ANDHRA" => "IN",
            "Bangalore." => "IN",
            "Andhra" => "IN",
            "Uttar" => "IN",
            "Tamil" => "IN",
            "Gautam" => "IN",
            "Jaipur" => "IN",
            "Gurgaon" => "IN",
            "Russian" => "RU",
            "Rajasthan" => "IN",
            "Guangdong" => "CN",
            "Meerut" => "IN",
            "Kurukshetra" => "IN",
            "Ghaziabad" => "IN",
            "Denmark" => "DK",
            "Mumbai" => "IN",
            "Amrvati" => "IN",
            "Kanpur" => "IN",
            "Maharashtra." => "IN",
            "MUMBAI" => "IN",
            "Vadodara" => "IN",
            "Saitama" => "JP",
            "Udupi" => "IN",
            "Mangalore" => "IN",
            "Germany" => "DE",
            "Odisha" => "IN",
            "Finland" => "FI",
            "Itanagar" => "IN",
            "TAMIL" => "IN",
            "COIMBATORE" => "IN",
            "Coimbatore" => "IN",
            "KARNATAKA" => "IN",
            "Karnataka" => "IN",
            "Noida" => "IN",
            "Delhi" => "IN",
            "Indore" => "IN",
            "Kompall" => "IN",
            "Kompally" => "IN",
            "Bangalore" => "IN",
            "Hyderabad" => "IN",
            "Chennai" => "IN",
            "Bengaluru" => "IN",
            "Telangana" => "IN",
            "TELANGANA-" => "IN",
            "TAMILNADU-" => "IN",
            "Ireland" => "IE",
            "Japan" => "JP",
            "Tokyo" => "JP",
            "Osaka" => "JP",
            "Utah" => "US",
            "USA" => "US",
            "California" => "US",
            "Washington" => "US",
            "NC" => "US",
            "Massachusetts" => "US",
            "Florida" => "US",
            "Pennsylvania" => "US",
            "Ontario" => "CA",
            "Colorado" => "US",
            "Seoul" => "KR",
            "Switzerland" => "CH",
            "Saudi" => "SA",
            _ => null
        };
        private List<string> MakeInids(string note, string subCode)
        {
            var inids = new List<string>();

            var inidsMatch = Regex.Match(note, @"(?<part1>.+)(?<inid57>\(57\).+)", RegexOptions.Singleline);

            if (inidsMatch.Success)
            {
                inids = Regex.Split(inidsMatch.Groups["part1"].Value.Trim(), @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();
                inids.Add(inidsMatch.Groups["inid57"].Value.Trim());
            }
            return inids;
        }
        private string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;
            if(subCode == "1")
            {
                foreach (var xElement in xElements)
                {
                    text += xElement.Value + "\n";
                }

                return text;
            }
            else if (subCode == "10")
            {
                foreach (var xElement in xElements)
                {
                    text += xElement.Value + " ";
                }

                return text.Replace("\r", "").Replace("\n", " ").Trim();
            }

            return null;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
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
