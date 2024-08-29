using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Integration;

namespace Diamond_MA_Maksim
{
    internal class Methods
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

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("OFFICE MAROCAIN DE LA PROPRIETE"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(12\)\s?BRE)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(12)") && Regex.Match(val, @".+(?<group>\(11\).+:.+\d\sA\d{1,2}.+)", RegexOptions.Singleline).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BA"));
                    }
                }
                else
                if(subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("OFFICE MAROCAIN DE LA PROPRIETE"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(12\)\s?BRE)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(12)") && Regex.Match(val, @".+(?<group>\(11\).+:.+\d\sB\d{1,2}.+)", RegexOptions.Singleline).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BA"));
                    }
                }
            }
            return statusEvents;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

            if (subCode == "1")
            {
                foreach (var xElement in xElements)
                {
                    text += xElement.Value + " ";
                }
            }
            else
            if (subCode == "2")
            {
                foreach (var xElement in xElements)
                {
                    text += xElement.Value + " ";
                }
            }

            return text;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "MA",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new(),
                NewBiblio = new()
            };

            CultureInfo culture = new("ru-RU");

            if (subCode is "1" or "2")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(12)"))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(12)", "").Trim();
                    }
                    else if (inid.StartsWith("(11)"))
                    {
                        var match = Regex.Match(inid, @".+:(?<num>.+)(?<kind>\D\d{1,2})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        var match = Regex.Match(inid, @".+:\s?(?<date>\d{2}.\d{2}.\d{4})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        var match = Regex.Match(inid.Trim(), @".+:\s?(?<ipc>.+)");

                        if (match.Success)
                        {
                            var ipcs = Regex.Split(match.Groups["ipc"].Value.Trim(), @";")
                                .Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var ipc in ipcs)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = ipc.Trim()
                                });
                            }
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        var match = Regex.Match(inid.Trim(), @".+:\s?(?<num>.+)");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Number = match.Groups["num"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        var match = Regex.Match(inid, @".+:\s?(?<date>\d{2}.\d{2}.\d{4})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(30)"))
                    {
                        var match = Regex.Match(inid.Trim(),
                            @".+:\s?(?<date>\d{2}.\d{2}.\d{4})\s?(?<code>\D{2})\s?(?<num>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Priorities.Add(new Priority()
                            {
                                Number = match.Groups["num"].Value.Trim(),
                                Country = match.Groups["code"].Value.Trim(),
                                Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(86)"))
                    {
                        var match = Regex.Match(inid.Trim(), @".+:\s?(?<num>.+)\s?(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(71)"))
                    {
                        var applicants = Regex
                            .Split(inid.Replace("(71) Demandeur(s) :", "").Trim(), @"(?=•)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var match = Regex.Match(applicant.Replace("•","").Trim(),
                                @"(?<name>.+?),\s?(?<adress>.+)\s?\((?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["country"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inid}");
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        var match = Regex.Match(inid.Trim(), @".+:\s?(?<all>.+)");

                        if (match.Success)
                        {
                            var inventors = Regex.Split(match.Groups["all"].Value.Trim(), @";")
                                .Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var inventor in inventors)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember()
                                {
                                    Name = inventor.Trim()
                                });    
                            }
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        var match = Regex.Match(inid.Trim(), @".+:\s?(?<all>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = match.Groups["all"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        var match = Regex.Match(inid.Trim(), @".+:\s?(?<all>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "FR",
                                Text = match.Groups["all"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        var match = Regex.Match(inid.Trim(), @".+:\s?(?<all>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Abstract()
                            {
                                Language = "FR",
                                Text = match.Groups["all"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else Console.WriteLine($"{inid}");
                }
            }
            return statusEvent;
        }
        internal List<string> MakeInids(string note, string subCode)
        {
            if (subCode is "1" or "2")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<inids>.+)\s(?<inid57>\(57\).+)\s?ROYAUME");

                if (match.Success)
                {
                    var inids = Regex.Split(match.Groups["inids"].Value.Trim(), @"(?=\(\d{2}\).+)")
                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                    inids.Add(match.Groups["inid57"].Value.Trim());

                    return inids;
                }
                else
                {
                    var match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                        @"(?<inids>.+)\s(?<inid57>\(57\).+)");

                    if (match1.Success)
                    {
                        var inids = Regex.Split(match1.Groups["inids"].Value.Trim(), @"(?=\(\d{2}\).+)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        inids.Add(match1.Groups["inid57"].Value.Trim());

                        return inids;
                    }
                    else Console.WriteLine($"{note}");
                }
            }
            return null;
        }
    }
}
