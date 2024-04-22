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
using System.Xml.Linq;
using Integration;

namespace Diamond_ID_Maksim
{
    class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        private const string I11 = "(11)";
        private const string I12 = "(12)";
        private const string I13 = "(13)";
        private const string I51 = "(51)";
        private const string I21 = "(21)";
        private const string I22 = "(22)";
        private const string I30 = "(30)";
        private const string I43 = "(43)";
        private const string I45 = "(45)";
        private const string I71 = "(71)";
        private const string I72 = "(72)";
        private const string I74 = "(74)";
        private const string I54 = "(54)";
        private const string I56 = "(56)";
        private const string I57 = "(57)";

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
                             .SkipWhile(val => !val.Value.Contains("(74) : Nama dan Alamat Konsultan Paten"))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(20\)\s\D)", RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith(@"(20)" + '\n' + "R"))
                        .ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BZ"));
                    }
                }
                if (subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("(74) : Nama dan Alamat Konsultan Paten"))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(20\)\s\D)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith(@"(20) R"))
                        .ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BZ"));
                    }
                }
                if (subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.Contains("(74) : Nama dan Alamat Konsultan Paten"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(20\)\nR.+)", RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith(@"(20)"))
                        .ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AB"));
                    }
                }

                if (subCode == "4")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(12\)\sP.+)", RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith(@"(12)"))
                        .ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
            }
            return statusEvents;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "ID",
                SectionCode = sectionCode,
                SubCode = subCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new()
                {
                    DOfPublication = new DOfPublication()
                },
                LegalEvent = new()
            };

            if (subCode == "1" || subCode == "2")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    var cleanNote = CleanInid(inid, subCode).Trim();

                    if (inid.StartsWith(I11))
                    {
                        statusEvent.Biblio.Publication.Number = cleanNote;
                    }
                    else if (inid.StartsWith(I13))
                    {
                        statusEvent.Biblio.Publication.Kind = inid.Replace("\r", "").Replace("\n", " ").Replace("(13)", "").Trim();
                    }
                    else if (inid.StartsWith(I21))
                    {
                        statusEvent.Biblio.Application.Number = cleanNote;
                    }
                    else if (inid.StartsWith(I22))
                    {
                        var date = MakeRightDate(cleanNote, subCode);

                        if (date != null)
                            statusEvent.Biblio.Application.Date = date;
                        else
                            Console.WriteLine($"{inid} --- 22");
                    }
                    else if (inid.StartsWith(I43))
                    {
                        var date = MakeRightDate(cleanNote, subCode);
                        if (date != null)
                            statusEvent.Biblio.Publication.Date = date;
                        else
                            Console.WriteLine($"{inid} --- 43");
                    }
                    else if (inid.StartsWith(I71))
                    {
                        var name = string.Empty;
                        var adress = string.Empty;
                        var lines = Regex.Split(cleanNote.Trim(), @"\n").Where(_ => !string.IsNullOrEmpty(_)).ToList();
                        if (lines.Count % 2 == 0)
                        {
                            for (int i = 0; i < lines.Count / 2; i++)
                            {
                                name += lines[i].Trim() + " ";
                            }

                            for (int i = lines.Count / 2; i < lines.Count; i++)
                            {
                                adress += lines[i].Trim() + " ";
                            }

                            var adressMatch = Regex.Match(adress.Trim(), @"(?<adress>.+)\s(?<country>.+)");
                            if (adressMatch.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = name.Trim(),
                                    Address1 = adressMatch.Groups["adress"].Value.Trim(),
                                    Country = MakeCountry(adressMatch.Groups["country"].Value.Trim())
                                });
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                name = lines[i].Trim();
                            }

                            for (int i = 1; i < lines.Count; i++)
                            {
                                adress += lines[i].Trim() + " ";
                            }

                            var adressMatch = Regex.Match(adress.Trim(), @"(?<adress>.+)\s(?<country>.+)");
                            if (adressMatch.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = name.Trim(),
                                    Address1 = adressMatch.Groups["adress"].Value.Trim(),
                                    Country = MakeCountry(adressMatch.Groups["country"].Value.Trim())
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith(I51))
                    {
                        var ipcs = Regex.Split(cleanNote.Trim(), @",").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var matchIpc = Regex.Match(ipc.Trim(), @"(?<Class>.+)\s(?<Edition>\d+\/\d+)");

                            if (matchIpc.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = matchIpc.Groups["Class"].Value.Replace(" ", "").Trim() + " " + matchIpc.Groups["Edition"].Value.Trim(),
                                });
                            }
                            else Console.WriteLine(ipc + "----51");
                        }
                    }
                    else if (inid.StartsWith(I72))
                    {
                        var inventorsList = Regex.Split(cleanNote.Trim(), @"\n").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        foreach (var inventor in inventorsList)
                        {
                            var matchInventor = Regex.Match(inventor.Trim(), @"(?<name>.+),\s?(?<country>\D{2})");
                            if (matchInventor.Success)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember()
                                {
                                    Name = matchInventor.Groups["name"].Value.Trim(),
                                    Country = matchInventor.Groups["country"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inventor} --- 72");
                        }
                    }
                    else if (inid.StartsWith(I74))
                    {
                        var matchAgent = Regex.Match(cleanNote.Replace("\r", "").Replace("\n", "~"), @"(?<Name>.+?)~(?<Adress>.+)");

                        if (matchAgent.Success)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = matchAgent.Groups["Name"].Value.Trim(),
                                Address1 = matchAgent.Groups["Adress"].Value.Replace("~", " ").Trim()
                            });
                        }
                        else
                            Console.WriteLine($"{cleanNote} --- 74");

                    }
                    else if (inid.StartsWith(I54))
                    {
                        if (cleanNote == "")
                        {
                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "ID",
                                Text = cleanNote.Replace("\r", "")
                                    .Replace("\n", " ")
                                    .Replace("(54)","")
                                    .Replace("Judul", "")
                                    .Replace("Invensi :", "").Trim()
                            });
                        }
                        else
                        {
                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "ID",
                                Text = cleanNote.Replace("\r", "").Replace("\n", " ")
                            });
                        }
                    }
                    else if (inid.StartsWith(I57))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Abstract()
                        {
                            Language = "ID",
                            Text = cleanNote.Replace("\r", "").Replace("\n", " ")
                        });
                    }
                    else if (inid.StartsWith(I30))
                    {
                        var dataFromInid30 = cleanNote.Replace("\r", "")
                            .Replace("\n", " ")
                            .Replace("(31) Nomor", "")
                            .Replace("(32) Tanggal", "")
                            .Replace("(33) Negara", "")
                            .Trim();

                        var prioritiesList = Regex.Split(dataFromInid30.Trim(), @"(?<=\s[A-Z]{2}\s)").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        foreach (var priority in prioritiesList)
                        {
                            var matchPriority = Regex.Match(priority.Trim(), @"(?<Number>.+)\s(?<Date>\d{2}\s.+\s\d{4})\s(?<Code>[A-Z]{2})");

                            if (matchPriority.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Priority()
                                {
                                    Number = matchPriority.Groups["Number"].Value.Trim(),
                                    Country = matchPriority.Groups["Code"].Value.Trim(),
                                    Date = MakeRightDate(matchPriority.Groups["Date"].Value.Trim(), subCode)
                                });
                            }
                            else
                                Console.WriteLine($"{priority} -- 30");
                        }
                    }
                }
            }
            if (subCode == "3")
            {
                foreach (var inid in MakeInids(note.Trim()))
                {
                    if (inid.StartsWith("(11)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<num>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                        }
                        else Console.WriteLine($"11 --- {inid}");
                    }
                    else if (inid.StartsWith("(13)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<kind>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"13 --- {inid}");
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<ipcs>.+)");

                        if (match.Success)
                        {
                            var ipcs = Regex.Split(match.Groups["ipcs"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var ipc in ipcs)
                            {
                                var ipcMatch = Regex.Match(ipc.Trim(), @"(?<class>.+)\s(?<num>\d+\/\d+)");

                                if (ipcMatch.Success)
                                {
                                    statusEvent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Class = ipcMatch.Groups["class"].Value.Replace(" ", "") + " " + ipcMatch.Groups["num"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"ipc --- {ipc}");
                            }
                        }
                        else Console.WriteLine($"51 --- {inid}");
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<num>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Number = match.Groups["num"].Value.Trim();
                        }
                        else Console.WriteLine($"21 --- {inid}");
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<date>.+)");

                        if (match.Success)
                        {
                            var dateMatch = Regex.Match(match.Groups["date"].Value.Trim(), @"(?<day>\d+)\s(?<month>\D+)\s(?<year>\d{4})");

                            if (dateMatch.Success)
                            {
                                var month = MakeMonth(dateMatch.Groups["month"].Value.Trim());
                                if (month is not null)
                                {
                                    statusEvent.Biblio.Application.Date = dateMatch.Groups["year"].Value.Trim() + "/" + month + "/" + dateMatch.Groups["day"].Value.Trim();
                                }
                                else Console.WriteLine($"====== {dateMatch.Groups["month"].Value.Trim()}");
                            }
                            else Console.WriteLine($"date --- {match.Groups["date"].Value.Trim()}");
                        }
                        else Console.WriteLine($"22 --- {inid}");
                    }
                    else if (inid.StartsWith("(33)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"Negara(?<prio>.+)");

                        if (match.Success)
                        {
                            var priorities = Regex.Split(match.Groups["prio"].Value.Trim(), @"(?<=[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var priority in priorities)
                            {
                                var prioMatch = Regex.Match(priority.Trim(), @"(?<num>.+)\s(?<day>\d+)\s(?<month>\D+)\s(?<year>\d{4})\s(?<code>\D{2})");

                                if (prioMatch.Success)
                                {
                                    var month = MakeMonth(prioMatch.Groups["month"].Value.Trim());

                                    if (month is not null)
                                    {
                                        statusEvent.Biblio.Priorities.Add(new Priority()
                                        {
                                            Number = prioMatch.Groups["num"].Value.Trim(),
                                            Country = prioMatch.Groups["code"].Value.Trim(),
                                            Date = prioMatch.Groups["year"].Value.Trim() + "/" + month + "/" + prioMatch.Groups["day"].Value.Trim()
                                        });
                                    }
                                    else Console.WriteLine($"===== {prioMatch.Groups["month"].Value.Trim()}");

                                }
                                else Console.WriteLine($" priority ---- {priority}");
                            }
                        }
                        else Console.WriteLine($"33 --- {inid}");
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<date>.+)");

                        if (match.Success)
                        {
                            var dateMatch = Regex.Match(match.Groups["date"].Value.Trim(), @"(?<day>\d+)\s(?<month>\D+)\s(?<year>\d{4})");

                            if (dateMatch.Success)
                            {
                                var month = MakeMonth(dateMatch.Groups["month"].Value.Trim());
                                if (month is not null)
                                {
                                    statusEvent.Biblio.Publication.Date = dateMatch.Groups["year"].Value.Trim() + "/" + month + "/" + dateMatch.Groups["day"].Value.Trim();
                                }
                                else Console.WriteLine($"====== {dateMatch.Groups["month"].Value.Trim()}");
                            }
                            else Console.WriteLine($"date --- {match.Groups["date"].Value.Trim()}");
                        }
                        else Console.WriteLine($"43 --- {inid}");
                    }
                    else if (inid.StartsWith("(71)"))
                    {
                        var match = Regex.Match(inid.Trim(), @":\n(?<name>.+)\n(?<adress>.+\n?.+\s)(?<country>.+)");

                        if (match.Success)
                        {
                            var country = MakeCountry(match.Groups["country"].Value.Trim());

                            if (country is not null)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = country
                                });
                            }
                            else Console.WriteLine($"===== {match.Groups["country"].Value.Trim()}");
                        }
                        else Console.WriteLine($"71 --- {inid}");
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        var match = Regex.Match(inid.Trim(), @":(?<inventors>.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            var inventors = Regex.Split(match.Groups["inventors"].Value.Trim(), "\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var inventor in inventors)
                            {
                                var inventorMatch = Regex.Match(inventor.Trim(), @"(?<name>.+),(?<code>[A-Z]{2})");

                                if (inventorMatch.Success)
                                {
                                    statusEvent.Biblio.Inventors.Add(new PartyMember()
                                    {
                                        Name = inventorMatch.Groups["name"].Value.Trim(),
                                        Country = inventorMatch.Groups["code"].Value.Trim()
                                    });
                                }
                            }
                        }
                        else Console.WriteLine($"72 --- {inid}");
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        var match = Regex.Match(inid.Trim(), @":\n(?<name>.+?)\n(?<adress>.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = "ID"
                            });
                        }
                        else Console.WriteLine($"74 --- {inid}");
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<title>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "ID",
                                Text = match.Groups["title"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"54 --- {inid}");
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @":(?<abstract>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Abstract()
                            {
                                Language = "ID",
                                Text = match.Groups["abstract"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"57 --- {inid}");
                    }
                }
            }
            if (subCode == "4")
            {
                var inids = MakeInids(note, subCode);

                foreach (var inid in inids)
                {
                    if (inid.StartsWith(I12))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace(I12,"").Trim();
                    }
                    if (inid.StartsWith(I11))
                    {
                        var match = Regex.Match(inid.Replace(I11,"").Trim(), @"(?<num>.+)\s(?<kind>B)", RegexOptions.Singleline);
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"11 ---- {inid}");
                    }
                    if (inid.StartsWith(I45))
                    {
                        var match = Regex.Match(inid.Replace(I45, "").Trim(),
                            @"(?<day>\d+)\s(?<month>.+)\s(?<year>\d+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            var day = match.Groups["day"].Value.Trim();
                            if (day.Length == 1)
                            {
                                day = "0" + day;
                            }

                            statusEvent.Biblio.DOfPublication.date_45 = match.Groups["year"].Value.Trim()
                                                                        + "/" + MakeMonth(match.Groups["month"].Value.Trim())
                                                                        + "/" + day;
                        }
                        else Console.WriteLine($"45 --- {inid}");
                    }
                    if (inid.StartsWith(I51))
                    {
                        var matchIpcs = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(),
                            @".+IPC.+(?<edit>\d+)\s?:(?<data>.+)",
                            RegexOptions.Singleline);

                        if (matchIpcs.Success)
                        {
                            var listIpcs = Regex.Split(matchIpcs.Groups["data"].Value.Trim(), ";")
                                .Where( _ => !string.IsNullOrEmpty(_))
                                .ToList();

                            foreach (var ipc in listIpcs)
                            {
                                var match = Regex.Match(ipc.Trim(),
                                    @"(?<part1>.+)\s(?<part2>\d+\/\d+)\((?<ver>.+)\)",
                                    RegexOptions.Singleline);

                                if (match.Success)
                                {
                                    statusEvent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Edition = int.Parse(matchIpcs.Groups["edit"].Value.Trim()),
                                        Class = match.Groups["part1"].Value.Replace(" ","").Trim() 
                                                + " " + match.Groups["part2"].Value.Trim(),
                                        Date = match.Groups["ver"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    var match2 = Regex.Match(ipc.Trim(),
                                        @"(?<part1>[A-Z]\s?\d{2}\s?[A-Z])\s?(?<part2>\d+\/\d+)",
                                        RegexOptions.Singleline);

                                    if (match2.Success)
                                    {
                                        statusEvent.Biblio.Ipcs.Add(new Ipc()
                                        {
                                            Edition = int.Parse(matchIpcs.Groups["edit"].Value.Trim()),
                                            Class = match2.Groups["part1"].Value.Replace(" ", "").Trim()
                                                    + " " + match2.Groups["part2"].Value.Trim(),
                                        });
                                    }
                                    else Console.WriteLine($"51 --- {inid}");
                                }
                            }
                        }
                        else
                        {
                            var splitIpcs2 = Regex.Split(inid.Replace("(51) Klasifikasi IPC","")
                                        .Replace("\r","").Replace("\n"," ").Trim(),
                                @",")
                                .Where(_ =>!string.IsNullOrEmpty(_))
                                .ToList();

                            foreach (var ipc in splitIpcs2)
                            {
                                var match = Regex.Match(ipc.Trim(),
                                    @"(?<fpart>[A-Z]\s?\d{2}\s?[A-Z]{1})\s?(?<spart>\d+\/\d+)");

                                if (match.Success)
                                {
                                    statusEvent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Class = match.Groups["fpart"].Value.Replace(" ", "").Trim()
                                                + " " + match.Groups["spart"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"51 --- {inid}");
                            }
                            
                        }
                    }
                    if (inid.StartsWith(I21))
                    {
                        statusEvent.Biblio.Application.Number = CleanInid(inid, subCode);
                    }
                    if (inid.StartsWith(I22))
                    {
                        var match = Regex.Match(CleanInid(inid, subCode),
                            @"(?<day>\d+)\s(?<month>.+)\s(?<year>\d+)",
                            RegexOptions.Singleline);

                        if (match.Success)
                        {
                            var day = match.Groups["day"].Value.Trim();
                            if (day.Length == 1)
                            {
                                day = "0" + day;
                            }

                            statusEvent.Biblio.Application.Date = match.Groups["year"].Value.Trim()
                                                                        + "/" + MakeMonth(match.Groups["month"].Value.Trim())
                                                                        + "/" + day;
                        }
                        else Console.WriteLine($"22 --- {inid}");
                    }
                    if (inid.StartsWith(I43))
                    {
                        var match = Regex.Match(CleanInid(inid, subCode),
                            @"(?<day>\d+)\s(?<month>.+)\s(?<year>\d+)",
                            RegexOptions.Singleline);

                        if (match.Success)
                        {
                            var day = match.Groups["day"].Value.Trim();
                            if (day.Length == 1)
                            {
                                day = "0" + day;
                            }

                            statusEvent.Biblio.Publication.Date = match.Groups["year"].Value.Trim()
                                                                  + "/" + MakeMonth(match.Groups["month"].Value.Trim())
                                                                  + "/" + day;
                        }
                        else Console.WriteLine($"43 --- {inid}");
                    }
                    if (inid.StartsWith(I56))
                    {
                        var listDoc = Regex.Split(CleanInid(inid, subCode).Replace("\r","").Replace("\n", " "), @"(?<=\s[A-Z]\d?\s)")
                            .Where(_ => !string.IsNullOrEmpty(_))
                            .ToList();

                        foreach (var doc in listDoc)
                        {
                            var match = Regex.Match(doc.Trim(),
                                @"(?<code>[A-Z]{2})\s?(?<num>.+)\s?(?<kind>[A-Z]\d?)",
                                RegexOptions.Singleline);

                            if (match.Success)
                            {
                                statusEvent.Biblio.PatentCitations.Add(new PatentCitation()
                                {
                                    Kind = match.Groups["kind"].Value.Trim(),
                                    Number = match.Groups["num"].Value.Trim(),
                                    Authority = match.Groups["code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine();
                        }
                    }
                    if (inid.StartsWith(I71))
                    {
                        var applicantsList = Regex.Split(CleanInid(inid, subCode),
                            @"\d{1,3}\.(?=.+)", 
                            RegexOptions.Singleline)
                            .Where(_=>!string.IsNullOrEmpty(_))
                            .ToList();

                        foreach (var applicant in applicantsList)
                        {
                            var lines = Regex.Split(applicant, @"\n")
                                .Where(_=>!string.IsNullOrEmpty(_))
                                .ToList();

                            var names = lines[0];
                            string country = null;
                            var adress = new StringBuilder();

                            var matchAdress = Regex.Match(lines[lines.Count - 1],
                                @"(?<adress>.+),\s?(?<country>.+)",
                                RegexOptions.Singleline);

                            if (matchAdress.Success)
                            {
                                var tmpCountry = MakeCountry(matchAdress.Groups["country"].Value.Trim());
                                if (tmpCountry == null)
                                {
                                    Console.WriteLine(MakeCountry(matchAdress.Groups["country"].Value.Trim()));
                                }
                                country = tmpCountry;

                                for (var i = 1; i < lines.Count - 1; i++)
                                {
                                    adress = adress.Append(lines[i].Trim() + " ");
                                }

                                adress = adress.Append(matchAdress.Groups["adress"].Value.Trim());
                            }
                            else
                            {
                                var tmpCountry = MakeCountry(lines[lines.Count - 1].Trim());
                                if (tmpCountry == null)
                                {
                                    Console.WriteLine(MakeCountry(lines[lines.Count - 1].Trim()));
                                }
                                country = tmpCountry;

                                for (var i = 1; i < lines.Count - 1; i++)
                                {
                                    adress = adress.Append(lines[i].Trim() + " ");
                                }
                            }

                            statusEvent.Biblio.Applicants.Add(new PartyMember()
                            {
                                Name = names,
                                Country = country,
                                Address1 = adress.ToString()
                            });
                        }
                    }
                    if (inid.StartsWith(I72))
                    {
                        var listInventors = Regex.Split(CleanInid(inid, subCode), @"\n")
                            .Where(_ => !string.IsNullOrEmpty(_))
                            .ToList();

                        foreach (var inventor in listInventors)
                        {
                            var match = Regex.Match(inventor.Trim(),
                                @"(?<name>.+),\s?(?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember()
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Country = match.Groups["country"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"72 --- {inventor}");
                        }
                    }
                    if (inid.StartsWith(I54))
                    {
                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Text = CleanInid(inid, subCode).Replace("\r","").Replace("\n"," ").Trim(),
                            Language = "ID"
                        });
                    }
                    if (inid.StartsWith(I74))
                    {
                        var match = Regex.Match(CleanInid(inid, subCode),
                            @"(?<inid74>.+)\s(?<note>Pemeri.+)",
                            RegexOptions.Singleline);

                        if (match.Success)
                        {
                            var lines = Regex.Split(match.Groups["inid74"].Value.Trim(), "\n")
                                .Where(_ => !string.IsNullOrEmpty(_))
                                .ToList();

                            var name = lines[0].Trim();
                            var country = MakeCountry(lines[lines.Count - 1]);
                            var adress = new StringBuilder();
                            for (var i = 1; i < lines.Count-1; i++)
                            {
                                adress = adress.Append(lines[i].Trim() + " ");
                            }

                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = name,
                                Country = country,
                                Address1 = adress.ToString()
                            });

                            var matchNote = Regex.Match(match.Groups["note"].Value.Trim(),
                                @":(?<group1>.+)Jum.+:(?<group2>.+)",
                                RegexOptions.Singleline);

                            if (matchNote.Success)
                            {
                                statusEvent.LegalEvent.Note =
                                    "|| Pemeriksa Paten | " + matchNote.Groups["group1"].Value.Trim() + '\n'
                                    + "|| Jumlah Klaim | " + matchNote.Groups["group2"].Value.Trim();
                                statusEvent.LegalEvent.Language = "ID";

                                statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                                {
                                    Language = "EN",
                                    Tr = "|| Patent examiner | " + matchNote.Groups["group1"].Value.Trim() + '\n'
                                         + "|| Number of claims | " + matchNote.Groups["group2"].Value.Trim()
                                });
                            }
                        }
                        else
                        {
                            var matchSecond = Regex.Match(CleanInid(inid, subCode),
                                @"(?<note>Pemeri.+)",
                                RegexOptions.Singleline);

                            if (matchSecond.Success)
                            {
                                var matchNote = Regex.Match(matchSecond.Groups["note"].Value.Trim(),
                                    @":(?<group1>.+)Jum.+:(?<group2>.+)",
                                    RegexOptions.Singleline);

                                if (matchNote.Success)
                                {
                                    statusEvent.LegalEvent.Note =
                                        "|| Pemeriksa Paten | " + matchNote.Groups["group1"].Value.Trim() + '\n'
                                        + "|| Jumlah Klaim | " + matchNote.Groups["group2"].Value.Trim();
                                    statusEvent.LegalEvent.Language = "ID";

                                    statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                                    {
                                        Language = "EN",
                                        Tr = "|| Patent examiner | " + matchNote.Groups["group1"].Value.Trim() + '\n'
                                             + "|| Number of claims | " + matchNote.Groups["group2"].Value.Trim()
                                    });
                                }
                            }
                            else Console.WriteLine($"74 --- {inid}");
                        }
                    }
                    if (inid.StartsWith(I57))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Abstract()
                        {
                            Text = CleanInid(inid, subCode).Replace("\r","").Replace("\n"," ").Trim(),
                            Language = "ID"
                        });
                    }
                    if (inid.StartsWith(I30))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(),
                            @"Negara(?<data>.+)");

                        if (match.Success)
                        {
                            var priorities = Regex.Split(match.Groups["data"].Value.Trim(),
                                @"(?<=\s[A-Z]{2}\s)")
                                .Where(_ => !string.IsNullOrEmpty(_))
                                .ToList();

                            foreach (var priority in priorities)
                            {
                                var priorityMatch = Regex.Match(priority.Trim(),
                                    @"(?<num>.+)\s(?<day>\d+)\s(?<month>.+)\s(?<year>\d+)\s(?<code>[A-Z]{2})");

                                if (priorityMatch.Success)
                                {
                                    var day = priorityMatch.Groups["day"].Value.Trim();
                                    if (day.Length == 1)
                                    {
                                        day = "0" + priorityMatch.Groups["day"].Value.Trim();
                                    }

                                    statusEvent.Biblio.Priorities.Add(new Priority()
                                    {
                                        Number = priorityMatch.Groups["num"].Value.Trim(),
                                        Country = priorityMatch.Groups["code"].Value.Trim(),
                                        Date = priorityMatch.Groups["year"].Value.Trim() + "/"
                                        + MakeMonth(priorityMatch.Groups["month"].Value.Trim()) + "/"
                                        + day
                                    });
                                }
                                else Console.WriteLine($"30 --- {priority}");
                            }
                        }
                    }
                }
            }
            return statusEvent;
        }
        internal string MakeText(List<XElement> xElements)
        {
            var sb = new StringBuilder();

            foreach (var xElement in xElements)
            {
                sb = sb.Append(xElement.Value + "\n");
            }
            return sb.ToString();
        }
        private List<string> MakeInids(string note, string subcode)
        {
            var inids = new List<string>();

            if (subcode == "1" || subcode == "2")
            {
                var match = Regex.Match(note, @"(?<FirstPart>\(20.+)(?<Inid30>\(30.+)(?<SecondPart>\(43.+)(?<Inid57>\(57.+)", RegexOptions.Singleline);

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["FirstPart"].Value + match.Groups["SecondPart"].Value, @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    inids.Add(match.Groups["Inid30"].Value.Trim());
                    inids.Add(match.Groups["Inid57"].Value.Trim());
                }
                else
                    Console.WriteLine($"{note} -- not create inid");
            }
            if (subcode == "4")
            {
                var match = Regex.Match(note, @"(?<FirstPart>\(12.+)(?<Inid30>\(30.+)(?<SecondPart>\(43.+)(?<Inid57>\(57.+)", RegexOptions.Singleline);

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["FirstPart"].Value + match.Groups["SecondPart"].Value, @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    inids.Add(match.Groups["Inid30"].Value.Trim());
                    inids.Add(match.Groups["Inid57"].Value.Trim());
                }
                else
                    Console.WriteLine($"{note} -- not create inid");
            }
            return inids;
        }
        private string CleanInid(string inid, string subCode)
        {
            var cleanInid = string.Empty;
            var match = Regex.Match(inid.Trim(), @"\d{2}\).+?:(?<inid>.+)", RegexOptions.Singleline);
            if (match.Success)
            {
                cleanInid = match.Groups["inid"].Value.Trim();
            }
            return cleanInid;
        }
        private string MakeRightDate(string inid, string subCode)
        {
            var date = string.Empty;
            if (subCode == "1" || subCode == "2")
            {
                var match = Regex.Match(inid.Trim(), @"(?<day>\d{2})(?<month>.+)(?<year>\d{4})");
                if (match.Success)
                {
                    date = match.Groups["year"].Value.Trim() + "/" + MakeMonth(match.Groups["month"].Value.Trim()) + "/" +
                           match.Groups["day"].Value.Trim();
                }
            }
            return date;
        }
        internal List<string> MakeInids(string note)
        {
            var inids = Regex.Split(note.Substring(0, note.IndexOf("(57)")).Trim(), @"(?=\(\d{2}\)\s?)").Where(val => !string.IsNullOrEmpty(val)).ToList();

            inids.Add(note.Substring(note.IndexOf("(57)")).Trim());

            return inids;
        }
        internal string MakeMonth(string month) => month switch
        {
            "JAN" => "01",
            "Januari" => "01",
            "FEB" => "02",
            "Februari" => "02",
            "MAR" => "03",
            "Maret" => "03",
            "APR" => "04",
            "April" => "04",
            "MAY" => "05",
            "Mei" => "05",
            "JUN" => "06",
            "Juni" => "06",
            "JUL" => "07",
            "Juli" => "07",
            "AUG" => "08",
            "Agustus" => "08",
            "SEP" => "09",
            "September" => "09",
            "OCT" => "10",
            "Oktober" => "10",
            "NOV" => "11",
            "November" => "11",
            "DEC" => "12",
            "Desember" => "12",
            _ => null
        };
        internal string MakeCountry(string country) => country switch
        {
            "France" => "FR",
            "Finland" => "FI",
            "Cimahi" => "ID",
            "Guangdong" => "CN",
            "Japan" => "JP",
            "Federation" => "RU",
            "America" => "US",
            "Bandung" => "ID",
            "Bogor" => "ID",
            "Netherlands" => "NL",
            "INDONESIA" => "ID",
            "Taiwan" => "TW",
            "Padang" => "ID",
            "JAPAN" => "JP",
            "Cirebon" => "ID",
            "Indonesia" => "ID",
            "Australia" => "AU",
            "Germany" => "DE",
            "Zealand" => "NZ",
            "Denmark" => "DK",
            "China" => "CN",
            "States of America" => "US",
            "MAKASSAR" => "ID",
            "Singapore" => "SG",
            "Republic of Korea" => "KR",
            "KINGDOM" => "UK",
            "Sweden" => "SE",
            "United States of America" => "US",
            "AUSTRALIA" => "AU",
            "THAILAND" => "TH",
            "Austria" => "AT",
            "SINGAPORE" => "SG",
            "DEUTSCHLAND" => "DE",
            "USA" => "US",
            "Belgium" => "BE",
            "Korea" => "KR",
            "Beijing" => "CN",
            "Kingdom" => "UK",
            "States" => "US",
            "STATES OF AMERICA" => "US",
            "NETHERLANDS" => "NL",
            "SPAIN" => "ES",
            "Depok" => "ID",
            "Banjarmasi" => "ID",
            "Italy" => "IT",
            "India" => "IN",
            "Colombia" => "CO",
            "Slovenia" => "SI",
            "Zurich" => "CH",
            "Switzerland" => "CH",
            "Canada" => "CA",
            "United Kingdom" => "UK",
            "Zhejiang" => "CN",
            "UNITED STATES OF AMERICA" => "US",
            "Spain" => "ES",
            "Makassar" => "ID",
            "INDIA" => "IN",
            "Mejayan" => "ID",
            "Manis" => "ID",
            "Surakarta" => "ID",
            "Semarang 5" => "ID",
            "MANADO" => "ID",
            "Manado" => "ID",
            "Malang" => "ID",
            "Dayeuhkolot" => "ID",
            "Sumedang" => "ID",
            "Russia" => "RU",
            "Timur" => "ID",
            "Jatiwarna" => "ID",
            "Jakarta" => "ID",
            "Surabaya" => "ID",
            _ => null
        };
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                _ = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
