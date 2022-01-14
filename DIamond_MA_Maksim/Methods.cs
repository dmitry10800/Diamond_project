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

namespace Diamond_MA_Maksim
{
    internal class Methods
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

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("OFFICE MAROCAIN DE LA PROPRIETE"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(12\)\s?BRE)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(12)") && Regex.Match(val, @".+(?<group>\(11\).+:.+\d\sA\d{1,2}.+)", RegexOptions.Singleline).Success).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BA"));
                    }
                }
                else
                if(subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("OFFICE MAROCAIN DE LA PROPRIETE"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(12\)\s?BRE)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(12)") && Regex.Match(val, @".+(?<group>\(11\).+:.+\d\sB\d{1,2}.+)", RegexOptions.Singleline).Success).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BA"));
                    }
                }
            }
            return statusEvents;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

            if (subCode == "1")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + " ";
                }
            }
            else
            if (subCode == "2")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + " ";
                }
            }

            return text;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "MA",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new(),
                NewBiblio = new()
            };

            CultureInfo culture = new("ru-RU");

            if (subCode is "1" or "2")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(12)"))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(12)", "").Trim();
                    }
                    else if (inid.StartsWith("(11)"))
                    {
                        Match match = Regex.Match(inid, @".+:(?<num>.+)(?<kind>\D\d{1,2})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        Match match = Regex.Match(inid, @".+:\s?(?<date>\d{2}.\d{2}.\d{4})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @".+:\s?(?<ipc>.+)");

                        if (match.Success)
                        {
                            List<string> ipcs = Regex.Split(match.Groups["ipc"].Value.Trim(), @";")
                                .Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string ipc in ipcs)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = ipc.Trim()
                                });
                            }
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @".+:\s?(?<num>.+)");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Number = match.Groups["num"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        Match match = Regex.Match(inid, @".+:\s?(?<date>\d{2}.\d{2}.\d{4})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(30)"))
                    {
                        Match match = Regex.Match(inid.Trim(),
                            @".+:\s?(?<date>\d{2}.\d{2}.\d{4})\s?(?<code>\D{2})\s?(?<num>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Priorities.Add(new Priority()
                            {
                                Number = match.Groups["num"].Value.Trim(),
                                Country = match.Groups["code"].Value.Trim(),
                                Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(86)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @".+:\s?(?<num>.+)\s?(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(71)"))
                    {
                        List<string> applicants = Regex
                            .Split(inid.Replace("(71) Demandeur(s) :", "").Trim(), @"(?=•)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match match = Regex.Match(applicant.Trim(),
                                @"(?<name>.+?),\s?(?<adress>.+)\s?\((?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["country"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inid}");
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @".+:\s?(?<all>.+)");

                        if (match.Success)
                        {
                            List<string> inventors = Regex.Split(match.Groups["all"].Value.Trim(), @";")
                                .Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string inventor in inventors)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember()
                                {
                                    Name = inventor.Trim()
                                });    
                            }
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @".+:\s?(?<all>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = match.Groups["all"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @".+:\s?(?<all>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "FR",
                                Text = match.Groups["all"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @".+:\s?(?<all>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Abstract()
                            {
                                Language = "FR",
                                Text = match.Groups["all"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else Console.WriteLine($"{inid}");
                }
            }
            return statusEvent;
        }
        internal List<string> MakeInids(string note, string subCode)
        {
            if (subCode is "1" or "2")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<inids>.+)\s(?<inid57>\(57\).+)\s?ROYAUME");

                if (match.Success)
                {
                    List<string> inids = Regex.Split(match.Groups["inids"].Value.Trim(), @"(?=\(\d{2}\).+)")
                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                    inids.Add(match.Groups["inid57"].Value.Trim());

                    return inids;
                }
                else
                {
                    Match match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                        @"(?<inids>.+)\s(?<inid57>\(57\).+)");

                    if (match1.Success)
                    {
                        List<string> inids = Regex.Split(match1.Groups["inids"].Value.Trim(), @"(?=\(\d{2}\).+)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        inids.Add(match1.Groups["inid57"].Value.Trim());

                        return inids;
                    }
                    else Console.WriteLine($"{note}");
                }
            }
            return null;
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
