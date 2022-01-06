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

namespace Diamond_RO_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int id = 1;

        private readonly string I11 = "(11)";
        private readonly string I51 = "(51)";
        private readonly string I21 = "(21)";
        private readonly string I22 = "(22)";
        private readonly string I41 = "(41)";
        private readonly string I71 = "(71)";
        private readonly string I72 = "(72)";
        private readonly string I73 = "(73)";
        private readonly string I74 = "(74)";
        private readonly string I86 = "(86)";
        private readonly string I87 = "(87)";
        private readonly string I30 = "(30)";
        private readonly string I54 = "(54)";
        private readonly string I57 = "(57)";
        private readonly string I57n = "(57n)";
        private readonly string I45 = "(45)";
        private readonly string I56 = "(56)";
        private readonly string I95 = "(95)";
        private readonly string I96 = "(96)";
        private readonly string I92 = "(92)";
        private readonly string I93 = "(93)";
        private readonly string I97 = "(97)";
        private readonly string I80 = "(80)";
        private readonly string I84 = "(84)";
        private readonly string I66 = "(66)";
        private readonly string I67 = "(67)";
        private readonly string I68 = "(68)";
        

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> events = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (FileInfo file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            foreach (string tetml in files)
            {
                CurrentFileName = tetml;

                tet = XElement.Load(tetml);

                if(subCode == "13")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Rezumatul invenţiei din cererea de brevet publicată serve"))
                        .TakeWhile(val => !val.Value.StartsWith("Cereri de brevet de invenţie publicate con"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(11\).+\(51\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "BZ"));
                    }
                }
                else if(subCode == "14")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Orice persoană interesată are dreptul să ceară,"))
                        .TakeWhile(val => !val.Value.StartsWith("BREVETE DE INVENŢIE"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(11\).+\(51\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "FG"));
                    }
                }
                else if(subCode == "16")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("1. Brevete de invenþie europene pentru care a fost"))
                       .TakeWhile(val => !val.Value.StartsWith("2. Brevete de invenþie europene pentru care a fost"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(51\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(51)")).ToList();

                    foreach (string note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "BZ"));
                    }

                }
                else if(subCode == "17")
                {                   
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("4. Brevete de invenţie europene cu efecte extinse"))
                       .TakeWhile(val => !val.Value.StartsWith("5. Transmiteri de dreptu i înregistrate la Oficiul de Stat"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(51\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(51)")).ToList();

                    foreach (string note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "BZ"));
                    }
                }
                else if (subCode == "20")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("Semnificaţia codurilor INID folosite în prez nta secţiune (norma ST 9 a Organizaţiei Mondiale"))
                       .TakeWhile(val => !val.Value.StartsWith("Cereri de certificat suplimentar de protecţie aranjate după denumirea solicitantului"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(51\)\s?[A-Z])").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(51)")).ToList();

                    foreach (string note in notes)
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

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(51\)\s?[A-Z])").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(51)")).ToList();

                    foreach (string note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "FG"));
                    }
                }
                else if(subCode == "23")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("7. Decãderi ale titularilor din drepturile conferite\n"+"acestora de brevetul de invenþie european, publicate"))
                       .TakeWhile(val => !val.Value.StartsWith("8. Brevete europene care nu au efecte de la început"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?<=EP\s\d{7})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string note in notes)
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

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?<=0\d{4}\n)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string note in notes)
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

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=a\s\d{4})").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("a 2")).ToList();

                    foreach (string note in notes)
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

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=a\s\d{4})").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("a 2")).ToList();

                    foreach (string note in notes)
                    {
                        events.Add(MakeConvertedPatent(note, subCode, "FA"));
                    }
                }
            }

            return events;
        }

        internal string MakeText (List<XElement> xElement)
        {
            string text = null;

            foreach (XElement element in xElement)
            {
                text += element.Value + "\n";
            }

            return text;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakeConvertedPatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatus = new()
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "RO",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = id++,
                LegalEvent = new()
            };

            Biblio biblio = new()
            {
                DOfPublication = new ()
            };

            CultureInfo cultureInfo = new("RU-ru");

            if (subCode == "13")
            {
                biblio.IntConvention = new IntConvention();

                foreach (string inid in MakeInids13(note))
                {
                    if (inid.StartsWith(I11))
                    {
                        Match match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<number>\d+)\s(?<kind>[A-Z]{1}\d+)");
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

                        List<string> ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc, @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

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
                        Match match = Regex.Match(inid.Replace(I41, ""), @"([0-9]{2}\/[0-9]{2}\/[0-9]{4})");
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
                        List<string> applicants = Regex.Split(inid.Replace(I71, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match match = Regex.Match(applicant, @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

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

                        List<string> inventors = Regex.Split(inid.Replace(I72, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor, @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

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
                        Match match = Regex.Match(inid.Replace(I57n, "").Trim(), @"(?<r>R.+:)\s(?<rn>[0-9]+)\s(?<f>F.+:)\s(?<fn>[0-9]+)\s?");

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
                            Match match1 = Regex.Match(inid.Replace(I57n, "").Trim(), @"(?<r>R.+:)\s(?<rn>[0-9]+)\s?");

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
                        Match match = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<code>[A-Z]{2})\s(?<number>.+)\s(?<date>[0-9]{2}\/[0-9]{2}\/[0-9]{4})");

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
                        Match match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<number>.+)\s(?<date>[0-9]{2}\/[0-9]{2}\/[0-9]{4})");

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
                        List<string> priorites = Regex.Split(inid.Replace(I30, "").Trim(), "").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        biblio.Priorities = new List<Priority>();

                        foreach (string priority in priorites)
                        {
                            Match match = Regex.Match(priority.Trim(), @"(?<date>.+)\s(?<code>[A-Z]{2})\s(?<number>.+)");
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
                        Match match = Regex.Match(inid.Replace(I74, "").Trim(), @"(?<name>.+?),\s(?<adress>.+)");
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

                foreach (string inid in MakeInids14(note))
                {
                    if (inid.StartsWith(I11))
                    {
                        Match match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<number>\d+)\s(?<kind>[A-Z]{1}\d+)");
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

                        List<string> ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc, @"(?<class>.+)\s?\((?<version>[0-9]{4}.[0-9]{2})\)?");

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
                        Match match = Regex.Match(inid.Replace(I41, ""), @"([0-9]{2}\/[0-9]{2}\/[0-9]{4})");
                        if (match.Success)
                        {
                            biblio.DOfPublication.date_41 = DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился в 41");
                    }
                    else
                    if (inid.StartsWith(I45))
                    {
                        Match match = Regex.Match(inid.Replace(I45, ""), @"([0-9]{2}\/[0-9]{2}\/[0-9]{4})");
                        if (match.Success)
                        {
                            biblio.DOfPublication.date_45 = DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        }
                        else Console.WriteLine($"{inid} не разбился в 45");
                    }
                    else
                    if (inid.StartsWith(I56))
                    {
                        List<string> patentCitations = Regex.Split(inid.Replace(I56, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        biblio.PatentCitations = new List<PatentCitation>();

                        foreach (string patentCitation in patentCitations)
                        {
                            Match match = Regex.Match(patentCitation, @"(?<code>[A-Z]{2})\s(?<number>.+)\s(?<kind>[A-Z]{1,2}[0-9]{0,2})");

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
                                Match match1 = Regex.Match(patentCitation, @"(?<code>[A-Z]{2}).?\s(?<number>.+)");

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
                                    Match match2 = Regex.Match(patentCitation, @"(?<number>.+)\s(?<kind>[A-Z]{1,2}[0-9]{0,2})");
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

                        Match match = Regex.Match(inid.Replace(I73, "").Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

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

                        List<string> inventors = Regex.Split(inid.Replace(I72, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor, @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

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
                        Match match = Regex.Match(inid.Replace(I74, "").Trim(), @"(?<name>.+?),\s(?<adress>.+)");
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
                        Match match = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<number>.+)\s?(?<date>[0-9]{2}\/[0-9]{2}\/[0-9]{4})");

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
                        Match match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<number>.+)\s?(?<date>[0-9]{2}\/[0-9]{2}\/[0-9]{4})");

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

                        Match match = Regex.Match(inid.Replace(I66, "").Trim(), @"(?<date>[0-9]{2}\/[0-9]{2}\/[0-9]{4})\s?(?<country>[A-Z]{2})\s(?<number>.+)");

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
            else if (subCode == "16")
            {
                biblio.EuropeanPatents = new();
                EuropeanPatent europeanPatent = new();
                biblio.IntConvention = new();
                biblio.IntConvention.DesignatedStates = new();

                foreach (string inid in MakeInids16(note))
                {
                    if (inid.StartsWith(I51))
                    {
                        biblio.Ipcs = new();

                        List<string> ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

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
                    if (inid.StartsWith(I11))
                    {
                        Match match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<number>.+)\s(?<kind>[A-Z]{1}\d+)");

                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["number"].Value.Trim();
                            biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился");
                    }
                    else
                    if (inid.StartsWith(I96))
                    {
                        Match match = Regex.Match(inid.Replace(I96, "").Trim(), @"(?<number>.+)\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                        if (match.Success)
                        {
                            europeanPatent.AppNumber = match.Groups["number"].Value.Trim();
                            europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился в 96");
                    }
                    else
                    if (inid.StartsWith(I97))
                    {
                        Match match = Regex.Match(inid.Replace(I97, "").Trim(), @"(?<number>.+)\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                        if (match.Success)
                        {
                            europeanPatent.PubNumber = match.Groups["number"].Value.Trim();

                            legalStatus.LegalEvent = new LegalEvent
                            {
                                Language = "EN",
                                Note = "|| Date of publication of EP application  | " + DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim()                            
                            };
                        }
                        else Console.WriteLine($"{inid} не разбился в 97");
                    }
                    else
                    if (inid.StartsWith(I80))
                    {
                        europeanPatent.PubDate = DateTime.Parse(inid.Replace(I80, "").Trim().TrimEnd(';'), cultureInfo).ToString("yyyy.MM.dd").Trim();

                        biblio.EuropeanPatents.Add(europeanPatent);
                    }
                    else
                    if (inid.StartsWith(I30))
                    {
                        biblio.Priorities = new();

                        List<string> priorities = Regex.Split(inid.Replace(I30, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priority in priorities)
                        {
                            Match match = Regex.Match(priority.Trim(), @"(?<code>[A-Z]{2})\s(?<number>.+)\s(?<date>.+)");

                            if (match.Success)
                            {
                                biblio.Priorities.Add(new Priority
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Number = match.Groups["number"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim()
                                });
                            }
                            else Console.WriteLine($"{priority} не разбился в 30");
                        }
                    }
                    else
                    if (inid.StartsWith(I84))
                    {
                        List<string> designatedStates = Regex.Split(inid.Replace(I84, "").Trim(), @",\s").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string item in designatedStates)
                        {
                            if (item.Length == 2)
                            {
                                biblio.IntConvention.DesignatedStates.Add(item);
                            }
                            else
                            if (item.Length == 3)
                            {
                                string tmp = item.Trim().TrimEnd(';');
                                biblio.IntConvention.DesignatedStates.Add(tmp);
                            }
                            else
                            {
                                List<string> designatedStates1 = Regex.Split(inid.Replace(I84, "").Trim(), @"\s").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (string item1 in designatedStates1)
                                {
                                    if (item1.Length == 2)
                                    {
                                        biblio.IntConvention.DesignatedStates.Add(item1);
                                    }
                                    else
                                    if (item1.Length == 3)
                                    {
                                        string tmp1 = item1.Trim().TrimEnd(';');
                                        biblio.IntConvention.DesignatedStates.Add(tmp1);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{item1}");
                                    }
                                }
                            }
                        }
                    }
                    else
                    if (inid.StartsWith(I73))
                    {
                        biblio.Assignees = new();

                        Match match = Regex.Match(inid.Replace(I73, "").Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

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
                    if (inid.StartsWith(I74))
                    {
                        Match match = Regex.Match(inid.Replace(I74, "").Trim(), @"(?<name>.+?),\s(?<adress>.+)");
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
                        else Console.WriteLine($"{inid} Не разделился в 74");
                    }
                    else
                    if (inid.StartsWith(I72))
                    {
                        biblio.Inventors = new();

                        List<string> inventors = Regex.Split(inid.Replace(I72, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

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
                    if (inid.StartsWith(I87))
                    {
                        Match match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<number>.+),\s(?<date>[0-9]{2}\/[0-9]{2}\/[0-9]{4})");

                        if (match.Success)
                        {

                            biblio.IntConvention.PctPublNumber = match.Groups["number"].Value.Trim();
                            biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();

                        }
                        else Console.WriteLine($"{inid} не разбился в 87");
                    }
                    else
                    if (inid.StartsWith(I41))
                    {
                        Match match = Regex.Match(inid.Replace(I41, ""), @"([0-9]{2}.[0-9]{2}.[0-9]{4})");
                        if (match.Success)
                        {
                            biblio.DOfPublication = new()
                            {
                                date_41 = DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim()
                            };

                        }
                        else Console.WriteLine($"{inid} не разбился в 41");
                    }
                    else Console.WriteLine($"{inid} Не обработан");
                }
                legalStatus.Biblio = biblio;
            }
            else if (subCode == "17")
            {
                biblio.EuropeanPatents = new();
                EuropeanPatent europeanPatent = new();

                List<string> text = new();

                foreach (string inid in MakeInids16(note))
                {
                    if (inid.StartsWith(I51))
                    {
                        biblio.Ipcs = new();

                        List<string> ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

                            if (match.Success)
                            {
                                biblio.Ipcs.Add(new Ipc
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["version"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{ipc} не разбилось");
                        }
                    }
                    else
                    if (inid.StartsWith(I11))
                    {
                        Match match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<number>.+)\s(?<kind>[A-Z]{1}\d+)");

                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["number"].Value.Trim();
                            biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился");
                    }
                    else
                    if (inid.StartsWith(I96))
                    {
                        Match match = Regex.Match(inid.Replace(I96, "").Trim(), @"(?<number>.+)\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                        if (match.Success)
                        {
                            europeanPatent.AppNumber = match.Groups["number"].Value.Trim();
                            europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился в 96");
                    }
                    else
                    if (inid.StartsWith(I97))
                    {
                        Match match = Regex.Match(inid.Replace(I97, "").Trim(), @"(?<number>.+)\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                        if (match.Success)
                        {
                            europeanPatent.PubNumber = match.Groups["number"].Value.Trim();
                            text.Add(DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim());
                        }
                        else Console.WriteLine($"{inid} не разбился в 97");
                    }
                    else
                    if (inid.StartsWith(I80))
                    {
                        Match match = Regex.Match(inid.Replace(I80, "").Trim(), @"(?<s>.+?:)\s(?<date>[0-9]{2}\.[0-9]{2}\.[0-9]{4});(?<note>.+:)\s?(?<date1>[0-9]{2}\.[0-9]{2}\.[0-9]{4})");
                        if (match.Success)
                        {
                            europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} не разбился 80");

                        legalStatus.LegalEvent = new LegalEvent
                        {
                            Note = "|| " + text[0] + " | " + text[1] + "\n" + "|| " + text[2] + " | " + text[3] + "\n" + "|| " + text[4]
                            + "\n" + "|| " + match.Groups["note"].Value.Trim() + " | " + DateTime.Parse(match.Groups["date1"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".","/").Trim(),
                            Language = "RO",
                            Translations = new List<NoteTranslation>
                                {
                                    new NoteTranslation
                                    {
                                        Language = "EN",
                                        Tr = "|| (45) Date of publication of the translation of the European patent dossier maintained in modified form | " + text[1] + "\n" +
                                        "|| Date of publication of the translation of the European patent dossier | " + text[3] + "\n" +
                                        "|| Date of publication of EP application | " + text[4] + "\n" +
                                        "|| Date of publication by the European patent office of the mention of maintaining the European patent in modified form | " + DateTime.Parse(match.Groups["date1"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".","/").Trim(),
                                        Type = "note"
                                    }
                                }
                        };


                        biblio.EuropeanPatents.Add(europeanPatent);
                    }
                    else
                    if (inid.StartsWith(I45))
                    {
                        Match match = Regex.Match(inid.Trim(), @"(?<s>.+?:)\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4});(?<note>.+:)\s(?<date1>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                        if (match.Success)
                        {
                            text.Add(match.Groups["s"].Value.Trim());
                            text.Add(DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim());
                            text.Add(match.Groups["note"].Value.Trim());
                            text.Add(DateTime.Parse(match.Groups["date1"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim());
                        }
                        else Console.WriteLine($"{inid} не разбилось 45");
                    }
                    else
                    if (inid.StartsWith(I30))
                    {
                        biblio.Priorities = new();

                        List<string> priorities = Regex.Split(inid.Replace(I30, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priority in priorities)
                        {
                            Match match = Regex.Match(priority.Trim(), @"(?<code>[A-Z]{2})\s(?<number>.+)\s(?<date>.+)");

                            if (match.Success)
                            {
                                biblio.Priorities.Add(new Priority
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Number = match.Groups["number"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim()
                                });
                            }
                            else Console.WriteLine($"{priority} не разбился в 30");
                        }
                    }
                    else
                    if (inid.StartsWith(I84))
                    {
                        biblio.IntConvention = new();
                        biblio.IntConvention.DesignatedStates = new();

                        List<string> designatedStates = Regex.Split(inid.Replace(I84, "").Trim(), @",\s").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string item in designatedStates)
                        {
                            if (item.Length == 2)
                            {
                                biblio.IntConvention.DesignatedStates.Add(item);
                            }
                            else
                            if (item.Length == 3)
                            {
                                string tmp = item.Trim().TrimEnd(';');
                                biblio.IntConvention.DesignatedStates.Add(tmp);
                            }
                            else
                            {
                                List<string> designatedStates1 = Regex.Split(inid.Replace(I84, "").Trim(), @"\s").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (string item1 in designatedStates1)
                                {
                                    if (item1.Length == 2)
                                    {
                                        biblio.IntConvention.DesignatedStates.Add(item1);
                                    }
                                    else
                                    if (item1.Length == 3)
                                    {
                                        string tmp1 = item1.Trim().TrimEnd(';');
                                        biblio.IntConvention.DesignatedStates.Add(tmp1);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{item1}");
                                    }
                                }
                            }
                        }
                    }
                    else
                    if (inid.StartsWith(I73))
                    {
                        biblio.Assignees = new();

                        Match match = Regex.Match(inid.Replace(I73, "").Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

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
                    if (inid.StartsWith(I74))
                    {
                        Match match = Regex.Match(inid.Replace(I74, "").Trim(), @"(?<name>.+?),\s(?<adress>.+)");
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
                        else Console.WriteLine($"{inid} Не разделился в 74");
                    }
                    else
                    if (inid.StartsWith(I72))
                    {
                        biblio.Inventors = new();

                        List<string> inventors = Regex.Split(inid.Replace(I72, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<country>[A-Z]{2})");

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
                    else Console.WriteLine($"{inid} Не обработан");

                    legalStatus.Biblio = biblio;
                }
            }
            else if (subCode == "20")
            {
                biblio.EuropeanPatents = new();
                EuropeanPatent europeanPatent = new();

                foreach (string inid in MakeInids16(note))
                {
                    if (inid.StartsWith(I51))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

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
                        Match match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<number>.+)\s(?<kind>[A-Z]{1}\d+)");

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
                        Match match = Regex.Match(inid.Replace(I68, "").Trim(), @"(?<num>.+)\/(?<date>\d{2}\/\d{2}\/\d{4})");

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
                        Match match = Regex.Match(inid.Replace(I92, "").Trim(), @"(?<num>.+)\/\/(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {

                            europeanPatent.Spc92Number = match.Groups["num"].Value.Trim();
                            europeanPatent.Spc92Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            
                        }
                        else Console.WriteLine($"{inid}  ---- 92");
                    }
                    else if (inid.StartsWith(I93))
                    {
                        Match match = Regex.Match(inid.Replace(I93, "").Trim(), @"(?<num>.+),\s(?<date>\d{2}\/\d{2}\/\d{4})\s?\/(?<note>.+);");

                        if (match.Success)
                        {
                            europeanPatent.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            europeanPatent.Number = match.Groups["num"].Value.Trim();


                            Match match1 = Regex.Match(match.Groups["note"].Value.Trim(), @"(?<text>.+?)(?<date>\d{2}.\d{2}.\d{4});(?<text1>.+?)(?<num>\d{1,5}\/\d{4})(?<text2>.+)");

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
                                Match match2 = Regex.Match(match.Groups["note"].Value.Trim(), @"(?<text1>Sol.+?)(?<num>\d{1,5}\/\d{1,4})(?<text2>.+)");

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
                        List<string> applicants = Regex.Split(inid.Replace(I71, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match match = Regex.Match(applicant, @"(?<name>.+INC\.|.+Inc\.|.+Limited|.+LIMITED),\s(?<adress>.+),\s(?<code>\D{2})");

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
                                Match match1 = Regex.Match(applicant, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>\D{2})");

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
                        Match match = Regex.Match(inid.Replace(I74, "").Trim(), @"(?<name>.+?),\s(?<adress>.+)");

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

                Match date = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")), @"\d{8}");
                if (date.Success)
                {
                    legalStatus.LegalEvent.Date = date.Value.Insert(4, "/").Insert(7, "/").Trim();
                }
            }
            else if (subCode == "22")
            {
                foreach (string inid in MakeInids(note,subCode))
                {
                    if (inid.StartsWith(I51))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>\d{4}.\d{2})\)?");

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
                        Match match = Regex.Match(inid.Replace(I11, "").Trim(), @"(?<number>.+)\s(?<kind>[A-Z]{1}\d+)");

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
                        Match match = Regex.Match(inid.Replace(I73, "").Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<code>\D{2})");

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
                        Match match = Regex.Match(inid.Replace(I72, "").Trim(), @"(?<name>.+?),\s(?<adress>.+),\s(?<code>\D{2})");

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
                        Match match = Regex.Match(inid.Replace(I57n, "").Trim(), @"(?<rev>Rev.+?):\s?(?<numCl>\d+)\s?(?<fig>Fig.+):\s?(?<numFig>\d+)");

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
                Match match = Regex.Match(note.Trim(), @"Brevet\s(?<owner>.+)\s(?<pNum>\D{2}\/\D{2}\s\d+)", RegexOptions.Singleline);
                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["pNum"].Value.Trim();

                    List<string> owners = Regex.Split(match.Groups["owner"].Value.Replace("\r", "").Replace("\n", " ").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string owner in owners)
                    {
                        Match match1 = Regex.Match(owner, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>[A-Z]{2})");

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

                    Match match2 = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")), @"\d{8}");
                    if (match2.Success)
                    {
                        legalStatus.LegalEvent.Date = match2.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }

                }
                else
                {
                    Match match1 = Regex.Match(note, @"(?<owner>.+)\s(?<pNum>\D{2}\/\D{2}\s\d+)", RegexOptions.Singleline);

                    if (match1.Success)
                    {
                        biblio.Publication.Number = match1.Groups["pNum"].Value.Trim();

                        List<string> owners = Regex.Split(match1.Groups["owner"].Value.Replace("\r", "").Replace("\n", " ").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string owner in owners)
                        {
                            Match match3 = Regex.Match(owner, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>[A-Z]{2})");

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

                        Match match2 = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")), @"\d{8}");
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

                Match match11 = Regex.Match(note.Trim(), @"Nr. CBI\s(?<name>.+?)\s(?<pNum>\d+)\s(?<aNum>.+)", RegexOptions.Singleline);
                if (match11.Success)
                {
                    biblio.Publication.Number = match11.Groups["pNum"].Value.Trim();
                    biblio.Application.Number = match11.Groups["aNum"].Value.Trim();

                    List<string> owners = Regex.Split(match11.Groups["name"].Value.Replace("\r", "").Replace("\n", " ").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string owner in owners)
                    {
                        Match match1 = Regex.Match(owner, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>[A-Z]{2})");

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

                    Match match2 = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")), @"\d{8}");
                    if (match2.Success)
                    {
                        legalStatus.LegalEvent.Date = match2.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }
                }
                else
                {
                    Match match = Regex.Match(note.Trim(), @"(?<name>.+?)\s(?<pNum>\d+)\s(?<aNum>.+)", RegexOptions.Singleline);
                    if (match.Success)
                    {
                        biblio.Publication.Number = match.Groups["pNum"].Value.Trim();
                        biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                        List<string> owners = Regex.Split(match.Groups["name"].Value.Replace("\r", "").Replace("\n", " ").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string owner in owners)
                        {
                            Match match1 = Regex.Match(owner, @"(?<name>.+?),\s(?<adress>.+),\s(?<code>[A-Z]{2})");

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

                        Match match2 = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")), @"\d{8}");
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
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<aNum>.+)\s.+\/(?<leDate>\d{2}\/\d{2}\/\d{4})\s(?<leNote>\d{1,2}\/\d{4})\/\/(?<date41>\d{2}\/\d{2}\/\d{4})");

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
                            Type = "note"
                        }
                    };

                    legalStatus.LegalEvent.Date = DateTime.Parse(match.Groups["leDate"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();

                    biblio.DOfPublication.date_41 = DateTime.Parse(match.Groups["date41"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();

                }
                else
                {
                    Match match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<aNum>.+)\s.+\/(?<leDate>\d{2}\/\d{2}\/\d{4})");
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

        internal List<string> MakeInids13 (string note)
        {
            string inid57 = note.Substring(note.IndexOf("(57)")).Replace("\r","").Replace("\n", " ").Trim();

            string note57 = null;

            List<string> inids = new();

            if (inid57.Contains("(11)"))
            {
                inid57 = Regex.Replace(inid57, @"(\(11\)\s[0-9]{6}\s[A-Z]{1}[0-9]{1}\s)", "");

            }

            Match match = Regex.Match(inid57, @"(?<inid57>.+)\s(?<note>Revend.+)");
            if (match.Success)
            {
                inid57 = match.Groups["inid57"].Value.Trim();
                note57 ="(57n) " + match.Groups["note"].Value.Trim();
            }
            else
            {
                Console.WriteLine($"{inid57} не нашло note57");
            }

            string noteWithOut57 = Regex.Replace(note.Replace("\r", "").Replace("\n", " "), @"(\(57\).+)", "").Trim();

            Match match1 = Regex.Match(noteWithOut57, @"(?<inid11>\(11\).+)\s(?<note>\(51.+)");

            if (match1.Success)
            {
                string tmp = Regex.Replace(match1.Groups["note"].Value.Trim(), @"\(11\)\s?[0-9]+\s[A-Z]+[0-9]+", "").Trim();

                inids = Regex.Split(tmp, @"(?=\([0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                inids.Add(match1.Groups["inid11"].Value.Trim());
            }   

            inids.Add(inid57);
            inids.Add(note57);

            return inids;
        }

        internal List<string> MakeInids14(string note)
        {
            string inid11 = note.Replace("\r", "").Replace("\n", " ").Trim().Substring(note.IndexOf("(11)"), note.IndexOf("(51)")).Trim();

            string inidsWithOutInid11 = Regex.Replace(note.Replace("\r", "").Replace("\n", " ").Trim(), @"\(11\)\s[0-9]{6,7}\s[A-Z][0-9]","").Trim();

            List<string> inids = Regex.Split(inidsWithOutInid11, @"(?=\([0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

            inids.Add(inid11);

            return inids;
        }

        internal List<string> MakeInids16(string note) => Regex.Split(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?=\([0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

        internal List<string> MakeInids (string note, string subCode)
        {
            List<string> inids = new();

            if (subCode == "22")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<inids>.+)\s(?<inid57>\(57\).+)\s(?<note>Reve.+)\(.+(?<inid45>\d{2}.\d{2}.\d{4})");

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
