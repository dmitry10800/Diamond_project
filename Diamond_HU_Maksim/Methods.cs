﻿using Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_HU_Maksim
{
    class Methods
    {
        private const string I11 = "( 11 )";
        private const string I21 = "( 21 )";
        private const string I96 = "( 96 )";
        private const string I97 = "( 97 )";
        private const string I73 = "( 73 )";
        private const string I72 = "( 72 )";
        private const string I74 = "( 74 )";
        private const string I54 = "( 54 )";
        private const string I51 = "( 51 )";
        private const string I13 = "( 13 )";
        private const string I30 = "( 30 )";

        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
        {
            var legalStatusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directoryInfo = new DirectoryInfo(path);

            var files = new List<string>();

            foreach (var file in directoryInfo.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            var xElements = new List<XElement>();

            foreach (var tetFile in files)
            {
                _currentFileName = tetFile;

                tet = XElement.Load(tetFile);

                if(subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Európai szabadalmak szövege fordításának benyújtása"))
                        .TakeWhile(val => !val.Value.StartsWith("Felszólalási eljárásban módosított európai szabadalom szövege fordításának benyújtása"))
                        .TakeWhile(val => !val.Value.StartsWith("Európai szabadalom igénypontokon kívüli szövegének magyar nyelvű fordítása"))
                        .ToList();

                    foreach (var note in BuildNotes(xElements))
                    {
                        legalStatusEvents.Add(SplitNote(note, subCode, "AG"));
                    }
                }
            }
            return legalStatusEvents;
        }

        internal string BuildText(List<XElement> xElements)
        {
            string fullText = null;

            foreach (var text in xElements)
            {
                fullText += text.Value.Trim() + "\n";
            }
            return fullText;
        }

        internal List<string> BuildNotes (List<XElement> xElements)
        {
           var text = BuildText(xElements);

            var notes = Regex.Split(text, @"(?=\(\s?11\s?\))", RegexOptions.Multiline).ToList();
            notes.RemoveAt(0);

            return notes;
        }

        internal Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string sub, string sectionCode)
        {
            var legalStatus = new Diamond.Core.Models.LegalStatusEvent();

            legalStatus.GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf"));

            legalStatus.CountryCode = "HU";

            legalStatus.SectionCode = sectionCode;

            legalStatus.SubCode = sub;

            legalStatus.Id = _id++;

            var biblioData = new Biblio();

            var europeanPatent = new EuropeanPatent();

            biblioData.EuropeanPatents = new List<EuropeanPatent>();

            foreach (var inid in SplitNoteToInid(note))
            {
                if (inid.StartsWith(I11))
                {
                    biblioData.Publication.Number = inid.Replace(I11, "").Replace("\r", "").Replace("\n", "").Trim();
                }
                else
                if (inid.StartsWith(I21))
                {
                    biblioData.Application.Number = inid.Replace(I21, "").Replace("\r", "").Replace("\n", "").Trim();
                }
                else
                if (inid.StartsWith(I96))
                {
                    var text = inid.Replace(I96, "").Replace("\r", "").Replace("\n", " ").Trim();

                    var match = Regex.Match(text, @"(?<number>.+)\s(?<date>\d{4}.\d{2}.\d{2})");

                    if (match.Success)
                    {
                        europeanPatent.AppNumber = match.Groups["number"].Value.Trim();
                        europeanPatent.AppDate = match.Groups["date"].Value.Trim();
                    }
                    else Console.WriteLine($"in 96 field this {text} don't matched");
                }
                else
                if (inid.StartsWith(I97))
                {
                    var text = inid.Replace(I97, "").Replace("\r", "").Replace("\n", " ").Trim();
                    var inids97 = Regex.Split(text, @"(?<=\d{4}.\d{2}.\d{2}.)", RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    for (var i = 0; i < inids97.Count; i++)
                    {
                        if (i == 0)
                        {
                            var match = Regex.Match(inids97[i], @"(?<number>.+)\s(?<kind>[A-Z]{1}\d+)\s(?<date>\d{4}.\d{2}.\d{2})");
                            if (match.Success)
                            {
                                europeanPatent.PubNumber = match.Groups["number"].Value.Trim();
                                europeanPatent.PubKind = match.Groups["kind"].Value.Trim();
                                europeanPatent.PubDate = match.Groups["date"].Value.Trim();
                            }

                            biblioData.EuropeanPatents.Add(europeanPatent);
                        }
                        else
                        {
                            var europeanPatent1 = new EuropeanPatent();

                            var match = Regex.Match(inids97[i], @"(?<number>.+)\s(?<kind>[A-Z]{1}\d+)\s(?<date>\d{4}.\d{2}.\d{2})");
                            if (match.Success)
                            {
                                europeanPatent1.PubNumber = match.Groups["number"].Value.Trim();
                                europeanPatent1.PubKind = match.Groups["kind"].Value.Trim();
                                europeanPatent1.PubDate = match.Groups["date"].Value.Trim();
                            }

                            biblioData.EuropeanPatents.Add(europeanPatent1);
                        }
                    }
                }
                else
                if (inid.StartsWith(I73))
                {
                    var text = inid.Replace(I73, "").Replace("\r", "").Replace("\n", " ").Trim();

                    var match = Regex.Match(text, @"(?<name>.+?),(?<adress>.+)\s\((?<code>[A-Z]{2})");

                    biblioData.Assignees = new List<PartyMember>();

                    if (match.Success)
                    {
                        biblioData.Assignees.Add(new PartyMember
                        {
                            Name = match.Groups["name"].Value.Trim(),
                            Address1 = match.Groups["adress"].Value.Trim(),
                            Country = match.Groups["code"].Value.Trim()
                        });
                    }
                    else Console.WriteLine($"in 73 field this {text} don't matched");
                }
                else
                if (inid.StartsWith(I72))
                {
                    var text = inid.Replace(I72, "").Trim();

                    var inventors = Regex.Split(text, "\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblioData.Inventors = new List<PartyMember>();

                    foreach (var inventor in inventors)
                    {
                        biblioData.Inventors.Add(new PartyMember
                        {
                            Name = inventor
                        });
                    }
                }
                else
                if (inid.StartsWith(I74))
                {
                    var text = inid.Replace(I74, "").Replace("\r", "").Replace("\n", " ").Trim();

                    var match = Regex.Match(text, @"(?<name>.+?),\s(?<adress>.+)\s\((?<code>[A-Z]{2})");

                    biblioData.Agents = new List<PartyMember>();

                    if (match.Success)
                    {
                        biblioData.Agents.Add(new PartyMember
                        {
                            Name = match.Groups["name"].Value.Trim(),
                            Address1 = match.Groups["adress"].Value.TrimEnd(','),
                            Country = match.Groups["code"].Value.Trim()
                        });
                    }
                    else Console.WriteLine($"in 74 field this {text} don't matched");
                }
                else
                if (inid.StartsWith(I54))
                {
                    var text = inid.Replace(I54, "").Replace("\r", "").Replace("\n", " ").Trim();

                    biblioData.Titles = new List<Title>
                    {
                        new Title
                        {
                            Text = text,
                            Language = "HU"
                        }
                    };
                }
                else
                if (inid.StartsWith(I51))
                {
                    var text = inid.Replace(I51, "").Replace("\r", "").Replace("\n", " ").Trim();

                    var ipcs = Regex.Split(text, @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblioData.Ipcs = new List<Ipc>();

                    foreach (var ipc in ipcs)
                    {
                        var match = Regex.Match(ipc, @"(?<class>.+)\s?\((?<date>.+)\)");

                        if (match.Success)
                        {
                            biblioData.Ipcs.Add(new Ipc
                            {
                                Class = match.Groups["class"].Value.Trim(),
                                Date = match.Groups["date"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"in 51 field this {ipc} don't matched");
                    }
                }
                else
                if (inid.StartsWith(I13))
                {
                    biblioData.Publication.Kind = inid.Replace(I13, "").Replace("\r", "").Replace("\n", "").Trim();
                }
                else
                if (inid.StartsWith(I30))
                {
                    var text = inid.Replace(I30, "").Replace("\r", "").Replace("\n", " ").Trim();

                    var field30 = Regex.Split(text, @"(?<=\b[A-Z]{2}\b)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblioData.Priorities = new List<Priority>();

                    foreach (var item in field30)
                    {
                        var match = Regex.Match(item, @"(?<number>.+)\s?(?<date>\d{4}.\d{2}.\d{2}).?\s(?<code>[A-Z]{2})");

                        if (match.Success)
                        {
                            biblioData.Priorities.Add(new Priority
                            {
                                Number = match.Groups["number"].Value.Trim(),
                                Date = match.Groups["date"].Value.Trim(),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"in 30 field this {item} don't matched in ------------ {biblioData.Application.Number}");
                    }
                }
                else Console.WriteLine($"This inid {inid} not procesed ");
            }

            legalStatus.Biblio = biblioData;

            return legalStatus;
        }

        internal List<string> SplitNoteToInid (string note) => Regex.Split(note, @"(?=\(\s?[0-9]{2}\s?\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
    }
}
