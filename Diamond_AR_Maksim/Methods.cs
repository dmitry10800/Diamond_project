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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_AR_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (FileInfo file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = new();

            foreach (string tetml in files)
            {
                CurrentFileName = tetml;

                tet = XElement.Load(tetml);

                if(subCode =="1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("SOLICITUDES DE PATENTE"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Replace("--", "").Trim(), @"(?=\(10\)\sA)").Where(val => !string.IsNullOrEmpty(val) && val.Contains("(21) P")).ToList();

                    foreach (string note in notes)
                    {
                         statusEvents.Add(MakePatent(note, subCode, "AZ"));
                    }
                }
                else
                if(subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                         //.SkipWhile(val => !val.Value.StartsWith("(10) Patente de Invención"))
                         .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements,subCode).Replace("--", "").Replace("<Primera>","").Trim(), @"(?=\(10\)\sP)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(10) P")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
                else
                if(subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                     //  .SkipWhile(val => !val.Value.StartsWith("(10) Patente de Invención"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Replace("--", "").Trim(), @"(?=\(10\)\s[A-Z])").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(10) M")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
                else
                if(subCode == "5")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("SOLICITUDES DE PATENTE"))
                        .ToList();
                    //   val.Contains("(21) M")
                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Replace("--", "").Trim(), @"(?=\(10\)\sA)").Where(val => !string.IsNullOrEmpty(val) && val.Contains("(21) M")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AZ"));
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
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new()
                
            };

            CultureInfo culture = new("ru-RU");
            LegalEvent legal = new();
            legal.Translations = new();
            NoteTranslation noteTranslation = new();
            DOfPublication dOfPublication = new();

            if (subCode == "1" || subCode == "5")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(10)"))
                    {
                        Match match = Regex.Match(inid.Replace("(10)", "").Trim(), @"(?<pNum>.+)\s(?<pKind>\D\d{1,2})");

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
                        List<string> priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(30)", "").Trim(), @"(?<=\d{2}\/\d{2}\/\d{4})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priority in priorities)
                        {
                            Match match = Regex.Match(priority.Trim(), @"(?<code>\D{2})\s(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

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
                        List<string> ipcs = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(51)", "").Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        List<string> fpart = new();
                        List<string> spart = new();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.TrimEnd(',').Trim(), @"(?<fpart>\D.+?\s?)(?<spart>\d+\/\d+)");
                            if(match.Success)
                            {
                                fpart.Add(match.Groups["fpart"].Value.Trim());
                                spart.Add(match.Groups["spart"].Value.Trim());
                            }
                            else
                            {
                                if (fpart.Count != 0 && fpart.Last() != null)
                                {
                                    string lastElem = fpart.Last();
                                    fpart.Add(lastElem);
                                }
                                spart.Add(ipc.TrimEnd(',').Trim());
                            }
                        }

                        for (int i = 0; i < fpart.Count; i++)
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
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(57)", "").Trim(), @"(?<abst>.+)\s(?<claim>Reivindicación\s1.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "ES",
                                Text = match.Groups["abst"].Value.Trim()
                            });

                            List<string> claims = Regex.Split(match.Groups["claim"].Value.Trim(), @"(?=Reivindicaci.n\s\d{1,3}:)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            int i = 1;

                            foreach (string claim in claims)
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
                            Match match1 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(57)", "").Trim(), @"Reivindicación\s1.+");

                            if (match1.Success)
                            {
                                List<string> claims = Regex.Split(match1.Value.Trim(), @"(?=Reivindicaci.n\s\d{1,3}:)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                int j = 1;

                                foreach (string claim in claims)
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
                        List<string> applicants = Regex.Split(inid.Replace("(71)", "").Trim(), @"(?<=,\s\D{2}$)", RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match match = Regex.Match(applicant.Trim(), @"(?<name>.+\n?.+)\n(?<adress>.+)\s(?<code>\D{2})");

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
                        List<string> inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(72)", "").Trim(), @"-").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
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
                        noteTranslation.Type = "note";

                    }
                    else
                    if (inid.StartsWith("(41)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<date>\d{2}\/\d{2}\/\d{4})\s.+?(?<num>\d+)");

                        if (match.Success)
                        {
                            dOfPublication.date_41 = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim();

                            legal.Note = "|| Publicación de trámite normal " + legal.Note + "|| Bol. Nro | " + match.Groups["num"].Value.Trim() + "\n";

                            noteTranslation.Tr = "|| Ordinary processing publication " + noteTranslation.Tr + "|| Bulletin number | " + match.Groups["num"].Value.Trim() + "\n";
                        }
                    }
                    else
                    if (inid.StartsWith("(62)"))
                    {
                        Match match = Regex.Match(inid.Replace("(62)", "").Trim(), @"(?<num>.+)(?<kind>\D\d+)");
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
            else
            if(subCode == "2" || subCode == "3")
            {
                foreach (string inid in MakeInids(note,subCode))
                {
                    if (inid.StartsWith("(10)"))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(10)", "").Replace("\r", "").Replace("\n", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(11)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"iva.+\s(?<num>.+)(?<kind>\D\d{1,2})");
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
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"\D\s\d+");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Number = match.Value.Trim();
                        }
                        else Console.WriteLine($"{inid} --- 21");
                    }
                    else
                    if (inid.StartsWith("(22)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"\d{2}\/\d{2}\/\d{4}");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 22");
                    }
                    else
                    if (inid.StartsWith("(24)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"(?<d1>\d{2}\/\d{2}\/\d{4}).+(?<d2>\d{2}\/\d{2}\/\d{4})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.EffectiveDate = DateTime.Parse(match.Groups["d1"].Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim();

                            legal.Note = "|| Fecha de Vencimiento | " + DateTime.Parse(match.Groups["d2"].Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim() + "\n";
                            legal.Language = "ES";

                            noteTranslation.Language = "EN";
                            noteTranslation.Tr = "|| Expiration date | " + DateTime.Parse(match.Groups["d2"].Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim() + "\n";
                            noteTranslation.Type = "note";

                        }
                        else Console.WriteLine($"{inid}---24");
                    }
                    else
                    if (inid.StartsWith("(30)"))
                    {
                        List<string> priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", "").Replace("(30) Prioridad Convenio de Paris ","")
                            .Replace("(30) Prioridad convenio de Paris ", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priority in priorities)
                        {
                            Match match = Regex.Match(priority.Trim(), @"(?<code>\D{2})\s?(?<num>.+)\s?(?<date>\d{2}\/\d{2}\/\d{4})");

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
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"\d{2}\/\d{2}\/\d{4}");
                        if (match.Success)
                        {
                            dOfPublication.date_47 = DateTime.Parse(match.Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 47");
                    }
                    else
                    if (inid.StartsWith("(51)"))
                    {
                        Match data = Regex.Match(inid.Replace("\r", "").Replace("\n", " "), @".+[:|\)|.](?<data>.+)");

                        if (data.Success)
                        {
                            List<string> ipcs = Regex.Split(data.Groups["data"].Value.Trim(), @"[,|;]").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            List<string> fpart = new();
                            List<string> spart = new();

                            foreach (string ipc in ipcs)
                            {
                                Match match = Regex.Match(ipc.TrimEnd(',').Trim(), @"(?<fpart>\D.+?\s?)(?<spart>\d+\/\d+)");
                                if (match.Success)
                                {
                                    fpart.Add(match.Groups["fpart"].Value.Trim());
                                    spart.Add(match.Groups["spart"].Value.Trim());
                                }
                                else
                                {
                                    string lastElem = fpart.Last();
                                    fpart.Add(lastElem);
                                    spart.Add(ipc.TrimEnd(',').Trim());
                                }
                            }

                            for (int i = 0; i < fpart.Count; i++)
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
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\s-\s(?<title>.+)");

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
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"ACI.N\s1?\.?(?<claim>.+)\s?S.guen?\s(?<num>\d{1,3})\s.+");

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
                            Match match1 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"ACI.N\s1?\.?(?<claim>.+)\s?(?<qual>Única.+)");
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
                        List<string> applicants = Regex.Split(inid.Replace("(71) Titular - ", "").Trim(), @"(?<=,\s\D{2}$)", RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match match = Regex.Match(applicant.Trim(), @"(?<name>.+\n?.+)\n(?<adress>.+)\s(?<code>\D{2})");

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
                        List<string> inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(72) Inventor - ", "").Trim(), @"-").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
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


                        noteTranslation.Tr = noteTranslation.Tr + "|| Ordinary processing publication || (74) Agent/s number | " + inid.Replace("\r", "").Replace("\n", " ").Replace("(74) Agente/s", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(45)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", "").Trim(), @"\d{2}\/\d{2}\/\d{4}");
                        if (match.Success)
                        {
                            dOfPublication.date_45 = DateTime.Parse(match.Value.Trim(), culture).ToString(@"yyyy/MM/dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 45");
                    }
                    else
                    if (inid.StartsWith("(62)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @"Nº(?<num>\D.+)(?<kind>\D\d+)");
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

            return statusEvent;
        }
        internal List<string> MakeInids(string note, string subCode)
        {
            List<string> inids = new();

            if (subCode == "1" || subCode == "5")
            {
                string inidsBefore57 = note.Substring(0, note.IndexOf("(57)")).Trim();

                string inidsAfter57 = note.Substring(note.IndexOf("(71)")).Trim();

                string inid57 = note.Substring(note.IndexOf("(57)")).Trim();
                inid57 = inid57.Substring(0, inid57.IndexOf("(71)")).Trim();
                
                inids = Regex.Split(inidsBefore57, @"(?=\(\d{2}\)\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                Match match = Regex.Match(inid57, @"(?<f57>\(57\)\s.+)\s?(?<f62>\(62\)\s.+)", RegexOptions.Singleline);
                if (match.Success)
                {
                    inids.Add(match.Groups["f57"].Value.Trim());
                    inids.Add(match.Groups["f62"].Value.Trim());
                }
                else
                {
                    Match match1 = Regex.Match(inid57, @"(?<f57>\(57\)\s.+)\s?(?<f83>\(83\)\s.+)", RegexOptions.Singleline);
                    if (match1.Success)
                    {
                        inids.Add(match1.Groups["f57"].Value.Trim());
                        inids.Add(match1.Groups["f83"].Value.Trim());
                    }
                    else inids.Add(inid57);
                }
         
                List<string> tmp = Regex.Split(inidsAfter57, @"(?=\(\d{2}\)\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();
                inids.AddRange(tmp);
            }
            else
            if (subCode == "2" || subCode == "3")
            {
                string inidsBefore57 = note.Substring(0, note.IndexOf("(57)")).Trim();
                string inidsAfter57 = note.Substring(note.IndexOf("(71) T")).Trim();

                string inid57 = note.Substring(note.IndexOf("(57)")).Trim();
                inid57 = inid57.Substring(0, inid57.IndexOf("(71) T")).Trim();

                inids = Regex.Split(inidsBefore57, @"(?=\(\d{2}\)\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                Match match = Regex.Match(inid57, @"(?<f57>\(57\)\s.+)\s?(?<f62>\(62\)\sD.+)", RegexOptions.Singleline);
                if (match.Success)
                {
                    inids.Add(match.Groups["f57"].Value.Trim());
                    inids.Add(match.Groups["f62"].Value.Trim());
                }
                else inids.Add(inid57);

                List<string> tmp = Regex.Split(inidsAfter57, @"(?=\(\d{2}\)\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();
                inids.AddRange(tmp);
            }

            return inids;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + "\n";
                }

            return text;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                if (SendToProduction == true)
                {
                    url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";  // продакшен
                }
                else
                {
                    url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";     // стейдж
                }
                HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = httpClient.PostAsync("", content).Result;
                string answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
