﻿using Integration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_IL_Maksim
{
    class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
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

                if (subCode == "19")
                {
                    using StreamReader textFromFile = new(tetml.Replace(".tetml",".txt"));

                    var notes = Regex.Split(textFromFile.ReadToEnd().Replace("\r", "").Replace("\t", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).Where(val => !val.StartsWith(" ")).ToList();

                    foreach (var note in notes)
                    {
                        foreach (var item in MakePatent(note, subCode, "BZ"))
                        {
                            statusEvents.Add(item);
                        }
                        
                    }
                }
            }
            return statusEvents;
        }

        internal List <Diamond.Core.Models.LegalStatusEvent> MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legal = new()
            {
                SectionCode = sectionCode,
                SubCode = subCode,
                CountryCode = "IL",
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf"))
            };

            List<Diamond.Core.Models.LegalStatusEvent> legals = new();

            Biblio biblio = new();

            var match = Regex.Match(note.Trim(), @"(?<gr1>\D+)\s?-\s?(?<gr2>.+)");

            if (match.Success)
            {
                var numbers = Regex.Split(match.Groups["gr2"].Value.Trim(), ",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                foreach (var number in numbers)
                {
                    biblio.Application.Number = number.Trim();

                    var match1 = Regex.Match(match.Groups["gr1"].Value.Trim(), @"[A-Z]");
                    if (match1.Success)
                    {
                        biblio.Applicants = new()
                        {
                            new PartyMember
                            {
                                Name = match.Groups["gr1"].Value.Trim(),
                                Language = "EN"
                            }
                        };
                    }
                    else
                    {
                        biblio.Applicants = new()
                        {
                            new PartyMember
                            {
                                Name = match.Groups["gr1"].Value.Trim(),
                                Language = "HE"
                            }
                        };
                    }                   
                    var date = Regex.Match(Path.GetFileName(_currentFileName.Replace(".txt", "")), @"[0-9]{8}");
                    if (date.Success)
                    {
                        biblio.DOfPublication = new()
                        {
                            date_45 = date.Value.Insert(4, "-").Insert(7, "-").Trim()
                        };
                    }
                    legal.Biblio = biblio;
                    legals.Add(legal);
                }
            }

            return legals;
        }
    }
}
