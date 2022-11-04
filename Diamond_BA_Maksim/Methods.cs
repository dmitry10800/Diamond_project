using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_BA_Maksim
{
    internal class Methods
    {
        private string CurrentFileName;
        private int Id = 1;
        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalStatusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = new();

            foreach (var item in files)
            {
                CurrentFileName = item;

                tet = XElement.Load(item);

                if(subCode is "4")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("OBJAVA PROŠIRENIH EVROPSKIH PA NATA UPISANIH U REGISTAR PATENATA"))
                       .TakeWhile(val => !val.Value.StartsWith("OBJAVLJIVANJE ZAHTJEVA A KONSEZUALNI PATENT"))
                       .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s[A-Z].+)", RegexOptions.Singleline)
                       .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (var note in notes)
                    {
                        legalStatusEvents.Add(MakeNote(note, subCode, "BA"));
                    }
                }
            }
            return legalStatusEvents;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakeNote(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatus = new()
            {
                Id = Id++,
                SectionCode = sectionCode,
                SubCode = subCode,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "BA",
                Biblio = new(),
                LegalEvent = new()
            };

            if(subCode is "4")
            {
                Integration.EuropeanPatent europeanPatent = new();

                foreach (var inid in MakeInids(note,subCode))
                {
                    if (inid.StartsWith("(11)"))
                    {
                        legalStatus.Biblio.Publication.Number = inid.Replace("(11)", "").Trim();
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        legalStatus.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        legalStatus.Biblio.Application.Date = inid.Replace("(22)", "").Replace("-", "/").Trim();
                    }
                    else if (inid.StartsWith("(26)"))
                    {
                        legalStatus.Biblio.Publication.Authority = inid.Replace("(26)", "").Trim();
                    }
                    else if (inid.StartsWith("(96)"))
                    {
                        var match = Regex.Match(inid.Replace("(96)", "").Trim(), @"(?<num>.+)\s(?<date>\d{4}\-\d{2}\-\d{2})");

                        if (match.Success)
                        {
                            europeanPatent.AppNumber = match.Groups["num"].Value.Trim();
                            europeanPatent.AppDate = match.Groups["date"].Value.Replace("-", "/").Trim();
                        }
                    }
                    else if (inid.StartsWith("(97)"))
                    {
                        europeanPatent.PubDate = inid.Replace("(97)", "").Replace("-", "/").Trim();
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        var ipcs = Regex.Split(inid.Replace("(51)", "").Replace("\r", "").Replace("\n", " ").Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            legalStatus.Biblio.Ipcs.Add(new Integration.Ipc
                            {
                                Class = ipc
                            });
                        }
                    }
                    else if (inid.StartsWith("(73)"))
                    {
                        var assigneesList = Regex.Split(inid.Replace("(73)", "").Trim(), @"(?<=\n[A-Z]{2}\n)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var assignee in assigneesList)
                        {
                            var assigMatch = Regex.Match(assignee.Trim(), @"(?<name>.+)\n(?<adress>(.+\n){0,5}.+)\n(?<country>[A-Z]{2})");

                            if (assigMatch.Success)
                            {
                                legalStatus.Biblio.Assignees.Add(new Integration.PartyMember
                                {
                                    Name = assigMatch.Groups["name"].Value.Trim(),
                                    Address1 = assigMatch.Groups["adress"].Value.Replace("\r", "").Replace("\n", " ").Trim(),
                                    Country = assigMatch.Groups["country"].Value.Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        var inventorsList = Regex.Split(inid.Replace("(72)", "").Trim(), @"(?<=\n[A-Z]{2}\n)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventorsList)
                        {
                            var inventorMatch = Regex.Match(inventor.Trim(), @"(?<name>.+)\n(?<adress>(.+\n){0,5}.+)\n(?<country>[A-Z]{2})");

                            if (inventorMatch.Success)
                            {
                                legalStatus.Biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = inventorMatch.Groups["name"].Value.Trim(),
                                    Address1 = inventorMatch.Groups["adress"].Value.Replace("\r", "").Replace("\n", " ").Trim(),
                                    Country = inventorMatch.Groups["country"].Value.Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        var agentsList = Regex.Split(inid.Replace("(74)", "").Trim(), @"(?<=\n[A-Z]{2}\n)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var agent in agentsList)
                        {
                            var agentMatch = Regex.Match(agent.Trim(), @"(?<name>.+)\n(?<adress>(.+\n){0,5}.+)\n(?<country>[A-Z]{2})");

                            if (agentMatch.Success)
                            {
                                legalStatus.Biblio.Agents.Add(new Integration.PartyMember
                                {
                                    Name = agentMatch.Groups["name"].Value.Trim(),
                                    Address1 = agentMatch.Groups["adress"].Value.Replace("\r", "").Replace("\n", " ").Trim(),
                                    Country = agentMatch.Groups["country"].Value.Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        legalStatus.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "HR",
                            Text = inid.Replace("(54)", "").Replace("\r", "").Replace("\n", " ").Trim()
                        });
                    }
                    else if (inid.StartsWith("(30)"))
                    {
                        var prioritiesList = Regex.Split(inid.Replace("(30)", "").Replace("\r", "").Replace("\n", " ").Trim(), @"(?<=\s[A-Z]{2}\s)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priority in prioritiesList)
                        {
                            var match = Regex.Match(priority.Trim(), @"(?<num>.+)\s(?<date>\d{4}\-\d{2}\-\d{2}).+(?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                legalStatus.Biblio.Priorities.Add(new Integration.Priority
                                {
                                    Country = match.Groups["country"].Value.Trim(),
                                    Date = match.Groups["date"].Value.Replace("-", "/"),
                                    Number = match.Groups["num"].Value.Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        legalStatus.Biblio.Claims.Add(new DiamondProjectClasses.Claim
                        {
                            Number = "1",
                            Text = inid.Replace("(57)", "").Replace("\r", "").Replace("\n", " ").Trim(),
                            Language = "HR"
                        });
                    }
                    else if (inid.StartsWith("(57n)"))
                    {
                        var match = Regex.Match(inid.Replace("(57n)", "").Replace("\r", "").Replace("\n", " ").Trim(), @"(?<text>.+):\s(?<num>\d{1,3})");

                        if (match.Success)
                        {
                            legalStatus.LegalEvent.Note = "|| " + match.Groups["text"].Value.Trim() + " | " + match.Groups["num"].Value.Trim();
                            legalStatus.LegalEvent.Language = "HR";

                            legalStatus.LegalEvent.Translations.Add(new Integration.NoteTranslation
                            {
                                Language = "EN",
                                Tr = "|| The number of other claims | " + match.Groups["num"].Value.Trim(),
                                Type = "INID"
                            });
                        }
                    }
                    else Console.WriteLine($"{inid} --- not processed");
                }
                legalStatus.Biblio.EuropeanPatents.Add(europeanPatent);
            }
            return legalStatus;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            var sb = new StringBuilder();

            foreach (var xElement in xElements)
            {
                sb = sb.Append(xElement.Value + " ");
            }
            return sb.ToString();
        }
        internal List<string> MakeInids (string note, string subCode)
        {
            List<string> inids = new();

            var match = Regex.Match(note, @"(?<firstPart>.+)\s(?<priority>\(31\).+)\s(?<secondPart>\(96\).+)\s(?<inid57>\(57\).+)\s(?<note>Broj.+)", RegexOptions.Singleline);

            if (match.Success)
            {
                inids = Regex.Split(match.Groups["firstPart"].Value.Trim() +" "+ match.Groups["secondPart"].Value.Trim(), @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                inids.Add("(30) " + match.Groups["priority"].Value.Replace("(31)", "").Replace("(32)", "").Replace("(33)", "").Trim());

                inids.Add(match.Groups["inid57"].Value.Trim());

                inids.Add("(57n) " + match.Groups["note"].Value.Trim());
            }
            else Console.WriteLine($"{note} - not split");

            return inids;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
