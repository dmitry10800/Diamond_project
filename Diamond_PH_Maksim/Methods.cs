﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Integration;

namespace Diamond_PH_Maksim
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
                if(subCode == "5")
                {
                    //xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                    //      .SkipWhile(val => !val.Value.StartsWith("LAPSED PATENTS"))
                    //      //.TakeWhile(val => !val.Value.StartsWith("INTELLECTUAL PROPERTY PHILIPPINES"))
                    //      .ToList();

                    StreamReader streamReader = new(_currentFileName.Replace(".tetml", ".txt"));

                    foreach (var note in Regex.Split(streamReader.ReadToEnd().Replace("\r", "").Replace("\n", " ").Trim(), @"(?=\d\/\d{4}\/\d+)")
                        .Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("1/")).ToList())
                    {
                        statusEvents.Add(SplitNote(note, subCode, "MM"));
                    }
                }
                else if (subCode is "12")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("1 INVENTIONS"))
                        .TakeWhile(val => !val.Value.StartsWith("RECORDALS OF CHANGE OF NAME OF "))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\d\/\d{4}\/)", RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d\/\d{4}\/").Match(val).Success).ToList();

                    foreach(var note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "KA"));
                    }
                }
                else if(subCode == "22")
                {
                    //xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                    //      .SkipWhile(val => !val.Value.StartsWith("1.1 FORFEITED INVENTION APPLICATIONS"))
                    //      .TakeWhile(val => !val.Value.StartsWith("INTELLECTUAL PROPERTY PHILIPPINES"))
                    //      .ToList();

                    StreamReader streamReader = new(_currentFileName.Replace(".tetml", ".txt"));

                    var notes = Regex.Split(streamReader.ReadToEnd().Replace("\r","").Replace("\n"," ").Trim(), @"(?=\d\/\d{4}\/\d{6,7})")
                        .Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith(@"1/")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "FA"));
                    }
                }
                else if(subCode == "29")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                         .SkipWhile(val => !val.Value.StartsWith("1.1 PUBLICATION OF INVENTION APPLICATIONS (PCT"))
                         .TakeWhile(val => !val.Value.StartsWith("1.1 PUBLICATION OF DIVISIONAL INVENTION APPLICATIONS (PCT)"))
                         .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=[0-9]{1,3}\sPCT\/.+)", RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d{1,3}\sPCT\/.+").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "BA"));
                    }
                }
                else if (subCode == "35")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                          .SkipWhile(val => !val.Value.StartsWith("REGISTERED UTILITY MODELS"))
                          .TakeWhile(x => !x.Value.StartsWith("1 INVENTIONS"))
                          .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\d\/\d{4}\/\d{6})")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("2")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "FG"));
                    }
                }
                else if (subCode is "7")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("1.1 EXPIRED UTILITY MODELS ORDER NO. 2022-2 Under RA 8293"))
                       // .TakeWhile(val => !val.Value.StartsWith("1.1 PUBLICATION OF DIVISIONAL INVENTION APPLICATIONS (PCT)"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\d\/\d{4}\/\d{6,7})", RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("2/")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "MK"));
                    }
                }
            }
            return statusEvents;
        }
        internal Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                Id = _id++,
                SectionCode = sectionCode,
                SubCode = subCode,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "PH",
                Biblio = new()
                {
                    DOfPublication = new()
                },
                LegalEvent = new()
            };

            CultureInfo culture = new("ru-Ru");

            if (subCode == "5")
            {
                var match = Regex.Match(note.Trim(), @"(?<aNum>\d+\/\d+\/\d+).+?(?<owner>[A-Z].+)\s(?<d43>\d{2}\/\d{2}\/\d{4})\s(?<title>([A-Z]|\d)\D+)");

             
                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                    foreach (var owner in Regex.Split(match.Groups["owner"].Value.Trim(), @";|\sand\s").Where(val => !string.IsNullOrEmpty(val)).ToList())
                    {
                        var match1 = Regex.Match(owner.Trim(), @"(?<name>\D+)\s\[(?<code>[A-Z]{2})");
                        if (match1.Success)
                        {
                            statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                            {
                                Name = match1.Groups["name"].Value.Trim(),
                                Country = match1.Groups["code"].Value.Trim()
                            });
                        }
                        else
                        {
                            statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                            {
                                Name = owner.Trim()
                            });
                        }
                    }

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "EN",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Groups["d43"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    var match2 = Regex.Match(_currentFileName.Replace(".txt", ""), @"\d{8}");

                    if (match2.Success)
                    {
                        statusEvent.LegalEvent.Date = match2.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }
                }
                else Console.WriteLine($"{note}");
            }
            if(subCode == "22")
            {
                var match = Regex.Match(note.Replace("\r","").Replace("\n", " ").Trim(), @"(?<appNum>\d\/\d{4}\/\d{6,7})\s?(?<day>\d+)\s(?<month>.+)\s(?<year>\d{4})\s?(?<title>.+)\s?\[(?<code>\D{2})");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                    statusEvent.Biblio.Application.Date = match.Groups["year"].Value.Trim() + "-" + MakeMonth( match.Groups["month"].Value.Replace(" ","").ToLower().Trim()) + "-" + match.Groups["day"].Value.Trim();

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "EN",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                    {
                        Country = match.Groups["code"].Value.Trim()
                    });

                    var match1 = Regex.Match(_currentFileName.Replace(".tetml", "").Trim(), @"\d{8}");

                    if (match1.Success)
                    {
                        statusEvent.LegalEvent.Date = match1.Value.Insert(4, @"-").Insert(7, @"-").Trim();
                    }

                }
                else Console.WriteLine($"{note} - wrong match");
            }
            if(subCode == "29")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<PCTAppNum>PCT.+?)\s(?<appNum>.+)\s(?<PCTdate>\d{2}\/\d{2}\/\d{4})\s(?<title>.+?)\s(?<ipcs>[A-Z]\d.+)");

                if (match.Success)
                {
                    statusEvent.Biblio.IntConvention.PctApplNumber = match.Groups["PCTAppNum"].Value.Trim();
                    statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["PCTdate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Replace(" ","").Trim();

                    var ipcs = Regex.Split(match.Groups["ipcs"].Value.Trim(), @";").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"[A-Z]\d{2}[A-Z]\s\d{1,4}\/\d+").Match(val).Success).ToList();

                    foreach (var ipc in ipcs)
                    {
                        statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                        {
                            Class = ipc.Trim()
                        });
                    }

                    var applicants = Regex.Split(match.Groups["title"].Value.Trim(), @"(?<=])").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    for (var i = 0; i < applicants.Count; i++)
                    {
                        if (i == 0)
                        {
                            var match1 = Regex.Match(applicants[i].Trim(), @"(?<title>.+)\s\[(?<code>\D{2})");

                            if (match1.Success)
                            {
                                statusEvent.Biblio.Titles.Add(new Integration.Title
                                {
                                    Language = "EN",
                                    Text = match1.Groups["title"].Value.Trim()
                                });

                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Country = match1.Groups["code"].Value.Trim()
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Titles.Add(new Integration.Title
                                {
                                    Language = "EN",
                                    Text = applicants[i].Trim()
                                });
                            }
                        }
                        else
                        {
                            var match1 = Regex.Match(applicants[i].Replace("and", "").Replace(";", "").Replace(",", "").Trim(), @"(?<name>.+)\s\[(?<code>\D{2})");

                            if (match1.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Country = match1.Groups["code"].Value.Trim(),
                                    Name = match1.Groups["name"].Value.Trim()
                                });
                            }
                        }

                    }

                }
                else {
                    var match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<PCTAppNum>PCT.+?)\s(?<appNum>.+)\s(?<PCTdate>\d{2}\/\d{2}\/\d{4})\s(?<title>.+)");
                    if (match1.Success)
                    {
                        statusEvent.Biblio.IntConvention.PctApplNumber = match1.Groups["PCTAppNum"].Value.Trim();
                        statusEvent.Biblio.Application.Date = DateTime.Parse(match1.Groups["PCTdate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        statusEvent.Biblio.Application.Number = match1.Groups["appNum"].Value.Replace(" ", "").Trim();

                        var applicants = Regex.Split(match1.Groups["title"].Value.Trim(), @"(?<=])").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        for (var i = 0; i < applicants.Count; i++)
                        {
                            if (i == 0)
                            {
                                var match2 = Regex.Match(applicants[i].Trim(), @"(?<title>.+)\s\[(?<code>\D{2})");

                                if (match2.Success)
                                {
                                    statusEvent.Biblio.Titles.Add(new Integration.Title
                                    {
                                        Language = "EN",
                                        Text = match2.Groups["title"].Value.Trim()
                                    });

                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                    {
                                        Country = match2.Groups["code"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    statusEvent.Biblio.Titles.Add(new Integration.Title
                                    {
                                        Language = "EN",
                                        Text = applicants[i].Trim()
                                    });
                                }
                            }
                            else
                            {
                                var match2 = Regex.Match(applicants[i].Replace("and", "").Replace(";", "").Replace(",", "").Trim(), @"(?<name>.+)\s\[(?<code>\D{2})");

                                if (match2.Success)
                                {
                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                    {
                                        Country = match2.Groups["code"].Value.Trim(),
                                        Name = match2.Groups["name"].Value.Trim()
                                    });
                                }
                            }

                        }
                    }
                    else Console.WriteLine($"{note}");
                }
            }
            if(subCode == "35")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<num>.+?)\s(?<assigner>.+)[\]|\)]\s(?<title>.+)\s(?<appDate>\d{2}.\d{2}.\d{4})");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["num"].Value.Trim();
                    statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["appDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "EN",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    var match1 = Regex.Match(match.Groups["assigner"].Value.Trim(), @"(?<name>.+)\s[\[|\(](?<code>[A-Z]{2})");

                    if (match1.Success)
                    {
                        statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                        {
                            Name = match1.Groups["name"].Value.Trim(),
                            Country = match1.Groups["code"].Value.Trim()
                        });
                    }
                    else Console.WriteLine($"{match.Groups["assigner"].Value.Trim()}");
                }
                else
                {
                    Console.WriteLine($"{note}");
                }
            }
            if (subCode is "12")
            {
                var checkMatch = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"\[(?<code>[A-Z]{2})\]");

                if (checkMatch.Groups["code"].Value.Trim() is "PH")
                {
                    var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                        @"(?<appNum>\d\/\d{4}.+)\s?(?<appDate>\d{2}.\d{2}.\d{4})\s?(?<applicant>.+)\s?(?<pubDate>\d{2}.\d{2}.\d{4})\s?(?<inid45>\d{2}.\d{2}.\d{4})\s?(?<title>.+)\s(?<leNote>\d+th.+)\s\d");

                    if (match.Success)
                    {
                        statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();

                        statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["appDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        var applicantsList = Regex.Split(match.Groups["applicant"].Value.Trim(), @";|\]\sAND").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicantsList)
                        {
                            var appl = Regex.Match(applicant.Trim(), @"(?<name>.+)\s\[(?<code>[A-Z]{2})");

                            if (appl.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = appl.Groups["name"].Value.Trim(),
                                    Country = appl.Groups["code"].Value.Trim()
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = applicant.Trim()
                                });
                            }
                        }

                        statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Groups["pubDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        statusEvent.Biblio.DOfPublication.date_45 = DateTime.Parse(match.Groups["inid45"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "EN",
                            Text = match.Groups["title"].Value.Trim()
                        });

                        statusEvent.LegalEvent.Note = "|| ANNUITY DUE | " + match.Groups["leNote"].Value.Trim();
                        statusEvent.LegalEvent.Language = "EN";

                        var leDate = Regex.Match(_currentFileName.Replace(".txt", ""), @"\d{8}");

                        if (leDate.Success)
                        {
                            statusEvent.LegalEvent.Date = leDate.Value.Insert(4, "/").Insert(7, "/").Trim();
                        }
                    }
                    else
                    {
                        Console.WriteLine("-------------");
                        Console.WriteLine($"{note}");
                    }
                }
                else
                {
                    var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                        @"(?<appNum>\d\/\d{4}.+)\s?(?<natDate>\d{2}.\d{2}.\d{4})\s?(?<applicant>.+)\s?(?<PCTpubDate>\d{2}.\d{2}.\d{4})\s?(?<inid45>\d{2}.\d{2}.\d{4})\s?(?<title>.+)\s(?<leNote>\d+th.+)\s\d");

                    if (match.Success)
                    {
                        statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();

                        statusEvent.Biblio.IntConvention.PctNationalDate = DateTime.Parse(match.Groups["natDate"].Value.Trim(),culture).ToString("yyyy.MM.dd").Replace(".","/").Trim();

                        var applicantsList = Regex.Split(match.Groups["applicant"].Value.Trim(), @";|\]\sAND").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicantsList)
                        {
                            var appl = Regex.Match(applicant.Trim(), @"(?<name>.+)\s\[(?<code>[A-Z]{2})");

                            if (appl.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = appl.Groups["name"].Value.Trim(),
                                    Country = appl.Groups["code"].Value.Trim()
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = applicant.Trim()
                                });
                            }
                        }

                        statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["PCTpubDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        statusEvent.Biblio.DOfPublication.date_45 = DateTime.Parse(match.Groups["inid45"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "EN",
                            Text = match.Groups["title"].Value.Trim()
                        });

                        statusEvent.LegalEvent.Note = "|| ANNUITY DUE | " + match.Groups["leNote"].Value.Trim();
                        statusEvent.LegalEvent.Language = "EN";

                        var leDate = Regex.Match(_currentFileName.Replace(".txt", ""), @"\d{8}");

                        if (leDate.Success)
                        {
                            statusEvent.LegalEvent.Date = leDate.Value.Insert(4, "/").Insert(7, "/").Trim();
                        }
                    }
                    else
                    {
                        var match2 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                            @"(?<appNum>\d\/\d{4}.+)\s?(?<natDate>\d{2}.\d{2}.\d{4})\s?(?<applicant>.+)\s?(?<PCTpubDate>\d{2}.\d{2}.\d{4})\s?(?<inid45>\d{2}.\d{2}.\d{4})\s?(?<title>.+)\d");

                        if (match2.Success)
                        {
                            statusEvent.Biblio.Application.Number = match2.Groups["appNum"].Value.Trim();

                            statusEvent.Biblio.IntConvention.PctNationalDate = DateTime.Parse(match2.Groups["natDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            var applicantsList = Regex.Split(match2.Groups["applicant"].Value.Trim(), @";|\]\sAND").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var applicant in applicantsList)
                            {
                                var appl = Regex.Match(applicant.Trim(), @"(?<name>.+)\s\[(?<code>[A-Z]{2})");

                                if (appl.Success)
                                {
                                    statusEvent.Biblio.Applicants.Add(new PartyMember()
                                    {
                                        Name = appl.Groups["name"].Value.Trim(),
                                        Country = appl.Groups["code"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    statusEvent.Biblio.Applicants.Add(new PartyMember()
                                    {
                                        Name = applicant.Trim()
                                    });
                                }
                            }

                            statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match2.Groups["PCTpubDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            statusEvent.Biblio.DOfPublication.date_45 = DateTime.Parse(match2.Groups["inid45"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "EN",
                                Text = match2.Groups["title"].Value.Trim()
                            });

                            statusEvent.LegalEvent.Language = "EN";

                            var leDate = Regex.Match(_currentFileName.Replace(".txt", ""), @"\d{8}");

                            if (leDate.Success)
                            {
                                statusEvent.LegalEvent.Date = leDate.Value.Insert(4, "/").Insert(7, "/").Trim();
                            }
                        }
                        else
                        {
                            Console.WriteLine("-------------");
                            Console.WriteLine($"{note}");
                        }
                    }
                }
            }
            if (subCode is "7")
            {

            }
            
            return statusEvent;
        }

        internal string MakeMonth(string month) => month switch
        {
            "january" => "01",
            "february" => "02",
            "march" => "03",
            "april" => "04",
            "may" => "05",
            "june" => "06",
            "july" => "07",
            "august" => "08",
            "september" => "09",
            "october" => "10",
            "november" => "11",
            "december" => "12",
            _ => null
        }; 
        internal string MakeText(List<XElement> xElements)
        {
            var text = string.Empty;
            
                foreach (var xElement in xElements)
                {
                    text += xElement.Value + " ";
                }
            return text;
        }
    }
}
