﻿using Integration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_RO_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        private const string I11 = "(11)";
        private const string I51 = "(51)";
        private const string I21 = "(21)";
        private const string I22 = "(22)";
        private const string I41 = "(41)";
        private const string I71 = "(71)";
        private const string I72 = "(72)";
        private const string I73 = "(73)";
        private const string I74 = "(74)";
        private const string I86 = "(86)";
        private const string I87 = "(87)";
        private const string I30 = "(30)";
        private const string I54 = "(54)";
        private const string I57 = "(57)";
        private const string I57n = "(57n)";
        private const string I45 = "(45)";
        private const string I56 = "(56)";
        private const string I95 = "(95)";
        private const string I96 = "(96)";
        private const string I92 = "(92)";
        private const string I93 = "(93)";
        private const string I97 = "(97)";
        private const string I80 = "(80)";
        private const string I84 = "(84)";
        private const string I66 = "(66)";
        private const string I67 = "(67)";
        private const string I68 = "(68)";

        private readonly Regex _classificationPattern = new(@"(?<Classification>.+)\s\((?<Version>.+)\)");
        private readonly Regex _publicationInfoPattern = new(@"(?<PubNumber>.+)\s*(?<PubKind>[A-Z]{1}\d?)");
        private readonly Regex _europeanAppPattern = new(@"(?<AppNumber>.+)\s(?<AppDate>\d{2}.\d{2}.\d{4})");
        private readonly Regex _europeanPubPattern = new(@"^(?<PubNumber>\d+)\s(?<NoteDate>\d{2}.\d{2}.\d{4})");
        private readonly Regex _priorityPattern = new(@"^(?<CountryPriority>\D{2})\s(?<NumberPriority>.+)\s(?<DatePriority>\d{2}.\d{2}.\d{4})");
        private readonly Regex _intConventionPattern = new(@"(?<PctPubNumber>.+),\s(?<PctPubDate>\d{2}.\d{2}.\d{4})");
        private readonly Regex _personPattern = new(@"(?<Name>.+),\s(?<Adress>.+),\s(?<Country>\D{2})$");
        private readonly Regex _datePattern = new(@"\d{2}.\d{2}.\d{4}");

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> events = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode is "11")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Modificãri în situaþia juridicã a brevetelor de invenþie europene cu efecte în România care au fost"))
                        .TakeWhile(val => !val.Value.StartsWith("Modificare nume/adresã mandatar:"))
                        .ToList();

                    if (xElements.Count == 0)
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("Modificări în situaţia juridică a brevetelor de invenţie europene cu efecte în România"))
                            .TakeWhile(val => !val.Value.StartsWith("Modificare nume/adresă mandatar:"))
                            .ToList();
                    }

                    var notes = Regex.Split(MakeText(xElements), @"(?=RO/EP.+)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("RO")).ToList();

                    foreach (var note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "TC/TE"));
                    }
                }
                else if (subCode is "12")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Modificare nume/adresã mandatar:"))
                        .TakeWhile(val => !val.Value.StartsWith("7. Decãderi ale titularilor din drepturile conferite"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=RO/EP.+)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("RO")).ToList();

                    foreach (var note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "TC/TE"));
                    }
                }
                else if (subCode == "13")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Rezumatul invenţiei din cererea de brevet publicată serve"))
                        .TakeWhile(val => !val.Value.StartsWith("Cereri de brevet de invenţie publicate con"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(11\).+\(51\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                    foreach (var note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "BZ"));
                    }
                }
                else if (subCode == "14")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Orice persoană interesată are dreptul să ceară,"))
                        .TakeWhile(val => !val.Value.StartsWith("BREVETE DE INVENŢIE"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(11\).+\(51\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                    foreach (var note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "FG"));
                    }
                }
                else if (subCode == "16")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("1. Brevete de invenþie europene pentru care a fost"))
                       .TakeWhile(val => !val.Value.StartsWith("2. Brevete de invenþie europene pentru care a fost"))
                       .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(51\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(51)")).ToList();

                    foreach (var note in notes)
                    {
                        events.Add(GetPatent16SubCode(note, subCode, "BZ"));
                    }

                }
                else if (subCode == "17")
                {                   
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("4. Brevete de invenþie europene cu efecte extinse"))
                       .TakeWhile(val => !val.Value.StartsWith("5. Transmiteri de dreptu i înregistrate la Oficiul de Stat"))
                       .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(51\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(51)")).ToList();

                    foreach (var note in notes)
                    {
                        events.Add(GetPatent17SubCode(note, subCode, "BZ"));
                    }
                }
                else if (subCode == "20")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("Semnificaţia codurilor INID folosite în prez nta secţiune (norma ST 9 a Organizaţiei Mondiale"))
                       .TakeWhile(val => !val.Value.StartsWith("Cereri de certificat suplimentar de protecţie aranjate după denumirea solicitantului"))
                       .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(51\)\s?[A-Z])").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(51)")).ToList();

                    foreach (var note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "NC"));
                    }
                }
                else if (subCode == "22")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("MODELE DE UTIL TATE ÎNREGISTRATE"))
                       .TakeWhile(val => !val.Value.StartsWith("CERERI DE MODEL DE UTILITATE"))
                       .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(51\)\s?[A-Z])").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(51)")).ToList();

                    foreach (var note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "FG"));
                    }
                }
                else if (subCode == "23")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("7. Decãderi ale titularilor din drepturile conferite\n"+"acestora de brevetul de invenþie european, publicate"))
                       .TakeWhile(val => !val.Value.StartsWith("8. Brevete europene care nu au efecte de la început"))
                       .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?<=EP\s\d{7})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "MZ"));
                    }
                }
                else if (subCode == "24")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("REPUBLICATĂ ÎN MONITORUL OFICIAL, PARTEA I, NR. 613/19.08.2014") && !val.Value.StartsWith("Nr. CBI"))
                        .TakeWhile(val => !val.Value.StartsWith("DIVERSE"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?<=0\d{4}\n)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "MM"));
                    }
                }
                else if (subCode == "27")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Cereri de brevet de invenţie pentru care s-a luat o hotărâre de respingere conform art. 27, alin. 2,"))
                        .TakeWhile(val => !val.Value.StartsWith("Cereri de brevet de invenţie declarate ca fiind retrase conform art. 27, din Legea nr. 64/1991"))
                        .TakeWhile(val => !val.Value.StartsWith("Cereri de brevet de invenţie pentru care s-a luat act de retragere conform art. 27, alin. 3, din"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=a\s\d{4})").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("a 2")).ToList();

                    foreach (var note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "FC"));
                    }
                }
                else if (subCode == "29")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Cereri de brevet de invenţie declarate ca fiind retrase conform art. 27, din Legea nr. 64/1991"))
                        .TakeWhile(val => !val.Value.StartsWith("CERERI DE BREVET DE INVENŢIE"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=a\s\d{4})").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("a 2")).ToList();

                    events.AddRange(notes.Select(note => MakeConvertedPatent(note, subCode, "FA")));
                }
                else if (subCode is "5")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("5. Transmiteri de drepturi înregistrate la Oficiul de Stat"))
                        .TakeWhile(val => !val.Value.StartsWith("6. Modificãri în situaþia juridicã a brevetelor de invenþie"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=RO\s?\/\s?[A-Z]{2}.+)").Where(x => !string.IsNullOrEmpty(x) && x.StartsWith("RO")).ToList();

                    events.AddRange(notes.Select(note => MakeConvertedPatent(note, subCode, "PC")));
                }
            }
            return events;
        }
        internal string MakeText (List<XElement> xElement)
        {
            string text = null;

            foreach (var element in xElement)
            {
                text += element.Value + "\n";
            }

            return text;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakeConvertedPatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatus = new()
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "RO",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                LegalEvent = new(),
                NewBiblio = new ()
            };

            Biblio biblio = new()
            {
                DOfPublication = new (),
                EuropeanPatents = new()
            };

            var cultureInfo = new CultureInfo("ru-Ru");
            if (subCode == "5")
            {
                note = Clean(note);
                var match = Regex.Match(note.Replace("Brevet de", "")
                    .Replace("Nr. cerere de", "")
                    .Replace("Titular", "")
                    .Replace("Succesor în drepturi", "")
                    .Replace("inventie nr.", "")
                    .Replace("brevet de", "")
                    .Replace("invenþie", "").Trim(), @"(?<pNum>RO\/[A-Z]{2}\s\d{7})\s?(?<aNum>\d+\.?\d+)\s?\(73\)(?<old>.+)\(73\)(?<new>.+)", RegexOptions.Singleline);

                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["pNum"].Value.Trim();
                    biblio.EuropeanPatents.Add(new EuropeanPatent()
                    {
                        AppNumber = match.Groups["aNum"].Value.Trim()
                    });

                    var oldAssignees = Regex.Split(match.Groups["old"].Value.Trim(), @";")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();

                    foreach (var assignee in oldAssignees)
                    {
                        var matchAssignee = Regex.Match(assignee.Replace("\r", "").Replace("\n", " ").Trim(),
                            @"(?<name>.+?),(?<adress>.+)\s(?<code>[A-Z]{2})");

                        if (matchAssignee.Success)
                        {
                            biblio.Assignees.Add(new PartyMember()
                            {
                                Country = matchAssignee.Groups["code"].Value.Trim(),
                                Address1 = matchAssignee.Groups["adress"].Value.TrimEnd(',').Trim(),
                                Name = matchAssignee.Groups["name"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{assignee} --- old is not matched");
                    }

                    var newAssignees = Regex.Split(match.Groups["new"].Value.Trim(), @";")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();

                    foreach (var assignee in newAssignees)
                    {
                        var matchAssignee = Regex.Match(assignee.Replace("\r", "").Replace("\n", " ").Trim(),
                            @"(?<name>.+?),(?<adress>.+)\s(?<code>[A-Z]{2})");

                        if (matchAssignee.Success)
                        {
                            legalStatus.NewBiblio.Assignees.Add(new PartyMember()
                            {
                                Country = matchAssignee.Groups["code"].Value.Trim(),
                                Address1 = matchAssignee.Groups["adress"].Value.TrimEnd(',').Trim(),
                                Name = matchAssignee.Groups["name"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{assignee} --- new is not matched");
                    }

                    var legalDate = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"\d{8}");
                    if (legalDate.Success)
                    {
                        legalStatus.LegalEvent.Date = legalDate.Value.Trim().Insert(4,"/").Insert(7,"/");
                    }
                }
                else Console.WriteLine($"{note} - not matched");

                legalStatus.Biblio = biblio;
            }
            else if (subCode is "11")
            {
                var commonMatch = Regex.Match(note, @"(?<pNum>RO\/EP\s?.+?)\s(?<aNum>\d.+)\s?\(73\)(?<assignees>.+)\s?\(73\)(?<newAssignees>.+)", RegexOptions.Singleline);

                if (commonMatch.Success)
                {
                    biblio.Publication.Number = commonMatch.Groups["pNum"].Value.Trim();
                    biblio.EuropeanPatents.Add(new EuropeanPatent()
                    {
                        AppNumber = commonMatch.Groups["aNum"].Value.Trim()
                    }); 

                    var oldAssigners = Regex
                        .Split(commonMatch.Groups["assignees"].Value.Replace("\r", "").Replace("\n", " ").Trim(), @";")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                    foreach (var oldAssigner in oldAssigners)
                    {
                        var match = Regex.Match(oldAssigner.Trim(), @"(?<name>.+?),(?<adress>.+)\(?(?<code>[A-Z]{2})");
                        if (match.Success)
                        {
                            biblio.Assignees.Add(new PartyMember()
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.TrimEnd(',').Trim(),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{oldAssigner} - not splited oldAssigner");
                    }

                    var newAssigners = Regex
                        .Split(commonMatch.Groups["newAssignees"].Value.Replace("\r", "").Replace("\n", " ").Trim(), @";")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                    foreach (var newAssigner in newAssigners)
                    {
                        var match = Regex.Match(newAssigner.Trim(), @"(?<name>.+?),(?<adress>.+)\s\(?(?<code>[A-Z]{2})");

                        if (match.Success)
                        {
                            legalStatus.NewBiblio.Assignees.Add(new PartyMember()
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.TrimEnd(',').Trim(),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else
                        {
                            var match2 = Regex.Match(newAssigner.Trim(), @"(?<name>.+?),(?<adress>.+)\s(?<code>.+)");

                            if (match2.Success)
                            {
                                var country = match2.Groups["adress"].Value.Trim();
                                if (country.Length != 2)
                                {
                                    country = country switch
                                    {
                                        "Switzerland" => "CH",
                                        _ => null
                                    };
                                }
                                legalStatus.NewBiblio.Assignees.Add(new PartyMember()
                                {
                                    Name = match2.Groups["name"].Value.Trim(),
                                    Address1 = match2.Groups["adress"].Value.TrimEnd(',').Trim(),
                                    Country = country
                                });
                            }
                            else Console.WriteLine($"{newAssigner} - not splited newAssigner");
                        }
                    }

                    var legalDate = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @".+(?<date>\d{8})_");
                    if (legalDate.Success)
                    {
                        legalStatus.LegalEvent.Date = legalDate.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                    }
                    legalStatus.Biblio = biblio;
                }
                else Console.WriteLine($"{note} - not split 11 sub");
            }
            else if (subCode is "12")
            {
                note = Clean(note);
                var commonMatch = Regex.Match(note, @"(?<pNum>RO\/EP\s?.+?)\s(?<aNum>\d.+)\s?\(74\)(?<agents>.+)\s?\(74\)(?<newAgents>.+)", RegexOptions.Singleline);

                if (commonMatch.Success)
                {
                    biblio.Publication.Number = commonMatch.Groups["pNum"].Value.Trim();
                    biblio.EuropeanPatents.Add(new EuropeanPatent()
                    {
                        AppNumber = commonMatch.Groups["aNum"].Value.Trim()
                    });

                    var oldAgents = Regex
                        .Split(commonMatch.Groups["agents"].Value.Replace("\r", "").Replace("\n", " ").Trim(), @";")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                    foreach (var oldAgent in oldAgents)
                    {
                        var match = Regex.Match(oldAgent.Trim(), @"(?<name>.+?),(?<adress>.+)");
                        if (match.Success)
                        {
                            biblio.Agents.Add(new PartyMember()
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{oldAgent} - not splited oldAgent");
                    }

                    var newAgents = Regex
                        .Split(commonMatch.Groups["newAgents"].Value.Replace("\r", "").Replace("\n", " ").Trim(), @";")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();
                    foreach (var newAgent in newAgents)
                    {
                        var match = Regex.Match(newAgent.Trim(), @"(?<name>.+?),(?<adress>.+)(Brevet)");
                        if (match.Success)
                        {
                            legalStatus.NewBiblio.Agents.Add(new PartyMember()
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim()
                            });
                        }
                        else
                        {
                            var match1 = Regex.Match(newAgent.Trim(), @"(?<name>.+?),(?<adress>.+)");
                            if (match1.Success)
                            {
                                legalStatus.NewBiblio.Agents.Add(new PartyMember()
                                {
                                    Name = match1.Groups["name"].Value.Trim(),
                                    Address1 = match1.Groups["adress"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{newAgent} - not splited newAgent");
                        }
                    }

                    var legalDate = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @".+(?<date>\d{8})_");
                    if (legalDate.Success)
                    {
                        legalStatus.LegalEvent.Date = legalDate.Groups["date"].Value.Insert(4,"/").Insert(7,"/").Trim();
                    }
                }
                else Console.WriteLine($"{note} - not split 12 sub");
                
                legalStatus.Biblio = biblio;
            }
            else if (subCode == "13")
            {
                biblio.IntConvention = new IntConvention();

                foreach (var inid in MakeInids13(note))
                {
                    if (inid.StartsWith(I11))
                    {
                        var match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<number>\d+)\s(?<kind>[A-Z]{1}\d+)");
                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["number"].Value.Trim();
                            biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился");
                    }
                    else
                    if (inid.StartsWith(I51))
                    {
                        biblio.Ipcs = new List<Ipc>();

                        var ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc, @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

                            if (match.Success)
                            {
                                biblio.Ipcs.Add(new Ipc
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["version"].Value.Trim()
                                });
                            }
                            else
                            {
                                biblio.Ipcs.Add(new Ipc
                                {
                                    Class = ipc.Trim(),
                                    Date = "2006.01"
                                });
                            }
                        }
                    }
                    else
                    if (inid.StartsWith(I21))
                    {
                        biblio.Application.Number = inid.Replace(I21, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I22))
                    {
                        biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                    }
                    else
                    if (inid.StartsWith(I41))
                    {
                        var match = Regex.Match(inid.Replace(I41, ""), @"([0-9]{2}\/[0-9]{2}\/[0-9]{4})");
                        if (match.Success)
                        {
                            biblio.DOfPublication = new()
                            {
                                date_41 = DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim()
                            };

                        }
                        else Console.WriteLine($"{inid} не разбился в 41");
                    }
                    else
                    if (inid.StartsWith(I71))
                    {
                        biblio.Applicants = new List<PartyMember>();
                        var applicants = Regex.Split(inid.Replace(I71, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var match = Regex.Match(applicant, @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                biblio.Applicants.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["country"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inid} не разбился 71");
                        }
                    }
                    else
                    if (inid.StartsWith(I72))
                    {
                        biblio.Inventors = new List<PartyMember>();

                        var inventors = Regex.Split(inid.Replace(I72, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            var match = Regex.Match(inventor, @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                biblio.Inventors.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["country"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inventor} не разбился в 72");
                        }
                    }
                    else
                    if (inid.StartsWith(I54))
                    {
                        biblio.Titles = new List<Title>
                        {
                            new Title
                            {
                                Text = inid.Replace(I54, "").Trim(),
                                Language = "RO"
                            }
                        };
                    }
                    else
                    if (inid.StartsWith(I57))
                    {
                        biblio.Abstracts = new List<Abstract>
                        {
                            new Abstract
                            {
                                Text = inid.Replace(I57,"").Trim(),
                                Language = "RO"
                            }
                        };
                    }
                    else
                    if (inid.StartsWith(I57n))
                    {
                        var match = Regex.Match(inid.Replace(I57n, "").Trim(), @"(?<r>R.+:)\s(?<rn>[0-9]+)\s(?<f>F.+:)\s(?<fn>[0-9]+)\s?");

                        if (match.Success)
                        {
                            legalStatus.LegalEvent = new LegalEvent
                            {
                                Note = "|| " + match.Groups["r"].Value.Trim() + " | " + match.Groups["rn"].Value.Trim() + "\n" + "|| " + match.Groups["f"].Value.Trim() + " | " + match.Groups["fn"].Value.Trim(),
                                Language = "RO",
                                Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| Claims | " + match.Groups["rn"].Value.Trim() + "\n" + "|| Figures | " + match.Groups["fn"].Value.Trim(),
                                        Type = "INID"
                                    }
                                }
                            };
                        }
                        else
                        {
                            var match1 = Regex.Match(inid.Replace(I57n, "").Trim(), @"(?<r>R.+:)\s(?<rn>[0-9]+)\s?");

                            if (match1.Success)
                            {
                                legalStatus.LegalEvent = new LegalEvent
                                {
                                    Note = "|| " + match.Groups["r"].Value.Trim() + " | " + match.Groups["rn"].Value.Trim(),
                                    Language = "RO",
                                    Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| Claims | " + match.Groups["rn"].Value.Trim(),
                                        Type = "INID"
                                    }
                                }
                                };
                            }
                            else Console.WriteLine($"{inid} не разбилось в 57n");
                        }
                    }
                    else
                    if (inid.StartsWith(I86))
                    {
                        var match = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<code>[A-Z]{2})\s(?<number>.+)\s(?<date>[0-9]{2}\/[0-9]{2}\/[0-9]{4})");

                        if (match.Success)
                        {
                            biblio.IntConvention.PctApplNumber = match.Groups["number"].Value.Trim();
                            biblio.IntConvention.PctApplCountry = match.Groups["code"].Value.Trim();
                            biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился в 86");
                    }
                    else
                    if (inid.StartsWith(I87))
                    {
                        var match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<number>.+)\s(?<date>[0-9]{2}\/[0-9]{2}\/[0-9]{4})");

                        if (match.Success)
                        {
                            biblio.IntConvention.PctPublNumber = match.Groups["number"].Value.Trim();
                            biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился в 87");
                    }
                    else
                    if (inid.StartsWith(I30))
                    {
                        var priorites = Regex.Split(inid.Replace(I30, "").Trim(), "").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        biblio.Priorities = new List<Priority>();

                        foreach (var priority in priorites)
                        {
                            var match = Regex.Match(priority.Trim(), @"(?<date>.+)\s(?<code>[A-Z]{2})\s(?<number>.+)");
                            if (match.Success)
                            {
                                biblio.Priorities.Add(new Priority
                                {
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim(),
                                    Country = match.Groups["code"].Value.Trim(),
                                    Number = match.Groups["number"].Value.Trim()
                                });
                            }
                        }

                    }
                    else
                    if (inid.StartsWith(I74))
                    {
                        var match = Regex.Match(inid.Replace(I74, "").Trim(), @"(?<name>.+?),\s(?<adress>.+)");
                        if (match.Success)
                        {
                            biblio.Agents = new List<PartyMember>
                            {
                                new PartyMember
                                {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = "RO"
                                }
                            };
                        }
                        else Console.WriteLine($"{inid} не разбился 74");
                    }
                    else Console.WriteLine($"{inid} не обработан");
                }

                legalStatus.Biblio = biblio;

            }
            else if (subCode == "14")
            {
                biblio.DOfPublication = new DOfPublication();

                biblio.IntConvention = new IntConvention();

                foreach (var inid in MakeInids14(note))
                {
                    if (inid.StartsWith(I11))
                    {
                        var match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<number>\d+)\s(?<kind>[A-Z]{1}\d+)");
                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["number"].Value.Trim();
                            biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился");
                    }
                    else
                    if (inid.StartsWith(I51))
                    {
                        biblio.Ipcs = new List<Ipc>();

                        var ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc, @"(?<class>.+)\s?\((?<version>[0-9]{4}.[0-9]{2})\)?");

                            if (match.Success)
                            {
                                biblio.Ipcs.Add(new Ipc
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["version"].Value.Trim()
                                });
                            }
                            else
                            {
                                Console.WriteLine($"{ipc} не разбился в 51");
                            }
                        }
                    }
                    else
                    if (inid.StartsWith(I21))
                    {
                        biblio.Application.Number = inid.Replace(I21, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I22))
                    {
                        biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                    }
                    else
                    if (inid.StartsWith(I41))
                    {
                        var match = Regex.Match(inid.Replace(I41, ""), @"([0-9]{2}\/[0-9]{2}\/[0-9]{4})");
                        if (match.Success)
                        {
                            biblio.DOfPublication.date_41 = DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился в 41");
                    }
                    else
                    if (inid.StartsWith(I45))
                    {
                        var match = Regex.Match(inid.Replace(I45, ""), @"([0-9]{2}\/[0-9]{2}\/[0-9]{4})");
                        if (match.Success)
                        {
                            biblio.DOfPublication.date_45 = DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        }
                        else Console.WriteLine($"{inid} не разбился в 45");
                    }
                    else
                    if (inid.StartsWith(I56))
                    {
                        var patentCitations = Regex.Split(inid.Replace(I56, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        biblio.PatentCitations = new List<PatentCitation>();

                        foreach (var patentCitation in patentCitations)
                        {
                            var match = Regex.Match(patentCitation, @"(?<code>[A-Z]{2})\s(?<number>.+)\s(?<kind>[A-Z]{1,2}[0-9]{0,2})");

                            if (match.Success)
                            {
                                biblio.PatentCitations.Add(new PatentCitation
                                {
                                    Kind = match.Groups["kind"].Value.Trim(),
                                    Number = match.Groups["number"].Value.Trim(),
                                    Authority = match.Groups["code"].Value.Trim()
                                });
                            }
                            else
                            {
                                var match1 = Regex.Match(patentCitation, @"(?<code>[A-Z]{2}).?\s(?<number>.+)");

                                if (match1.Success)
                                {
                                    biblio.PatentCitations.Add(new PatentCitation
                                    {
                                        Number = match1.Groups["number"].Value.Trim(),
                                        Authority = match1.Groups["code"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    var match2 = Regex.Match(patentCitation, @"(?<number>.+)\s(?<kind>[A-Z]{1,2}[0-9]{0,2})");
                                    if (match2.Success)
                                    {
                                        biblio.PatentCitations.Add(new PatentCitation
                                        {
                                            Number = match2.Groups["number"].Value.Trim(),
                                            Kind = match2.Groups["kind"].Value.Trim(),
                                        });
                                    }
                                    else
                                    {
                                        biblio.PatentCitations.Add(new PatentCitation
                                        {
                                            Number = patentCitation

                                        });
                                    }

                                }
                            }
                        }
                    }
                    else
                    if (inid.StartsWith(I73))
                    {
                        biblio.Assignees = new List<PartyMember>();

                        var match = Regex.Match(inid.Replace(I73, "").Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

                        if (match.Success)
                        {
                            biblio.Assignees.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = match.Groups["country"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} Не разбился в 73");
                    }
                    else
                    if (inid.StartsWith(I72))
                    {
                        biblio.Inventors = new List<PartyMember>();

                        var inventors = Regex.Split(inid.Replace(I72, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            var match = Regex.Match(inventor, @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                biblio.Inventors.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["country"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inventor} не разбился в 72 --- {biblio.Publication.Number}");
                        }
                    }
                    else
                    if (inid.StartsWith(I54))
                    {
                        biblio.Titles = new List<Title>
                        {
                            new Title
                            {
                                Text = inid.Replace(I54, "").Trim(),
                                Language = "RO"
                            }
                        };
                    }
                    else
                    if (inid.StartsWith(I74))
                    {
                        var match = Regex.Match(inid.Replace(I74, "").Trim(), @"(?<name>.+?),\s(?<adress>.+)");
                        if (match.Success)
                        {
                            biblio.Agents = new List<PartyMember>
                            {
                                new PartyMember
                                {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = "RO"
                                }
                            };
                        }
                        else Console.WriteLine($"{inid} не разбился 74");
                    }
                    else
                    if (inid.StartsWith(I86))
                    {
                        var match = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<number>.+)\s?(?<date>[0-9]{2}\/[0-9]{2}\/[0-9]{4})");

                        if (match.Success)
                        {
                            biblio.IntConvention.PctApplNumber = match.Groups["number"].Value.Trim();
                            biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился в 86");
                    }
                    else
                    if (inid.StartsWith(I87))
                    {
                        var match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<number>.+)\s?(?<date>[0-9]{2}\/[0-9]{2}\/[0-9]{4})");

                        if (match.Success)
                        {
                            biblio.IntConvention.PctPublNumber = match.Groups["number"].Value.Trim();
                            biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился в 87");
                    }
                    else
                    if (inid.StartsWith(I66))
                    {
                        biblio.Related = new List<RelatedDocument>();

                        var match = Regex.Match(inid.Replace(I66, "").Trim(), @"(?<date>[0-9]{2}\/[0-9]{2}\/[0-9]{4})\s?(?<country>[A-Z]{2})\s(?<number>.+)");

                        if (match.Success)
                        {
                            biblio.Related.Add(new RelatedDocument
                            {
                                Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim(),
                                Number = match.Groups["number"].Value.Trim(),
                                Format = "66",
                                Source = match.Groups["country"].Value.Trim()
                            });
                        }
                    }
                    else Console.WriteLine($"{inid} не обработан");
                }

                legalStatus.Biblio = biblio;

            }
            else if (subCode == "20")
            {
                biblio.EuropeanPatents = new();
                EuropeanPatent europeanPatent = new();

                foreach (var inid in MakeInids16(note))
                {
                    if (inid.StartsWith(I51))
                    {
                        var ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

                            if (match.Success)
                            {
                                biblio.Ipcs.Add(new Ipc
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["version"].Value.Trim()
                                });
                            }
                            else
                            {
                                biblio.Ipcs.Add(new Ipc
                                {
                                    Class = ipc.Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith(I11))
                    {
                        var match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<number>.+)\s(?<kind>[A-Z]{1}\d+)");

                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["number"].Value.Trim();
                            biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился");
                    }
                    else if (inid.StartsWith(I21))
                    {
                        biblio.Application.Number = inid.Replace(I21, "").Trim();
                    }
                    else if (inid.StartsWith(I22))
                    {
                        biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Replace(";", "").Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith(I68))
                    {
                        var match = Regex.Match(inid.Replace(I68, "").Trim(), @"(?<num>.+)\/(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            biblio.Related.Add(new RelatedDocument
                            {
                                Number = match.Groups["num"].Value.Trim(),
                                Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
                                Source = "68"
                            });
                        }
                        else Console.WriteLine($"{inid}  ----- 68");
                    }
                    else if (inid.StartsWith(I54))
                    {
                        biblio.Titles.Add(new Title
                        {
                            Language = "RO",
                            Text = inid.Replace(I54, "").Trim().TrimEnd(';')
                        });
                    }
                    else if (inid.StartsWith(I92))
                    {
                        var match = Regex.Match(inid.Replace(I92, "").Trim(), @"(?<num>.+)\/\/(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {

                            europeanPatent.Spc92Number = match.Groups["num"].Value.Trim();
                            europeanPatent.Spc92Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            
                        }
                        else Console.WriteLine($"{inid}  ---- 92");
                    }
                    else if (inid.StartsWith(I93))
                    {
                        var match = Regex.Match(inid.Replace(I93, "").Trim(), @"(?<num>.+),\s(?<date>\d{2}\/\d{2}\/\d{4})\s?\/(?<note>.+);");

                        if (match.Success)
                        {
                            europeanPatent.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            europeanPatent.Number = match.Groups["num"].Value.Trim();


                            var match1 = Regex.Match(match.Groups["note"].Value.Trim(), @"(?<text>.+?)(?<date>\d{2}.\d{2}.\d{4});(?<text1>.+?)(?<num>\d{1,5}\/\d{4})(?<text2>.+)");

                            if (match1.Success)
                            {
                                legalStatus.LegalEvent = new LegalEvent
                                {
                                    Note = "|| (93) | " + match1.Groups["text"].Value.Trim() + " | " + DateTime.Parse(match1.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim() +
                                    " || " + match1.Groups["text1"].Value.Trim() + " " + match1.Groups["num"].Value.Trim() + " " + match1.Groups["text2"].Value.Trim().TrimEnd(';'),
                                    Language = "RO",
                                    Translations = new List<NoteTranslation> {
                                        new NoteTranslation
                                        {
                                            Language = "EN",
                                            Type = "INID",
                                            Tr = "|| (93) | Notification date | " + DateTime.Parse(match1.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim() +
                                                " || Application for the supplementary protection certificate was made on the basis of Regulation (EC) No " + match1.Groups["num"].Value.Trim() +
                                                " of the European Parliament and of the Council concerning the supplementary protection certificate for medicinal products"
                                        }
                                    }
                                };
                            }
                            else 
                            {
                                var match2 = Regex.Match(match.Groups["note"].Value.Trim(), @"(?<text1>Sol.+?)(?<num>\d{1,5}\/\d{1,4})(?<text2>.+)");

                                if (match2.Success)
                                {
                                    legalStatus.LegalEvent = new LegalEvent
                                    {
                                        Note = "|| " + match2.Groups["text1"].Value.Trim() + " " + match2.Groups["num"].Value.Trim() + " " + match2.Groups["text2"].Value.Trim().TrimEnd(';'),
                                        Language = "RO",
                                        Translations = new List<NoteTranslation> {
                                        new NoteTranslation
                                        {
                                            Language = "EN",
                                            Type = "INID",
                                            Tr = " || Application for the supplementary protection certificate was made on the basis of Regulation (EC) No " + match2.Groups["num"].Value.Trim() +
                                                " of the European Parliament and of the Council concerning the supplementary protection certificate for medicinal products"
                                        }
                                    }
                                    };
                                }
                                else Console.WriteLine($"{match.Groups["note"].Value.Trim()}");
                            } 
                        }
                        else Console.WriteLine($"{inid} ---- 93");
                    }
                    else if (inid.StartsWith(I71))
                    {
                        var applicants = Regex.Split(inid.Replace(I71, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var match = Regex.Match(applicant, @"(?<name>.+INC\.|.+Inc\.|.+Limited|.+LIMITED),\s(?<adress>.+),\s(?<code>\D{2})");

                            if (match.Success)
                            {
                                biblio.Applicants.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim()
                                });
                            }
                            else
                            {
                                var match1 = Regex.Match(applicant, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>\D{2})");

                                if (match1.Success)
                                {
                                    biblio.Applicants.Add(new PartyMember
                                    {
                                        Name = match1.Groups["name"].Value.Trim(),
                                        Country = match1.Groups["code"].Value.Trim(),
                                        Address1 = match1.Groups["adress"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{applicant} --- 71");
                            }
                        }
                    }
                    else if (inid.StartsWith(I74))
                    {
                        var match = Regex.Match(inid.Replace(I74, "").Trim(), @"(?<name>.+?),\s(?<adress>.+)");

                        if (match.Success)
                        {
                            biblio.Agents.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim().TrimEnd(';')
                            });
                        }
                        else Console.WriteLine($"{inid} ---- 74");
                    }
                    else if (inid.StartsWith(I95))
                    {
                        europeanPatent.Patent = inid.Replace(I95, "").Trim();
                    }

                    else Console.WriteLine($"{inid} Не обработан");
                }

                biblio.EuropeanPatents.Add(europeanPatent);
                legalStatus.Biblio = biblio;

                var date = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"\d{8}");
                if (date.Success)
                {
                    legalStatus.LegalEvent.Date = date.Value.Insert(4, "/").Insert(7, "/").Trim();
                }
            }
            else if (subCode == "22")
            {
                foreach (var inid in MakeInids(note,subCode))
                {
                    if (inid.StartsWith(I51))
                    {
                        var ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>\d{4}.\d{2})\)?");

                            if (match.Success)
                            {
                                biblio.Ipcs.Add(new Ipc
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["version"].Value.Trim()
                                });
                            }
                            else
                            {
                                biblio.Ipcs.Add(new Ipc
                                {
                                    Class = ipc.Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith(I11))
                    {
                        var match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<number>.+)\s(?<kind>[A-Z]{1}\d+)");

                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["number"].Value.Trim();
                            biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился");
                    }
                    else if (inid.StartsWith(I21))
                    {
                        biblio.Application.Number = inid.Replace(I21, "").Trim();
                    }
                    else if (inid.StartsWith(I22))
                    {
                        biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Replace(";", "").Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith(I73))
                    {
                        var match = Regex.Match(inid.Replace(I73, "").Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<code>\D{2})");

                        if (match.Success)
                        {
                            biblio.Assignees.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 73");
                    }
                    else if (inid.StartsWith(I72))
                    {
                        var match = Regex.Match(inid.Replace(I72, "").Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<code>\D{2})");

                        if (match.Success)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = match.Groups["code"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 73");
                    }
                    else if (inid.StartsWith(I54))
                    {
                        biblio.Titles.Add(new Title
                        {
                            Language = "RO",
                            Text = inid.Replace(I54, "").Trim().TrimEnd(';')
                        });
                    }
                    else if (inid.StartsWith(I57))
                    {
                        biblio.Abstracts.Add(new Abstract
                        {
                            Language = "RO",
                            Text = inid.Replace(I57, "").Trim()
                        });
                    }
                    else if (inid.StartsWith(I57n))
                    {
                        var match = Regex.Match(inid.Replace(I57n, "").Trim(), @"(?<rev>Rev.+?):\s?(?<numCl>\d+)\s?(?<fig>Fig.+):\s?(?<numFig>\d+)");

                        if (match.Success)
                        {
                            legalStatus.LegalEvent = new LegalEvent
                            {
                                Note = "|| " + match.Groups["rev"].Value.Trim() + " | " + match.Groups["numCl"].Value.Trim() + "\n" + "|| " + match.Groups["fig"].Value.Trim() + " | " + match.Groups["numFig"].Value.Trim(),
                                Language = "RO",
                                Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| Claims | " + match.Groups["numCl"].Value.Trim() + "\n" + "|| Figures | " + match.Groups["numFig"].Value.Trim(),
                                        Type = "INID"
                                    }
                                }
                            };
                        }
                    }
                    else if (inid.StartsWith(I45))
                    {
                        biblio.DOfPublication.date_45 = DateTime.Parse(inid.Replace(I45,"").Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".","/").Trim();
                    }
                    else if (inid.StartsWith(I67))
                    {
                        biblio.Related.Add(new RelatedDocument
                        {
                            Number = inid.Replace(I67, "").Trim(),
                            Source = "67"
                        });
                    }
                    else Console.WriteLine($"{inid} - not process");
                }
                legalStatus.Biblio = biblio;
            }
            else if (subCode == "23")
            {
                var match = Regex.Match(note.Trim(), @"Brevet\s(?<owner>.+)\s(?<pNum>\D{2}\/\D{2}\s\d+)", RegexOptions.Singleline);
                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["pNum"].Value.Trim();

                    var owners = Regex.Split(match.Groups["owner"].Value.Replace("\r", "").Replace("\n", " ").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var owner in owners)
                    {
                        var match1 = Regex.Match(owner, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>[A-Z]{2})");

                        if (match1.Success)
                        {
                            biblio.Assignees.Add(new PartyMember
                            {
                                Name = match1.Groups["name"].Value.Trim(),
                                Address1 = match1.Groups["adress"].Value.Trim(),
                                Country = match1.Groups["code"].Value.Trim()
                            });
                        }
                    }

                    var match2 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"\d{8}");
                    if (match2.Success)
                    {
                        legalStatus.LegalEvent.Date = match2.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }

                }
                else
                {
                    var match1 = Regex.Match(note, @"(?<owner>.+)\s(?<pNum>\D{2}\/\D{2}\s\d+)", RegexOptions.Singleline);

                    if (match1.Success)
                    {
                        biblio.Publication.Number = match1.Groups["pNum"].Value.Trim();

                        var owners = Regex.Split(match1.Groups["owner"].Value.Replace("\r", "").Replace("\n", " ").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var owner in owners)
                        {
                            var match3 = Regex.Match(owner, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>[A-Z]{2})");

                            if (match3.Success)
                            {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = match3.Groups["name"].Value.Trim(),
                                    Address1 = match3.Groups["adress"].Value.Trim(),
                                    Country = match3.Groups["code"].Value.Trim()
                                });
                            }
                        }

                        var match2 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"\d{8}");
                        if (match2.Success)
                        {
                            legalStatus.LegalEvent.Date = match2.Value.Insert(4, "/").Insert(7, "/").Trim();
                        }
                    }
                    else Console.WriteLine($"{note}");
                }
                legalStatus.Biblio = biblio;
            }
            else if (subCode == "24")
            {
                var match11 = Regex.Match(note.Trim(), @"Nr. CBI\s(?<name>.+?)\s(?<pNum>\d+)\s(?<aNum>.+)", RegexOptions.Singleline);
                if (match11.Success)
                {
                    biblio.Publication.Number = match11.Groups["pNum"].Value.Trim();
                    biblio.Application.Number = match11.Groups["aNum"].Value.Trim();

                    var owners = Regex.Split(match11.Groups["name"].Value.Replace("\r", "").Replace("\n", " ").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var owner in owners)
                    {
                        var match1 = Regex.Match(owner, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>[A-Z]{2})");

                        if (match1.Success)
                        {
                            biblio.Assignees.Add(new PartyMember
                            {
                                Name = match1.Groups["name"].Value.Trim(),
                                Address1 = match1.Groups["adress"].Value.Trim(),
                                Country = match1.Groups["code"].Value.Trim()
                            });
                        }
                    }

                    var match2 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"\d{8}");
                    if (match2.Success)
                    {
                        legalStatus.LegalEvent.Date = match2.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }
                }
                else
                {
                    var match = Regex.Match(note.Trim(), @"(?<name>.+?)\s(?<pNum>\d+)\s(?<aNum>.+)", RegexOptions.Singleline);
                    if (match.Success)
                    {
                        biblio.Publication.Number = match.Groups["pNum"].Value.Trim();
                        biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                        var owners = Regex.Split(match.Groups["name"].Value.Replace("\r", "").Replace("\n", " ").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var owner in owners)
                        {
                            var match1 = Regex.Match(owner, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>[A-Z]{2})");

                            if (match1.Success)
                            {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = match1.Groups["name"].Value.Trim(),
                                    Address1 = match1.Groups["adress"].Value.Trim(),
                                    Country = match1.Groups["code"].Value.Trim()
                                });
                            }
                        }

                        var match2 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"\d{8}");
                        if (match2.Success)
                        {
                            legalStatus.LegalEvent.Date = match2.Value.Insert(4, "/").Insert(7, "/").Trim();
                        }
                    }
                    else Console.WriteLine($"{note}");
                }
                legalStatus.Biblio = biblio;
            }
            else if(subCode == "27" || subCode == "29")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<aNum>.+)\s.+\/(?<leDate>\d{2}\/\d{2}\/\d{4})\s(?<leNote>\d{1,2}\/\d{4})\/\/(?<date41>\d{2}\/\d{2}\/\d{4})");

                if (match.Success)
                {
                    biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                    legalStatus.LegalEvent.Note = "|| Numărul BOPI în care este publicată cererea de brevet de inventive | " + match.Groups["leNote"].Value.Trim();
                    legalStatus.LegalEvent.Language = "RO";
                    legalStatus.LegalEvent.Translations = new()
                    {
                        new NoteTranslation
                        {
                            Language = "EN",
                            Tr = "|| Number of the Official Bulletin of Industrial Property in which the patent application was published | " + match.Groups["leNote"].Value.Trim(),
                            Type = "INID"
                        }
                    };

                    legalStatus.LegalEvent.Date = DateTime.Parse(match.Groups["leDate"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();

                    biblio.DOfPublication.date_41 = DateTime.Parse(match.Groups["date41"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();

                }
                else
                {
                    var match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<aNum>.+)\s.+\/(?<leDate>\d{2}\/\d{2}\/\d{4})");
                    if (match1.Success)
                    {
                        biblio.Application.Number = match1.Groups["aNum"].Value.Trim();
                        legalStatus.LegalEvent.Date = DateTime.Parse(match1.Groups["leDate"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                    }
                    else Console.WriteLine($"{note}");
                }

                legalStatus.Biblio = biblio;
            }
            return legalStatus;
        }
        internal Diamond.Core.Models.LegalStatusEvent GetPatent16SubCode(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent record = new()
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "RO",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                LegalEvent = new(),
                Biblio = new()
                {
                    IntConvention = new IntConvention()
                }
            };

            var cultureInfo = new CultureInfo("ru-Ru");
            var europeanPatent = new EuropeanPatent();

            foreach (var rawRecord in MakeInids16(note))
            {
                var cleanRawRecord = Clean(rawRecord).Trim();

                if (rawRecord.StartsWith(I51))
                {
                    var ipcs = Regex.Split(cleanRawRecord, @",").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var ipc in ipcs)
                    {
                        var match = _classificationPattern.Match(ipc.Trim());
                        if (match.Success)
                        {
                            record.Biblio.Ipcs.Add(new Ipc
                            {
                                Class = match.Groups["Classification"].Value.Trim(),
                                Date = match.Groups["Version"].Value.Trim()
                            });
                        }
                    }
                }
                if (rawRecord.StartsWith(I11))
                {
                    var match = _publicationInfoPattern.Match(cleanRawRecord);
                    if (match.Success)
                    {
                        record.Biblio.Publication.Number = match.Groups["PubNumber"].Value;
                        record.Biblio.Publication.Kind = match.Groups["PubKind"].Value;
                    }
                }
                if (rawRecord.StartsWith(I96))
                {
                    var match = _europeanAppPattern.Match(cleanRawRecord);
                    if (match.Success)
                    {
                        europeanPatent.AppNumber = match.Groups["AppNumber"].Value;
                        europeanPatent.AppDate = DateTime.Parse(match.Groups["AppDate"].Value, cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                }
                if (rawRecord.StartsWith(I97))
                {
                    var match = _europeanPubPattern.Match(cleanRawRecord);
                    if (match.Success)
                    {
                        europeanPatent.PubNumber = match.Groups["PubNumber"].Value;

                        record.LegalEvent.Note = @"|| Date of publication of EP application | " +
                                                 DateTime.Parse(match.Groups["NoteDate"].Value, cultureInfo)
                                                     .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        record.LegalEvent.Language = "EN";
                    }

                }
                if (rawRecord.StartsWith(I30))
                {
                    var priorities = Regex.Split(cleanRawRecord, @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var priority in priorities)
                    {
                        var match = _priorityPattern.Match(priority.Trim());
                        if (match.Success)
                        {
                            record.Biblio.Priorities.Add(new Priority()
                            {
                                Country = match.Groups["CountryPriority"].Value.Trim(),
                                Number = match.Groups["NumberPriority"].Value.Trim(),
                                Date = DateTime.Parse(match.Groups["DatePriority"].Value, cultureInfo)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                            });
                        }
                    }
                }
                if (rawRecord.StartsWith(I80))
                {
                    var match = _datePattern.Match(cleanRawRecord);
                    if (match.Success)
                    {
                        europeanPatent.PubDate =
                            DateTime.Parse(match.Value, cultureInfo)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                }
                if (rawRecord.StartsWith(I84))
                {
                    var designatedStates = Regex.Split(cleanRawRecord, @",").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var state in designatedStates)
                    {
                        var match = Regex.Match(state.Trim(), @"[A-Z]{2}");
                        if (match.Success)
                        {
                            record.Biblio.IntConvention.DesignatedStates.Add(match.Value);
                        }
                    }
                }
                if (rawRecord.StartsWith(I87))
                {
                    var match = _intConventionPattern.Match(cleanRawRecord);
                    if (match.Success)
                    {
                        record.Biblio.IntConvention.PctPublNumber = match.Groups["PctPubNumber"].Value;
                        record.Biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["PctPubDate"].Value, cultureInfo)
                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                }
                if (rawRecord.StartsWith(I73))
                {
                    record.Biblio.Assignees = ParsePersons(cleanRawRecord);
                }
                if (rawRecord.StartsWith(I72))
                {
                    record.Biblio.Inventors = ParsePersons(cleanRawRecord);
                }
                if (rawRecord.StartsWith(I74))
                {
                    record.Biblio.Agents = ParsePersons(cleanRawRecord);
                }
                if (rawRecord.StartsWith(I54))
                {
                    record.Biblio.Titles.Add(new Title
                    {
                        Text = cleanRawRecord,
                        Language = "RO"
                    });
                }
            }
            record.Biblio.EuropeanPatents.Add(europeanPatent);

            return record;
        }
        internal Diamond.Core.Models.LegalStatusEvent GetPatent17SubCode(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent record = new()
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "RO",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                LegalEvent = new(),
                Biblio = new()
                {
                    IntConvention = new IntConvention()
                }
            };

            var cultureInfo = new CultureInfo("ru-Ru");
            var europeanPatent = new EuropeanPatent();
            var notesRO = new Dictionary<string, string>();
            var notesEN = new Dictionary<string, string>();

            foreach (var rawRecord in MakeInids16(note))
            {
                var cleanRawRecord = Clean(rawRecord).Trim();

                if (rawRecord.StartsWith(I51))
                {
                    var ipcs = Regex.Split(cleanRawRecord, @",").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var ipc in ipcs)
                    {
                        var match = _classificationPattern.Match(ipc.Trim());
                        if (match.Success)
                        {
                            record.Biblio.Ipcs.Add(new Ipc
                            {
                                Class = match.Groups["Classification"].Value.Trim(),
                                Date = match.Groups["Version"].Value.Trim()
                            });
                        }
                    }
                }
                if (rawRecord.StartsWith(I11))
                {
                    var match = _publicationInfoPattern.Match(cleanRawRecord);
                    if (match.Success)
                    {
                        record.Biblio.Publication.Number = match.Groups["PubNumber"].Value;
                        record.Biblio.Publication.Kind = match.Groups["PubKind"].Value;
                    }
                }
                if (rawRecord.StartsWith(I96))
                {
                    var match = _europeanAppPattern.Match(cleanRawRecord);
                    if (match.Success)
                    {
                        europeanPatent.AppNumber = match.Groups["AppNumber"].Value;
                        europeanPatent.AppDate = DateTime.Parse(match.Groups["AppDate"].Value, cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                }
                if (rawRecord.StartsWith(I97))
                {
                    var match = Regex.Match(cleanRawRecord, @"(?<PubNumber>.+)\s(?<NoteDate>\d{2}.\d{2}.\d{4})");
                    if (match.Success)
                    {
                        europeanPatent.PubNumber = match.Groups["PubNumber"].Value;

                        var date = DateTime.Parse(match.Groups["NoteDate"].Value, cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        notesRO.Add("|| ", date);
                        notesEN.Add("|| Date of publication of EP application | ", date);
                    }
                }
                if (rawRecord.StartsWith(I80))
                {
                    var match = Regex.Match(cleanRawRecord, @"european:\s(?<Date>\d{2}.\d{2}.\d{4});(?<Note>.+):\s(?<DateNote>\d{2}.\d{2}.\d{4})");
                    if (match.Success)
                    {
                        europeanPatent.PubDate =
                            DateTime.Parse(match.Groups["Date"].Value, cultureInfo)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        var date = DateTime.Parse(match.Groups["DateNote"].Value, cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        notesRO.Add("|| " + match.Groups["Note"].Value.Trim() + " | ", date);
                        notesEN.Add("|| Date of publication by the European patent office of the mention of maintaining the European patent in modified form | ", date);

                    }
                }
                if (rawRecord.StartsWith(I30))
                {
                    var priorities = Regex.Split(cleanRawRecord, @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var priority in priorities)
                    {
                        var match = _priorityPattern.Match(priority.Trim());
                        if (match.Success)
                        {
                            record.Biblio.Priorities.Add(new Priority()
                            {
                                Country = match.Groups["CountryPriority"].Value,
                                Number = match.Groups["NumberPriority"].Value,
                                Date = DateTime.Parse(match.Groups["DatePriority"].Value, cultureInfo)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                            });
                        }
                    }
                }
                if (rawRecord.StartsWith(I84))
                {
                    var designatedStates = Regex.Split(cleanRawRecord, @",").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var state in designatedStates)
                    {
                        var match = Regex.Match(state.Trim(), @"[A-Z]{2}");
                        if (match.Success)
                        {
                            record.Biblio.IntConvention.DesignatedStates.Add(match.Value);
                        }
                    }
                }
                if (rawRecord.StartsWith(I74))
                {
                    record.Biblio.Agents = ParsePersons(cleanRawRecord);
                }
                if (rawRecord.StartsWith(I54))
                {
                    record.Biblio.Titles.Add(new Title
                    {
                        Text = cleanRawRecord,
                        Language = "RO"
                    });
                }
                if (rawRecord.StartsWith(I73))
                {
                    var assignees = Regex.Split(cleanRawRecord, @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var assignee in assignees)
                    {
                        var match = Regex.Match(assignee, @"(?<Name>.+?),\s(?<Adress>.+),\s(?<Country>\D{2})$");

                        if (match.Success)
                        {
                            record.Biblio.Assignees.Add(new PartyMember()
                            {
                                Name = match.Groups["Name"].Value,
                                Address1 = match.Groups["Adress"].Value,
                                Country = match.Groups["Country"].Value
                            });
                        }
                    }
                }
                if (rawRecord.StartsWith(I72))
                {
                    var inventors = Regex.Split(cleanRawRecord, @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var inventor in inventors)
                    {
                        var match = Regex.Match(inventor, @"(?<Name>.+?,.+?),\s(?<Adress>.+),\s(?<Country>\D{2})$");

                        if (match.Success)
                        {
                            record.Biblio.Inventors.Add(new PartyMember()
                            {
                                Name = match.Groups["Name"].Value,
                                Address1 = match.Groups["Adress"].Value,
                                Country = match.Groups["Country"].Value
                            });
                        }

                    }
                }
                if (rawRecord.StartsWith(I45))
                {
                    var notesTmp = Regex.Split(cleanRawRecord, @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    for (var i = 0; i < notesTmp.Count; i++)
                    {
                        var match = Regex.Match(notesTmp[i].Trim(), @"(?<Text>.+):\s(?<Date>\d{2}.\d{2}.\d{4})");
                        if (match.Success)
                        {
                            var date = DateTime.Parse(match.Groups["Date"].Value, cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            if (i == 0)
                            {
                                notesRO.Add("|| " + match.Groups["Text"].Value + " | ", date);
                                notesEN.Add("|| (45) Date of publication of the translation of the European patent dossier maintained in modified form | ", date);
                            }

                            if (i == 1)
                            {
                                notesRO.Add("|| " + match.Groups["Text"].Value + " | ", date);
                                notesEN.Add("|| Date of publication of the translation of the European patent dossier | ", date);
                            }
                        }
                    }
                }
            }

            var sbRO = new StringBuilder();
            var sbEN = new StringBuilder();

            foreach (var entry in notesRO)
            {
                sbRO = sbRO.Append(entry.Key).Append(entry.Value).AppendLine();
            }

            foreach (var entry in notesEN)
            {
                sbEN = sbEN.Append(entry.Key).Append(entry.Value).AppendLine();
            }

            record.LegalEvent.Note = sbRO.ToString().TrimEnd();
            record.LegalEvent.Language = "RO";

            record.LegalEvent.Translations.Add(new NoteTranslation()
            {
                Language = "EN",
                Type = "INID",
                Tr = sbEN.ToString().TrimEnd()
            });

            return record;
        }
        internal List<string> MakeInids13 (string note)
        {
            var inid57 = note.Substring(note.IndexOf("(57)")).Replace("\r","").Replace("\n", " ").Trim();

            string note57 = null;

            List<string> inids = new();

            if (inid57.Contains("(11)"))
            {
                inid57 = Regex.Replace(inid57, @"(\(11\)\s[0-9]{6}\s[A-Z]{1}[0-9]{1}\s)", "");

            }

            var match = Regex.Match(inid57, @"(?<inid57>.+)\s(?<note>Revend.+)");
            if (match.Success)
            {
                inid57 = match.Groups["inid57"].Value.Trim();
                note57 ="(57n) " + match.Groups["note"].Value.Trim();
            }
            else
            {
                Console.WriteLine($"{inid57} не нашло note57");
            }

            var noteWithOut57 = Regex.Replace(note.Replace("\r", "").Replace("\n", " "), @"(\(57\).+)", "").Trim();

            var match1 = Regex.Match(noteWithOut57, @"(?<inid11>\(11\).+)\s(?<note>\(51.+)");

            if (match1.Success)
            {
                var tmp = Regex.Replace(match1.Groups["note"].Value.Trim(), @"\(11\)\s?[0-9]+\s[A-Z]+[0-9]+", "").Trim();

                inids = Regex.Split(tmp, @"(?=\([0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                inids.Add(match1.Groups["inid11"].Value.Trim());
            }   

            inids.Add(inid57);
            inids.Add(note57);

            return inids;
        }
        internal List<string> MakeInids14(string note)
        {
            var inid11 = note.Replace("\r", "").Replace("\n", " ").Trim().Substring(note.IndexOf("(11)"), note.IndexOf("(51)")).Trim();

            var inidsWithOutInid11 = Regex.Replace(note.Replace("\r", "").Replace("\n", " ").Trim(), @"\(11\)\s[0-9]{6,7}\s[A-Z][0-9]","").Trim();

            var inids = Regex.Split(inidsWithOutInid11, @"(?=\([0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

            inids.Add(inid11);

            return inids;
        }
        internal List<string> MakeInids16(string note) => Regex.Split(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?=\([0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
        internal List<string> MakeInids (string note, string subCode)
        {
            List<string> inids = new();

            if (subCode == "22")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<inids>.+)\s(?<inid57>\(57\).+)\s(?<note>Reve.+)\(.+(?<inid45>\d{2}.\d{2}.\d{4})");

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["inids"].Value.Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    inids.Add(match.Groups["inid57"].Value.Trim());
                    inids.Add("(57n) " + match.Groups["note"].Value.Trim());
                    inids.Add("(45) " + match.Groups["inid45"].Value.Trim());

                    return inids;
                }
                else Console.WriteLine($"{note} -- not split");
            }

            return inids;
        }
        public string Clean(string str)
        {
            str = str.Replace("ª", "Ş")
                .Replace("³", "ł")
                .Replace("¹", "ą")
                //.Replace(, "ę")
                .Replace("º", "ş")
                .Replace("Þ", "Ţ");
            return Regex.Replace(str, @"^\(\d{2}\)\s", string.Empty).Trim();
        }
        private List<PartyMember> ParsePersons(string str)
        {
            var persons = new List<PartyMember>();

            var personsTmp = Regex.Split(str, @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();

            foreach (var assignee in personsTmp)
            {
                var match = _personPattern.Match(assignee);

                if (match.Success)
                {
                    persons.Add(new PartyMember()
                    {
                        Name = match.Groups["Name"].Value,
                        Address1 = match.Groups["Adress"].Value,
                        Country = match.Groups["Country"].Value
                    });
                }
                else
                {
                    var matchAgent = Regex.Match(assignee, @"(?<Name>.+),\s(?<Adress>.+)");
                    if (matchAgent.Success)
                    {
                        persons.Add(new PartyMember()
                        {
                            Name = matchAgent.Groups["Name"].Value,
                            Address1 = matchAgent.Groups["Adress"].Value,
                            Country = "RO"
                        });
                    }
                }
            }
            return persons;
        }
    }
}
