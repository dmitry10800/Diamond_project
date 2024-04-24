using Integration;
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
using DiamondProjectClasses;

namespace Diamond_BG_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        private const string I11 = "(11)";
        private const string I51= "(51)";
        private const string I52= "(52)";
        private const string I21 = "(21)";
        private const string I22 = "(22)";
        private const string I24 = "(24)";
        private const string I31 = "(31)";
        private const string I32 = "(32)";
        private const string I33 = "(33)";
        private const string I86 = "(86)";
        private const string I87 = "(87)";
        private const string I71 = "(71)";
        private const string I72 = "(72)";
        private const string I73 = "(73)";
        private const string I74 = "(74)";
        private const string I54 = "(54)";
        private const string I57 = "(57)";
        private const string I57n = "(57n)";
        private const string I41 = "(41)";
        private const string I96 = "(96)";
        private const string I97 = "(97)";

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            var files = directory.GetFiles("*.tetml", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

            List<XElement> xElements = new();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                var tet = XElement.Load(tetml);

                if( subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("Раздел: > Изобретения > Публикувани заявки"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Европейски патенти > Издадени ЕП след процедура по опозиция, съгласно чл. 103 от ЕПК"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Изобретения > Издадени патенти"))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(51\) I)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(51) I")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BA"));
                    }
                }
                else if (subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("Раздел: > Изобретения > Издадени патенти"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Европейски патенти > Заявки за европейски патенти"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Европейски патенти > Издадени европейски патенти"))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\(11\)\s\d)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
                else if (subCode == "4")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("Раздел: > Европейски патенти > Заявки за европейски патенти"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Европейски патенти > Издадени европейски патенти"))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\(11\)\s)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BA"));
                    }
                }
                else if (subCode == "5")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("Раздел: > Европейски патенти > Издадени европейски патенти"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Прекратили действието си обекти на закрила > Прекратили действието си сортове и породи"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Европейски патенти > Коригиран превод на описание към ЕП съгласно правило 140 от"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Европейски патенти > Издадени ЕП след процедура по опозиция, съгласно чл. 103 от ЕПК"))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\(11\)\s)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
                else if (subCode == "7")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("Раздел: > Полезни модели > Издадени свидетелства за регистрация"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Прекратили действието си обекти на закрила > Прекратили действието си патенти"))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\(11\)\s\d)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
                else if (subCode == "21")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("> Прекратили действието си обекти на закрила > Прекратили действието си eвропейски"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Сертификати за допълнителна закрила > Издадени сертификати за допълнителна закрила"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Възстановили действието си обекти на закрила > Възстановени европейски патенти"))
                             .TakeWhile(val => !val.Value.StartsWith("Раздел: > Прекратили действието си обекти на закрила > Европейски патенти - отказ от право"))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=BG)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("BG")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "MM"));
                    }
                }
            }
            return statusEvents;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            StringBuilder sb = new();

            if (subCode is "1" or "3" or "5")
            {
                foreach (var xElement in xElements)
                {
                    sb.Append(xElement.Value.Replace("\r", "").Replace("\n", " ") + " ");
                }
            }
            else
            {
                foreach (var xElement in xElements)
                {
                    sb.Append(xElement.Value + " ");
                }
            }
            return sb.ToString();
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "BG",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
            };

            Biblio biblio = new()
            {
                Ipcs = new List<Ipc>(),
                Priorities = new List<Priority>(),
                IntConvention = new IntConvention(),
                Applicants = new List<PartyMember>(),
                Inventors = new List<PartyMember>(),
                Agents = new List<PartyMember>(),
                Titles = new List<Title>(),
                Abstracts = new List<Abstract>(),
                Assignees = new List<PartyMember>(),
                Claims = new List<Claim>(),
                EuropeanPatents = new List<EuropeanPatent>(),
                Cpcs = new List<Cpc>()
            };

            LegalEvent legal = new()
            {
                Translations = new(),
            };

            Priority priority = new();

            IntConvention intConvention = new();

            CultureInfo culture = new("ru-RU");

            EuropeanPatent europeanPatent = new();

            NoteTranslation noteTranslation = new();

            if (subCode == "1")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I51))
                    {
                        var ipcrs = Regex.Split(inid.Replace("(51) Int. Cl.", "").Trim(), @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipcr in ipcrs)
                        {
                            var match = Regex.Match(ipcr.Trim(), @"(?<class>.+)\s\((?<date>.+)\)");

                            if (match.Success)
                            {
                                var match1 = Regex.Match(match.Groups["class"].Value.Trim(), @"(?<f>.+)\s(?<s>.+)");

                                if (match1.Success)
                                {
                                    biblio.Ipcs.Add(new Ipc
                                    {
                                        Class = match1.Groups["f"].Value.Replace(" ", "").Trim() + " " + match1.Groups["s"].Value.Trim(),
                                        Date = match.Groups["date"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{ipcr} 51 field");
                            }
                        }
                    }
                    if (inid.StartsWith(I21))
                    {
                        biblio.Application.Number = inid.Replace(I21, "").Trim();
                    }
                    if (inid.StartsWith(I22))
                    {
                        biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    if (inid.StartsWith(I31))
                    {
                        if (inid.Replace(I31, "").Trim() != "") priority.Number = inid.Replace(I31, "").Trim();
                    }
                    if (inid.StartsWith(I32))
                    {
                        if(inid.Replace(I32, "").Trim() != "") priority.Date = DateTime.Parse(inid.Replace(I32, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();   
                    }
                    if (inid.StartsWith(I33))
                    {
                        if (inid.Replace(I33, "").Trim() != "") priority.Country = inid.Replace(I33, "").Trim();
                    }
                    if (inid.StartsWith(I86))
                    {
                        if (inid.Replace(I86, "").Trim() != "") 
                        { 
                            var match = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<num>.+),\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");
                            if (match.Success)
                            {
                                intConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                                intConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else Console.WriteLine($"{inid} - 86");
                        }
                    }
                    if (inid.StartsWith(I87))
                    {
                        if (inid.Replace(I87, "").Trim() != "")
                        {
                            var match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<num>.+),\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");
                            if (match.Success)
                            {
                                intConvention.PctPublNumber = match.Groups["num"].Value.Trim();
                                intConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else Console.WriteLine($"{inid} - 87");
                        }
                    }
                    if (inid.StartsWith(I71))
                    {
                        var applicants = Regex.Split(inid.Replace(I71, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var match = Regex.Match(applicant, @"(?<name>.+?),\s(?<adress>.+)");

                            if (match.Success)
                            {
                                biblio.Applicants.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{applicant} - 71");
                        }
                    }
                    if (inid.StartsWith(I72))
                    {
                        var inventors = Regex.Split(inid.Replace(I72, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    if (inid.StartsWith(I74))
                    {
                        var agents = Regex.Split(inid.Replace(I74, "").Trim(), @"(?<=[\s|\.][0-9]{1,2}\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var agent in agents)
                        {
                            var match = Regex.Match(agent.Trim(), @"(?<name>.+?),\s(?<adress>.+)");

                            if (match.Success)
                            {
                                biblio.Agents.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{agent} - 74");
                        }
                    }
                    if (inid.StartsWith(I54))
                    {
                        biblio.Titles.Add(new Title
                        {
                            Language = "BG",
                            Text = inid.Replace(I54, "").Trim()
                        });
                    }
                    if (inid.StartsWith(I57))
                    {
                        biblio.Abstracts.Add(new Abstract
                        {
                            Language = "BG",
                            Text = inid.Replace(I57, "").Replace("Раздел: > Изобретения > Публикувани заявки", "").Trim()
                        });
                    }
                    if (inid.StartsWith(I57n))
                    {
                        var match = Regex.Match(inid.Replace(I57n, "").Trim(), @"(?<numClaims>[0-9]+)\s(?<claim>.+),\s(?<numFigures>[0-9]+)\s(?<figures>.+)");

                        if (match.Success)
                        {
                            statusEvent.LegalEvent = new LegalEvent
                            {
                                Language = "BG",
                                Note = "|| " + match.Groups["claim"].Value.Trim() + " | " + match.Groups["numClaims"].Value.Trim() + "\n" +"|| " + match.Groups["figures"].Value.Trim() + " | " + match.Groups["numFigures"].Value.Trim(),
                                Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| claims | " +match.Groups["numClaims"].Value.Trim()+"\n"+"|| figures | "+match.Groups["numFigures"].Value.Trim(),
                                        Type = "note"
                                    }
                                }
                            };
                        }
                        else
                        {
                            var match1 = Regex.Match(inid.Replace(I57n, "").Trim(), @"(?<numClaims>[0-9]+)\s(?<claim>.+)");

                            if (match1.Success)
                            {
                                statusEvent.LegalEvent = new LegalEvent
                                {
                                    Language = "BG",
                                    Note = "|| " + match.Groups["claim"].Value.Trim() + " | " + match.Groups["numClaims"].Value.Trim(),
                                    Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| claims | " +match.Groups["numClaims"].Value.Trim(),
                                        Type = "note"
                                    }
                                }
                                };
                            }
                            else Console.WriteLine($"{inid} - 57n ");
                        }                     
                    }
                    if (inid.StartsWith(I52))
                    {
                        var cpcs = Regex.Split(inid.Replace(I52,"").Replace("CPC","").Replace("\r","").Replace("\n"," ").Trim(),
                                @"\)")
                            .Where(_ => !string.IsNullOrEmpty(_))
                            .ToList();

                        foreach (var cpc in cpcs)
                        {
                            var match = Regex.Match(cpc.Trim(), @"(?<gr1>\D\s?\d{2}\s?\D)\s?(?<gr2>\d+\/\d+)");

                            if (match.Success)
                            {
                                biblio.Cpcs.Add(new Cpc()
                                {
                                    Class = match.Groups["gr1"].Value.Replace(" ","").Trim() 
                                            + " "
                                            + match.Groups["gr2"].Value.Trim()
                                });
                            }
                        }
                    }
                    else Console.WriteLine($"{inid}");
                }

                biblio.Priorities.Add(priority);
                biblio.IntConvention = intConvention;

                statusEvent.Biblio = biblio;
            }
            if(subCode == "3")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I11))
                    {
                        var match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<num>[0-9]+)\s(?<kind>[A-Z]{1,2}.+)");

                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} - 11 ");
                    }
                    if (inid.StartsWith(I51))
                    {
                        var ipcs = Regex.Split(inid.Replace("(51) Int. Cl.", "").Trim(), @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<date>.+)\)");

                            if (match.Success)
                            {
                                var match1 = Regex.Match(match.Groups["class"].Value.Trim(), @"(?<f>.+)\s(?<s>.+)");

                                if (match1.Success)
                                {
                                    biblio.Ipcs.Add(new Ipc
                                    {
                                        Class = match1.Groups["f"].Value.Replace(" ", "").Trim() + " " + match1.Groups["s"].Value.Trim(),
                                        Date = match.Groups["date"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{ipc} 51 field");
                            }
                        }
                    }
                    if (inid.StartsWith(I21))
                    {
                        biblio.Application.Number = inid.Replace(I21, "").Trim();
                    }
                    if (inid.StartsWith(I22))
                    {
                        biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    if (inid.StartsWith(I24))
                    {
                        biblio.Application.EffectiveDate = DateTime.Parse(inid.Replace(I24, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    if (inid.StartsWith(I31))
                    {
                        if (inid.Replace(I31, "").Trim() != "") priority.Number = inid.Replace(I31, "").Trim();
                    }
                    if (inid.StartsWith(I32))
                    {
                        if (inid.Replace(I32, "").Trim() != "") priority.Date = DateTime.Parse(inid.Replace(I32, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    if (inid.StartsWith(I33))
                    {
                        if (inid.Replace(I33, "").Trim() != "") priority.Country = inid.Replace(I33, "").Trim();
                    }
                    if (inid.StartsWith(I41))
                    {
                        var match = Regex.Match(inid.Replace(I41, "").Trim(), @".+\/(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");
                        if (match.Success)
                        {
                            biblio.DOfPublication = new DOfPublication
                            {
                                date_41 = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                            };
                        }
                        else Console.WriteLine($"{inid} - 41");
                    }
                    if (inid.StartsWith(I86))
                    {
                        if (inid.Replace(I86, "").Trim() != "")
                        {
                            var match = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<num>.+),\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");
                            if (match.Success)
                            {
                                intConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                                intConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else Console.WriteLine($"{inid} - 86");
                        }
                    }
                    if (inid.StartsWith(I87))
                    {
                        if (inid.Replace(I87, "").Trim() != "")
                        {
                            var match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<num>.+),\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");
                            if (match.Success)
                            {
                                intConvention.PctPublNumber = match.Groups["num"].Value.Trim();
                                intConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else Console.WriteLine($"{inid} - 87");
                        }
                    }
                    if (inid.StartsWith(I72))
                    {
                        var inventors = Regex.Split(inid.Replace(I72, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    if (inid.StartsWith(I73))
                    {
                        var assignes = Regex.Split(inid.Replace(I73, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var assign in assignes)
                        {
                            var match = Regex.Match(assign, @"(?<name>.+?),\s(?<adress>.+)");

                            if (match.Success)
                            {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{assign} - 73");
                        }
                    }
                    if (inid.StartsWith(I74))
                    {
                        var agents = Regex.Split(inid.Replace(I74, "").Trim(), @"(?<=[а-я]\.?[\s|\.][0-9]{1,2}\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var agent in agents)
                        {
                            var match = Regex.Match(agent.Trim(), @"(?<name>.+?),\s(?<adress>.+)");

                            if (match.Success)
                            {
                                biblio.Agents.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{agent} - 74");
                        }
                    }
                    if (inid.StartsWith(I54))
                    {
                        biblio.Titles.Add(new Title
                        {
                            Language = "BG",
                            Text = inid.Replace(I54, "").Trim()
                        });
                    }
                    if (inid.StartsWith(I57n))
                    {
                        var match = Regex.Match(inid.Replace(I57n, "").Trim(), @"(?<numClaims>[0-9]+)\s(?<claim>.+),\s(?<numFigures>[0-9]+)\s(?<figures>.+)");

                        if (match.Success)
                        {
                            statusEvent.LegalEvent = new LegalEvent
                            {
                                Language = "BG",
                                Note = "|| " + match.Groups["claim"].Value.Trim() + " | " + match.Groups["numClaims"].Value.Trim() + "\n" + "|| " + match.Groups["figures"].Value.Trim() + " | " + match.Groups["numFigures"].Value.Trim(),
                                Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| claims | " +match.Groups["numClaims"].Value.Trim()+"\n"+"|| figures | "+match.Groups["numFigures"].Value.Trim(),
                                        Type = "note"
                                    }
                                }
                            };
                        }
                        else
                        {
                            var match1 = Regex.Match(inid.Replace(I57n, "").Trim(), @"(?<numClaims>[0-9]+)\s(?<claim>.+)");

                            if (match1.Success)
                            {
                                statusEvent.LegalEvent = new LegalEvent
                                {
                                    Language = "BG",
                                    Note = "|| " + match.Groups["claim"].Value.Trim() + " | " + match.Groups["numClaims"].Value.Trim(),
                                    Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| claims | " +match.Groups["numClaims"].Value.Trim(),
                                        Type = "note"
                                    }
                                }
                                };
                            }
                            else Console.WriteLine($"{inid} - 57n ");
                        }
                    }
                    if (inid.StartsWith(I57))
                    {
                        var claims = Regex.Split(inid.Replace(I57, "").Trim(), @"(?=\.\s[0-9])").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var claim in claims)
                        {
                            var match = Regex.Match(claim, @"((?<num>[0-9]{1,3})\.\s(?<text>.+))");
                            if (match.Success)
                            {
                                biblio.Claims.Add(new DiamondProjectClasses.Claim
                                {
                                    Language = "BG",
                                    Text = match.Groups["text"].Value.Trim(),
                                    Number = match.Groups["num"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{claim} - 57");
                        }                           
                    }
                    if (inid.StartsWith(I52))
                    {
                        var cpcs = Regex.Split(inid.Replace(I52, "").Replace("CPC", "").Replace("\r", "").Replace("\n", " ").Trim(),
                                @"\)")
                            .Where(_ => !string.IsNullOrEmpty(_))
                            .ToList();

                        foreach (var cpc in cpcs)
                        {
                            var match = Regex.Match(cpc.Trim(), @"(?<gr1>\D\s?\d{2}\s?\D)\s?(?<gr2>\d+\/\d+)");

                            if (match.Success)
                            {
                                biblio.Cpcs.Add(new Cpc()
                                {
                                    Class = match.Groups["gr1"].Value.Replace(" ", "").Trim()
                                            + " "
                                            + match.Groups["gr2"].Value.Trim()
                                });
                            }
                        }
                    }
                    else Console.WriteLine($"{inid} not process");
                }

                biblio.Priorities.Add(priority);
                biblio.IntConvention = intConvention;

                statusEvent.Biblio = biblio;
            }
            if(subCode == "4")
            {
                foreach (var inid in MakeInids(note,subCode))
                {
                    if (inid.StartsWith(I11))
                    {
                        biblio.Publication.Kind = inid.Replace(I11, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I21))
                    {
                        biblio.Application.Number = inid.Replace(I21, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I22))
                    {
                        biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith(I24))
                    {
                        biblio.Application.EffectiveDate = DateTime.Parse(inid.Replace(I24, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith(I31))
                    {
                        if (inid.Replace(I31, "").Trim() != "") priority.Number = inid.Replace(I31, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I32))
                    {
                        if (inid.Replace(I32, "").Trim() != "") priority.Date = DateTime.Parse(inid.Replace(I32, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith(I33))
                    {
                        if (inid.Replace(I33, "").Trim() != "") priority.Country = inid.Replace(I33, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I72))
                    {
                        var inventors = Regex.Split(inid.Replace(I72, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    else
                    if (inid.StartsWith(I73))
                    {
                        var assignes = Regex.Split(inid.Replace(I73, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var assign in assignes)
                        {
                            var match = Regex.Match(assign, @"(?<name>.+?),\s(?<adress>.+),\s\((?<code>.+)\)");

                            if (match.Success)
                            {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(match.Groups["code"].Value.Trim())
                                });

                                if (MakeCountryCode(match.Groups["code"].Value.Trim()) == null) Console.WriteLine($"{match.Groups["code"].Value.Trim()}");
                            }
                            else Console.WriteLine($"{assign} - 73");
                        }
                    }
                    else
                    if (inid.StartsWith(I74))
                    {
                        var match = Regex.Match(inid.Replace(I74, "").Trim(), @"(?<name>.+?),\s(?<adress>.+)");

                        if (match.Success)
                        {
                            biblio.Agents.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} - 74");
                    }
                    else
                    if (inid.StartsWith(I54))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace(I54, "").Trim(), @"(?<title>.+)\s(?<note>\d+)\s(?<claim>пре.+)");

                        if (match.Success)
                        {
                            biblio.Titles.Add(new Title
                            {
                                Language = "BG",
                                Text = match.Groups["title"].Value.Trim()
                            });

                            legal.Note = "|| Претенции | " + match.Groups["note"].Value.Trim();
                            legal.Language = "BG";

                            noteTranslation.Language = "EN";
                            noteTranslation.Type = "INID";
                            noteTranslation.Tr = "|| Claims | " + match.Groups["note"].Value.Trim();

                          
                        }
                        else Console.WriteLine($"{inid}  --- 54");
                    }
                    else
                    if (inid.StartsWith(I51))
                    {
                        var ipcs = Regex.Split(inid.Replace("(51) Int. Cl.", "").Trim(), @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<date>.+)\)");

                            if (match.Success)
                            {
                                var match1 = Regex.Match(match.Groups["class"].Value.Trim(), @"(?<f>.+)\s(?<s>.+)");

                                if (match1.Success)
                                {
                                    biblio.Ipcs.Add(new Ipc
                                    {
                                        Class = match1.Groups["f"].Value.Replace(" ", "").Trim() + " " + match1.Groups["s"].Value.Trim(),
                                        Date = match.Groups["date"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{ipc} 51 field");
                            }
                        }
                    }
                    else Console.WriteLine($"{inid}");
                }

                legal.Translations.Add(noteTranslation);
                biblio.Priorities.Add(priority);
                statusEvent.LegalEvent = legal;
                statusEvent.Biblio = biblio;
            }
            if(subCode == "5")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I11))
                    {
                        var match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<num>.+)\s(?<kind>\D{1,2}.+)");

                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} - 11 ");
                    }
                    if (inid.StartsWith(I51))
                    {
                        var ipcs = Regex.Split(inid.Replace("(51) Int. Cl.", "").Trim(), @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<date>.+)\)");

                            if (match.Success)
                            {
                                var match1 = Regex.Match(match.Groups["class"].Value.Trim(), @"(?<f>.+)\s(?<s>.+)");

                                if (match1.Success)
                                {
                                    biblio.Ipcs.Add(new Ipc
                                    {
                                        Class = match1.Groups["f"].Value.Replace(" ", "").Trim() + " " + match1.Groups["s"].Value.Trim(),
                                        Date = match.Groups["date"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{ipc} 51 field");
                            }
                            
                        }
                    }
                    if (inid.StartsWith(I97))
                    {
                        var match = Regex.Match(inid.Replace(I97, "").Trim(), @"(?<num>.+)[,|\.]\s?(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                        if (match.Success)
                        {
                            europeanPatent.PubNumber = match.Groups["num"].Value.Trim();
                            europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".","/").Trim();
                        }
                        else Console.WriteLine($"{inid} - 97");
                    }
                    if (inid.StartsWith(I96))
                    {
                        var match = Regex.Match(inid.Replace(I96, "").Trim(), @"(?<num>.+)[,|\.]\s?(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                        if (match.Success)
                        {
                            europeanPatent.AppNumber = match.Groups["num"].Value.Trim();
                            europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} - 96");
                    }
                    if (inid.StartsWith(I24))
                    {
                        biblio.Application.EffectiveDate = DateTime.Parse(inid.Replace(I24, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    if (inid.StartsWith(I31))
                    {
                        if (inid.Replace(I31, "").Trim() != "") priority.Number = inid.Replace(I31, "").Trim();
                    }
                    if (inid.StartsWith(I32))
                    {
                        if (inid.Replace(I32, "").Trim() != "") priority.Date = DateTime.Parse(inid.Replace(I32, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    if (inid.StartsWith(I33))
                    {
                        if (inid.Replace(I33, "").Trim() != "") priority.Country = inid.Replace(I33, "").Trim();
                    }
                    if (inid.StartsWith(I86))
                    {
                        if (inid.Replace(I86, "").Trim() != "")
                        {
                            var match = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<num>.+),\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");
                            if (match.Success)
                            {
                                intConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                                intConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else Console.WriteLine($"{inid} - 86");
                        }
                    }
                    if (inid.StartsWith(I87))
                    {
                        if (inid.Replace(I87, "").Trim() != "")
                        {
                            var match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<num>.+),\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");
                            if (match.Success)
                            {
                                intConvention.PctPublNumber = match.Groups["num"].Value.Trim();
                                intConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else Console.WriteLine($"{inid} - 87");
                        }
                    }
                    if (inid.StartsWith(I72))
                    {
                        var inventors = Regex.Split(inid.Replace(I72, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    if (inid.StartsWith(I73))
                    {
                        var assignes = Regex.Split(inid.Replace(I73, "").Trim(), @"(?<=\])").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var assign in assignes)
                        {
                            var match = Regex.Match(assign, @"(?<name>.+?),\s(?<adress>.+),\s\[(?<code>[A-Z]{2})");

                            if (match.Success)
                            {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim()                                   
                                });
                            }
                            else Console.WriteLine($"{assign} - 73");
                        }
                    }
                    if (inid.StartsWith(I74))
                    { 
                        var agents = Regex.Split(inid.Replace(I74, "").Trim(), @"(?<=\s[0-9]{1,2}\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var agent in agents)
                        {
                            var match = Regex.Match(agent.Trim(), @"(?<name>.+?),\s(?<adress>.+)");

                            if (match.Success)
                            {
                                biblio.Agents.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{agent} - 74");
                        } 
                    }
                    if (inid.StartsWith(I54))
                    {
                        var match = Regex.Match(inid.Replace(I54, "").Trim(), @"(?<title>.+)\s(?<note>[0-9]{1,3}\sпр.+)");

                        if (match.Success)
                        {
                            biblio.Titles.Add(new Title
                            {
                                Language = "BG",
                                Text = match.Groups["title"].Value.Trim()
                            });

                            var match1 = Regex.Match(match.Groups["note"].Value.Replace("Раздел: > Европейски патенти > Издадени европейски патенти","").Trim(), @"(?<numClaims>[0-9]+)\s(?<claim>.+),\s(?<numFigures>[0-9]+)\s(?<figures>.+)");

                            if (match1.Success)
                            {
                                statusEvent.LegalEvent = new LegalEvent
                                {
                                    Language = "BG",
                                    Note = "|| " + match1.Groups["claim"].Value.Trim() + " | " + match1.Groups["numClaims"].Value.Trim() + "\n" + "|| " + match1.Groups["figures"].Value.Trim() + " | " + match1.Groups["numFigures"].Value.Trim(),
                                    Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| claims | " +match1.Groups["numClaims"].Value.Trim()+"\n"+"|| figures | "+match1.Groups["numFigures"].Value.Trim(),
                                        Type = "note"
                                    }
                                }
                                };
                            }
                            else
                            {
                                var match2 = Regex.Match(inid.Replace(I57n, "").Replace("Раздел: > Европейски патенти > Издадени европейски","").Trim(), @"(?<numClaims>[0-9]+)\s(?<claim>.+)");

                                if (match2.Success)
                                {
                                    statusEvent.LegalEvent = new LegalEvent
                                    {
                                        Language = "BG",
                                        Note = "|| " + match2.Groups["claim"].Value.Trim() + " | " + match2.Groups["numClaims"].Value.Trim(),
                                        Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| claims | " + match2.Groups["numClaims"].Value.Trim(),
                                        Type = "note"
                                    }
                                }
                                    };
                                }
                            }
                        }
                        else Console.WriteLine($"{inid} - 54");                      
                    }
                    if (inid.StartsWith(I52))
                    {
                        var cpcs = Regex.Split(inid.Replace(I52, "").Replace("CPC", "").Replace("\r", "").Replace("\n", " ").Trim(),
                                @"\)")
                            .Where(_ => !string.IsNullOrEmpty(_))
                            .ToList();

                        foreach (var cpc in cpcs)
                        {
                            var match = Regex.Match(cpc.Trim(), @"(?<gr1>\D\s?\d{2}\s?\D)\s?(?<gr2>\d+\/\d+)");

                            if (match.Success)
                            {
                                biblio.Cpcs.Add(new Cpc()
                                {
                                    Class = match.Groups["gr1"].Value.Replace(" ", "").Trim()
                                            + " "
                                            + match.Groups["gr2"].Value.Trim()
                                });
                            }
                        }
                    }
                    else Console.WriteLine($"{inid} not process");
                }
                biblio.EuropeanPatents.Add(europeanPatent);
                biblio.Priorities.Add(priority);
                biblio.IntConvention = intConvention;

                statusEvent.Biblio = biblio;
            }
            if(subCode == "7")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I11))
                    {
                        var match = Regex.Match(inid.Replace(I11, "").Replace("\r", "").Replace("\n", " ").Trim(), @"(?<num>[0-9]+)\s(?<kind>[A-Z]{1,2}.+)");

                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} - 11 ");
                    }
                    if (inid.StartsWith(I51))
                    {
                        var ipcs = Regex.Split(inid.Replace("(51) Int. Cl.", "").Replace("\r", "").Replace("\n", " ").Trim(), @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<date>.+)\)");

                            if (match.Success)
                            {

                                var match1 = Regex.Match(match.Groups["class"].Value.Trim(), @"(?<f>.+)\s(?<s>.+)");

                                if (match1.Success)
                                {
                                    biblio.Ipcs.Add(new Ipc
                                    {
                                        Class = match1.Groups["f"].Value.Replace(" ", "").Trim()+" "+ match1.Groups["s"].Value.Trim(),
                                        Date = match.Groups["date"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{ipc} 51 field");
                            }
                            
                        }
                    }
                    if (inid.StartsWith(I21))
                    {
                        biblio.Application.Number = inid.Replace(I21, "").Replace("\r", "").Replace("\n", " ").Trim();
                    }
                    if (inid.StartsWith(I22))
                    {
                        biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Replace("\r", "").Replace("\n", " ").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    if (inid.StartsWith(I24))
                    {
                        var match = Regex.Match(inid.Replace(I24, "").Replace("\r", "").Replace("\n", " "), @"(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4}).+");

                        if (match.Success)
                        {
                            biblio.Application.EffectiveDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }

                    }
                    if (inid.StartsWith(I41))
                    {

                        var match = Regex.Match(inid.Replace(I41, "").Replace("\r", "").Replace("\n", " ").Trim(), @".+\/(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");
                        if (match.Success)
                        {
                            biblio.DOfPublication = new DOfPublication
                            {
                                date_41 = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                            };
                        }
                        else Console.WriteLine($"{inid} - 41");
                    }
                    if (inid.StartsWith(I72))
                    {
                        var inventors = Regex.Split(inid.Replace(I72, "").Replace("\r", "").Replace("\n", " ").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    if (inid.StartsWith(I73))
                    {
                        var assignes = Regex.Split(inid.Replace(I73, "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var assign in assignes)
                        {
                            var match = Regex.Match(assign, @"(?<name>.+?),\s(?<adress>.+)");

                            if (match.Success)
                            {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{assign} - 73");
                        }
                    }
                    if (inid.StartsWith(I74))
                    {
                        if (inid.Replace(I74, "").Trim() != "")
                        {
                            var agents = Regex.Split(inid.Replace(I74, "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var agent in agents)
                            {

                                var match = Regex.Match(agent.Trim(), @"(?<name>.+?),\s(?<adress>.+)");

                                if (match.Success)
                                {
                                    biblio.Agents.Add(new PartyMember
                                    {
                                        Name = match.Groups["name"].Value.Trim(),
                                        Address1 = match.Groups["adress"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{inid} - 74");

                            }
                        }
                    }
                    if (inid.StartsWith(I54))
                    {
                        biblio.Titles.Add(new Title
                        {
                            Language = "BG",
                            Text = inid.Replace(I54, "").Replace("\r", "").Replace("\n", " ").Trim()
                        });
                    }
                    if (inid.StartsWith(I57n))
                    {
                        var match = Regex.Match(inid.Replace(I57n, "").Replace("\r", "").Replace("\n", " ").Trim(), @"(?<numClaims>[0-9]+)\s(?<claim>.+),\s(?<numFigures>[0-9]+)\s(?<figures>.+)");

                        if (match.Success)
                        {
                            statusEvent.LegalEvent = new LegalEvent
                            {
                                Language = "BG",
                                Note = "|| " + match.Groups["claim"].Value.Trim() + " | " + match.Groups["numClaims"].Value.Trim() + "\n" + "|| " + match.Groups["figures"].Value.Trim() + " | " + match.Groups["numFigures"].Value.Trim(),
                                Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| claims | " +match.Groups["numClaims"].Value.Trim()+"\n"+"|| figures | "+match.Groups["numFigures"].Value.Trim(),
                                        Type = "note"
                                    }
                                }
                            };
                        }
                        else
                        {
                            var match1 = Regex.Match(inid.Replace(I57n, "").Trim(), @"(?<numClaims>[0-9]+)\s(?<claim>.+)");

                            if (match1.Success)
                            {
                                statusEvent.LegalEvent = new LegalEvent
                                {
                                    Language = "BG",
                                    Note = "|| " + match1.Groups["claim"].Value.Trim() + " | " + match1.Groups["numClaims"].Value.Trim(),
                                    Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| claims | " +match1.Groups["numClaims"].Value.Trim(),
                                        Type = "note"
                                    }
                                }
                                };
                            }
                            else Console.WriteLine($"{inid} - 57n ");
                        }
                    }
                    if (inid.StartsWith(I57))
                    {
                        var claims = Regex.Split(inid.Replace(I57, "").Replace("\r", "").Replace("\n", " ").Trim(), @"(?=\.\s[0-9])").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var claim in claims)
                        {
                            var match = Regex.Match(claim, @"((?<num>[0-9]{1,3})\.\s(?<text>.+))");
                            if (match.Success)
                            {
                                biblio.Claims.Add(new DiamondProjectClasses.Claim
                                {
                                    Language = "BG",
                                    Text = match.Groups["text"].Value.Trim(),
                                    Number = match.Groups["num"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{claim} - 57");
                        }
                    }
                    if (inid.StartsWith(I52))
                    {
                        var cpcs = Regex.Split(inid.Replace(I52, "").Replace("CPC", "").Replace("\r", "").Replace("\n", " ").Trim(),
                                @"\)")
                            .Where(_ => !string.IsNullOrEmpty(_))
                            .ToList();

                        foreach (var cpc in cpcs)
                        {
                            var match = Regex.Match(cpc.Trim(), @"(?<gr1>\D\s?\d{2}\s?\D)\s?(?<gr2>\d+\/\d+)");

                            if (match.Success)
                            {
                                biblio.Cpcs.Add(new Cpc()
                                {
                                    Class = match.Groups["gr1"].Value.Replace(" ", "").Trim()
                                            + " "
                                            + match.Groups["gr2"].Value.Trim()
                                });
                            }
                        }
                    }
                    else Console.WriteLine($"{inid} not process");
                }
                biblio.Priorities.Add(priority);
                biblio.IntConvention = intConvention;

                statusEvent.Biblio = biblio;
            }
            if(subCode == "21")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Replace("> Прекратили действието си обекти на закрила > Прекратили действието си eвропейски","").Trim(), @"(?<appNum>BG.+?),.+№\s?:(?<pubNum>\d+),\s(?<title>.+)Р");

                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();
                    biblio.Application.Number = match.Groups["appNum"].Value.Trim();

                    biblio.Titles.Add(new Title
                    {
                        Language = "BG",
                        Text = match.Groups["title"].Value.Trim()
                    });
                }
                else
                {
                    Console.WriteLine($"{note} -------------------------");
                }

                var match1 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "").Trim()), @"[0-9]{8}");

                if (match1.Success)
                {
                    statusEvent.LegalEvent = new LegalEvent
                    {
                        Date = match1.Value.Insert(4, @"/").Insert(7, @"/").Trim()
                    };

                }

                statusEvent.Biblio = biblio;

            }

            return statusEvent;
        }

        internal string MakeCountryCode(string country) => country switch
        {
            "Czech Republic" => "CZ",
            _ => null
        };
        internal List<string> MakeInids (string note, string subCode)
        {
            List<string> inids = new();
            if (subCode == "1" || subCode == "3" || subCode == "7")
            {
                var noteWithOut57 = note.Substring(0, note.IndexOf("(57)"));

                inids = Regex.Split(noteWithOut57, @"(?=\([0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                var field57 = note.Substring(note.IndexOf("(57)"));

                var match = Regex.Match(field57.Replace("\r","").Replace("\n"," ").Trim(), @"(?<f57>.+)\.\s(?<note>[0-9]+\sпр.+?)\s(?<else>[А-Я])");
                if (match.Success)
                {
                    inids.Add(match.Groups["f57"].Value.Trim());
                    inids.Add("(57n) " + match.Groups["note"].Value.Trim());
                }
                else
                {
                    var match1 = Regex.Match(field57.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<f57>.+)\.\s(?<note>[0-9]+\sпр.+)");
                    if (match1.Success)
                    {
                        inids.Add(match1.Groups["f57"].Value.Trim());
                        inids.Add("(57n) " + match1.Groups["note"].Value.Trim());
                    }
                    else Console.WriteLine($"{field57} - don't process");
                }
            }
            else if (subCode == "5" || subCode == "4")
            {
                inids = Regex.Split(note, @"(?=\([0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
            }
            return inids;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : // продакшен
                    @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; // стейдж
                HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
