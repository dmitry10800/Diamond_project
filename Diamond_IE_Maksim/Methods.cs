using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Integration;

namespace Diamond_IE_Maksim
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

                if(subCode == "52")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Patents Expired"))
                        .TakeWhile(val => !val.Value.StartsWith("Request for Grant of Supplementary Protection Certificate"))
                        .TakeWhile(val => !val.Value.StartsWith("Application for Restoration of Lapsed Patents – Section 37"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=^S?\d{6,})", RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val) && new Regex(@"^\d+.*").Match(val).Success)
                        .ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "MK"));
                    }
                }

                if (subCode == "1" || subCode == "6")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Patents Lapsed Through Non-Payment of Renewal Fees"))
                        .TakeWhile(val => !val.Value.StartsWith("Request for Grant of Supplementary Protection Certificate"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=^\d+.+|^S\d\d+.+)", RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    if (subCode == "1")
                    {
                        notes = notes.Where(_ =>new Regex(@"^\d+.+").Match(_).Success).ToList();
                    }

                    if (subCode == "6")
                    {
                        notes = notes.Where(_ => new Regex(@"^S\d\d+.+").Match(_).Success).ToList();
                    }

                    var sectionCode = subCode switch
                    {
                        "1" => "MM",
                        "6" => "MM",
                        _ => null
                    };

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, sectionCode));
                    }
                }
            }
            return statusEvents;
        }
        
        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                SectionCode = sectionCode,
                SubCode = subCode,
                CountryCode = "IE",
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            if(subCode == "52")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<pNum>\d+)\s(?<title>.+?)\.\s(?<assignee>.+)");
                if (match.Success)
                {
                    statusEvent.Biblio.Publication.Number = match.Groups["pNum"].Value.Trim();
                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "EN",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    foreach (var assignee in Regex.Split(match.Groups["assignee"].Value.Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList())
                    {
                        statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                        {
                            Name = assignee
                        });
                    }

                    var match1 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"\d{8}");

                    if (match1.Success)
                    {
                        statusEvent.LegalEvent.Date = match1.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }
                }
                else Console.WriteLine($"{note}");
            }

            if (subCode == "1")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<pubNum>\d+)\s(?<ipcr>.+\/\d{2})\.\s(?<title>.+)\.\s(?<assignee>.+)");
                if (match.Success)
                {
                    statusEvent.Biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();
                    statusEvent.Biblio.Titles.Add(new Title()
                    {
                        Text = match.Groups["title"].Value.Trim(),
                        Language = "EN"
                    });

                    var people = Regex.Split(match.Groups["assignee"].Value.Trim(), @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();
                    foreach (var person in people)
                    {
                        statusEvent.Biblio.Assignees.Add(new PartyMember()
                        {
                            Name = person
                        });
                    }

                    var ipcrMatch = Regex.Match(match.Groups["ipcr"].Value.Trim(), @".+\s\((?<version>\d+\.\d+)\)(?<classification>.+)");
                    if (ipcrMatch.Success)
                    {
                        var classification = Regex.Split(ipcrMatch.Groups["classification"].Value.Trim(), ";").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        foreach (var item in classification)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Ipc()
                            {
                                Class = item.Trim(),
                                Date = ipcrMatch.Groups["version"].Value.Trim()
                            });
                        }
                    }
                }
                else
                {
                    var matchSecond = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<pubNum>\d+)\s(?<title>.+)\.\s(?<assignee>.+)");
                    if (matchSecond.Success)
                    {
                        statusEvent.Biblio.Publication.Number = matchSecond.Groups["pubNum"].Value.Trim();
                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Text = matchSecond.Groups["title"].Value.Trim(),
                            Language = "EN"
                        });

                        var people = Regex.Split(matchSecond.Groups["assignee"].Value.Trim(), @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();
                        foreach (var person in people)
                        {
                            statusEvent.Biblio.Assignees.Add(new PartyMember()
                            {
                                Name = person
                            });
                        }
                    }
                    else Console.WriteLine(note);
                }

                var eventDateMatch = Regex.Match(statusEvent.GazetteName, @"\d{8}");
                if (eventDateMatch.Success)
                {
                    statusEvent.LegalEvent.Date = eventDateMatch.Value.Trim().Insert(4, "/").Insert(7, "/");
                }
            }

            if (subCode == "6")
            {
                var match = Regex.Match(note.Replace("\r","").Replace("\n"," ").Trim(), @"(?<pubNum>S\d+)\s(?<ipcr>.+\/\d{2})\.\s(?<title>.+)\.\s(?<assignee>.+)");
                if (match.Success)
                {
                    statusEvent.Biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();
                    statusEvent.Biblio.Titles.Add(new Title()
                    {
                        Text = match.Groups["title"].Value.Trim(),
                        Language = "EN"
                    });

                    var people = Regex.Split(match.Groups["assignee"].Value.Trim(), @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();
                    foreach (var person in people)
                    {
                        statusEvent.Biblio.Assignees.Add(new PartyMember()
                        {
                            Name = person
                        });
                    }

                    var ipcrMatch = Regex.Match(match.Groups["ipcr"].Value.Trim(), @".+\s\((?<version>\d+\.\d+)\)(?<classification>.+)");
                    if (ipcrMatch.Success)
                    {
                        var classification = Regex.Split(ipcrMatch.Groups["classification"].Value.Trim(), ";").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        foreach (var item in classification)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Ipc()
                            {
                                Class = item.Trim(),
                                Date = ipcrMatch.Groups["version"].Value.Trim()
                            });
                        }
                    }

                    var eventDateMatch = Regex.Match(statusEvent.GazetteName, @"\d{8}");
                    if (eventDateMatch.Success)
                    {
                        statusEvent.LegalEvent.Date = eventDateMatch.Value.Trim().Insert(4, "/").Insert(7, "/");
                    }
                }
            }
            return statusEvent;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

            foreach (var xElement in xElements)
            {
                text += xElement.Value + "\n";
            }

            return text;
        }
    }
}
