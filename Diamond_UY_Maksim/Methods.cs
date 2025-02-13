using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_UY_Maksim
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

                if(subCode == "8")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("RESOLUCIONES DE PATENTES, ABANDONADAS"))
                             .TakeWhile(val => !val.Value.StartsWith("RESOLUCIONES DE MODELOS DE UTILIDAD, ABANDONADAS"))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=No\.\s)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("N")).ToList();

                     foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FA"));
                    }
                }
            }

            return statusEvents;
        }


        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "UY",
                SectionCode = sectionCode,
                SubCode = subCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            var culture = new CultureInfo("ru-RU");
            if(subCode == "8")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"\.\s?(?<appNum>\d+)\s(?<leDate>\d{2}\/\d{2}\/\d{4})\s(?<title>.+)\.\s(?<assigner>.+)\s(?<agent>\d+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["leDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "ES",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                    {
                        Name = match.Groups["assigner"].Value.Trim()
                    });

                    statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                    {
                        Name = match.Groups["agent"].Value.Trim()
                    });

                    statusEvent.LegalEvent.Note = "|| Agent number | " + match.Groups["agent"].Value.Trim();
                    statusEvent.LegalEvent.Language = "EN";
                }
                else
                {
                    var match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"\.\s?(?<appNum>\d+)\s(?<leDate>\d{2}\/\d{2}\/\d{4})\s(?<title>.+?)\s(?<assigner>[A-Z][a-z].+)\s(?<agent>\d+)");

                    if (match1.Success)
                    {
                        statusEvent.Biblio.Application.Number = match1.Groups["appNum"].Value.Trim();
                        statusEvent.LegalEvent.Date = DateTime.Parse(match1.Groups["leDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "ES",
                            Text = match1.Groups["title"].Value.Trim()
                        });

                        statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                        {
                            Name = match1.Groups["assigner"].Value.Trim()
                        });

                        statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                        {
                            Name = match1.Groups["agent"].Value.Trim()
                        });

                        statusEvent.LegalEvent.Note = "|| Agent number | " + match1.Groups["agent"].Value.Trim();
                        statusEvent.LegalEvent.Language = "EN";
                    }
                    else 
                    {
                        var match2 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"\.\s?(?<appNum>\d+)\s(?<leDate>\d{2}\/\d{2}\/\d{4})\s(?<title>.+?)\s(?<agent>\d+)");

                        if (match2.Success)
                        {
                            statusEvent.Biblio.Application.Number = match2.Groups["appNum"].Value.Trim();
                            statusEvent.LegalEvent.Date = DateTime.Parse(match2.Groups["leDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "ES",
                                Text = match2.Groups["title"].Value.Trim()
                            });

                            statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                            {
                                Name = match2.Groups["agent"].Value.Trim()
                            });

                            statusEvent.LegalEvent.Note = "|| Agent number | " + match2.Groups["agent"].Value.Trim();
                            statusEvent.LegalEvent.Language = "EN";
                        }
                        else {
                            var match3 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"\.\s?(?<appNum>\d+)\s(?<leDate>\d{2}\/\d{2}\/\d{4})\s(?<title>.+)");

                            if (match3.Success)
                            {
                                statusEvent.Biblio.Application.Number = match3.Groups["appNum"].Value.Trim();
                                statusEvent.LegalEvent.Date = DateTime.Parse(match3.Groups["leDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                                statusEvent.Biblio.Titles.Add(new Integration.Title
                                {
                                    Language = "ES",
                                    Text = match3.Groups["title"].Value.Trim()
                                });
                            }
                            else Console.WriteLine(note.Replace("\r", "").Replace("\n", " ").Trim());
                        } 
                    }
                    
                } 
            }

            return statusEvent;
        }

        internal string MakeText (List<XElement> xElements, string subCode)
        {
            var text = string.Empty;

            if(subCode == "8")
            {
                foreach (var xElement in xElements)
                {
                    text += xElement.Value + " ";
                }
            }
            return text;
        }
    }
}
