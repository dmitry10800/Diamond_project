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
        private readonly string I31 = "(31)";
        private readonly string I32 = "(32)";
        private readonly string I33 = "(33)";
        private readonly string I96 = "(96)";
        private readonly string I86 = "(86)";
        private readonly string I87 = "(87)";
        private readonly string I97 = "(97)";
        private readonly string I54 = "(54)";
        private readonly string I73 = "(73)";
        private readonly string I72 = "(72)";
        private readonly string I71 = "(71)";
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
                          .SkipWhile(val => !val.Value.StartsWith("OBJAVA PROŠIRENIH EVROPSKIH PATENATA"+"\n"+"Publication of extended european patents"))
                          .TakeWhile(val => !val.Value.StartsWith("OBJAVA ZAHTJEVA ZA PROŠIRENJE EVROPSKIH PRIJAVA PATENATA"))
                          .TakeWhile(val => !val.Value.StartsWith("OBJAVA UPISA PROMJENA"))
                          .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s\d)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith(@"(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note.Replace("..", ""), subCode, "FG"));
                    }
                }
                else
                if(subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                         .SkipWhile(val => !val.Value.StartsWith("Publication of request for the extension of the effects of european patent applications"))
                         .TakeWhile(val => !val.Value.StartsWith("INDEKS BROJEVA ZAHTJEVA ZA PROŠIRENJE EVROPSKIH PRIJAVA PATENTA"))
                         .ToList();

                    foreach (string note in Regex.Split(MakeText(xElements, subCode), @"(?=\(51\)\s)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(51)")).ToList())
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AC"));
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
            Priority priority1 = new();

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
                            intConvention.PctApplCountry = match.Groups["kind"].Value.Trim();
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
                            Match match = Regex.Match(assigny.Trim(), @"(?<name>.+?)\n(?<adress>.+)\/\s(?<code>[A-Z]{2})", RegexOptions.Singleline);
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
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+?)\n(?<adress>.+)\/\s(?<code>[A-Z]{2})", RegexOptions.Singleline);
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
                            Match match = Regex.Match(agent.Trim(), @"(?<name>.+?)\n(?<adress>.+)\/\s(?<code>[A-Z]{2})", RegexOptions.Singleline);
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
            else
            if(subCode == "3")
            {
                foreach (string inid in MakeInids(note,subCode))
                {
                    if (inid.StartsWith(I51))
                    {
                        foreach (string ipc in Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(51) MKP", "").Trim(), @"(?<=\s\d{4})").Where(val => !string.IsNullOrEmpty(val)).ToList())
                        {
                            Match match = Regex.Match(ipc, @"(?<class>.+)\s(?<date>\d{4})");
                            if (match.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["date"].Value.Trim() + "/01/01"
                                });
                            }
                            else Console.WriteLine($"{ipc} -- 51");
                        }
                    }
                    else
                    if (inid.StartsWith(I96))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(96)", "").Trim(), @"(?<aNum>.+)\s(?<month>\d{1,2})\/(?<day>\d{1,2})\/(?<year>\d{4})");

                        if (match.Success)
                        {
                            europeanPatent.AppNumber = match.Groups["aNum"].Value.Trim();
                            europeanPatent.AppDate = match.Groups["year"].Value.Trim() + "/" + match.Groups["month"].Value.Trim() + "/" + match.Groups["day"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} ---- 96");
                    }
                    else
                    if (inid.StartsWith(I97))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(97)", "").Trim(), @"(?<pNum>.+).\s(?<month>\d{1,2})\/(?<day>\d{1,2})\/(?<year>\d{4})");

                        if (match.Success)
                        {
                            europeanPatent.PubNumber = match.Groups["pNum"].Value.Trim();
                            europeanPatent.PubDate = match.Groups["year"].Value.Trim() + "/" + match.Groups["month"].Value.Trim() + "/" + match.Groups["day"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} ---- 97");
                    }
                    else
                    if (inid.StartsWith(I54))
                    {
                        statusEvent.Biblio.Titles.Add(new Title
                        {
                            Language = "EN",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(54)", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith(I72))
                    {
                        List<string> inventors = Regex.Split(inid.Replace("(72)", "").Trim(), @"(?<=[A-Z]{2}$)", RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+?,.+?),\s(?<adress>.+),\s(?<code>\D{2})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inventor} ---- 72");
                        }
                    }
                    else
                    if (inid.StartsWith(I71))
                    {
                        List<string> applicants = Regex.Split(inid.Replace("(71)", "").Trim(), @"(?<=[A-Z]{2}$)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match match = Regex.Match(applicant.Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<code>\D{2})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{applicant} --- 71");
                        }
                    }
                    else
                    if (inid.StartsWith(I31))
                    {
                        priority1.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(31)", "").Trim();
                    }
                    else
                    if (inid.StartsWith(I32))
                    {
                        priority1.Date = DateTime.Parse(inid.Replace("\r", "").Replace("\n", " ").Replace("(32)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith(I33))
                    {
                        List<string> prior = Regex.Split(inid.Replace("(33)", "").Replace("\r","").Replace("\n"," ").Trim(), @"(?<=[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        for (int i = 0; i < prior.Count; i++)
                        {
                            if(i == 0)
                            {
                                priority1.Country = prior[i].Trim();                               
                            }
                            else
                            {
                                Match match = Regex.Match(prior[i].Trim(), @"(?<num>\d+).+(?<date>\d{2}.\d{2}.\d{4}).+(?<code>[A-Z]{2})");

                                if (match.Success)
                                {
                                    statusEvent.Biblio.Priorities.Add(new Priority
                                    {
                                        Number = match.Groups["num"].Value.Trim(),
                                        Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
                                        Country = match.Groups["code"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{prior[i]} --- 33 ");
                            }
                            

                        }
                    }
                    else Console.WriteLine($"{inid} ----- don't proc.");
                }

                statusEvent.Biblio.EuropeanPatents.Add(europeanPatent);
                statusEvent.Biblio.Priorities.Add(priority1);
            }

            return statusEvent;

        }

        internal List<string> MakeInids(string note, string subcode)
        {
            List<string> inids = new();

            if(subcode == "2")
            {
                if (note.Contains("(57)"))
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
                else
                {
                    inids = Regex.Split(note.Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                }
               
            }
            else
            if(subcode == "3")
            {
                inids = Regex.Split(note.Substring(note.IndexOf("(96)")).Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                Match match = Regex.Match(note.Substring(0, note.IndexOf("(96)")).Trim(), @"(?<f51>.+)\s(?<i97>\(97\)\s\D{2}\s\d+)\s(?<f51c>.+)",RegexOptions.Singleline);

                if (match.Success)
                {
                    inids.Add(match.Groups["f51"].Value.Trim() + "\n" + match.Groups["f51c"].Value.Trim());
                }
                else 
                {
                    Match match1 = Regex.Match(note.Substring(0, note.IndexOf("(96)")).Trim(), @"(?<f51>.+)\s(?<i97>\(97\)\s\D{2}\s\d+)", RegexOptions.Singleline);

                    if (match1.Success)
                    {
                        inids.Add(match.Groups["f51"].Value.Trim());
                    }
                    else Console.WriteLine($"{note} --- inid 51 error");                
                }
                
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
            else
            if (subCode == "3")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + "\n";
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
