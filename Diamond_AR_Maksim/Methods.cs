﻿using Integration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_AR_Maksim
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

                if(subCode =="1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("SOLICITUDES DE PATENTE"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Replace("--", "").Trim(), @"(?=\(10\)\sA)").Where(val => !string.IsNullOrEmpty(val) && val.Contains("(21) P")).ToList();

                    foreach (var note in notes)
                    {
                         statusEvents.Add(MakePatent(note, subCode, "AZ"));
                    }
                }
                else if(subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                         //.SkipWhile(val => !val.Value.StartsWith("(10) Patente de Invención"))
                         .ToList();

                    var notes = Regex.Split(MakeText(xElements,subCode).Replace("--", "").Replace("<Primera>","").Trim(), @"(?=\(10\)\sP)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(10) P")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
                else if(subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                     //  .SkipWhile(val => !val.Value.StartsWith("(10) Patente de Invención"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Replace("--", "").Trim(), @"(?=\(10\)\s[A-Z])").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(10) M")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
                else if(subCode == "5")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("SOLICITUDES DE PATENTE"))
                        .ToList();
                    //   val.Contains("(21) M")
                    var notes = Regex.Split(MakeText(xElements, subCode).Replace("--", "").Trim(), @"(?=\(10\)\sA)").Where(val => !string.IsNullOrEmpty(val) && val.Contains("(21) M")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AZ"));
                    }
                }
                else if (subCode == "6")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("ANEXO"))
                        .TakeWhile(val => !val.Value.StartsWith("República Argentina - Poder Ejecutivo Nacional"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"\s").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("AR") && new Regex(@"AR\d{5,7}B[1,2,3]").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "MM"));
                    }
                }
                else if (subCode == "7")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("ANEXO"))
                        .TakeWhile(val => !val.Value.StartsWith("República Argentina - Poder Ejecutivo Nacional"))
                        .ToList();
                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"\s").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("AR") && new Regex(@"AR\d{5,7}[A-Z]4").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "MM"));
                    }
                }
                else if (subCode == "8")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Nº de Orden Acta Fecha de Presentación Agente Fecha de Vista"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\d{1,2}\s\d{11}.+)").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d.+").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "GB/PC"));
                    }
                }
                else if (subCode == "9")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Expediente Actuación Agente Expediente Actuación Agente"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=2\d{10})").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d{11}\s?Desistida [F|f]orzosa\s?\d+").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FB/MZ"));
                    }
                }
                else if (subCode == "10")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Expediente Actuación Agente Expediente Actuación Agente"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=2\d{10})")
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d{11}\s?Desistida [V|v]oluntaria\s?\d+").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FA/MA"));
                    }
                }
                else if (subCode == "11")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Expediente Actuación Agente Expediente Actuación Agente"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=2\d{10})")
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d{11}\s?Abandonada\s?\d+").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FA/MA"));
                    }
                }
                else if (subCode == "12")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Expediente Actuación Agente Expediente Actuación Agente"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=2\d{10})")
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d{11}\s?Denegada\s?\d+").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FC"));
                    }
                }
                else if (subCode == "13")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Expediente Actuación Agente Expediente Actuación Agente"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=2\d{10})")
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d{11}\s?Examen de [F|f]ondo\s?\d+").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "EZ"));
                    }
                }
                else if (subCode == "14")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Expediente Actuación Agente Expediente Actuación Agente"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=2\d{10})")
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d{11}\s?Revoca [D|d]isposici.n\s?\d+").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FB/MG"));
                    }
                }
            }
            return statusEvents;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                SectionCode = sectionCode,
                SubCode = subCode,
                CountryCode = "AR",
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
            };

            CultureInfo culture = new("ru-RU");
            LegalEvent legal = new();
            legal.Translations = new();
            NoteTranslation noteTranslation = new();
            DOfPublication dOfPublication = new();

            if (subCode == "1" || subCode == "5")
            {
                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(10)"))
                    {
                        var match = Regex.Match(inid.Replace("(10)", "").Trim(), @"(?<pNum>.+)\s(?<pKind>\D\d{1,2})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["pNum"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["pKind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} --- 10");
                    }
                    else
                    if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(22)"))
                    {
                        statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith("(30)"))
                    {
                        var priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(30)", "").Trim(), @"(?<=\d{2}\/\d{2}\/\d{4})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priority in priorities)
                        {
                            var match = Regex.Match(priority.Trim(), @"(?<code>\D{2})\s(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Integration.Priority
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Number = match.Groups["num"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".","/").Trim()
                                });
                            }
                            else Console.WriteLine($"{match} --- 30");
                        }
                    }
                    else
                    if (inid.StartsWith("(51)"))
                    {
                        var ipcs = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(51)", "").Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        List<string> fpart = new();
                        List<string> spart = new();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc.TrimEnd(',').Trim(), @"(?<fpart>\D.+?\s?)(?<spart>\d+\/\d+)");
                            if(match.Success)
                            {
                                fpart.Add(match.Groups["fpart"].Value.Trim());
                                spart.Add(match.Groups["spart"].Value.Trim());
                            }
                            else
                            {
                                if (fpart.Count != 0 && fpart.Last() != null)
                                {
                                    var lastElem = fpart.Last();
                                    fpart.Add(lastElem);
                                }
                                spart.Add(ipc.TrimEnd(',').Trim());
                            }
                        }

                        for (var i = 0; i < fpart.Count; i++)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Ipc
                            {
                                Class = fpart[i] + " " + spart[i]
                            });
                        }
                    }
                    else
                    if (inid.StartsWith("(54)"))
                    {
                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "ES",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(54)", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith("(57)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(57)", "").Trim(), @"(?<abst>.+)\s(?<claim>Reivindicación\s1.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "ES",
                                Text = match.Groups["abst"].Value.Trim()
                            });

                            var claims = Regex.Split(match.Groups["claim"].Value.Trim(), @"(?=Reivindicaci.n\s\d{1,3}:)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            var i = 1;

                            foreach (var claim in claims)
                            {
                                statusEvent.Biblio.Claims.Add(new DiamondProjectClasses.Claim
                                {
                                    Language = "ES",
                                    Number = i++.ToString(),
                                    Text = claim.Replace("Reivindicación ", "").Trim()
                                });
                            }
                        }
                        else
                        {
                            var match1 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(57)", "").Trim(), @"Reivindicación\s1.+");

                            if (match1.Success)
                            {
                                var claims = Regex.Split(match1.Value.Trim(), @"(?=Reivindicaci.n\s\d{1,3}:)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                var j = 1;

                                foreach (var claim in claims)
                                {
                                    statusEvent.Biblio.Claims.Add(new DiamondProjectClasses.Claim
                                    {
                                        Language = "ES",
                                        Number = j++.ToString(),
                                        Text = claim.Replace("Reivindicación ", "").Trim()
                                    });
                                }
                            }
                            else
                            {
                                statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                                {
                                    Language = "ES",
                                    Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(57)", "").Trim()
                                });
                            }
                        }
                    }
                    else
                    if (inid.StartsWith("(71)"))
                    {
                        var applicants = Regex.Split(inid.Replace("(71)", "").Trim(), @"(?<=,\s\D{2}$)", RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var match = Regex.Match(applicant.Trim(), @"(?<name>.+\n?.+)\n(?<adress>.+)\s(?<code>\D{2})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim()
                                });

                            }
                            else Console.WriteLine($"{applicant} ------ 71");
                        }
                    }
                    else
                    if (inid.StartsWith("(72)"))
                    {
                        var inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(72)", "").Trim(), @"-").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    else
                    if (inid.StartsWith("(74)"))
                    {
                        statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                        {
                            Name = inid.Replace("\r", "").Replace("\n", " ").Replace("(74)", "").Trim()
                        });

                        legal.Note =legal.Note + "|| (74) Agente/s Nro | " + inid.Replace("\r", "").Replace("\n", " ").Replace("(74)", "").Trim() + "\n";
                        legal.Language = "ES";
                        

                        noteTranslation.Language = "EN";
                        noteTranslation.Tr = noteTranslation.Tr + "|| (74) Agent/s number | " + inid.Replace("\r", "").Replace("\n", " ").Replace("(74)", "").Trim() + "\n";
                        noteTranslation.Type = "INID";

                    }
                    else
                    if (inid.StartsWith("(41)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<date>\d{2}\/\d{2}\/\d{4})\s.+?(?<num>\d+)");

                        if (match.Success)
                        {
                            dOfPublication.date_41 = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim();

                            legal.Note = legal.Note + "|| Bol. Nro | " + match.Groups["num"].Value.Trim() + "\n";
                            legal.Language = "ES";

                            noteTranslation.Language = "EN";
                            noteTranslation.Tr = noteTranslation.Tr + "|| Bulletin number | " + match.Groups["num"].Value.Trim() + "\n";
                            noteTranslation.Type = "INID";
                        }
                    }
                    else
                    if (inid.StartsWith("(62)"))
                    {
                        var match = Regex.Match(inid.Replace("(62)", "").Trim(), @"(?<num>.+)(?<kind>\D\d+)");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Related.Add(new RelatedDocument
                            {
                                Number = match.Groups["num"].Value.Trim(),
                                
                                Type = match.Groups["kind"].Value.Trim()
                            });
                        }  
                    }
                    else
                    if (inid.StartsWith("(83)"))
                    {
                        legal.Note = "|| (83) | " + inid.Replace("(83)","").Replace("\r","").Replace("\n"," ").Trim() + "\n";
                        noteTranslation.Tr = "|| (83) | " + inid.Replace("(83)", "").Replace("\r", "").Replace("\n", " ").Trim() + "\n";
                    }
                    else Console.WriteLine($"{inid} ---- don't process");
                }

                legal.Translations.Add(noteTranslation);
                statusEvent.LegalEvent = legal;
                statusEvent.Biblio.DOfPublication = dOfPublication;
            }
            else if(subCode == "2" || subCode == "3")    
            {
                foreach (var inid in MakeInids(note,subCode))
                {
                    if (inid.StartsWith("(10)"))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(10)", "").Replace("\r", "").Replace("\n", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(11)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"iva.+\s(?<num>.+)(?<kind>\D\d{1,2})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid}  ---   10");
                    }
                    else
                    if (inid.StartsWith("(21)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"\D\s\d+");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Number = match.Value.Trim();
                        }
                        else Console.WriteLine($"{inid} --- 21");
                    }
                    else
                    if (inid.StartsWith("(22)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"\d{2}\/\d{2}\/\d{4}");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 22");
                    }
                    else
                    if (inid.StartsWith("(24)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"(?<d1>\d{2}\/\d{2}\/\d{4}).+(?<d2>\d{2}\/\d{2}\/\d{4})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.EffectiveDate = DateTime.Parse(match.Groups["d1"].Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim();

                            legal.Note = "|| Fecha de Vencimiento | " + DateTime.Parse(match.Groups["d2"].Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim() + "\n";
                            legal.Language = "ES";

                            noteTranslation.Language = "EN";
                            noteTranslation.Tr = "|| Expiration date | " + DateTime.Parse(match.Groups["d2"].Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim() + "\n";
                            noteTranslation.Type = "INID";

                        }
                        else Console.WriteLine($"{inid}---24");
                    }
                    else
                    if (inid.StartsWith("(30)"))
                    {
                        var priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", "").Replace("(30) Prioridad Convenio de Paris ","")
                            .Replace("(30) Prioridad convenio de Paris ", "")
                            .Replace("(30) Prioridad Convenio de París ", "")
                            .Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priority in priorities)
                        {
                            var match = Regex.Match(priority.Trim(), @"(?<code>\D{2})\s?(?<num>.+)\s?(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Priority
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Number = match.Groups["num"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim()
                                });
                            }
                            else Console.WriteLine($"{priority}----30");
                        }
                    }
                    else
                    if (inid.StartsWith("(47)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"\d{2}\/\d{2}\/\d{4}");
                        if (match.Success)
                        {
                            dOfPublication.date_47 = DateTime.Parse(match.Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 47");
                    }
                    else
                    if (inid.StartsWith("(51)"))
                    {
                        var data = Regex.Match(inid.Replace("\r", "").Replace("\n", " "), @".+[:|\)|.](?<data>.+)");

                        if (data.Success)
                        {
                            var ipcs = Regex.Split(data.Groups["data"].Value.Trim(), @"[,|;]").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            List<string> fpart = new();
                            List<string> spart = new();

                            foreach (var ipc in ipcs)
                            {
                                var match = Regex.Match(ipc.TrimEnd(',').Trim(), @"(?<fpart>\D.+?\s?)(?<spart>\d+\/\d+)");
                                if (match.Success)
                                {
                                    fpart.Add(match.Groups["fpart"].Value.Trim());
                                    spart.Add(match.Groups["spart"].Value.Trim());
                                }
                                else
                                {
                                    var lastElem = fpart.Last();
                                    fpart.Add(lastElem);
                                    spart.Add(ipc.TrimEnd(',').Trim());
                                }
                            }

                            for (var i = 0; i < fpart.Count; i++)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc
                                {
                                    Class = fpart[i] + " " + spart[i]
                                });
                            }
                        }
                        else Console.WriteLine($"{inid}");                   
                    }
                    else
                    if (inid.StartsWith("(54)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\s-\s(?<title>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Title
                            {
                                Language = "ES",
                                Text = match.Groups["title"].Value.Trim()
                            });
                        }
                    }
                    else
                    if (inid.StartsWith("(57)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"ACI.N\s1?\.?(?<claim>.+)\s?S.guen?\s(?<num>\d{1,3})\s.+");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Claims.Add(new DiamondProjectClasses.Claim
                            {
                                Number = "1",
                                Language = "ES",
                                Text = match.Groups["claim"].Value.Trim()
                            });

                            legal.Note = legal.Note + "|| REIVINDICACIÓNS | Siguen " + match.Groups["num"].Value.Trim() + " Reivindicaciones\n";

                            noteTranslation.Tr = noteTranslation.Tr + "|| CLaims | " + match.Groups["num"].Value.Trim() + " Claims follow\n";
                        }
                        else
                        {
                            var match1 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"ACI.N\s1?\.?(?<claim>.+)\s?(?<qual>Única.+)");
                            if (match1.Success)
                            {
                                statusEvent.Biblio.Claims.Add(new DiamondProjectClasses.Claim
                                {
                                    Number = "1",
                                    Language = "ES",
                                    Text = match1.Groups["claim"].Value.Trim()
                                });

                                legal.Note = legal.Note + "|| REIVINDICACIÓNS | " + match1.Groups["qual"].Value.Trim() + " \n";

                                noteTranslation.Tr = noteTranslation.Tr + "|| CLaims | Single Claim\n";
                            }
                            else
                            {
                                statusEvent.Biblio.Claims.Add(new DiamondProjectClasses.Claim
                                {
                                    Number = "1",
                                    Language = "ES",
                                    Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(57) REIVINDICACIÓN 1.","").Trim()
                                });
                            }
                        }
                    }
                    else
                    if (inid.StartsWith("(71)"))
                    {
                        var applicants = Regex.Split(inid.Replace("(71) Titular - ", "").Trim(), @"(?<=,\s\D{2}$)", RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var match = Regex.Match(applicant.Trim(), @"(?<name>.+\n?.+)\n(?<adress>.+)\s(?<code>\D{2})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim()
                                });

                            }
                            else Console.WriteLine($"{applicant} ------ 71");
                        }
                    }
                    else
                    if (inid.StartsWith("(72)"))
                    {
                        var inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(72) Inventor - ", "").Trim(), @"-").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    else
                    if (inid.StartsWith("(74)"))
                    {
                        statusEvent.Biblio.Agents.Add(new PartyMember
                        {
                            Name = inid.Replace("\r", "").Replace("\n", " ").Replace("(74) Agente/s", "").Trim()
                        });

                        legal.Note = legal.Note + "|| (74) Agente/s Nro. | " + inid.Replace("\r", "").Replace("\n", " ").Replace("(74) Agente/s", "").Trim();


                        noteTranslation.Tr = noteTranslation.Tr + "|| (74) Agent/s number | " + inid.Replace("\r", "").Replace("\n", " ").Replace("(74) Agente/s", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(45)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"\d{2}\/\d{2}\/\d{4}");
                        if (match.Success)
                        {
                            dOfPublication.date_45 = DateTime.Parse(match.Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 45");
                    }
                    else
                    if (inid.StartsWith("(62)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"Nº(?<num>\D.+)(?<kind>\D\d+)");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Related.Add(new RelatedDocument
                            {
                                Number = match.Groups["num"].Value.Trim(),

                                Type = match.Groups["kind"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}");
                    }

                    else Console.WriteLine($"{inid} -- don't processed");
                }

                legal.Translations.Add(noteTranslation);
                statusEvent.LegalEvent = legal;
                statusEvent.Biblio.DOfPublication = dOfPublication;
            }
            else if (subCode == "6" || subCode == "7")
            {
                var match = Regex.Match(note.Trim(), @"(?<num>.+)(?<kind>[A-Z]\d)");
                if (match.Success)
                {
                    statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                    statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();

                    var date = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"\d{8}");
                    if (date.Success)
                    {
                        legal.Date = date.Value.Insert(4,"/").Insert(7,"/").Trim();
                    }
                    statusEvent.LegalEvent = legal;
                }
            }
            else if (subCode == "8")
            {
                var match = Regex.Match(note.Trim(),
                    @"\s(?<appnum>\d+)\s(?<leNote>\d{2}\/\d{2}\/\d{4})\s(?<agent>\d+)\s(?<leDate>\d{2}\/\d{2}\/\d{4})");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["appnum"].Value.Trim();
                    if (match.Groups["agent"].Value.Trim() != "0")
                    {
                        statusEvent.Biblio.Agents.Add(new PartyMember()
                        {
                            Name = match.Groups["agent"].Value.Trim()
                        });
                    }

                    legal.Date = DateTime.Parse(match.Groups["leDate"].Value.Trim(), culture).ToString("yyyy.MM.dd")
                        .Replace(".", "/").Trim();

                    legal.Language = "ES";
                    if (match.Groups["agent"].Value.Trim() != "0")
                    {
                        legal.Note = "|| Fecha de Presentación | " + DateTime.Parse(match.Groups["leNote"].Value.Trim(), culture)
                            .ToString("yyyy.MM.dd")
                            .Replace(".", "/").Trim() + " || Agente/s Nro. | " + match.Groups["agent"].Value.Trim();

                        legal.Translations.Add(new NoteTranslation()
                        {
                            Language = "EN",
                            Tr = "|| Application date | " + DateTime.Parse(match.Groups["leNote"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd")
                                .Replace(".", "/").Trim() + " || Agent/s number | " + match.Groups["agent"].Value.Trim(),
                            Type = "INID"
                    });
                    }
                    else
                    {
                        legal.Note = "|| Fecha de Presentación | " + DateTime.Parse(match.Groups["leNote"].Value.Trim())
                            .ToString("yyyy.MM.dd")
                            .Replace(".", "/").Trim();

                        legal.Translations.Add(new NoteTranslation()
                        {
                            Language = "EN",
                            Tr = "|| Application date | " + DateTime.Parse(match.Groups["leNote"].Value.Trim())
                                .ToString("yyyy.MM.dd")
                                .Replace(".", "/").Trim(),
                            Type = "INID"
                        });
                    }

                    statusEvent.LegalEvent = legal;
                }
            }
            else if (subCode is "9" or "10" or "11" or "12" or "13" or "14")
            {
                var match = Regex.Match(note, @"(?<appNum>\d{11})\s?.+?(?<agent>\d+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();

                    if (match.Groups["agent"].Value.Trim() != "0")
                    {
                        statusEvent.Biblio.Agents.Add(new PartyMember()
                        {
                            Name = match.Groups["agent"].Value.Trim()
                        });

                        legal.Note = "|| Agente/s Nro. | " + match.Groups["agent"].Value.Trim();
                        legal.Language = "ES";

                        legal.Translations.Add(new NoteTranslation()
                        {
                            Language = "EN",
                            Tr = "|| Agent/s number | " + match.Groups["agent"].Value.Trim(),
                            Type = "INID"
                        });
                    }

                    var date = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"\d{8}");
                    if (date.Success)
                    {
                        legal.Date = date.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }

                    statusEvent.LegalEvent = legal;
                }
            }

            return statusEvent;
        }
        internal List<string> MakeInids(string note, string subCode)
        {
            List<string> inids = new();

            if (subCode == "1" || subCode == "5")
            {
                var inidsBefore57 = note.Substring(0, note.IndexOf("(57)")).Trim();

                var inidsAfter57 = note.Substring(note.IndexOf("(71)")).Trim();

                var inid57 = note.Substring(note.IndexOf("(57)")).Trim();
                inid57 = inid57.Substring(0, inid57.IndexOf("(71)")).Trim();
                
                inids = Regex.Split(inidsBefore57, @"(?=\(\d{2}\)\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                var match = Regex.Match(inid57, @"(?<f57>\(57\)\s.+)\s?(?<f62>\(62\)\s.+)", RegexOptions.Singleline);
                if (match.Success)
                {
                    inids.Add(match.Groups["f57"].Value.Trim());
                    inids.Add(match.Groups["f62"].Value.Trim());
                }
                else
                {
                    var match1 = Regex.Match(inid57, @"(?<f57>\(57\)\s.+)\s?(?<f83>\(83\)\s.+)", RegexOptions.Singleline);
                    if (match1.Success)
                    {
                        inids.Add(match1.Groups["f57"].Value.Trim());
                        inids.Add(match1.Groups["f83"].Value.Trim());
                    }
                    else inids.Add(inid57);
                }
         
                var tmp = Regex.Split(inidsAfter57, @"(?=\(\d{2}\)\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();
                inids.AddRange(tmp);
            }
            else
            if (subCode == "2" || subCode == "3")
            {
                var inidsBefore57 = note.Substring(0, note.IndexOf("(57)")).Trim();
                var inidsAfter57 = note.Substring(note.IndexOf("(71) T")).Trim();

                var inid57 = note.Substring(note.IndexOf("(57)")).Trim();
                inid57 = inid57.Substring(0, inid57.IndexOf("(71) T")).Trim();

                inids = Regex.Split(inidsBefore57, @"(?=\(\d{2}\)\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                var match = Regex.Match(inid57, @"(?<f57>\(57\)\s.+)\s?(?<f62>\(62\)\sD.+)", RegexOptions.Singleline);
                if (match.Success)
                {
                    inids.Add(match.Groups["f57"].Value.Trim());
                    inids.Add(match.Groups["f62"].Value.Trim());
                }
                else inids.Add(inid57);

                var tmp = Regex.Split(inidsAfter57, @"(?=\(\d{2}\)\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();
                inids.AddRange(tmp);
            }

            return inids;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = string.Empty;

                foreach (var xElement in xElements)
                {
                    text += xElement.Value + "\n";
                }

            return subCode is "6" or "7" or "9" or "10" or "11" or "12" or "13" or "14" ? text.Replace("\r", "").Replace("\n", " ").Trim() : text;
        }
    }
}
