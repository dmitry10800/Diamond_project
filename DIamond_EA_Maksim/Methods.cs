using Integration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DIamond_EA_Maksim
{
    class Methods
    {

        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
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

                if(subCode == "5")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("FA9A ОТЗЫВ ЗАЯВКИ "))
                             .TakeWhile(val => !val.Value.StartsWith("ЕВРАЗИЙСКАЯ ПАТЕНТНАЯ ОРГАНИЗАЦИЯ (ЕАПО)"))
                             .ToList();

                    List<string> notes = Regex.Split(BuildText(xElements).Replace("\r", "").Replace("\n", " "), @"(?=\d{9}\s)")
                        .Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("2")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "FA9A"));
                    }
                }
                else
                if (subCode == "9")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("PC1A РЕГИСТРАЦИЯ ПЕРЕДАЧИ ПРАВА НА ЕВРАЗИЙСКУЮ ЗАЯВКУ ПУТЕМ УСТУПКИ ПРАВА (79)"))
                             .TakeWhile(val => !val.Value.StartsWith("ЕВРАЗИЙСКАЯ ПАТЕНТНАЯ ОРГАНИЗАЦИЯ (ЕАПО)"))
                             .ToList();

                    List<string> notes = Regex.Split(BuildText(xElements).Replace("\r", "").Replace("\n", " "), @"(?=\d{9}\s[A-Z])")
                        .Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("2")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "PC1A"));
                    }
                }
                else
                if (subCode == "11")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("MK4A ПРЕКРАЩЕНИЕ ДЕЙСТВИЯ ЕВРАЗИЙСКОГО ПАТЕНТА ВВИДУ ИСТЕЧЕНИЯ СРОКА ДЕЙСТВИЯ "))
                            .TakeWhile(val => !val.Value.StartsWith("ЕВРАЗИЙСКАЯ ПАТЕНТНАЯ ОРГАНИЗАЦИЯ (ЕАПО)"))
                            .ToList();

                    List<string> notes = Regex.Split(BuildText(xElements).Replace("\r", "").Replace("\n", " "), @"(?=0\d{5}\s)")
                        .Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("0")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "MK"));
                    }
                }
                else
                 if (subCode  == "12")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("MM4A ДОСРОЧНОЕ ПРЕКРАЩЕНИЕ ДЕЙСТВИЯ ЕВРАЗИЙСКОГО ПАТЕНТА ИЗ-ЗА НЕУПЛАТЫ В УСТАНОВЛЕННЫЙ СРОК"))
                            .TakeWhile(val => !val.Value.StartsWith("Период публикации сведений в бюллетене"))
                            .TakeWhile(val => !val.Value.StartsWith("ЕВРАЗИЙСКАЯ ПАТЕНТНАЯ ОРГАНИЗАЦИЯ (ЕАПО)"))
                            .ToList();

                    List<string> notes = Regex.Split(BuildText(xElements).Replace("\r", "").Replace("\n", " "), @"(?=0\d{5}\s)")
                        .Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("0")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "MK"));
                    }
                }
                else
                if (subCode == "14")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                           .SkipWhile(val => !val.Value.StartsWith("NF4A ВОССТАНОВЛЕНИЕ ПРАВА НА ЕВРАЗИЙСКИЙ ПАТЕНТ (18)"))
                           .TakeWhile(val => !val.Value.StartsWith("ЕВРАЗИЙСКАЯ ПАТЕНТНАЯ ОРГАНИЗАЦИЯ (ЕАПО)"))
                           .ToList();

                    List<string> notes = Regex.Split(BuildText(xElements).Replace("\r", "").Replace("\n", " "), @"(?=\d{6}\s[A-Z])")
                        .Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("0")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "NF4A"));
                    }
                }
                else
                if (subCode == "31")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("PC4A РЕГИСТРАЦИЯ ПЕРЕДАЧИ ПРАВА НА ЕВРАЗИЙСКИЙ ПАТЕНТ ПУТЕМ УСТУПКИ ПРАВА (53)") && !val.Value.StartsWith("PC4A РЕГИСТРАЦИЯ ПЕРЕДАЧИ ПРАВА НА ЕВРАЗИЙСКИЙ ПАТЕНТ ПУТЕМ УСТУПКИ ПРАВА (86)"))
                             .TakeWhile(val => !val.Value.StartsWith("ЕВРАЗИЙСКАЯ ПАТЕНТНАЯ ОРГАНИЗАЦИЯ (ЕАПО)"))
                             .ToList();

                    List<string> notes = Regex.Split(BuildText(xElements).Replace("\r","").Replace("\n"," "), @"(?=[0-9]{6}\s[A-Z])")
                        .Where(val => !string.IsNullOrEmpty(val)).Where( val => val.StartsWith("0")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "PC4A"));
                    }
                }
            }
            return statusEvents;
        }
        internal string BuildText(List<XElement> xElements)
        {
            string fullText = null;

            foreach (XElement text in xElements)
            {
                fullText += text.Value.Trim() + " ";
            }
            return fullText;
        }

        internal Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legal = new()
            {
                CountryCode = "EA",
                SectionCode = sectionCode,
                SubCode = subCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf"))              
            };

            Biblio biblio = new()
            {
                Assignees = new(),
                Applicants = new()
            };

            LegalEvent legalEvent = new()
            {
                Translations = new()
            };
            NoteTranslation noteTranslation = new();

            legal.NewBiblio= new Biblio()
            {
                Applicants = new(),
                Assignees = new()
            };

            if (subCode == "5")
            {
                Match match = Regex.Match(note.Trim(), @"(?<appNum>\d+)\s(?<pubKind>\D\d+)\s(?<date43>\d{4}.\d{2}.\d{2})\s(?<note>.+)\s(?<leDate>\d{4}.\d{2}.\d{2})\s(?<noteDate>\d{4}.\d{2}.\d{2})");

                if (match.Success)
                {
                    biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                    biblio.Publication.Kind = match.Groups["pubKind"].Value.Trim();

                    biblio.Publication.Date = match.Groups["date43"].Value.Replace(".", "/").Trim();

                    legalEvent.Note = "|| № Бюллетеня, в котором была опубликована заявка | " + match.Groups["note"].Value.Replace("N o", "№").Trim()
                        + "\n" + "|| Дата публикации извещения | " + match.Groups["noteDate"].Value.Replace(".", "/").Trim();
                    legalEvent.Language = "RU";

                    noteTranslation.Language = "EN";
                    noteTranslation.Type = "note";
                    noteTranslation.Tr = "|| Application publication Bulletin No. | " + match.Groups["note"].Value.Replace("N o", "No.").Trim() 
                        + "\n" + "|| Publication date of notice | " + match.Groups["noteDate"].Value.Replace(".", "/").Trim();

                    legalEvent.Translations.Add(noteTranslation);

                    legalEvent.Date = match.Groups["leDate"].Value.Replace(".","/").Trim();
                }
                else
                {
                    Match match1 = Regex.Match(note.Trim(), @"(?<appNum>\d+)\s(?<date43>\d{4}.\d{2}.\d{2})\s(?<leDate>\d{4}.\d{2}.\d{2})\s(?<noteDate>\d{4}.\d{2}.\d{2})\s(?<pubKind>\D\d+)\s(?<note>.+)");

                    if (match1.Success)
                    {
                        biblio.Application.Number = match1.Groups["appNum"].Value.Trim();
                        biblio.Publication.Kind = match1.Groups["pubKind"].Value.Trim();

                        biblio.Publication.Date = match1.Groups["date43"].Value.Replace(".", "/").Trim();

                        legalEvent.Note = "|| № Бюллетеня, в котором была опубликована заявка | " + match1.Groups["note"].Value.Replace("N o", "№").Trim()
                            + "\n" + "|| Дата публикации извещения | " + match1.Groups["noteDate"].Value.Replace(".", "/").Trim();
                        legalEvent.Language = "RU";

                        noteTranslation.Language = "EN";
                        noteTranslation.Type = "note";
                        noteTranslation.Tr = "|| Application publication Bulletin No. | " + match1.Groups["note"].Value.Replace("N o", "No.").Trim()
                            + "\n" + "|| Publication date of notice | " + match1.Groups["noteDate"].Value.Replace(".", "/").Trim();

                        legalEvent.Translations.Add(noteTranslation);

                        legalEvent.Date = match1.Groups["leDate"].Value.Replace(".", "/").Trim();
                    }
                    else Console.WriteLine($"{note} - 5");

                }
                legal.LegalEvent = legalEvent;
                legal.Biblio = biblio;
            }
            else
            if( subCode == "9")
            {
                Match match = Regex.Match(note.Trim(), @"(?<appNum>\d+)\s(?<appKind>\D\d+)\s(?<date43>\d{4}.\d{2}.\d{2})\s(?<note>N\s?o\s?\d+)\s(?<applicants>.+)\s(?<leDate>\d{4}.\d{2}.\d{2})\s(?<noteDate>\d{4}.\d{2}.\d{2})");

                if (match.Success)
                {
                    biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                    biblio.Publication.Kind = match.Groups["appKind"].Value.Trim();

                    biblio.Publication.Date = match.Groups["date43"].Value.Replace(".", "/").Trim();

                    legalEvent.Note = "|| № Бюллетеня, в котором была опубликована заявка | " + match.Groups["note"].Value.Replace("N o", "№").Trim()
                        + "\n" + "|| Дата публикации извещения | " + match.Groups["noteDate"].Value.Replace(".", "/").Trim();
                    legalEvent.Language = "RU";

                    noteTranslation.Language = "EN";
                    noteTranslation.Type = "note";
                    noteTranslation.Tr = "|| Application publication Bulletin No. | " + match.Groups["note"].Value.Replace("N o", "No.").Trim()
                        + "\n" + "|| Publication date of notice | " + match.Groups["noteDate"].Value.Replace(".", "/").Trim();

                    legalEvent.Translations.Add(noteTranslation);

                    legalEvent.Date = match.Groups["leDate"].Value.Replace(".", "/").Trim();

                    List<string> applicantsTemp = Regex.Split(match.Groups["applicants"].Value.Trim(), @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                    List<string> applicants = new();

                    foreach (string applicantTemp in applicantsTemp)
                    {
                        if (applicantTemp.Contains(";"))
                        {
                            applicants = Regex.Split(applicantTemp, @";").Where(val => !string.IsNullOrEmpty(val)).ToList();
                        }
                        else applicants.Add(applicantTemp);
                    }

                    if (applicants.Count % 2 != 0)
                    {
                        for (int i = 0; i < applicants.Count; i++)
                        {
                            if(i == applicants.Count - 1)
                            {
                                Match match1 = Regex.Match(applicants[i].Trim(), @"(?<name>.+)\((?<country>[A-Z]{2})");
                                if (match1.Success)
                                {
                                    legal.NewBiblio.Applicants.Add(new PartyMember
                                    {
                                        Name = match1.Groups["name"].Value.Trim(),
                                        Country = match1.Groups["country"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{applicants[i]} - не обработался в Новом Библио ");
                            }
                            else
                            {
                                Match match1 = Regex.Match(applicants[i].Trim(), @"(?<name>.+)\((?<country>[A-Z]{2})");
                                if (match1.Success)
                                {
                                    biblio.Applicants.Add(new PartyMember
                                    {
                                        Name = match1.Groups["name"].Value.Trim(),
                                        Country = match1.Groups["country"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    biblio.Applicants.Add(new PartyMember
                                    {
                                        Name = applicants[i].Trim()
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < applicants.Count/2; i++)
                        {
                            Match match1 = Regex.Match(applicants[i].Trim(), @"(?<name>.+)\((?<country>[A-Z]{2})");
                            if (match1.Success)
                            {
                                biblio.Applicants.Add(new PartyMember
                                {
                                    Name = match1.Groups["name"].Value.Trim(),
                                    Country = match1.Groups["country"].Value.Trim()
                                });
                            }
                            else
                            {
                                biblio.Applicants.Add(new PartyMember
                                {
                                    Name = applicants[i].Trim()
                                });
                            }
                        }


                        for (int i = applicants.Count/2; i < applicants.Count; i++)
                        {
                            Match match1 = Regex.Match(applicants[i].Trim(), @"(?<name>.+)\((?<country>[A-Z]{2})");
                            if (match1.Success)
                            {
                                legal.NewBiblio.Applicants.Add(new PartyMember
                                {
                                    Name = match1.Groups["name"].Value.Trim(),
                                    Country = match1.Groups["country"].Value.Trim()
                                });
                            }
                            else
                            {
                                biblio.Applicants.Add(new PartyMember
                                {
                                    Name = applicants[i].Trim()
                                });
                            }
                        }
                    }

                }
                else Console.WriteLine($"{note}");

                legal.LegalEvent = legalEvent;
                legal.Biblio = biblio;
            }
            else
            if(subCode == "11")
            {
                Match match = Regex.Match(note.Trim(), @"(?<pubNum>\d+)\s(?<pubKind>[A-Z]\d+)\s(?<date45>\d{4}.\d{2}.\d{2})\s(?<noteNum>N\s?o\s?\d+)\s(?<appNum>\d+)\s(?<appDate>\d{4}.\d{2}.\d{2})\s(?<leDate>\d{4}.\d{2}.\d{2})\s(?<leNote>.+)\s(?<leNoteDate>\d{4}.\d{2}.\d{2})");

                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();
                    biblio.Publication.Kind = match.Groups["pubKind"].Value.Trim();

                    biblio.DOfPublication = new()
                    {
                        date_45 = match.Groups["date45"].Value.Replace(".", "/").Trim()
                    };

                    legalEvent.Note = "|| № Бюллетеня, в котором был опубликован патент | " + match.Groups["noteNum"].Value.Replace("N o", "№").Trim()
                        + "\n" + "|| Код государства, в котором истек срок действия патента | " + match.Groups["leNote"].Value.Trim()
                    + "\n" + "|| Дата публикации извещения | " + match.Groups["leNoteDate"].Value.Replace(".", "/").Trim();
                    legalEvent.Language = "RU";

                    noteTranslation.Language = "EN";
                    noteTranslation.Type = "note";
                    noteTranslation.Tr = "|| Eurasian patent publication Bulletin No. | " + match.Groups["noteNum"].Value.Replace("N o", "No.").Trim()
                        + "\n" + "|| Country code where the patent is expired | " + match.Groups["leNote"].Value.Trim()
                    + "\n" + "|| Publication date of notice | " + match.Groups["leNoteDate"].Value.Replace(".", "/").Trim();

                    legalEvent.Translations.Add(noteTranslation);
                    biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                    biblio.Application.Date = match.Groups["appDate"].Value.Replace(".", "/").Trim();

                    legalEvent.Date = match.Groups["leDate"].Value.Replace(".", "/").Trim();

                }
                else
                {
                    Match match1 = Regex.Match(note.Trim(), @"(?<pubNum>\d+)\s(?<date45>\d{4}.\d{2}.\d{2})\s(?<appNum>\d+)\s(?<appDate>\d{4}.\d{2}.\d{2})\s(?<leDate>\d{4}.\d{2}.\d{2})\s(?<leNoteDate>\d{4}.\d{2}.\d{2})\s(?<pubKind>[A-Z]\d+)\s(?<noteNum>N\s?o\s?\d+)\s(?<leNote>.+)");

                    if (match1.Success)
                    {
                        biblio.Publication.Number = match1.Groups["pubNum"].Value.Trim();
                        biblio.Publication.Kind = match1.Groups["pubKind"].Value.Trim();

                        biblio.DOfPublication = new()
                        {
                            date_45 = match1.Groups["date45"].Value.Replace(".", "/").Trim()
                        };

                        legalEvent.Note = "|| № Бюллетеня, в котором был опубликован патент | " + match1.Groups["noteNum"].Value.Replace("N o", "№").Trim()
                            + "\n" + "|| Код государства, в котором истек срок действия патента | " + match1.Groups["leNote"].Value.Trim()
                        + "\n" + "|| Дата публикации извещения | " + match1.Groups["leNoteDate"].Value.Replace(".", "/").Trim();
                        legalEvent.Language = "RU";

                        noteTranslation.Language = "EN";
                        noteTranslation.Type = "note";
                        noteTranslation.Tr = "|| Eurasian patent publication Bulletin No. | " + match1.Groups["noteNum"].Value.Replace("N o", "No.").Trim()
                            + "\n" + "|| Country code where the patent is expired | " + match1.Groups["leNote"].Value.Trim()
                        + "\n" + "|| Publication date of notice | " + match1.Groups["leNoteDate"].Value.Replace(".", "/").Trim();

                        legalEvent.Translations.Add(noteTranslation);
                        biblio.Application.Number = match1.Groups["appNum"].Value.Trim();
                        biblio.Application.Date = match1.Groups["appDate"].Value.Replace(".", "/").Trim();

                        legalEvent.Date = match1.Groups["leDate"].Value.Replace(".", "/").Trim();
                    }
                    else Console.WriteLine($"{note} 11 sub");
                }


                legal.LegalEvent = legalEvent;
                legal.Biblio = biblio;
            }
            else
            if (subCode == "12")
            {
                Match match = Regex.Match(note.Trim(), 
                    @"(?<pubNum>\d+)\s(?<date45>\d{4}.\d{2}.\d{2})\s(?<leNote1>.+)\s(?<leDate>\d{4}.\d{2}.\d{2})\s(?<leNote2>.+)\s(?<leNoteDate>\d{4}.\d{2}.\d{2})\s(?<pubKind>\D{1}\d{1,2})\s(?<leNum>.+)");

                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();
                    biblio.Publication.Kind = match.Groups["pubKind"].Value.Trim();

                    biblio.DOfPublication = new()
                    {
                        date_45 = match.Groups["date45"].Value.Replace(".", "/").Trim()
                    };

                    legalEvent.Note = "|| № Бюллетеня, в котором был опубликован патент | " + match.Groups["leNum"].Value.Replace("N o", "№").Trim()
                        + "\n" + "|| Код государства, на территории которого прекращено действие патента | " + match.Groups["leNote1"].Value.Trim()
                        + "\n" + "|| Код государства, на территории которого продолжается действие патента | " + match.Groups["leNote2"].Value.Trim()
                    + "\n" + "|| Дата публикации извещения | " + match.Groups["leNoteDate"].Value.Replace(".", "/").Trim();
                    legalEvent.Language = "RU";

                    noteTranslation.Language = "EN";
                    noteTranslation.Type = "note";
                    noteTranslation.Tr = "|| Eurasian patent publication Bulletin No. | " + match.Groups["leNum"].Value.Replace("N o", "No.").Trim()
                        + "\n" + "|| Code of country on which territory the patent is terminated | " + match.Groups["leNote1"].Value.Trim()
                        + "\n" + "|| Code of country on which territory the patent is valid | " + match.Groups["leNote2"].Value.Trim()
                    + "\n" + "|| Publication date of notice | " + match.Groups["leNoteDate"].Value.Replace(".", "/").Trim();

                    legalEvent.Translations.Add(noteTranslation);

                    legalEvent.Date = match.Groups["leDate"].Value.Replace(".", "/").Trim();

                }
                else
                {
                    Match match1 = Regex.Match(note.Trim(),
                        @"(?<pubNum>\d+)\s(?<leNum>.+)\s(?<date45>\d{4}.\d{2}.\d{2})\s(?<leNote1>.+)\s(?<leDate>\d{4}.\d{2}.\d{2})\s(?<leNote2>.+)\s(?<leNoteDate>\d{4}.\d{2}.\d{2})\s(?<pubKind>\D{1}\d{1,2})");

                    if (match1.Success)
                    {
                        biblio.Publication.Number = match1.Groups["pubNum"].Value.Trim();
                        biblio.Publication.Kind = match1.Groups["pubKind"].Value.Trim();

                        biblio.DOfPublication = new()
                        {
                            date_45 = match1.Groups["date45"].Value.Replace(".", "/").Trim()
                        };

                        legalEvent.Note = "|| № Бюллетеня, в котором был опубликован патент | " + match1.Groups["leNum"].Value.Replace("N o", "№").Trim()
                            + "\n" + "|| Код государства, на территории которого прекращено действие патента | " + match1.Groups["leNote1"].Value.Trim()
                            + "\n" + "|| Код государства, на территории которого продолжается действие патента | " + match1.Groups["leNote2"].Value.Trim()
                        + "\n" + "|| Дата публикации извещения | " + match1.Groups["leNoteDate"].Value.Replace(".", "/").Trim();
                        legalEvent.Language = "RU";

                        noteTranslation.Language = "EN";
                        noteTranslation.Type = "note";
                        noteTranslation.Tr = "|| Eurasian patent publication Bulletin No. | " + match1.Groups["leNum"].Value.Replace("N o", "No.").Trim()
                            + "\n" + "|| Code of country on which territory the patent is terminated | " + match1.Groups["leNote1"].Value.Trim()
                            + "\n" + "|| Code of country on which territory the patent is valid | " + match1.Groups["leNote2"].Value.Trim()
                        + "\n" + "|| Publication date of notice | " + match1.Groups["leNoteDate"].Value.Replace(".", "/").Trim();

                        legalEvent.Translations.Add(noteTranslation);

                        legalEvent.Date = match1.Groups["leDate"].Value.Replace(".", "/").Trim();
                    }
                    else
                    {
                        Match match2 = Regex.Match(note.Trim(), 
                            @"(?<pubNum>\d+)\s(?<pubKind>\D{1}\d{1,2})\s(?<date45>\d{4}.\d{2}.\d{2})\s(?<leNum>.+?)\s(?<leNote1>[A-Z].+)\s(?<leDate>\d{4}.\d{2}.\d{2})\s(?<leNote2>.+)\s(?<leNoteDate>\d{4}.\d{2}.\d{2})");

                        if (match2.Success)
                        {
                            biblio.Publication.Number = match2.Groups["pubNum"].Value.Trim();
                            biblio.Publication.Kind = match2.Groups["pubKind"].Value.Trim();

                            biblio.DOfPublication = new()
                            {
                                date_45 = match2.Groups["date45"].Value.Replace(".", "/").Trim()
                            };

                            legalEvent.Note = "|| № Бюллетеня, в котором был опубликован патент | " + match2.Groups["leNum"].Value.Replace("N o", "№").Trim()
                                + "\n" + "|| Код государства, на территории которого прекращено действие патента | " + match2.Groups["leNote1"].Value.Trim()
                                + "\n" + "|| Код государства, на территории которого продолжается действие патента | " + match2.Groups["leNote2"].Value.Trim()
                            + "\n" + "|| Дата публикации извещения | " + match2.Groups["leNoteDate"].Value.Replace(".", "/").Trim();
                            legalEvent.Language = "RU";

                            noteTranslation.Language = "EN";
                            noteTranslation.Type = "note";
                            noteTranslation.Tr = "|| Eurasian patent publication Bulletin No. | " + match2.Groups["leNum"].Value.Replace("N o", "No.").Trim()
                                + "\n" + "|| Code of country on which territory the patent is terminated | " + match2.Groups["leNote1"].Value.Trim()
                                + "\n" + "|| Code of country on which territory the patent is valid | " + match2.Groups["leNote2"].Value.Trim()
                            + "\n" + "|| Publication date of notice | " + match2.Groups["leNoteDate"].Value.Replace(".", "/").Trim();

                            legalEvent.Translations.Add(noteTranslation);

                            legalEvent.Date = match2.Groups["leDate"].Value.Replace(".", "/").Trim();
                        }
                        else
                        {
                            Match match3 = Regex.Match(note.Trim(), @"(?<pubNum>\d+)\s(?<date45>\d{4}.\d{2}.\d{2})\s(?<leNum>.+?)\s(?<leDate>\d{4}.\d{2}.\d{2})\s(?<leNote2>.+)\s(?<leNoteDate>\d{4}.\d{2}.\d{2})\s(?<pubKind>\D{1}\d{1,2})");

                            if (match3.Success)
                            {
                                biblio.Publication.Number = match3.Groups["pubNum"].Value.Trim();
                                biblio.Publication.Kind = match3.Groups["pubKind"].Value.Trim();

                                biblio.DOfPublication = new()
                                {
                                    date_45 = match3.Groups["date45"].Value.Replace(".", "/").Trim()
                                };

                                legalEvent.Note = "|| № Бюллетеня, в котором был опубликован патент | " 
                                    + "\n" + "|| Код государства, на территории которого прекращено действие патента | " + match3.Groups["leNote1"].Value.Trim()
                                    + "\n" + "|| Код государства, на территории которого продолжается действие патента | " + match3.Groups["leNote2"].Value.Trim()
                                + "\n" + "|| Дата публикации извещения | " + match3.Groups["leNoteDate"].Value.Replace(".", "/").Trim();
                                legalEvent.Language = "RU";

                                noteTranslation.Language = "EN";
                                noteTranslation.Type = "note";
                                noteTranslation.Tr = "|| Eurasian patent publication Bulletin No. | " 
                                    + "\n" + "|| Code of country on which territory the patent is terminated | " + match3.Groups["leNote1"].Value.Trim()
                                    + "\n" + "|| Code of country on which territory the patent is valid | " + match3.Groups["leNote2"].Value.Trim()
                                + "\n" + "|| Publication date of notice | " + match3.Groups["leNoteDate"].Value.Replace(".", "/").Trim();

                                legalEvent.Translations.Add(noteTranslation);

                                legalEvent.Date = match3.Groups["leDate"].Value.Replace(".", "/").Trim();
                            }
                            else Console.WriteLine($"{note}");
                        }
                       

                    }
                   
                }


                legal.LegalEvent = legalEvent;
                legal.Biblio = biblio;
            }
            else
            if (subCode == "14")
            {
                Match match = Regex.Match(note.Trim(), @"(?<pubNum>\d+)\s(?<pubKind>[A-Z]\d+)\s(?<date45>\d{4}.\d{2}.\d{2})\s(?<noteNum>N\s?o\s?\d+)\s(?<appDate>\d{4}.\d{2}.\d{2})\s(?<leNoteNumber>.+?)\s(?<leNoteCountry>[A-Z].+)\s(?<leDate>\d{4}.\d{2}.\d{2})");

                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();
                    biblio.Publication.Kind = match.Groups["pubKind"].Value.Trim();

                    biblio.DOfPublication = new()
                    {
                        date_45 = match.Groups["date45"].Value.Replace(".", "/").Trim()
                    };

                    biblio.Application.Date = match.Groups["appDate"].Value.Replace(".", "/").Trim();

                    legalEvent.Date = match.Groups["leDate"].Value.Replace(".", "/").Trim();

                    legalEvent.Note = "|| № Бюллетеня, в котором был опубликован патент | " + match.Groups["noteNum"].Value.Replace("N o", "№").Trim()
                        + "\n" + "|| Право на патент восстановлено за следующие годы действия патента | " + match.Groups["leNoteNumber"].Value.Trim()
                    + "\n" + "|| Код государтсва, в отношении которого восстановлено право  на патент | " + match.Groups["leNoteCountry"].Value.Trim();
                    legalEvent.Language = "RU";

                    noteTranslation.Language = "EN";
                    noteTranslation.Type = "note";
                    noteTranslation.Tr = "|| Eurasian patent publication Bulletin No. | " + match.Groups["noteNum"].Value.Replace("N o", "No.").Trim()
                        + "\n" + "|| Right to patent is restored for the following years of patent validity | " + match.Groups["leNoteNumber"].Value.Trim()
                    + "\n" + "|| Code of country in respect of which right to patent is restored | " + match.Groups["leNoteCountry"].Value.Trim();
                    legalEvent.Translations.Add(noteTranslation);
                }
                else Console.WriteLine($"{note} - 14");
                legal.LegalEvent = legalEvent;
                legal.Biblio = biblio;
            }
            else
            if (subCode == "31")
            {
                Match match = Regex.Match(note.Trim(), @"(?<pubNum>[0-9]+)\s?(?<pubKind>[A-Z][0-9]+)\s?(?<date45>[0-9]{4}.[0-9]{2}.[0-9]{2}).+(?<note1>[N|№]\s?o?\s?\d+)\s?(?<name1>.+?)\((?<code1>[A-Z]{2})\)\s?(?<name2>.+)\((?<code2>[A-Z]{2})\)\s?(?<evDate>[0-9]{4}.[0-9]{2}.[0-9]{2})\s?(?<noteNum>.+)\s(?<noteDate>[0-9]{4}.[0-9]{2}.[0-9]{2})");

                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();
                    biblio.Publication.Kind = match.Groups["pubKind"].Value.Trim();

                    biblio.DOfPublication = new DOfPublication
                    {
                        date_45 = match.Groups["date45"].Value.Trim()
                    };

                    biblio.Assignees.Add(new PartyMember
                    {
                        Name = match.Groups["name1"].Value.Trim(),
                        Country = match.Groups["code1"].Value.Trim()
                    });

                    legal.NewBiblio.Assignees.Add(new PartyMember
                    {
                        Name = match.Groups["name2"].Value.Trim(),
                        Country = match.Groups["code2"].Value.Trim()
                    });

                    legalEvent.Note = "|| № Бюллетеня, в котором был опубликован патент | " + match.Groups["note1"].Value.Replace("N", "№").Replace(" ", "").Replace("o","").Trim() +
                        " || Номер регистрации документа об уступке права | " + match.Groups["noteNum"].Value.Trim() +
                        " || Дата публикации извещения | " + match.Groups["noteDate"].Value.Trim();
                    legalEvent.Language = "RU";

                    noteTranslation.Language = "EN";
                    noteTranslation.Type = "note";
                    noteTranslation.Tr = "|| Eurasian patent publication Bulletin No. | " + match.Groups["note1"].Value.Replace("N", "№").Replace(" ", "").Replace("o", "").Trim() +
                        " || Registration Number of the document of assignment of rights | " + match.Groups["noteNum"].Value.Trim() +
                        " || Publication date of notice | " + match.Groups["noteDate"].Value.Trim();

                    legalEvent.Translations.Add(noteTranslation);

                    legalEvent.Date = match.Groups["evDate"].Value.Trim();
                }
                else
                {
                    Console.WriteLine($"{note}");
                }
                legal.LegalEvent = legalEvent;
                legal.Biblio = biblio;
            }

            return legal;
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
