﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Integration;

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
                if (subCode is "12")
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
                else if (subCode is "19")
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
                else if (subCode == "24")
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
            }
            return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode , string sectionCode)
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

            CultureInfo culture = new("ru-RU");

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
                    //    Type = "note",
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
            else if (subCode is "12")
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
            else if (subCode is "19")
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
        internal string MakeCountryCode(string country) => country switch
        {
            "PAISES BAJOS ( HOLANDA )" => "NL",
            "ESTADOS UNIDOS DE AMÉRICA" => "US",
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
            _ => null
        };
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            var text = new StringBuilder();

            if(subCode is "24" or "19")
            {
                foreach (var xElement in xElements)
                {
                    text = text.Append(xElement.Value + "\n");
                }
            }
            else if (subCode is "12" )
            {
                foreach (var xElement in xElements)
                {
                    text = text.Append(xElement.Value + " ");
                }
            }
            var tmp = text.ToString();
            return tmp;
        }
    }
}
