using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Diamond.Core.Models;
using Integration;
using SixLabors.ImageSharp;

namespace Diamond_VE_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;
        private string _resolutionDate;

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
                    //var imageFiles = GetImages(tet, subCode);
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("SOLICITADAS DE PATENTES"))
                        .TakeWhile(val => !val.Value.StartsWith("Total de Solicitudes"))
                        .ToList();

                    var text = MakeText(xElements, subCode).Trim();
                    var notes = Regex.Split(text, @"(?=\(11\)\s\d)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();
                    var match = Regex.Match(text, @"Caracas,\s(?<day>\d{2})\s?de\s?(?<month>[a-zA-Z]+)\s?de\s?(?<year>\d{4})");

                    if (match.Success)
                    {
                        _resolutionDate =
                            $"{match.Groups["year"].Value.Trim()}/{MakeMonth(match.Groups["month"].Value.Trim())}/{match.Groups["date"].Value.Trim()}";
                    }
                    else
                    {
                        Console.WriteLine("Bad resolution date");
                    }
                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatentWithImage(note, subCode, "CA"));
                    }
                }
                if (subCode == "12")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("12_SOLICITADAS DE PATENTES EXTRANJERAS"))
                        .TakeWhile(val => !val.Value.StartsWith("Publiquese,"))
                        .ToList();

                    var text = MakeText(xElements, subCode).Trim();

                    var notes = Regex.Split(text, @"(?=\(11\)\s\d)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    var match = Regex.Match(text, @"Caracas,\s(?<day>\d{2})(?<month>.+?)(?<year>\d{4})");

                    if (match.Success)
                    {
                        var month = match.Groups["month"].Value.Trim() switch
                        {
                            "de febrero de" => "02",
                            "de septiembre de" => "09",
                            "de noviembre de" => "11",
                            _ => null
                        };

                        if (month is null)
                        {
                            Console.WriteLine($"{match.Groups["month"].Value.Trim()}");
                            break;
                        }
                        _resolutionDate = match.Groups["year"].Value.Trim() + "/" + month + "/" + match.Groups["day"].Value.Trim();
                    }
                    else Console.WriteLine("Bad resolution date");

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "CA"));
                    }
                }
                if (subCode == "19")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("19_LA PROPIEDAD INTELECTUAL - REGISTRO DE LA PROPIEDAD INDUSTRIAL"))
                        .TakeWhile(val => !val.Value.StartsWith("Publiquese,"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\(21\)\s\d)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(21)")).ToList();

                    var match = Regex.Match(MakeText(xElements, subCode).Trim(), @"Caracas,\s(?<day>\d{2})(?<month>.+)(?<year>\d{4})");

                    if (match.Success)
                    {
                        var month = match.Groups["month"].Value.Trim() switch
                        {
                            "de febrero de" => "02",
                            "de marzo de" => "03",
                            "de noviembre de" => "11",
                            _ => null
                        };

                        if (month is null)
                        {
                            Console.WriteLine($"{match.Groups["month"].Value.Trim()}");
                            break;
                        }
                        _resolutionDate = match.Groups["year"].Value.Trim() + "/" + month + "/" + match.Groups["day"].Value.Trim();
                    }
                    else Console.WriteLine("Bad resolution date");

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
                if (subCode == "24")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                          .SkipWhile(val => !val.Value.StartsWith("SOLICITUDES EXTRANJERAS DE PATENTE DE INVENCIÓN" + "\n" + "NEGADAS DE OFICIO"))
                          .TakeWhile(val => !val.Value.StartsWith("Total de Solicitudes"))
                          .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\d{4}\-\d+)").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"^\d+.*").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FC"));
                    }
                }
                if (subCode == "71")
                {
                    var imageFiles = GetImages(tet, subCode);

                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .ToList();
                    var elementBeforeTarget = xElements
                        .TakeWhile(val =>
                            !val.Value.StartsWith("PATENTES DE INVENCIÓN CONCEDIDAS", StringComparison.Ordinal))
                        .Reverse()
                        .Take(5)
                        .Reverse()
                        .ToList();

                    foreach (var element in elementBeforeTarget)
                    {
                        var match = Regex.Match(element.Value,
                            @"\b(?<date>\d{1,2}) de (?<month>[a-zA-Z]+) de (?<year>\d{4})\b",
                            RegexOptions.Singleline);

                        if (match.Success)
                        {
                            _resolutionDate =
                                $"{match.Groups["year"].Value.Trim()}/{MakeMonth(match.Groups["month"].Value.Trim())}/{match.Groups["date"].Value.Trim()}";
                        }
                        else
                        {
                            Console.WriteLine("No data");
                        }
                    }


                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("PATENTES DE INVENCIÓN CONCEDIDAS", StringComparison.Ordinal))
                        .TakeWhile(val => !val.Value.StartsWith("Total de Solicitudes"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\(21\).+)").
                        Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(21)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatentWithImage(note, subCode, "FG", imageFiles));
                    }
                }
            }
            return statusEvents;
        }

        internal LegalStatusEvent MakePatent (string note, string subCode , string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                Id = _id++,
                CountryCode = "VE",
                SectionCode = sectionCode,
                SubCode = subCode,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new()
                {
                    DOfPublication = new()
                },
                LegalEvent = new()
            };
            var culture = new CultureInfo("ru-RU");

            if(subCode == "24")
            {
                var match = Regex.Match(note.Trim(), @"(?<aNum>\d{4}\-\d+)\s(?<title>.+)Tramitante:(?<f74>.+?)\n(?<f71>.+)COMENTARIO:(?<note>.+)",RegexOptions.Singleline);
                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Text = match.Groups["title"].Value.Trim(),
                        Language = "ES"
                    });

                    statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                    {
                        Name = match.Groups["f74"].Value.Trim()
                    });

                    var applicants = Regex.Split(match.Groups["f71"].Value.Replace("\r","").Replace("\n"," ").Trim(), @"(?<=s:.+,\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var applicant in applicants)
                    {
                        var match1 = Regex.Match(applicant.Trim().TrimEnd(','), @"(?<name>.+)Dom.+:(?<country>.+)", RegexOptions.Singleline);

                        if (match1.Success)
                        {
                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = match1.Groups["name"].Value.Trim(),
                                Country = MakeCountryCode(match1.Groups["country"].Value.Trim())
                            });
                        }
                        else Console.WriteLine($"{applicant} --- 71 /0  -----  { match.Groups["aNum"].Value.Trim()}");

                        if (MakeCountryCode(match1.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{match1.Groups["country"].Value.Trim()}");
                    }

                    statusEvent.LegalEvent.Note = match.Groups["note"].Value.Trim();
                    statusEvent.LegalEvent.Language = "ES";
                    //Integration.NoteTranslation noteTranslation = new()
                    //{
                    //    Language = "EN",
                    //    Type = "INID",
                    //    Tr = "THIS APPLICATION CONTRAVES ARTICLES 8 (MORE THAN AN INVENT), 14.1 (USE OF UNPAIRED TERMS) AND 59.2A (MEMORY WITH LACK OF CLARITY), AND IS UNCURRENT IN THE PROHIBITIONS PROVIDED IN ARTICLE 15.7 (CONTRAVES THE METROLOGY LAW)."
                    //};

                    //statusEvent.LegalEvent.Translations.Add(noteTranslation);
                }
                else 
                {
                    var match1 = Regex.Match(note.Trim(), @"(?<aNum>\d{4}\-\d+)\sTramitante:(?<f74>.+?)\n(?<title>.+)\.\s(?<f71>.+)\sCOMENTARIO:(?<note>.+)", RegexOptions.Singleline);

                    if (match1.Success)
                    {
                        statusEvent.Biblio.Application.Number = match1.Groups["aNum"].Value.Trim();

                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Text = match1.Groups["title"].Value.Trim(),
                            Language = "ES"
                        });

                        statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                        {
                            Name = match1.Groups["f74"].Value.Trim()
                        });

                        var applicants = Regex.Split(match1.Groups["f71"].Value.Trim(), @"(?<=s:.+,\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var match2 = Regex.Match(applicant.Trim().TrimEnd(','), @"(?<name>.+)Dom.+:(?<country>.+)", RegexOptions.Singleline);

                            if (match2.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = match2.Groups["name"].Value.Trim(),
                                    Country = MakeCountryCode(match2.Groups["country"].Value.Trim())
                                });
                            }
                            else Console.WriteLine($"{applicant} --- 71 /1 -----  { match1.Groups["aNum"].Value.Trim()}");

                            if (MakeCountryCode(match2.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{match2.Groups["country"].Value.Trim()}");
                        }

                        statusEvent.LegalEvent.Note = match1.Groups["note"].Value.Trim();
                        statusEvent.LegalEvent.Language = "ES";
                    }
                    else
                    {
                        var match2 = Regex.Match(note.Trim(), @"(?<aNum>\d{4}\-\d+)\s(?<title>.+)\.\s(?<f71>.+)\sTramitante:(?<f74>.+?)\nCOMENTARIO:(?<note>.+)", RegexOptions.Singleline);

                        if (match2.Success)
                        {
                            statusEvent.Biblio.Application.Number = match2.Groups["aNum"].Value.Trim();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Text = match2.Groups["title"].Value.Trim(),
                                Language = "ES"
                            });

                            statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                            {
                                Name = match2.Groups["f74"].Value.Trim()
                            });

                            var applicants = Regex.Split(match2.Groups["f71"].Value.Trim(), @"(?<=s:.+,\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var applicant in applicants)
                            {
                                var match3 = Regex.Match(applicant.Trim().TrimEnd(','), @"(?<name>.+)Dom.+:(?<country>.+)", RegexOptions.Singleline);

                                if (match3.Success)
                                {
                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                    {
                                        Name = match3.Groups["name"].Value.Trim(),
                                        Country = MakeCountryCode(match3.Groups["country"].Value.Trim())
                                    });
                                }
                                else Console.WriteLine($"{applicant} --- 71 /2-----  { match2.Groups["aNum"].Value.Trim()}");

                                if (MakeCountryCode(match3.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{match3.Groups["country"].Value.Trim()}");
                            }

                            statusEvent.LegalEvent.Note = match2.Groups["note"].Value.Trim();
                            statusEvent.LegalEvent.Language = "ES";
                        }
                        else
                        {
                            var match3 = Regex.Match(note.Trim(), @"(?<aNum>\d{4}\-\d+)\sTramitante:(?<f74>.+?)\n(?<title>.+?\n.+?)\.?\s(?<f71>.+)\sCOMENTARIO:(?<note>.+)", RegexOptions.Singleline);

                            if (match3.Success)
                            {
                                statusEvent.Biblio.Application.Number = match3.Groups["aNum"].Value.Trim();

                                statusEvent.Biblio.Titles.Add(new Integration.Title
                                {
                                    Text = match3.Groups["title"].Value.Trim(),
                                    Language = "ES"
                                });

                                statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                                {
                                    Name = match3.Groups["f74"].Value.Trim()
                                });

                                var applicants = Regex.Split(match3.Groups["f71"].Value.Trim(), @"(?<=s:.+,\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var applicant in applicants)
                                {
                                    var match4 = Regex.Match(applicant.Trim().TrimEnd(','), @"(?<name>.+)Dom.+:(?<country>.+)", RegexOptions.Singleline);

                                    if (match4.Success)
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = match4.Groups["name"].Value.Trim(),
                                            Country = MakeCountryCode(match4.Groups["country"].Value.Trim())
                                        });
                                    }
                                    else Console.WriteLine($"{applicant} --- 71/3 -----  { match3.Groups["aNum"].Value.Trim()}");

                                    if (MakeCountryCode(match4.Groups["country"].Value.Trim()) == null) Console.WriteLine($"{match4.Groups["country"].Value.Trim()}");
                                }
                                statusEvent.LegalEvent.Note = match3.Groups["note"].Value.Trim();
                                statusEvent.LegalEvent.Language = "ES";
                            }
                            else Console.WriteLine($"{ note}");
                        } 
                    }
                }
            }
            if (subCode == "12")
            {
                List<string> inids = new();

                var splitNote = Regex.Match(note.Replace("\r","").Replace("\n"," ").Trim(), @"(?<allinids>.+)\s(?<inid57>\(57\).+)_");

                if (splitNote.Success)
                {
                    inids = Regex.Split(splitNote.Groups["allinids"].Value.Trim(), @"(?=\(\d{2}\)\s)")
                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                    inids.Add(splitNote.Groups["inid57"].Value.Trim());
                }
                else
                {
                    var splitNoteLast = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<allinids>.+)\s(?<inid57>\(57\).+)");

                    if (splitNoteLast.Success)
                    {
                        inids = Regex.Split(splitNoteLast.Groups["allinids"].Value.Trim(), @"(?=\(\d{2}\)\s)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        inids.Add(splitNoteLast.Groups["inid57"].Value.Trim());
                    }
                    else Console.WriteLine($"{note} -- not split");
                }

                foreach (var inid in inids)
                {
                    if (inid.StartsWith("(11)"))
                    {
                        statusEvent.Biblio.Publication.Number = inid.Replace("(11)", "").Trim();
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)","").Trim(), culture).ToString("yyyy.MM.dd").Replace(".","/").Trim();
                    }
                    else if (inid.StartsWith("(30)"))
                    {
                        var priorities = Regex.Split(inid.Replace("(30)","").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priority in priorities)
                        {
                            var match = Regex.Match(priority.Trim(), @"(?<num>.+)\s(?<code>[A-Z]{2}).+(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Priority()
                                {
                                    Number = match.Groups["num"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                });
                            }
                            else Console.WriteLine($"{priority} --- 30");
                        }
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        if (inid.Replace("(51)", "").Trim() is not "")
                        {
                            statusEvent.LegalEvent.Note = "|| (51) | " + inid.Replace("(51)", "").Trim().TrimEnd(';').Trim();
                            statusEvent.LegalEvent.Language = "EN";
                        }

                        var ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc.Trim(), @".+=(?<ipc>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = match.Groups["ipc"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{ipc} - 51");
                        }
                    }
                    else if (inid.StartsWith("(73)"))
                    {

                        if (inid.Replace("(73)", "").Trim().StartsWith("1)"))
                        {
                            var match = Regex.Match(inid.Replace("(73)", "").Trim(), @"(?<name>.+)Domicilio:(?<adress>.+)Pa.s:(?<country>.+)");

                            List<string> namesList = new();
                            List<string> adressList = new();

                            if (match.Success)
                            {
                                namesList = Regex.Split(match.Groups["name"].Value.Trim(), @"\d+\)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                adressList = Regex.Split(match.Groups["adress"].Value.Trim(), @"\d+\)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                for (var i = 0; i < namesList.Count; i++)
                                {
                                    statusEvent.Biblio.Assignees.Add(new PartyMember()
                                    {
                                        Name = namesList[i].Trim().TrimEnd(','),
                                        Address1 = adressList[i].Trim(),
                                        Country = MakeCountryCode(match.Groups["country"].Value.Trim())
                                    });

                                    if (MakeCountryCode(match.Groups["country"].Value.Trim()) is null)
                                    {
                                        Console.WriteLine($"{match.Groups["country"].Value.Trim()}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            var match = Regex.Match(inid.Replace("(73)", "").Trim(), @"(?<name>.+)Domicilio:(?<adress>.+)Pa.s:(?<country>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Assignees.Add(new PartyMember()
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(match.Groups["country"].Value.Trim())
                                });

                                if (MakeCountryCode(match.Groups["country"].Value.Trim()) is null)
                                {
                                    Console.WriteLine($"{match.Groups["country"].Value.Trim()}");
                                }
                            }
                            else Console.WriteLine($"{inid} -- 73");
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        var inventors = Regex.Split(inid.Replace("(72)","").Trim(), @";").Where(val=>!string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            statusEvent.Biblio.Inventors.Add(new PartyMember()
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        var agents = Regex.Split(inid.Replace("(74)","").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var agent in agents)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = agent.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "ES",
                            Text = inid.Replace("(54)", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Abstract()
                        {
                            Language = "ES",
                            Text = inid.Replace("(57)","").Replace("_","").Trim()
                        });
                    }
                    else Console.WriteLine($"{inid} --- not process");
                }

                statusEvent.Biblio.Application.EffectiveDate = _resolutionDate;
            }
            if (subCode == "19")
            {
                var inids = Regex.Split(note.Trim(), @"(?=\(\d{2}\))", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                foreach (var inid in inids)
                {
                    if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        var match = Regex.Match(inid.Replace("(22)", "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }                  
                    }
                    else if (inid.StartsWith("(11)"))
                    {
                        statusEvent.Biblio.Publication.Number = inid.Replace("(11)", "").Trim();
                    }
                    else if (inid.StartsWith("(45)"))
                    {
                        var match = Regex.Match(inid.Replace("(45)", "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.DOfPublication.date_45 = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                    }
                    else if (inid.StartsWith("(73)"))
                    {
                        var assigneesList = Regex.Split(inid.Replace("(73)",""), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var assigneer in assigneesList)
                        {
                            statusEvent.Biblio.Assignees.Add(new PartyMember()
                            {
                                Name = assigneer.Trim().TrimEnd(',').TrimEnd(';')
                            });
                        }
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        var agentsList = Regex
                            .Split(
                                inid.Replace("(74)", "").Replace("\r", "").Replace("\n", " ").Trim().TrimEnd(',')
                                    .TrimEnd(';'), @"-").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var agent in agentsList)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = agent.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        if (inid.Replace("(51)", "").Replace("\r","").Replace("\n", " ").Trim() is not "")
                        {
                            statusEvent.LegalEvent.Note = "|| (51) | " + inid.Replace("(51)", "").Trim().TrimEnd(';').Trim();
                            statusEvent.LegalEvent.Language = "EN";
                        }

                        var ipcs = Regex.Split(inid.Replace("(51)", "").Replace("\r", "").Replace("\n", " ").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc.Trim(), @".+=\s?(?<ipc>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = match.Groups["ipc"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{ipc} - 51");
                        }
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n"," ").Trim(), @"54\)\s(?<text>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "ES",
                                Text = match.Groups["text"].Value.Replace("_","").Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}  --- 54");
                    }
                    else Console.WriteLine($"{inid} --- not process");
                }

                statusEvent.Biblio.Application.EffectiveDate = _resolutionDate;
            }
            return statusEvent;
        }

        private LegalStatusEvent MakePatentWithImage(string note,
            string subCode, string sectionCode, Dictionary<string, string> imagesDictionary = null)
        {
            var statusEvent = new LegalStatusEvent()
            {
                Id = _id++,
                CountryCode = "VE",
                SectionCode = sectionCode,
                SubCode = subCode,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new Biblio
                {
                    DOfPublication = new DOfPublication(),
                    IntConvention = new IntConvention()
                },
                LegalEvent = new LegalEvent()
            };
            var culture = new CultureInfo("ru-RU");

            if (subCode == "1")
            {
                var inids = new List<string>();

                var splitNote = Regex.Match(note.Trim(),
                    @"(?<allinids>.+)\s(?<inid57>\(57\).+)_",
                    RegexOptions.Singleline);

                if (splitNote.Success)
                {
                    inids = Regex.Split(splitNote.Groups["allinids"].Value.Trim(), @"(?=\(\d{2}\)\s)")
                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                    inids.Add(splitNote.Groups["inid57"].Value.Trim());
                }
                else
                {
                    var splitNoteLast = Regex.Match(note.Trim(),
                        @"(?<allinids>.+)\s(?<inid57>\(57\).+)",
                        RegexOptions.Singleline);

                    if (splitNoteLast.Success)
                    {
                        inids = Regex.Split(splitNoteLast.Groups["allinids"].Value.Trim(), @"(?=\(\d{2}\)\s)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        inids.Add(splitNoteLast.Groups["inid57"].Value.Trim());
                    }
                    else Console.WriteLine($"{note} -- not split");
                }

                var leNote51inid = "|| (51) | ";
                var leNote57inidEs = "|| EQ. | ";
                var leNote57inidEn = "|| Also published as | ";
                var isHas87inid = false;

                foreach (var inid in inids)
                {
                    if (inid.StartsWith("(11)"))
                    {
                        statusEvent.Biblio.Publication.Number = inid.Replace("\r","").Replace("\n"," ").Replace("(11)", "").Trim();
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(21)", "").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("\r", "").Replace("\n", " ").Replace("(22)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(30)"))
                    {
                        var priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(30)", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priority in priorities)
                        {
                            var match = Regex.Match(priority.Trim(), @"(?<num>.+)\s(?<code>[A-Z]{2}).+(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Priority()
                                {
                                    Number = match.Groups["num"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                });
                            }
                            else Console.WriteLine($"{priority} --- 30");
                        }
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        if (inid.Replace("(51)", "").Trim() is not "")
                        {
                            leNote51inid += inid.Replace("(51)", "").Trim().TrimEnd(';').Trim();
                        }

                        var ipcs = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(51)", "").Trim(), @";")
                            .Where(val => !string.IsNullOrEmpty(val))
                            .ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc.Trim(), @".+=(?<ipc>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = match.Groups["ipc"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{ipc} - 51");
                        }
                    }
                    else if (inid.StartsWith("(73)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(73)", "").Trim(),
                            @"(?<name>.+)Domicilio:(?<adress>.+)Pa.s:(?<country>.+)");

                        if (match.Success)
                        {
                            var country = MakeCountryCode(match.Groups["country"].Value.Trim());

                            if (country == null)
                            {
                                Console.WriteLine($"{match.Groups["country"].Value.Trim()}");
                            }
                            else
                            {
                                statusEvent.Biblio.Assignees.Add(new PartyMember()
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = country
                                });
                            }
                        }
                        else Console.WriteLine($"{inid} -- 73");
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        var inventors = Regex.Split(inid.Replace("\r","").Replace("\n"," ").Replace("(72)", "").Trim(), @";")
                            .Where(val => !string.IsNullOrEmpty(val))
                            .ToList();

                        foreach (var inventor in inventors)
                        {
                            statusEvent.Biblio.Inventors.Add(new PartyMember()
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        var agents = Regex.Split(inid.Replace("(74)", "").Trim(), @";")
                            .Where(val => !string.IsNullOrEmpty(val))
                            .ToList();

                        foreach (var agent in agents)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = agent.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "ES",
                            Text = inid.Replace("\r","").Replace("\n"," ").Replace("(54)", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        var cleanInid = inid.Replace("\r", "").Replace("\n", " ").Replace("(57)", "");
                        var match = Regex.Match(cleanInid,
                            @"(?<inid57>.+\.)\s(?<inid87>[A-Z]{2}\d.+?)\s__",
                            RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Abstract()
                            {
                                Language = "ES",
                                Text = match.Groups["inid57"].Value.Trim()
                            });

                            var keys = Regex.Split(match.Groups["inid87"].Value.Trim(), " ")
                                .Where(_ => !string.IsNullOrEmpty(_))
                                .ToList();

                            var stringKeys = new StringBuilder(); 

                            foreach (var key in keys)
                            {
                                if (key.StartsWith("WO"))
                                {
                                    statusEvent.Biblio.IntConvention.PctPublNumber = key.Trim();
                                }
                                else if (key.StartsWith("EP"))
                                {
                                    statusEvent.Biblio.EuropeanPatents.Add(new EuropeanPatent()
                                    {
                                        PubNumber = key
                                    });
                                }
                                else
                                {
                                    stringKeys = stringKeys.Append(key + " ");
                                    isHas87inid = true;
                                }
                            }
                            leNote57inidEs += stringKeys.ToString();
                            leNote57inidEn += stringKeys.ToString();

                        }
                        else
                        {
                            var match1 = Regex.Match(cleanInid,
                                @"(?<inid57>.+\.)\s(?<inid87>[A-Z]{2}\d.+)",
                                RegexOptions.Singleline);

                            if (match1.Success)
                            {
                                statusEvent.Biblio.Abstracts.Add(new Abstract()
                                {
                                    Language = "ES",
                                    Text = match1.Groups["inid57"].Value.Trim()
                                });

                                var keys = Regex.Split(match1.Groups["inid87"].Value.Trim(), " ")
                                    .Where(_ => !string.IsNullOrEmpty(_))
                                    .ToList();

                                var stringKeys = new StringBuilder();

                                foreach (var key in keys)
                                {
                                    if (key.StartsWith("WO"))
                                    {
                                        statusEvent.Biblio.IntConvention.PctPublNumber = key.Trim();
                                    }
                                    else if (key.StartsWith("EP"))
                                    {
                                        statusEvent.Biblio.EuropeanPatents.Add(new EuropeanPatent()
                                        {
                                            PubNumber = key
                                        });
                                    }
                                    else
                                    {
                                        stringKeys = stringKeys.Append(key + " ");
                                        isHas87inid = true;
                                    }
                                }
                                leNote57inidEs += stringKeys.ToString();
                                leNote57inidEn += stringKeys.ToString();
                            }
                            else
                            {
                                statusEvent.Biblio.Abstracts.Add(new Abstract()
                                {
                                    Language = "ES",
                                    Text = cleanInid.Replace("_", "")
                                });
                            }
                        }
                    }
                    else Console.WriteLine($"{inid} --- not process");
                }

                if (isHas87inid)
                {
                    statusEvent.LegalEvent.Note = leNote57inidEs + leNote51inid.Trim();
                    statusEvent.LegalEvent.Language = "ES";

                    statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                    {
                        Tr = leNote57inidEn + leNote51inid.Trim(),
                        Language = "EN",
                        Type = "INID"
                    });
                }
                else
                {
                    statusEvent.LegalEvent.Note = leNote51inid.Trim();
                    statusEvent.LegalEvent.Language = "EN";
                }

                statusEvent.Biblio.Application.EffectiveDate = _resolutionDate;

            }

            if (subCode == "71")
            {
                var inids = Regex.Split(note.Trim(),
                    @"(?=\(\d{2}\))")
                    .Where(val => !string.IsNullOrEmpty(val)).ToList();

                foreach (var inid in inids)
                {
                    if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    if (inid.StartsWith("(22)"))
                    {
                        var match = Regex.Match(inid.Replace("(22)", "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Trim())
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                    }
                    if (inid.StartsWith("(11)"))
                    {
                        statusEvent.Biblio.Publication.Number = inid.Replace("(11)", "").Trim();
                    }
                    if (inid.StartsWith("(45)"))
                    {
                        var match = Regex.Match(inid.Replace("(45)", "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.DOfPublication.date_45 = DateTime.Parse(match.Groups["date"].Value.Trim().Trim())
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                    }
                    if (inid.StartsWith("(73)"))
                    {
                        var assigneesList = Regex.Split(inid.Replace("(73)", ""), @"\n")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var assigner in assigneesList)
                        {
                            statusEvent.Biblio.Assignees.Add(new PartyMember()
                            {
                                Name = assigner.Trim().TrimEnd(',').TrimEnd(';')
                            });
                        }
                    }
                    if (inid.StartsWith("(74)"))
                    {
                        var agentsList = Regex.Split(inid.Replace("(74)", "").Trim(), @"\s-\s", RegexOptions.Singleline)
                            .Where(val => !string.IsNullOrEmpty(val))
                            .ToList();

                        foreach (var agent in agentsList)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = agent.Trim()
                            });
                        }
                    }
                    if (inid.StartsWith("(51)"))
                    {
                        statusEvent.LegalEvent.Note = "|| (51) | " + inid.Replace("(51)", "").Trim().TrimEnd(';').Trim();
                        statusEvent.LegalEvent.Language = "EN";

                        var ipcs = Regex.Split(inid.Replace("(51)", "").Trim(),
                            ";")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var matchIpc = Regex.Match(ipc.Trim(),
                                @".+=(?<ipc>.+)");
                            if (matchIpc.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = matchIpc.Groups["ipc"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{ipc} - 51");
                        }
                    }
                    if (inid.StartsWith("(54)"))
                    {
                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "ES",
                            Text = inid.Replace("(54)","").Replace("_", "").Trim()
                        });
                    }
                }
                statusEvent.Biblio.Abstracts.Add(new Abstract());
                statusEvent.Biblio.Application.EffectiveDate = _resolutionDate;
                AddAbstractScreenShot(statusEvent, imagesDictionary);
            }
            return statusEvent;
        }
        internal string MakeCountryCode(string country) => country switch
        {
            "PAISES BAJOS ( HOLANDA )" => "NL",
            "PAISES" => "NL",
            "ESTADOS UNIDOS DE AMÉRICA" => "US",
            "ESTADOS UNIDOS DE" => "US",
            "ESTADOS UNIDOS" => "US",
            "ESPAÑA" => "ES",
            "JAPON" => "JP",
            "FRANCIA" => "FR",
            "REINO UNIDO" => "GB",
            "GRECIA" => "GR",
            "ALEMANIA" => "DE",
            "SUIZA" => "CH",
            "ITALIA" => "IT",
            "ARGENTINA" => "AR",
            "IRLANDA" => "IE",
            "INDIA" => "IN",
            "REPUBLICA DE COREA" => "KR",
            "BRASIL" => "BR",
            "AUSTRIA" => "AT",
            "NUEVA ZELANDA" => "NZ",
            "AUSTRALIA" => "AU",
            "SINGAPUR" => "SG",
            "CHINA" => "CN",
            "CANADA" => "CA",
            "COLOMBIA" => "CO",
            "BELGICA" => "BE",
            "NORUEGA" => "NO",
            "ISRAEL" => "IL",
            "VENEZUELA" => "VE",
            "MALASIA" => "MY",
            "SUECIA" => "SE",
            _ => null
        };
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            var text = new StringBuilder();

            if(subCode is "24" or "19" or "1")
            {
                foreach (var xElement in xElements)
                {
                    text = text.Append(xElement.Value + "\n");
                }
            }
            if (subCode is "12")
            {
                foreach (var xElement in xElements)
                {
                    text = text.Append(xElement.Value + " ");
                }
            }

            if (subCode == "71")
            {
                text = xElements.Aggregate(text, (current, xElement) => current.Append(xElement.Value + " "));
            }
            return text.ToString();
        }
        private static string MakeMonth(string month) => month switch
        {
            "agosto" => "08",
            "febrero" => "02",
            "septiembre" => "09",
            "noviembre" => "11",
            _ => null
        };
        private Dictionary<string, string> GetImages(XElement tet, string subCode)
        {
            var result = new Dictionary<string, string>();
            try
            {
                var iDAndFilenameInfo = new Dictionary<string, string>();
                var imagesList = tet.Descendants()
                    .Where(x => x.Name.LocalName == "Image")
                    .Select(x => (x.Attribute("id")?.Value, x.Attribute("filename")?.Value)).ToList();
                foreach (var i in imagesList)
                {
                    if (!string.IsNullOrEmpty(i.Item1) && !string.IsNullOrEmpty(i.Item2))
                    {
                        if (!iDAndFilenameInfo.ContainsKey(i.Item1))
                        {
                            iDAndFilenameInfo.Add(i.Item1, i.Item2);
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Duplicate Image ID {i.Item1} found. Skipping.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Invalid image entry found. ID: {i.Item1}, Filename: {i.Item2}");
                    }
                }
                var iDAndPatentKeyInfo = new Dictionary<string, string>();
                var pages = tet.Descendants().Elements().Where(x => x.Name.LocalName == "Page").ToList();
                foreach (var page in pages)
                {
                    var numberPattern = GetNumberPattern(subCode);

                    var pageImage = page.Descendants()
                        .Where(x => x.Name.LocalName == "PlacedImage")
                        .Select(img => new
                        {
                            ImageId = (string)img.Attribute("image"),
                            X = double.Parse(img.Attribute("x").Value, CultureInfo.InvariantCulture),
                            Y = double.Parse(img.Attribute("y").Value, CultureInfo.InvariantCulture),
                            Width = double.Parse(img.Attribute("width").Value, CultureInfo.InvariantCulture),
                            Height = double.Parse(img.Attribute("height").Value, CultureInfo.InvariantCulture)
                        })
                        .ToList();
                    if (!pageImage.Any())
                    {
                        continue;
                    }
                    var textBlocks = page.Descendants()
                        .Where(x => x.Name.LocalName == "Box")
                        .Select(box => new
                        {
                            Box = box,
                            LLX = double.Parse(box.Attribute("llx").Value, CultureInfo.InvariantCulture),
                            LLY = double.Parse(box.Attribute("lly").Value, CultureInfo.InvariantCulture),
                            URX = double.Parse(box.Attribute("urx").Value, CultureInfo.InvariantCulture),
                            URY = double.Parse(box.Attribute("ury").Value, CultureInfo.InvariantCulture)
                        })
                        .ToList();

                    foreach (var image in pageImage)
                    {
                        var matchingBlocks = textBlocks
                            .Where(block =>
                                image.Y >= block.LLY &&
                                image.Y <= block.URY)
                            .OrderByDescending(block => block.LLY)
                            .ToList();

                        var pageElements = matchingBlocks
                            .SelectMany(block => block.Box.Descendants())
                            .Where(x => x.Name.LocalName == "Text")
                            .ToList();

                        var patentNumber = pageElements.Where(x => numberPattern.Match(x.Value).Success).ToList();
                        foreach (var element in patentNumber)
                        {
                            var patentNumberValue = numberPattern.Match(element.Value).Groups["Number"].Value.Trim();
                            if (!string.IsNullOrWhiteSpace(patentNumberValue))
                            {
                                iDAndPatentKeyInfo.Add(patentNumberValue, image.ImageId);
                            }
                        }
                    }
                }
                foreach (var pair in iDAndPatentKeyInfo)
                {
                    if (iDAndFilenameInfo.TryGetValue(pair.Value, out var filename))
                    {
                        result.Add(pair.Key, filename);
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Image ID {pair.Value} for patent {pair.Key} not found in iDAndFilenameInfo.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Getting images failed. {e.Message}");
                throw;
            }

            return result;
        }
        private static Regex GetNumberPattern(string subCode)
        {
            return subCode switch
            {
                "71" => new Regex(@"(?=\(11\)\s(?<Number>\d[^\(]+))"),
             //   "72" => new Regex(@"(?=)"),
                _ => null
            };
        }
        private void AddAbstractScreenShot(LegalStatusEvent statusEvent, Dictionary<string, string> imagesDictionary)
        {
            var patentKey = statusEvent.Biblio.Publication.Number;
            if (string.IsNullOrWhiteSpace(patentKey) || imagesDictionary == null)
            {
                return;
            }

            var patentImageInfo = imagesDictionary.Where(pair => pair.Key == patentKey)
                .Select(pair => pair.Value)
                .ToList();

            if (patentImageInfo.Any())
            {
                var pathToFolder = Path.GetDirectoryName(_currentFileName);
                if (pathToFolder != null)
                {
                    foreach (var image in patentImageInfo)
                    {
                        var pathToImageFile = Path.Combine(pathToFolder, image);
                        var imageString = ConvertTiffToPngString(pathToImageFile);
                        if (!string.IsNullOrWhiteSpace(imageString))
                        {
                            var imageValue = $"data:image/png;base64,{imageString}";
                            var id = GetUniqueScreenShotId();
                            var idText = $"<img id=\"{id}\">";
                            var tmpAbstract = statusEvent.Biblio.Abstracts.FirstOrDefault();
                            if (tmpAbstract == null)
                            {
                                return;
                            }
                            tmpAbstract.Text += idText;
                            statusEvent.Biblio.Abstracts = new List<Abstract> { tmpAbstract };
                            statusEvent.Biblio.ScreenShots.Add(new ScreenShot()
                            {
                                Id = id,
                                Data = imageValue
                            });
                        }
                    }
                }
            }
        }
        private static string ConvertTiffToPngString(string path)
        {
            using var image = SixLabors.ImageSharp.Image.Load(path);
            using var extractedImageStream = new MemoryStream();
            image.SaveAsPng(extractedImageStream);
            var extractedImageFrameBytes = extractedImageStream.ToArray();
            return Convert.ToBase64String(extractedImageFrameBytes);
        }
        internal static string GetUniqueScreenShotId()
        {
            return $"{GetRandomString(4)}-{GetRandomString(12)}";
        }
        private static string GetRandomString(int stringLength)
        {
            var sb = new StringBuilder();
            var guIds = (stringLength - 1) / 32 + 1;
            for (var i = 1; i <= guIds; i++)
            {
                sb.Append(Guid.NewGuid().ToString("N"));
            }
            return sb.ToString(0, stringLength);
        }
    }
}