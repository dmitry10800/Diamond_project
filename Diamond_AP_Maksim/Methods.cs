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

namespace Diamond_AP_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        private readonly string I21 = "(21)";
        private readonly string I22 = "(22)";
        private readonly string I23 = "(23)";
        private readonly string I31 = "(31)";
        private readonly string I32 = "(32)";
        private readonly string I33 = "(33)";
        private readonly string I54 = "(54)";
        private readonly string I71 = "(71)";
        private readonly string I72 = "(72)";
        private readonly string I74 = "(74)";
        private readonly string I84 = "(84)";
        private readonly string I51 = "(51)";
        private readonly string I75 = "(75)";
        private readonly string I86 = "(86)";
        private readonly string I96 = "(96)";

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
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
                       .SkipWhile(val => !val.Value.StartsWith("Patent\n"+ "Applications\n"))
                       .TakeWhile(val => !val.Value.StartsWith("Pa ent Applications Renew d")).ToList();


                    List<string> notes = Regex.Split(MakeText(xElements).Trim(), @"(?=\(21\))").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(21)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note.Replace("■", "").Replace("PATENTS", "").Replace("·", "").Replace("►", "").Replace("▶",""), subCode, "AF"));
                    }

                }
                else
                if(subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                      .SkipWhile(val => !val.Value.StartsWith("FORM "))
                      .TakeWhile(val => !val.Value.StartsWith("Classification Index of Granted Patents")).ToList();

                    List<string> notes = Regex.Split(MakeText(xElements).Trim(), @"(?=FORM\s)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("FORM ")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note.Replace("Patents Granted (Contd.)", ""), subCode, "FG"));
                    }
                }
                else
                if (subCode == "7")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("I Patent No. Application No. Date"))
                       .TakeWhile(val => !val.Value.StartsWith("UTILITY MODELS")).ToList();


                    List<string> notes = Regex.Split(MakeText(xElements).Trim(), @"(?=AP\s?\d{4,5}\s)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("AP ")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "ND"));
                    }

                }
                else
                if (subCode == "10")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("Application No. Date Fee Paid Valid Until Anniversary"))
                       .TakeWhile(val => !val.Value.StartsWith("Abandoned")).ToList();


                    List<string> notes = Regex.Split(MakeText(xElements).Trim(), @"(?=AP\s?\/\s?P\s?\/)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AD"));
                    }

                }
                else
                if (subCode == "20")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("PATENT APPLICATIONS LAPSED/WITHDRAWN"))
                        .TakeWhile(val => !val.Value.StartsWith("Utility Model")).ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(21\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(21)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FA"));
                    }
                }
            }

            return statusEvents;
        }

        internal string MakeText(List<XElement> xElements)
        {
            string text = null;

            foreach (XElement xElement in xElements)
            {
                text += xElement.Value + " ";
            }

            text = text.Replace("\r", "").Replace("\n", " ").Replace("●●", " ")
                .Replace("Patent Applications Lapsed/Withdrawn (Contd.)", " ")
                .Replace("Patent Applications Filed (Contd.)"," ")
                .Replace("Patents Renewed (Contd.)","")
                .Replace("Application No. Date Fee Paid Valid Until Anniversary", "")
                .Replace("Patent Applications Renewed (Contd.)", "")
                .Replace("I Patent No. Application No. Date Fee Paid Valid Until Anniversary I","")
                
                .Trim();

            return text;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legal = new()
            {
                CountryCode = "AP",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf"))
            };

            Biblio biblio = new();

            LegalEvent legalEvent = new();

            CultureInfo culture = new("RU-ru");
       
            if (subCode == "1")
            {

                Priority priority = new();

                foreach (string inid in MakeInids(note))
                {
                    if (inid.StartsWith(I21))
                    {
                        biblio.Application.Number = inid.Replace(I21, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I22))
                    {
                        biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith(I23))
                    {
                        biblio.Application.OtherDate = DateTime.Parse(inid.Replace(I23, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith(I31))
                    {
                        priority.Number = inid.Replace(I31, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I32))
                    {
                        priority.Date = DateTime.Parse(inid.Replace(I32, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith(I33))
                    {
                        priority.Country = inid.Replace(I33, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I54))
                    {
                        biblio.Titles.Add(new Title
                        {
                            Language = "EN",
                            Text = inid.Replace(I54, "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith(I71))
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = inid.Replace(I71, "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith(I72))
                    {
                        List<string> inventors = Regex.Split(inid.Replace(I72, "").Replace("et al","").Trim(), @",|\sand\s").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    else
                    if (inid.StartsWith(I74))
                    {
                        biblio.Agents.Add(new PartyMember
                        {
                            Name = inid.Replace(I74, "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith(I84))
                    {
                        List<string> conventions = Regex.Split(inid.Replace(I84, ""), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        biblio.IntConvention.DesignatedStates = new();

                        foreach (string convention in conventions)
                        {
                            biblio.IntConvention.DesignatedStates.Add(convention.Trim());
                        }
                    }
                    else
                    if (inid.StartsWith(I96))
                    {
                        Match match = Regex.Match(inid.Replace(I96, "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4})\s(?<num>.+)");

                        if (match.Success)
                        {
                            biblio.EuropeanPatents.Add(new EuropeanPatent
                            {
                                AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
                                AppNumber = match.Groups["num"].Value.Replace("·","").Replace("►","").Trim()
                            });
                        }
                    }
                    else 
                    if (inid.StartsWith(I51))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        
                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<num>.+)\s\((?<date>.+)\)");

                            if (match.Success)
                            {
                                biblio.Ipcs.Add(new Ipc
                                {
                                    Class = match.Groups["num"].Value.Trim(),
                                    Date = match.Groups["date"].Value.Trim()
                                });
                            }
                        }
                    }
                    else
                    if (inid.StartsWith(I75))
                    {

                        biblio.InvOrApps=new()
                        {
                            new PartyMember
                            {
                                Name = inid.Replace(I75, "").Trim()
                            }
                        };
                    }
                    else
                    if (inid.StartsWith(I86))
                    {
                        Match match = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4})\s(?<num>.+)");

                        if (match.Success)
                        {
                            biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                            biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                    }

                    else Console.WriteLine($"{inid}");

                }
                biblio.Priorities.Add(priority);
                legal.Biblio = biblio;
            }
            else
            if(subCode == "3")
            {
                DOfPublication dOfPublication = new();
                Priority priority = new();
                IntConvention intConvention = new();

                foreach (string inid in MakeInids(note.Replace("et al","").Trim()))
                {
                    if (inid.StartsWith("(12)"))
                    {
                        biblio.Publication.LanguageDesignation = inid.Replace("(12)", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(11)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @":\s?(?<num>[A-Z].+\d)");

                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["num"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} --------------------- 11");
                    }
                    else
                    if (inid.StartsWith("(73)"))
                    {
                        Match match = Regex.Match(inid.Replace("(73)","").Trim(), @"\)\s(?<name>.+?),\s?(?<adress>.+),\s?(?<country>.+)");

                        if (match.Success)
                        {
                            biblio.Assignees.Add(new PartyMember
                            {
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Name = match.Groups["name"].Value.Trim(),
                                Country = MakeCountryCode(match.Groups["country"].Value.Trim())
                            });

                            if (MakeCountryCode(match.Groups["country"].Value.Trim()) == null) Console.WriteLine($" ------------------------------------------ {match.Groups["country"].Value.Trim()}");
                        }
                        else Console.WriteLine($"{inid} --------------------- 73");
                    }
                    else
                    if (inid.StartsWith("(21)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @":\s?(?<num>[A-Z].+\d)");

                        if (match.Success)
                        {
                            biblio.Application.Number = match.Groups["num"].Value.Replace(" ","").Trim();
                        }
                        else Console.WriteLine($"{inid} --------------------- 21");
                    }
                    else
                    if (inid.StartsWith("(22)"))
                    {
                        Match match = Regex.Match(inid.Replace(" ","").Trim(), @":\s?(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Replace(":",".").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --------------------- 22");
                    }
                    else
                    if (inid.StartsWith("(45)"))
                    {
                        Match match = Regex.Match(inid.Replace(" ", "").Trim(), @":\s?(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            dOfPublication.date_45 = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --------------------- 45");
                    }
                    else
                    if (inid.StartsWith("(32)"))
                    {
                        List<string> priorities = Regex.Split(inid.Replace("(32) Date", "").Trim(), @"(?<=\.\d{4})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string item in priorities)
                        {
                            Match match = Regex.Match(item.Trim(), @"(?<code>[A-Z]{2})\s(?<num>.+)\s(?<date>\d{2}.\d{2}.\d{4})");

                            if (match.Success)
                            {
                                priority.Country = match.Groups["code"].Value.Trim();
                                priority.Number = match.Groups["num"].Value.Trim();
                                priority.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }
                        }

                    }
                    else
                    if (inid.StartsWith("(72)"))
                    {
                        List<string> inventors = Regex.Split(inid.Replace("(72) Inventors", "").Trim(), @"([A-Z]{2,10})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        List<string> formatedInv = new();

                        if (inventors.Count % 2 == 0)
                        {
                            for (int i = 0; i < inventors.Count; i++)
                            {
                                formatedInv.Add(inventors[i] + inventors[++i]);
                            }
                        }
                        foreach (string inventor in formatedInv)
                        {
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+?),\s(?<country>.+)");

                            if (match.Success)
                            {
                                biblio.Inventors.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Country = MakeCountryCode(match.Groups["country"].Value.Trim())
                                });

                                if (MakeCountryCode(match.Groups["country"].Value.Trim()) == null) Console.WriteLine($" ------------------------------------------ {match.Groups["country"].Value.Trim()}");
                            }

                            else Console.WriteLine($"{inventor} --------------------- 72");
                        }
                    }
                    else
                    if (inid.StartsWith("(84)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @"[A-Z]{2}.+");

                        if (match.Success)
                        {
                            List<string> designatedStates = Regex.Split(match.Value.ToUpper().Trim(), @"\s").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            intConvention.DesignatedStates = designatedStates;

                        }
                    }
                    else
                    if (inid.StartsWith("(74)"))
                    {
                        Match match = Regex.Match(inid.Replace("(74) Representative", "").Trim(), @"(?<name>.+?),\s(?<country>.+)");

                        if (match.Success)
                        {
                            biblio.Agents.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Country = MakeCountryCode(match.Groups["country"].Value.Trim())
                            });

                            if (MakeCountryCode(match.Groups["country"].Value.Trim()) == null) Console.WriteLine($" ------------------------------------------ {match.Groups["country"].Value.Trim()}");
                        }
                        else Console.WriteLine($"{inid} --------------------- 74");
                    }
                    else
                    if (inid.StartsWith("(54)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @"le\s(?<title>.+)");

                        if (match.Success)
                        {
                            biblio.Titles.Add(new Title
                            {
                                Text = match.Groups["title"].Value.Trim(),
                                Language = "EN"
                            });
                        }
                    }
                    else
                    if (inid.StartsWith("(57)"))
                    {
                        biblio.Abstracts.Add(new Abstract
                        {
                            Language = "EN",
                            Text = inid.Replace("(57) Abstract", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith("(51)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @"on\s?:\s?(?<data>.+)");

                        if (match.Success)
                        {
                            List<string> ipcs = Regex.Split(match.Groups["data"].Value.Trim(), @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string ipc in ipcs)
                            {
                                Match match1 = Regex.Match(ipc.Trim(), @"(?<edi>.+)\s\((?<date>.+)\)");

                                if (match1.Success)
                                {
                                    biblio.Ipcs.Add(new Ipc
                                    {
                                        Class = match1.Groups["edi"].Value.Trim(),
                                        Date = match1.Groups["date"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{ipc} ----------------- ipc in 51");
                            }
                        }

                        else Console.WriteLine($"{inid} ---------------- 51");
                    }
                    else Console.WriteLine($"{inid}  - don't process");

                }

                biblio.IntConvention = intConvention;
                biblio.DOfPublication = dOfPublication;
                biblio.Priorities.Add(priority);
                legal.Biblio = biblio;
            }
            else
            if(subCode == "7")
            {
                Match match = Regex.Match(note.Trim(), @"(?<pNum>AP\s?\d{4,5})\s(?<aNum>.+)\s(?<eventDate>\d{2}\s?\.\s?\d{2}\s?\.\s?\d{4})\s(?<legalDate>\d{2}\s?\.\s?\d{2}\s?\.\s?\d{4})\s(?<note>.+)");

                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["pNum"].Value.Trim();

                    biblio.Application.Number = match.Groups["aNum"].Value.Replace(" ","").Trim();

                    legalEvent.Date = DateTime.Parse(match.Groups["eventDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".","/").Trim();

                    legalEvent.Note = "|| Valid Until | " + DateTime.Parse(match.Groups["legalDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n"
                        + "|| Anniversary | " + match.Groups["note"].Value.Trim();

                    legal.Biblio = biblio;
                    legal.LegalEvent = legalEvent;
                }
                else Console.WriteLine($"{note}");
            }
            else
            if(subCode == "10")
            {
                Match match = Regex.Match(note.Trim(), @"(?<aNum>.+)\s(?<eventDate>\d{2}\s?\.\s?\d{2}\s?\.\s?\d{4})\s(?<legalDate>\d{2}\s?\.\s?\d{2}\s?\.\s?\d{4})\s(?<note>.+yr)");

                if (match.Success)
                {
                    biblio.Application.Number = match.Groups["aNum"].Value.Replace(" ", "").Trim();

                    legalEvent.Date = DateTime.Parse(match.Groups["eventDate"].Value.Replace(" ", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    legalEvent.Note = "|| Valid Until | " + DateTime.Parse(match.Groups["legalDate"].Value.Replace(" ", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n"
                        + "|| Anniversary | " + match.Groups["note"].Value.Trim();

                    legal.Biblio = biblio;
                    legal.LegalEvent = legalEvent;
                }
                else Console.WriteLine($"{note}");
            }
            else
            if(subCode == "20")
            {
                foreach (string inid in MakeInids(note))
            {
                if (inid.StartsWith(I21))
                {
                    biblio.Application.Number = inid.Replace(I21, "").Trim();
                }
                else
                if (inid.StartsWith(I23))
                {
                    biblio.Application.OtherDate = DateTime.Parse(inid.Replace(I23, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                }
                else
                if (inid.StartsWith(I54))
                {
                    biblio.Titles = new()
                    {
                        new Title
                        {
                            Language = "EN",
                            Text = inid.Replace(I54, "").Trim()
                        }
                    };
                }
                else
                if (inid.StartsWith(I71))
                {
                    biblio.Applicants = new();

                    List<string> temp = Regex.Split(inid.Replace(I71, "").Trim(), ",").Where(val => !string.IsNullOrEmpty(val))
                        .Where(val => !val.StartsWith(" et al") && !val.StartsWith("et al"))
                        .ToList();

                    List<string> applicants = new();

                    foreach (string tmp in temp)
                    {
                        if (tmp.Contains("and "))
                        {
                            Match match = Regex.Match(tmp.Trim(), @"(?<name1>.+)\s?and\s?(?<name2>.+)");

                            if (match.Success)
                            {
                                applicants.Add(match.Groups["name1"].Value.Trim());
                                applicants.Add(match.Groups["name2"].Value.Trim());
                            }
                            else
                            {
                                applicants.Add(tmp.Replace("and", "").Trim());
                            }

                        }
                        else applicants.Add(tmp);
                    }

                    foreach (string applicant in applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant.Trim()
                        });
                    }
                }
                else
                if (inid.StartsWith(I72))
                {
                    biblio.Inventors = new();

                    List<string> temp = Regex.Split(inid.Replace(I72, "").Trim(), ",").Where(val => !string.IsNullOrEmpty(val))
                        .Where(val => !val.StartsWith(" et al") && !val.StartsWith("et al"))
                        .ToList();

                    List<string> inventors = new();

                    foreach (string tmp in temp)
                    {
                        if (tmp.Contains("and "))
                        {
                            Match match = Regex.Match(tmp.Trim(), @"(?<name1>.+)\s?and\s?(?<name2>.+)");

                            if (match.Success)
                            {
                                inventors.Add(match.Groups["name1"].Value.Trim());
                                inventors.Add(match.Groups["name2"].Value.Trim());
                            }
                            else
                            {
                                inventors.Add(tmp.Replace("and", "").Trim());
                            }
                        }
                        else inventors.Add(tmp);
                    }

                    foreach (string inventor in inventors)
                    {
                        biblio.Inventors.Add(new PartyMember
                        {
                            Name = inventor.Trim()
                        });
                    }
                }
                else
                if (inid.StartsWith(I74))
                {
                    biblio.Agents = new()
                    {
                        new PartyMember
                        {
                            Name = inid.Replace(I74, "").Trim()
                        }
                    };                   
                }
                else
                if (inid.StartsWith(I84))
                {
                    biblio.IntConvention = new();

                    biblio.IntConvention.DesignatedStates = new();

                    List<string> designatedStates = Regex.Split(inid.Replace(I84, "").Trim(), ",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string designatedState in designatedStates)
                    {
                        biblio.IntConvention.DesignatedStates.Add(designatedState.Trim());
                    }
                }
                else
                if (inid.StartsWith(I51))
                {
                    biblio.Ipcs = new();

                    List<string> ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string ipc in ipcs)
                    {
                        Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s?\((?<date>.+)\)");
                        if (match.Success)
                        {
                            biblio.Ipcs.Add(new Ipc
                            {
                                Class = match.Groups["class"].Value.Trim(),
                                Date = match.Groups["date"].Value.Trim()
                            });
                        }
                    }
                }
                else
                if (inid.StartsWith(I75))
                {
                    biblio.InvOrApps = new();

                    List<string> temp = Regex.Split(inid.Replace(I75, "").Trim(), ",").Where(val => !string.IsNullOrEmpty(val))
                        .Where(val => !val.StartsWith(" et al") && !val.StartsWith("et al"))
                        .ToList();

                    List<string> invOrApps = new();

                    foreach (string tmp in temp)
                    {
                        if (tmp.Contains("and "))
                        {
                            Match match = Regex.Match(tmp.Trim(), @"(?<name1>.+)\s?and\s?(?<name2>.+)");

                            if (match.Success)
                            {
                                invOrApps.Add(match.Groups["name1"].Value.Trim());
                                invOrApps.Add(match.Groups["name2"].Value.Trim());
                            }
                            else Console.WriteLine($"{tmp}  72field");
                        }
                        else invOrApps.Add(tmp);
                    }
                    foreach (string invOrApp in invOrApps)
                    {
                        biblio.InvOrApps.Add(new PartyMember
                        {
                            Name = invOrApp.Trim()
                        });
                    }
                }
                else Console.WriteLine($"{inid}");    
            }

                Match date = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")), @"[0-9]{8}");

                if (date.Success)
                {
                    legal.LegalEvent = new LegalEvent()
                    {
                        Date = date.Value.Insert(4, "/").Insert(7, "/").Trim()
                    };
                }

                legal.Biblio = biblio;
            }

            return legal;
        }




        internal string MakeCountryCode(string country) => country switch
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
            "Belg ium" => "BE",
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
            "Peoples Republic of China" => "CN",
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
            "The Netherlands" => "NL",
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
            "United Ki ngdom" => "GB",
            "Uni ted Kingdom" => "GB",
            "United States Minor Outlying Islands" => "UM",
            "United States of America" => "US",
            "United Stites of America" => "US",
            "Un ited States of America" => "US",
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
            "Zi mbabwe" => "ZW",
            "Åland Islands" => "AX",
            _ => null
        };
        internal List<string> MakeInids(string note) => Regex.Split(note.Trim(), @"(?=\(\s?[0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
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