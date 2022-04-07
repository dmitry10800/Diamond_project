using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Integration;
using Newtonsoft.Json;

namespace PY_Diamond_Maksim
{
    internal class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = directory.GetFiles("*.tetml", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

            XElement tet;

            List<XElement> xElements = new();

            foreach (string tetml in files)
            {
                CurrentFileName = tetml;

                tet = XElement.Load(tetml);
              
                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                          .SkipWhile(val => !val.Value.StartsWith("(19) DIRECCIÓN NACIONAL"))
                         //.TakeWhile(val => !val.Value.StartsWith(""))
                          .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\(\s?11\s?\)\sN)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BB"));
                    }
                }
            }
            return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                Id = Id++,
                CountryCode = "PY",
                SectionCode = sectionCode,
                SubCode = subCode,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new()
                {
                    DOfPublication = new()
                },
                LegalEvent = new()
            };

            CultureInfo culture = new("ru-RU");

            if (subCode is "1")
            {
                Match match = Regex.Match(note.Replace("\r","").Replace("\n"," ").Trim(), @"(?<all>.+)\s(?<inid57>\(57.+)\(\s?19\s?\)\sD");

                List<string> inids = new();

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["all"].Value.Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                    inids.Add(match.Groups["inid57"].Value.Trim());
                }
                else
                {
                    Match matchN = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<all>.+)\s(?<inid57>\(57.+)\sDIR\s");
                    if (matchN.Success)
                    {
                        inids = Regex.Split(matchN.Groups["all"].Value.Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                        inids.Add(matchN.Groups["inid57"].Value.Trim());
                    }
                    else
                    {
                        Match matchN1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<all>.+)\s(?<inid57>\(57.+)");
                        if (matchN1.Success)
                        {
                            inids = Regex.Split(matchN1.Groups["all"].Value.Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                            inids.Add(matchN1.Groups["inid57"].Value.Trim());
                        }
                        else Console.WriteLine($"{note} --- not split");
                    }
                }
                foreach (string inid in inids)
                {
                    if (inid.StartsWith("(11)"))
                    {
                        Match pubMatch = Regex.Match(inid.Trim(), @"(?<inid11>PY.+?\d)(?<inid13>[A-Z])");

                        if (pubMatch.Success)
                        {
                            statusEvent.Biblio.Publication.Number = pubMatch.Groups["inid11"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = pubMatch.Groups["inid13"].Value.Trim();
                        }
                        else
                        {
                            Match pubMatch1 = Regex.Match(inid.Trim(), @"(?<inid11>PY.+\d)");

                            if (pubMatch1.Success)
                            {
                                statusEvent.Biblio.Publication.Number = pubMatch1.Groups["inid11"].Value.Trim();
                            }
                            else Console.WriteLine($"{inid} --- 11/13");
                        }
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        Match match43 = Regex.Match(inid.Trim(), @"\s(?<inid43day>\d{2})\s(?<inid43month>.+)\s(?<inid43year>\d{4})");

                        if (match43.Success)
                        {
                            string month = match43.Groups["inid43month"].Value.Trim() switch
                            {
                                "de Febrero de" => "02",
                                "de febrero de" => "02",
                                _ => ""
                            };

                            if (month is not "") statusEvent.Biblio.Publication.Date = match43.Groups["inid43year"].Value.Trim() + "/" + month + "/" + match43.Groups["inid43day"].Value.Trim();
                            else Console.WriteLine($"{match43.Groups["inid43month"].Value.Trim()} --- month");

                        }
                        else Console.WriteLine($"{inid} --- 43");
                    }
                    else if (inid.StartsWith("(12)"))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(12)", "").Replace("_","").Trim();
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        Match match21 = Regex.Match(inid.Trim(), @"(?<inid21>\d{4}.+)");

                        if (match21.Success)
                        {
                            statusEvent.Biblio.Application.Number = match21.Groups["inid21"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} --- 21");
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        Match match22 = Regex.Match(inid.Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                        if (match22.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime
                                .Parse(match22.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd")
                                .Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 22");
                    }
                    else if (inid.StartsWith("(71)"))
                    {
                        Match match71 = Regex.Match(inid.Trim(), @"Solicitante:?(?<name>.+)Domicilio(?<adress>.+),\s?(?<country>.+)\sSolic");

                        if (match71.Success)
                        {
                            string countryCode = MakeCountryCode(match71.Groups["country"].Value.Trim());

                            if (countryCode is not null)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = match71.Groups["name"].Value.Trim(),
                                    Address1 = match71.Groups["adress"].Value.Trim(),
                                    Country = countryCode
                                });
                            }
                            else Console.WriteLine($"{match71.Groups["country"].Value.Trim()}");
                        }
                        else
                        {
                            Match match71n = Regex.Match(inid.Trim(), @"Solicitante:?(?<name>.+)Domicilio(?<adress>.+),\s?(?<country>.+)");

                            if (match71n.Success)
                            {
                                string countryCode = MakeCountryCode(match71n.Groups["country"].Value.Trim());

                                if (countryCode is not null)
                                {
                                    statusEvent.Biblio.Applicants.Add(new PartyMember()
                                    {
                                        Name = match71n.Groups["name"].Value.Trim(),
                                        Address1 = match71n.Groups["adress"].Value.Trim(),
                                        Country = countryCode
                                    });
                                }
                                else Console.WriteLine($"{match71n.Groups["country"].Value.Trim()}");
                            }
                            else Console.WriteLine($"{inid} --- 71");
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        Match match72 = Regex.Match(inid.Trim(), @"Inventor:\s(?<name>.+)Domicilio\s(?<adress>.+)");

                        if (match72.Success)
                        {
                            List<string> nameList = Regex
                                .Split(match72.Groups["name"].Value.Trim(), @"\d{1,2}\.\s").Where(val =>
                                    !string.IsNullOrEmpty(val) && new Regex(@"\d\d?\..+").Match(val).Success).ToList();

                            List<string> adresList = Regex
                                .Split(match72.Groups["adress"].Value.Trim(), @"(?=\d{1,2}\.\s)").Where(val =>
                                    !string.IsNullOrEmpty(val) && new Regex(@"\d\d?\..+").Match(val).Success).ToList();

                            for (int i = 0; i < nameList.Count; i++)
                            {
                                Match matchAdr = Regex.Match(adresList[i].Trim(), @"\d\d?\.\s(?<adress>.+),\s(?<country>.+)");

                                if (matchAdr.Success)
                                {
                                    string code = MakeCountryCode(matchAdr.Groups["country"].Value.Trim());
                                    if (code is not null)
                                    {
                                        statusEvent.Biblio.Inventors.Add(new PartyMember()
                                        {
                                            Name = nameList[i].Trim(),
                                            Address1 = matchAdr.Groups["adress"].Value.Trim(),
                                            Country = code
                                        });
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{matchAdr.Groups["country"].Value.Trim()}");
                                    }

                                }
                            }
                        }
                        else Console.WriteLine($"{inid} --- 72");
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        Match match54 = Regex.Match(inid.Trim(), @"lo:?\s(?<text>.+)");

                        if (match54.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "ES",
                                Text = match54.Groups["text"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 54");
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        Match match74 = Regex.Match(inid.Trim(), @"te:?\s(?<text>.+)-");

                        if (match74.Success)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = match74.Groups["text"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 74");
                    }
                    else if (inid.StartsWith("(30)"))
                    {
                        Match match30 = Regex.Match(inid.Trim(), @"es:x?\s(?<num>.+)\s?-\s?(?<date>\d{2}.\d{2}.\d{4})\s?-\s?(?<code>[A-Z]{2})");

                        if (match30.Success)
                        {
                            statusEvent.Biblio.Priorities.Add(new Priority()
                            {
                                Number = match30.Groups["num"].Value.Trim(),
                                Date = DateTime.Parse(match30.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
                                Country = match30.Groups["code"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 30");
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        Match match57 = Regex.Match(inid.Trim(), @"men:?\s(?<text>.+)");

                        if (match57.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Abstract()
                            {
                                Language = "ES",
                                Text = match57.Groups["text"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 57");
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        Match match51 = Regex.Match(inid.Trim(), @"(?<edi>\d\d?):\s(?<inid51>.+)\sEn");

                        if (match51.Success)
                        {
                            List<string> ipcs = Regex.Split(match51.Groups["inid51"].Value.Trim(), @",|;").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string ipc in ipcs)
                            {
                                Match noteIpc = Regex.Match(ipc.Trim(), @"(?<class>.+)\((?<date>.+)\)");

                                if (noteIpc.Success)
                                {
                                    statusEvent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Edition = int.Parse(match51.Groups["edi"].Value.Trim(), culture),
                                        Class = noteIpc.Groups["class"].Value.Trim(),
                                        Date = noteIpc.Groups["date"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    statusEvent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Edition = int.Parse(match51.Groups["edi"].Value.Trim(), culture),
                                        Class = ipc.Trim()
                                    });
                                }
                            }
                        }
                        else Console.WriteLine($"{inid} --- 51");
                    }
                    else Console.WriteLine($"{inid} --- not process");
                }
            }
            return statusEvent;
        }

        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = "";

            if (subCode is "1")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + " ";
                }
            }
            return text;
        }
        internal string MakeCountryCode(string country) => country switch
        {
            "Países Bajos" => "NL",
            "Francia" => "FR",
            "Suiza" => "CH",
            "Alemania" => "DE",
            "EEUU" => "US",
            "Paraguay" => "PY",
            "Brasil" => "BR",
            "Canadá" => "CA",
            "Québec H3B 3M5 Canadá" => "CA",
            "México" => "MX",
            "España" => "ES",
            "India" => "IN",
            _ => null
        };
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
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
