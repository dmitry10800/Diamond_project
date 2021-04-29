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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_ME_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        private readonly string I11 = "(11)";
        private readonly string I13 = "(13)";
        private readonly string I51 = "(51)";
        private readonly string I21 = "(21)";
        private readonly string I22 = "(22)";
        private readonly string I30 = "(30)";
        private readonly string I96 = "(96)";
        private readonly string I86 = "(86)";
        private readonly string I87 = "(87)";
        private readonly string I97 = "(97)";
        private readonly string I54 = "(54)";
        private readonly string I73 = "(73)";
        private readonly string I72 = "(72)";
        private readonly string I74 = "(74)";
        private readonly string I57 = "(57)";
        private readonly string I57n = "(57n)";

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

                if(subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                          .SkipWhile(val => !val.Value.StartsWith("OBJAVA PROŠIRENIH EVROPSKIH PATENATA"+"\n"+ "Publication of extended european patents"))
                          .TakeWhile(val => !val.Value.StartsWith("OBJAVA UPISA PROMJENA"))
                          .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s\d)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith(@"(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
            }
            return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                Id = Id++,
                SubCode = subCode,
                SectionCode = sectionCode,
                CountryCode = "ME",
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            CultureInfo culture = new("RU-ru");

            EuropeanPatent europeanPatent = new();
            IntConvention intConvention = new();

            if(subCode == "2")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I11))
                    {
                        statusEvent.Biblio.Publication.Number = inid.Replace(I11, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I13))
                    {
                        Match match = Regex.Match(inid.Replace(I13, ""), @"[A-Z]");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Kind = match.Value;
                        }
                        else Console.WriteLine($"{inid}  - I13");
                    }
                    else
                    if (inid.StartsWith(I51))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace(I51, "").Replace("\r", "").Replace("\n", " ").Trim(), @"(?<=\))")
                            .Where(val => !string.IsNullOrEmpty(val))
                            .ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<num>.+)\((?<date>.+)\)");
                            if (match.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                {
                                    Date = match.Groups["date"].Value.Trim(),
                                    Class = match.Groups["num"].Value.Trim()
                                });
                            }
                        }
                    }
                    else
                    if (inid.StartsWith(I21))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace(I21, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I22))
                    {
                        statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith(I30))
                    {
                        List<string> priorities = Regex.Split(inid.Replace(I30, "").Replace("\r","").Replace("\n"," ").Trim(), @"(?<=[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priority in priorities)
                        {
                            Match match = Regex.Match(priority.Trim(), @"(?<num>.+)\s(?<date>\d{2}.\d{2}.\d{4})\s(?<code>[A-Z]{2})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Integration.Priority
                                {
                                    Number = match.Groups["num"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
                                    Country = match.Groups["code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{priority}  --- 30 ");
                        }
                    }
                    else
                    if (inid.StartsWith(I96))
                    {
                        Match match = Regex.Match(inid.Replace(I96, "").Trim(), @"(?<num>.+)\/(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            europeanPatent.AppNumber = match.Groups["num"].Value.Trim();
                            europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 96");
                    }
                    else
                    if (inid.StartsWith(I86))
                    {
                        Match match = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<kind>.+)\s(?<num>.+)\/(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            intConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                            intConvention.PctApplKind = match.Groups["kind"].Value.Trim();
                            intConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<num>.+)\/(?<date>\d{2}.\d{2}.\d{4})");

                            if (match1.Success)
                            {
                                intConvention.PctApplNumber = match1.Groups["num"].Value.Trim();
                                intConvention.PctApplDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else Console.WriteLine($"{inid} --- 86");
                        } 
                    }
                    else
                    if (inid.StartsWith(I87))
                    {
                        Match match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<num>.+)\/(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            intConvention.PctPublNumber = match.Groups["num"].Value.Trim();
                            intConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 87");
                    }
                    else
                    if (inid.StartsWith(I97))
                    {
                        List<string> notes = Regex.Split(inid.Replace(I97, "").Trim(), @"(?=[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        for (int i = 0; i < notes.Count; i++)
                        {
                            Match match = Regex.Match(notes[i].Trim(), @"(?<num>.+)\/(?<date>\d{2}.\d{2}.\d{4})");

                            if (match.Success)
                            {
                                if (i == 0)
                                {
                                    europeanPatent.PubNumber = match.Groups["num"].Value.Trim();
                                    europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                }
                                else
                                {
                                    statusEvent.Biblio.EuropeanPatents.Add(new EuropeanPatent
                                    {
                                        PubNumber = match.Groups["num"].Value.Trim(),
                                        PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                    });
                                }
                            }
                            else Console.WriteLine($"{notes[i]} --- 97");
                        }
                    }
                    else
                    if (inid.StartsWith(I54))
                    {
                        Match match = Regex.Match(inid.Replace(I54, "").Replace("\r", "").Replace("\n", " ").Trim(), @"me(?<me>.+)\sen(?<en>.+)");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Title
                            {
                                Language = "ME",
                                Text = match.Groups["me"].Value.Trim()                               
                            });
                            statusEvent.Biblio.Titles.Add(new Title
                            {
                                Language = "EN",
                                Text = match.Groups["en"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 54");
                    }
                    else
                    if (inid.StartsWith(I73))
                    {
                        List<string> assignees = Regex.Split(inid.Replace(I73, "").Trim(), @"(?<=\/\s[A-Z]{2})", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string assigny in assignees)
                        {
                            Match match = Regex.Match(assigny.Trim(), @"(?<name>.+?)\n(?<adress>.+)\/\s(?<code>[A-Z]{2})");
                            if (match.Success)
                            {
                                statusEvent.Biblio.Assignees.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{assigny} --- 73");                           
                        }
                    }
                    else
                    if (inid.StartsWith(I72))
                    {
                        List<string> inventors = Regex.Split(inid.Replace(I72, "").Trim(), @"(?<=\/\s?[A-Z]{2})", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+?)\n(?<adress>.+)\/\s(?<code>[A-Z]{2})");
                            if (match.Success)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inventor} --- 72");
                        }
                    }
                    else
                    if (inid.StartsWith(I74))
                    {
                        List<string> agents = Regex.Split(inid.Replace(I74, "").Trim(), @"(?<=\/\s[A-Z]{2})", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string agent in agents)
                        {
                            Match match = Regex.Match(agent.Trim(), @"(?<name>.+?)\n(?<adress>.+)\/\s(?<code>[A-Z]{2})");
                            if (match.Success)
                            {
                                statusEvent.Biblio.Agents.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{agent} --- 74");
                        }
                    }
                    else
                    if (inid.StartsWith(I57))
                    {
                        statusEvent.Biblio.Claims.Add(new DiamondProjectClasses.Claim
                        {
                            Text = inid.Replace("(57) 1.", "").Replace("(57)1.", "").Replace(I57, "").Trim(),
                            Number = "1",
                            Language = "ME"
                        });
                    }
                    else
                    if (inid.StartsWith(I57n))
                    {
                        Match match = Regex.Match(inid.Replace(I57n, "").Trim(), @".+?(?<num>\d+).+");

                        if (match.Success)
                        {
                            statusEvent.LegalEvent = new()
                            {
                                Note = inid.Replace(I57n, "").Trim(),
                                Language = "ME",
                                Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Type = "note",
                                        Language = "EN",
                                        Tr = "The patent contains further " + match.Groups["num"].Value.Trim() + " claims"
                                    }
                                }
                            };
                        }
                        else Console.WriteLine($"{inid} --- 57n");
                    }
                    else Console.WriteLine($"{inid} not processed");
                }
                statusEvent.Biblio.EuropeanPatents.Add(europeanPatent);
                statusEvent.Biblio.EuropeanPatents.Reverse();
                statusEvent.Biblio.IntConvention = intConvention;
            }

            return statusEvent;

        }

        internal List<string> MakeInids(string note, string subcode)
        {
            List<string> inids = new();

            if(subcode == "2")
            {
                inids = Regex.Split(note.Substring(0, note.IndexOf("(57) ")), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                Match match = Regex.Match(note.Substring(note.IndexOf("(57) ")), @"(?<inid57>.+)\s(?<note57>P.+)", RegexOptions.Singleline);

                if (match.Success)
                {
                    inids.Add(match.Groups["inid57"].Value.Trim());
                    inids.Add("(57n) " + match.Groups["note57"].Value.Trim());
                }
                else Console.WriteLine($"{note.Substring(note.IndexOf("(57) "))} - match failed");
            }

            return inids;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

            if(subCode == "2")
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
