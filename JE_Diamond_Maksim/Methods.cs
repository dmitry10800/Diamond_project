﻿using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Integration;

namespace Diamond_JE_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalStatus = new();

            DirectoryInfo directory = new(path);

            var files = directory.GetFiles("*.tetml", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

            XElement tet;

            List<XElement> xElements = new();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Jersey Registration"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Jersey\s?Registration.+)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Jersey")).ToList();

                    foreach (var note in notes)
                    {
                       legalStatus.Add(MakePatent(note, subCode, "FG"));
                    }
                }
            }

            return legalStatus;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legal = new()
            {
                CountryCode = "JE",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new()
                {
                    DOfPublication = new ()
                },
                LegalEvent = new()
            };

            CultureInfo culture = new("ru-RU");

            if (subCode == "1")
            {
                var match = Regex.Match(note,
                    @"(?<kind>[A-Z]\s?\d+)\s?(?<UkReg>([A-Z]{1,2})?\s?\d+\s?[A-Z]?(\d+)?)\s?(?<firstReg>\d{2}\/\d{2}\/\d{4})\s?(?<field45>\d{2}\/\d{2}\/\d{4})\s?Invention(?<title>.+)\s?Agent(?<field74>.+)\s?Proprietor(?<field73>.+)\s?POA.+Remarks(?<note>.+)\s?Updates\s?(?<update>.+)");

                if (match.Success)
                {
                    legal.Biblio.Publication.Number = match.Groups["kind"].Value.Trim();

                    legal.Biblio.DOfPublication.date_45 = DateTime.Parse(match.Groups["field45"].Value.Trim(), culture)
                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    legal.Biblio.Titles.Add(new Title()
                    {
                        Text = match.Groups["title"].Value.Trim(),
                        Language = "EN"
                    });

                    legal.Biblio.Agents.Add(new PartyMember()
                    {
                        Name = match.Groups["field74"].Value.Trim()
                    });

                    var assigness = Regex.Split(match.Groups["field73"].Value.Trim(), @"(?<=[A-Z]{2}\/)")
                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var assigner in assigness)
                    {
                        legal.Biblio.Assignees.Add(new PartyMember()
                        {
                            Name = assigner.TrimEnd('/')
                        });
                    }
                    var update = Regex.Match(match.Groups["update"].Value.Trim(),
                        @".+(?<leNote>\d{2}\/\d{2}\/\d{4})\s?(?<leDate>\d{2}\/\d{2}\/\d{4})");

                    if (update.Success)
                    {
                        legal.LegalEvent.Language = "EN";
                        legal.LegalEvent.Note = "|| UK Registration | " + match.Groups["UkReg"].Value.Trim() +
                                                " || First Registration Date | "
                                                + DateTime.Parse(match.Groups["firstReg"].Value.Trim(), culture)
                                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                                + " || Remarks | " + match.Groups["note"].Value.Trim() + " || Update Reg | " + DateTime
                            .Parse(update.Groups["leNote"].Value.Trim(), culture).ToString("yyyy.MM.dd")
                            .Replace(".", "/").Trim();

                        legal.LegalEvent.Date = DateTime
                            .Parse(update.Groups["leDate"].Value.Trim(), culture).ToString("yyyy.MM.dd")
                            .Replace(".", "/").Trim();
                    }
                    else
                    {
                        legal.LegalEvent.Note = "|| UK Registration | " + match.Groups["UkReg"].Value.Trim() +
                                                " || First Registration Date | "
                                                + DateTime.Parse(match.Groups["firstReg"].Value.Trim(), culture)
                                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                                + " || Remarks | " + match.Groups["note"].Value.Trim();

                        legal.LegalEvent.Language = "EN";
                    }
                }
                else
                {
                    var match2 = Regex.Match(note,
                        @"(?<kind>[A-Z]\s?\d+)\s?(?<UkReg>([A-Z]{1,2})?\s?\d+\s?[A-Z]?(\d+)?)\s?Invention(?<title>.+)\s?Agent(?<field74>.+)\s?Proprietor(?<field73>.+)\s?POA.+Remarks(?<note>.+)\s?Updates");

                    if (match2.Success)
                    {
                        legal.Biblio.Publication.Number = match2.Groups["kind"].Value.Trim();

                        legal.Biblio.Titles.Add(new Title()
                        {
                            Text = match2.Groups["title"].Value.Trim(),
                            Language = "EN"
                        });

                        legal.Biblio.Agents.Add(new PartyMember()
                        {
                            Name = match2.Groups["field74"].Value.Trim()
                        });

                        var assigness = Regex.Split(match.Groups["field73"].Value.Trim(), @"(?<=[A-Z]{2}\/)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var assigner in assigness)
                        {
                            legal.Biblio.Assignees.Add(new PartyMember()
                            {
                                Name = assigner.TrimEnd('/')
                            });
                        }

                        var update = Regex.Match(match.Groups["update"].Value.Trim(),
                            @".+(?<leNote>\d{2}\/\d{2}\/\d{4})\s?(?<leDate>\d{2}\/\d{2}\/\d{4})");

                        if (update.Success)
                        {
                            legal.LegalEvent.Language = "EN";
                            legal.LegalEvent.Note = "|| UK Registration | " + match2.Groups["UkReg"].Value.Trim()
                                                                            + " || Remarks | " + match2.Groups["note"].Value.Trim() + " || Update Reg | " + DateTime
                                .Parse(update.Groups["leNote"].Value.Trim(), culture).ToString("yyyy.MM.dd")
                                .Replace(".", "/").Trim();

                            legal.LegalEvent.Date = DateTime
                                .Parse(update.Groups["leDate"].Value.Trim(), culture).ToString("yyyy.MM.dd")
                                .Replace(".", "/").Trim();
                        }
                        else
                        {
                            legal.LegalEvent.Note = "|| UK Registration | " + match2.Groups["UkReg"].Value.Trim()
                                                                            + " || Remarks | " + match2.Groups["note"].Value.Trim();

                            legal.LegalEvent.Language = "EN";
                        }
                    }
                    else Console.WriteLine(note);
                }
            }

            return legal;
        }

        internal string MakeText(List<XElement> xElements, string subCode)
        {
            var text = string.Empty;

            if (subCode == "1")
            {
                foreach (var item in xElements)
                {
                    text += item.Value + " ";
                }
            }
            return text.Replace("\r", "").Replace("\n", " ").Trim();
        }
    }
}
