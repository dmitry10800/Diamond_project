﻿using Integration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_AL_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            var files = directory.GetFiles("*.tetml", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

            XElement tet;

            List<XElement> xElements = new();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                switch (subCode)
                {
                    case "3":
                        {
                            xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                                .SkipWhile(val => !val.Value.StartsWith("PATENTA TË LËSHUARA"))
                                .TakeWhile(val => !val.Value.StartsWith("NDRYSHIMI I ADRESËS SË"))
                                .ToList();

                            var notes = Regex.Split(MakeText(xElements), @"(?=\(11\)\s\d)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                            statusEvents.AddRange(notes.Select(note => MakePatent(note.Replace("Buletini i Pronësisë Industriale", ""), subCode, "FG")));
                            break;
                        }
                    case "17":
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("PATENTA/ MODELE PËRDORIMI TË PAVLEFSHME"))
                            .TakeWhile(val => !val.Value.StartsWith("NDRYSHIME NË ADRESËN E PRONARIT TË"))
                            .ToList();

                        var notes = Regex.Split(MakeText(xElements), @"(?=\(11\)\s\d)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                        statusEvents.AddRange(notes.Select(note => MakePatent(note, subCode, "MA")));
                        break;
                    }
                    case "19":
                        {
                            xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                                .SkipWhile(val => !val.Value.StartsWith("PATENTA TË SKADUARA"))
                                .ToList();

                            var notes = Regex.Split(MakeText(xElements), @"(?=\(\s11\s\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("( 11 )")).ToList();

                            statusEvents.AddRange(notes.Select(note => MakePatent(note, subCode, "MK")));
                            break;
                        }
                }
            }
            return statusEvents;
        }
        internal string MakeText(List<XElement> xElements)
        {
            return xElements.Aggregate<XElement, string>(null, (current, xElement) => current + (xElement.Value + "\n"));
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legal = new()
            {
                SectionCode = sectionCode,
                SubCode = subCode,
                Id = _id++,
                CountryCode = "AL",
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new Biblio
                {
                    DOfPublication = new DOfPublication(),
                    EuropeanPatents = new List<EuropeanPatent>()
                },
                LegalEvent = new LegalEvent()
            };

            var culture = new CultureInfo("ru-RU");
            var europeanPatent = new EuropeanPatent();

            switch (subCode)
            {
                case "3":
                    {
                        foreach (var inid in MakeInids(note, subCode))
                        {
                            if (inid.StartsWith("(11)"))
                            {
                                legal.Biblio.Publication.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(11)", "").Trim();
                            }
                            else if (inid.StartsWith("(97)"))
                            {
                                var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(97)", "").Trim(), @"(?<num>.+)\s?\/\s?(?<date>\d{2}.\d{2}.\d{4})");

                                if (match.Success)
                                {
                                    europeanPatent.PubNumber = match.Groups["num"].Value.Trim();
                                    europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                }
                                else Console.WriteLine($"{inid}");
                            }
                            else if (inid.StartsWith("(96)"))
                            {
                                var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(96)", "").Trim(), @"(?<num>.+)\s?\/\s?(?<date>\d{2}.\d{2}.\d{4})");

                                if (match.Success)
                                {
                                    europeanPatent.AppNumber = match.Groups["num"].Value.Trim();
                                    europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                }
                                else Console.WriteLine($"{inid}");
                            }
                            else if (inid.StartsWith("(22)"))
                            {
                                legal.Biblio.Application.Date = DateTime.Parse(inid.Replace("\r", "").Replace("\n", " ").Replace("(22)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else if (inid.StartsWith("(21)"))
                            {
                                legal.Biblio.Application.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(21)", "").Replace(" ", "").Trim();
                            }
                            else if (inid.StartsWith("(54)"))
                            {
                                var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(54)", "").Trim(), @"(?<title>.+)\s(?<date>\d{2}.\d{2}.\d{4})");

                                if (match.Success)
                                {
                                    legal.Biblio.Application.EffectiveDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                                    legal.Biblio.Titles.Add(new Title
                                    {
                                        Language = "SQ",
                                        Text = match.Groups["title"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{inid}");
                            }
                            else if (inid.StartsWith("(30)"))
                            {
                                var priorites = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(30)", "").Trim(), @";|and|,").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var priorite in priorites)
                                {
                                    var match = Regex.Match(priorite.Trim(), @"(?<num>.+)\s(?<date>\d{2}.\d{2}.\d{4})\s(?<code>\D{2})");

                                    if (match.Success)
                                    {
                                        legal.Biblio.Priorities.Add(new Priority
                                        {
                                            Country = match.Groups["code"].Value.Trim(),
                                            Number = match.Groups["num"].Value.Trim(),
                                            Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                        });
                                    }
                                    else
                                    {
                                        legal.Biblio.Priorities.Add(new Priority
                                        {
                                            Number = priorite
                                        });
                                    }
                                }
                            }
                            else if (inid.StartsWith("(71)"))
                            {
                                var applicants = Regex.Split(inid.Replace("(71)", "").Replace("/", "").Trim(), @"(?<=,\s\D{2}\n)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var applicant in applicants)
                                {
                                    var match = Regex.Match(applicant.Trim(), @"(?<name>.+)\n(?<adress>.+),\s(?<code>\D{2})");

                                    if (match.Success)
                                    {
                                        legal.Biblio.Applicants.Add(new PartyMember
                                        {
                                            Country = match.Groups["code"].Value.Trim(),
                                            Name = match.Groups["name"].Value.Trim(),
                                            Address1 = match.Groups["adress"].Value.Trim()
                                        });
                                    }
                                    else
                                    {
                                        var match2 = Regex.Match(applicant.Trim(), @"(?<name>.+)\n(?<adress>.+)");

                                        if (match2.Success)
                                        {
                                            legal.Biblio.Applicants.Add(new PartyMember
                                            {
                                                Name = match2.Groups["name"].Value.Trim(),
                                                Address1 = match2.Groups["adress"].Value.Trim()
                                            });
                                        }
                                        else Console.WriteLine($"{applicant} -- 71");
                                    }
                                }
                            }
                            else if (inid.StartsWith("(72)"))
                            {
                                var inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(72)", "").Replace("/", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                if (inventors.Count % 2 != 0)
                                {
                                    var middleIndex = inventors.Count / 2;
                                    var middleElementValue = inventors[middleIndex];

                                    var match = Regex.Match(middleElementValue.Trim(), @"(?<name>.+,.+),(?<adress>.+)", RegexOptions.Singleline);

                                    var namesInventors = inventors.GetRange(0, middleIndex);
                                    var adressInventors = inventors.GetRange(middleIndex + 1, inventors.Count - middleIndex - 1);

                                    if (match.Success)
                                    {
                                        adressInventors.Insert(0, match.Groups["adress"].Value.Trim());
                                        namesInventors.Insert(namesInventors.Count, match.Groups["name"].Value.Trim());
                                    }

                                    for (int i = 0; i < namesInventors.Count; i++)
                                    {
                                        legal.Biblio.Inventors.Add(new PartyMember
                                        {
                                            Name = namesInventors[i],
                                            Address1 = adressInventors[i]
                                        });
                                    }
                                }
                                else
                                {
                                    foreach (var inventor in inventors)
                                    {
                                        var match = Regex.Match(inventor.Trim(), @"(?<name>.+)\s\((?<adress>.+)\)");

                                        if (match.Success)
                                        {
                                            legal.Biblio.Inventors.Add(new PartyMember
                                            {
                                                Name = match.Groups["name"].Value.Trim(),
                                                Address1 = match.Groups["adress"].Value.Trim()
                                            });
                                        }
                                        else Console.WriteLine($"{inventor} -- 72");
                                    }
                                }
                            }
                            else if (inid.StartsWith("(74)"))
                            {
                                var match = Regex.Match(inid.Replace("(74)", "").Replace("/", "").Trim(), @"(?<name>.+)\n(?<adress>.+)");

                                if (match.Success)
                                {
                                    legal.Biblio.Agents.Add(new PartyMember
                                    {
                                        Name = match.Groups["name"].Value.Trim(),
                                        Address1 = match.Groups["adress"].Value.Trim(),
                                        Country = "AL"
                                    });
                                }
                            }
                            else if (inid.StartsWith("(57)"))
                            {
                                var count = 1;

                                var claims = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(57)", "").Trim(), @"(?>\d{1,3}\.\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var claim in claims)
                                {
                                    legal.Biblio.Claims.Add(new DiamondProjectClasses.Claim
                                    {
                                        Language = "SQ",
                                        Text = claim.Trim(),
                                        Number = count.ToString()
                                    });

                                    count++;
                                }
                            }
                            else Console.WriteLine($"{inid} - not process");
                        }
                        var match11 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                        if (match11.Success)
                        {
                            legal.Biblio.DOfPublication.date_45 = match11.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                        }

                        legal.Biblio.EuropeanPatents.Add(europeanPatent);
                        break;
                    }
                case "17":
                {
                    foreach (var inid in MakeInids(note, subCode))
                    {
                        if (inid.StartsWith("(11)"))
                        {
                            legal.Biblio.Publication.Number = inid.Replace("(11)","").Trim();
                        }
                        else if (inid.StartsWith("(97)"))
                        {
                            var match = Regex.Match(inid.Replace("(97)", "").Trim(),
                                @"(?<num>.+)//(?<date>.+)");

                            if (match.Success)
                            {
                                europeanPatent.PubNumber = match.Groups["num"].Value.Trim();
                                europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else
                            {
                                var match2 = Regex.Match(inid.Replace("(97)", "").Trim(),
                                    @"(?<num>.+)/(?<date>.+)");

                                if (match2.Success)
                                {
                                    europeanPatent.PubNumber = match2.Groups["num"].Value.Trim();
                                    europeanPatent.PubDate = DateTime.Parse(match2.Groups["date"].Value.Trim(), culture)
                                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                }
                                else Console.WriteLine($"{inid} -- 97");
                            }
                        }
                        else if (inid.StartsWith("(96)"))
                        {
                            var match = Regex.Match(inid.Replace("(96)", "").Trim(),
                                @"(?<num>.+)//(?<date>.+)");

                            if (match.Success)
                            {
                                europeanPatent.AppNumber = match.Groups["num"].Value.Trim();
                                europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else
                            {
                                var match2 = Regex.Match(inid.Replace("(96)", "").Trim(),
                                    @"(?<num>.+)/(?<date>.+)");

                                if (match2.Success)
                                {
                                    europeanPatent.AppNumber = match2.Groups["num"].Value.Trim();
                                    europeanPatent.AppDate = DateTime.Parse(match2.Groups["date"].Value.Trim(), culture)
                                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                }
                                else Console.WriteLine($"{inid} -- 96");
                            }
                        }
                        else if (inid.StartsWith("(21)"))
                        {
                            legal.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                        }
                        else if (inid.StartsWith("(22)"))
                        {
                            legal.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else if (inid.StartsWith("(54)"))
                        {
                            legal.Biblio.Titles.Add(new Title()
                            {
                                Language = "SQ",
                                Text = inid.Replace("(54)", "").Replace("\r", "").Replace("\n", " ").Trim()
                            });
                        }
                        else if (inid.StartsWith("(73)"))
                        {
                            var assigneesList = Regex.Split(inid.Replace("(73)", "").Replace("\r", "").Replace("\n", " ").Trim(), 
                                @";").Where(a => !string.IsNullOrEmpty(a)).ToList();

                            foreach (var item in assigneesList)
                            {
                                var match = Regex.Match(item, @"(?<name>.+)//(?<adress>.+)/\s(?<code>\D{2}).+");

                                if (match.Success)
                                {
                                    legal.Biblio.Assignees.Add(new PartyMember()
                                    {
                                        Name = match.Groups["name"].Value.Trim(),
                                        Address1 = match.Groups["adress"].Value.Trim(),
                                        Country = match.Groups["code"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    var match2 = Regex.Match(item, @"(?<name>.+)//(?<adress>.+)(?<code>[A-Z]{2}).?");
                                    if (match2.Success)
                                    {
                                        legal.Biblio.Assignees.Add(new PartyMember()
                                        {
                                            Name = match2.Groups["name"].Value.Trim(),
                                            Address1 = match2.Groups["adress"].Value.Trim(),
                                            Country = match2.Groups["code"].Value.Trim()
                                        });
                                    }
                                    else
                                    {
                                        var match3 = Regex.Match(item, @"(?<name>.+)/(?<adress>.+)(?<code>[A-Z]{2}).?");
                                        if (match3.Success)
                                        {
                                            legal.Biblio.Assignees.Add(new PartyMember()
                                            {
                                                Name = match3.Groups["name"].Value.Trim(),
                                                Address1 = match3.Groups["adress"].Value.Trim(),
                                                Country = match3.Groups["code"].Value.Trim()
                                            });
                                        }
                                        else Console.WriteLine($"{item} --- 73");
                                    }
                                }
                            }
                        }
                        else if (inid.StartsWith("(74)"))
                        {
                            var match = Regex.Match(inid.Replace("(74)", "").Replace("\r", "").Replace("\n", " ").Trim(),
                                @"(?<name>.+)//(?<adress>.+)");
                            if (match.Success)
                            {
                                legal.Biblio.Agents.Add(new PartyMember()
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                });
                            }
                            else Console.WriteLine($"{inid} --- 74");
                        }
                        else Console.WriteLine($"{inid}");
                    }

                    legal.Biblio.EuropeanPatents.Add(europeanPatent);
                    var date = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"[0-9]{8}");
                    if (date.Success)
                    {
                        legal.LegalEvent.Date = date.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }
                    break;
                }
                case "19":
                    {
                        foreach (var inid in MakeInids(note, subCode))
                        {
                            if (inid.StartsWith("( 11 )") || inid.StartsWith("(11 )") || inid.StartsWith("( 11)") || inid.StartsWith("(11)"))
                            {
                                var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<num>[0-9]{1,5})\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                                if (match.Success)
                                {
                                    legal.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                                    legal.Biblio.DOfPublication = new DOfPublication()
                                    {
                                        date_45 = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim()
                                    };
                                }
                                else
                                {
                                    var match1 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<num>[0-9]{1,5})");

                                    if (match1.Success)
                                    {
                                        legal.Biblio.Publication.Number = match1.Groups["num"].Value.Trim();
                                    }
                                    else Console.WriteLine($"{inid}  ================== 11");
                                }
                            }
                            else
                            if (inid.StartsWith("( 97 )") || inid.StartsWith("(97 )") || inid.StartsWith("( 97)") || inid.StartsWith("(97)"))
                            {
                                var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<num>.+)\/\s?(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                                if (match.Success)
                                {
                                    europeanPatent.PubNumber = match.Groups["num"].Value.Trim();
                                    europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                                }
                                else
                                {
                                    var match1 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<num>.+)\/");
                                    if (match1.Success)
                                    {
                                        europeanPatent.PubNumber = match1.Groups["num"].Value.Trim();
                                    }
                                    else Console.WriteLine($"{inid} ======== 97");
                                }
                            }
                            else
                            if (inid.StartsWith("( 96 )") || inid.StartsWith("(96 )") || inid.StartsWith("( 96)") || inid.StartsWith("(96)"))
                            {
                                var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<num>.+)\/\s?(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                                if (match.Success)
                                {
                                    europeanPatent.AppNumber = match.Groups["num"].Value.Trim();
                                    europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                                }
                                else
                                {
                                    var match1 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<num>.+)\/");
                                    if (match1.Success)
                                    {
                                        europeanPatent.AppNumber = match1.Groups["num"].Value.Trim();
                                    }
                                    else Console.WriteLine($"{inid} ======== 96");
                                }
                            }
                            else
                            if (inid.StartsWith("( 21 )") || inid.StartsWith("(21 )") || inid.StartsWith("( 21)") || inid.StartsWith("(21)"))
                            {
                                var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<num>.+)");

                                if (match.Success)
                                {
                                    legal.Biblio.Application.Number = match.Groups["num"].Value.Trim();
                                }
                                else Console.WriteLine($"{inid} =========== 21");
                            }
                            else
                            if (inid.StartsWith("( 22 )") || inid.StartsWith("(22 )") || inid.StartsWith("( 22)") || inid.StartsWith("(22)"))
                            {
                                var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)\s?(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                                if (match.Success)
                                {
                                    legal.Biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                                }
                                else Console.WriteLine($"{inid} ============= 22");
                            }
                            else
                            if (inid.StartsWith("( 54 )") || inid.StartsWith("(54 )") || inid.StartsWith("( 54)") || inid.StartsWith("(54)"))
                            {

                                var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)\s?(?<text>.+)");

                                if (match.Success)
                                {
                                    legal.Biblio.Titles = new()
                                {
                                    new Title
                                    {
                                        Language = "SQ",
                                        Text = match.Groups["text"].Value.Trim()
                                    }
                                };
                                }
                                else Console.WriteLine($"{inid} ================ 54");
                            }
                            else
                            if (inid.StartsWith("( 30 )") || inid.StartsWith("(30 )") || inid.StartsWith("( 30)") || inid.StartsWith("(30)"))
                            {
                                var priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("( 30)", "").Trim(), @"and").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                legal.Biblio.Priorities = new();

                                foreach (var priority in priorities)
                                {
                                    var match = Regex.Match(priority.Trim(), @"(?<num>.+)\s(?<date>[0-9]{2}.\s?[0-9]{2}.\s?[0-9]{4})\s(?<code>[A-Z]{2})");

                                    if (match.Success)
                                    {
                                        legal.Biblio.Priorities.Add(new Priority
                                        {
                                            Number = match.Groups["num"].Value.Trim(),
                                            Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim(),
                                            Country = match.Groups["code"].Value.Trim()
                                        });
                                    }
                                    else Console.WriteLine($"{priority} ================ 30");
                                }
                            }
                            else
                            if (inid.StartsWith("( 73 )") || inid.StartsWith("(73 )") || inid.StartsWith("( 73)") || inid.StartsWith("(73)"))
                            {
                                legal.Biblio.Assignees = new();

                                var match = Regex.Match(inid.Replace("( 73 )", "").Trim(), @"(?<name>.*)\n(?<adress>.*\n?.*\n?.*\n?.*)");
                                if (match.Success)
                                {
                                    var match1 = Regex.Match(match.Groups["adress"].Value.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<adres>.+)\s(?<code>[A-Z]{2}$)");
                                    if (match1.Success)
                                    {
                                        legal.Biblio.Assignees.Add(new PartyMember
                                        {
                                            Name = match.Groups["name"].Value.Trim(),
                                            Address1 = match1.Groups["adres"].Value.Trim(),
                                            Country = match1.Groups["code"].Value.Trim()
                                        });
                                    }
                                    else Console.WriteLine($"{inid} ========== 73/adress");
                                }
                                else Console.WriteLine($"{inid} ========== 73");
                            }
                            else
                            if (inid.StartsWith("( 72 )") || inid.StartsWith("(72 )") || inid.StartsWith("( 72)") || inid.StartsWith("(72)"))
                            {
                                legal.Biblio.Inventors = new();

                                var inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("( 72 )", "").Trim(), @"\)\s?;").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var inventor in inventors)
                                {
                                    var match = Regex.Match(inventor.Trim(), @"(?<name>.+?)\s?\((?<adress>.*)\)?");

                                    if (match.Success)
                                    {
                                        legal.Biblio.Inventors.Add(new PartyMember
                                        {
                                            Name = match.Groups["name"].Value.Trim(),
                                            Address1 = match.Groups["adress"].Value.TrimEnd(')').Trim()
                                        });
                                    }
                                    else Console.WriteLine($"{inventor} ============ 72");
                                }
                            }
                            else Console.WriteLine($"{inid} =========== не обработан");
                        }

                        var date = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"[0-9]{8}");
                        if (date.Success)
                        {
                            legal.LegalEvent.Date = date.Value.Insert(4, "/").Insert(7, "/").Trim();
                        }
                        legal.Biblio.EuropeanPatents.Add(europeanPatent);
                        break;
                    }
            }

            return legal;
        }
        internal List<string> MakeInids(string note, string subCode)
        {
            var inids = new List<string>();

            if (subCode == "3")
            {
                var match = Regex.Match(note.Trim(), @"(?<inids>.+?)\s(?<inid57>\(57.+)", RegexOptions.Singleline);

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["inids"].Value.Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                    inids.Add(match.Groups["inid57"].Value.Trim());
                }
                else Console.WriteLine($"{note}  --- not spliting");
            }
            else if (subCode == "19")
            {
                inids = Regex.Split(note.Trim(), @"(?=\(\s{0,2}[0-9]{2}\s{0,2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
            }
            else if (subCode == "17")
            {
                inids = Regex.Split(note.Trim(), @"(?=\(\d{2}\)\s)", RegexOptions.Singleline)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
            }
            return inids;
        }
    }
}
