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

namespace PL_Diamond_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode, string newOrOld)
        {
            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = new();

            DirectoryInfo directoryInfo = new(path);

            List<string> files = new();

            foreach (FileInfo file in directoryInfo.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            if (newOrOld == "new")
            {
                foreach (string tetml in files)
                {
                    CurrentFileName = tetml;

                    tet = XElement.Load(tetml);

                    if (subCode == "10")
                    {

                    }
                    else if (subCode == "25")
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("Poniższe zestawienie zawiera kolejno: numer patentu europejskiego,"))
                             .TakeWhile(val => !val.Value.StartsWith("OGŁOSZENIA"))
                             .ToList();

                        List<string> notes = Regex.Split(BuildText(xElements), @"(?=\([0-9]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        if (!notes[0].StartsWith(@"("))
                        {
                            notes.RemoveAt(0);
                        }

                        foreach (string note in notes)
                        {
                            convertedPatents.Add(SplitNoteNew(note, subCode, "BZ/RC"));
                        }
                    }
                    else if(subCode == "31")
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                           .SkipWhile(val => !val.Value.StartsWith("WYGAŚNIĘCIE PRAWA"))
                           .TakeWhile(val => !val.Value.StartsWith("WPISY I ZMIANY W REJESTRZE NIEUWZGLĘDNIONE"))
                           .ToList();

                        List<string> notes = Regex.Split(BuildText(xElements), @"(?=\([A-Z])").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\(\D\d{1,2}\)").Match(val).Success).ToList();

                        foreach (string note in notes)
                        {
                            convertedPatents.Add(SplitNoteNew(note, subCode, "MZ"));
                        }
                    }
                    else if (subCode == "32")
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("europejskiego, datę wygaśnięcia oraz zakres wygaśnięcia."))
                            .TakeWhile(val => !val.Value.StartsWith("Wpisy i zmiany w rejestrze nieuwzględnione") 
                            && !val.Value.StartsWith("w innych samodzielnych ogłoszeniach") 
                            && !val.Value.StartsWith("WPISY I ZMIANY W REJESTRZE NIEUWZGLĘDNIONE"))
                            .ToList();

                        List<string> notes = Regex.Split(BuildText(xElements), @"(?=\([A-Z])").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        if (!notes[0].StartsWith(@"("))
                        {
                            notes.RemoveAt(0);
                        }

                        foreach (string note in notes)
                        {
                            convertedPatents.Add(SplitNoteNew(note, subCode, "MZ"));
                        }
                    }
                    else if (subCode == "47")
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("(T5) (97)") && !val.Value.StartsWith("(T3) (97)"))
                            .TakeWhile(val => !val.Value.StartsWith("Wygaśnięcie prawa"))
                            .ToList();

                        List<string> notes = Regex.Split(BuildText(xElements), @"(?=\([A-Z][0-9]\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string note in notes)
                        {
                            convertedPatents.Add(SplitNoteNew(note.Trim(), subCode, "RH"));
                        }

                    }
                    else if (subCode is "51")
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("UNIEWAŻNIENIE PRAWA"))
                            .TakeWhile(val => !val.Value.StartsWith("WYGAŚNIĘCIE PRAWA"))
                            .ToList();

                        List<string> notesList = Regex.Split(BuildText(xElements), @"(?=\(T\d\)\s)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(T")).ToList();

                        foreach (string note in notesList)
                        {
                            convertedPatents.Add(SplitNoteNew(note.Trim(), subCode, "MF"));
                        }
                    }
                }
            }
            else if (newOrOld == "old")
            {
                foreach (string tetml in files)
                {
                    CurrentFileName = tetml;

                    tet = XElement.Load(tetml);

                    Diamond.Core.Models.LegalStatusEvent legalEvent = new();

                    if (subCode == "10")
                    {
                        xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                       .SkipWhile(e => !e.Value.StartsWith("PATENTU EUROPEJSKIEGO"))
                       .TakeWhile(e => !e.Value.StartsWith("DECYZJE O ZMIANIE LUB UCHYLENIU DECYZJI") && !e.Value.StartsWith("B. WZORY ŻYTKOWE"))
                       .ToList();

                        List<string> notes = Regex.Split(BuildText(xElements), @"(?=\()").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        if (!notes[0].StartsWith(@"("))
                        {
                            notes.RemoveAt(0);
                        }

                        foreach (string note in notes)
                        {
                            convertedPatents.Add(SplitNoteOld(note, subCode, "MK"));
                        }
                    }
                    else if(subCode == "25")
                    {
                        xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                       .SkipWhile(e => !e.Value.StartsWith("WPISY I ZMIANY W WYODRĘBNIONEJ" + "\n" + "CZĘŚCI REJESTRU PATENTOWEGO" +"\n" + "(nieuwzględnione w innych samodzielnych ogłoszeniach)"))
                       .TakeWhile(e => !e.Value.StartsWith("DECYZJE O UNIEWAŻNIENIU PATENTU"))
                       .ToList();

                        List<string> notes = Regex.Split(BuildText(xElements), @"(?=\([A-Z]\d)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        if (!notes[0].StartsWith(@"("))
                        {
                            notes.RemoveAt(0);
                        }

                        foreach (string note in notes)
                        {
                            convertedPatents.Add(SplitNoteOld(note, subCode, "BZ/RC"));
                        }
                    }
                    else if(subCode == "32")
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("Poniższe zestawienie zawiera kolejno: numer wygasłego patentu,") && !val.Value.StartsWith("datę wygaśnięcia oraz zakres wygaśnięcia."))
                            .TakeWhile(val => !val.Value.StartsWith("Poniższe zestawienie zawiera kolejno: numer zgłoszenia albo numer") && !val.Value.StartsWith("patentu, numer i rok wydania Wiadomości Urzędu Patentowego,"))
                            .ToList();

                        List<string> notes = Regex.Split(BuildText(xElements), @"(?=\([A-Z]\d)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        if (!notes[0].StartsWith(@"("))
                        {
                            notes.RemoveAt(0);
                        }
                        foreach (string note in notes)
                        {
                            convertedPatents.Add(SplitNoteOld(note, subCode, "MZ"));
                        }
                    }
                    else if(subCode == "46")
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("T5 (97)") && !val.Value.StartsWith("T3 (97)"))
                            .TakeWhile(val => !val.Value.StartsWith("DECYZJE O ODMOWIE UDZIELENIA PATENTU"))
                            .ToList();

                        List<string> notes = Regex.Split(BuildText(xElements), @"(?=T[0-9])").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string note in notes)
                        {
                            convertedPatents.Add(SplitNoteOld(note.Trim(), subCode, "RH"));
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Wrong style");
            }

            return convertedPatents;
        }
        internal Diamond.Core.Models.LegalStatusEvent SplitNoteOld(string note, string subCode, string sectionCode)
        {
            string text = note.Replace("\r", "").Replace("\n", " ").Trim();

            Diamond.Core.Models.LegalStatusEvent legalEvent = new()
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),

                SubCode = subCode,

                SectionCode = sectionCode,

                CountryCode = "PL",

                Id = Id++
            };

            LegalEvent legal = new();

            Biblio biblio = new()
            {
                EuropeanPatents = new List<EuropeanPatent>(),
                Assignees = new List<PartyMember>()
            };

            EuropeanPatent europeanPatent = new();

            NoteTranslation noteTranslation = new();

            CultureInfo cultureInfo = new("RU-ru");


            if (subCode == "10")
            {
                Match match = Regex.Match(text, @"\((?<kind>[A-Z]+\d+)\)\s?(?<number>\d+)\s(?<date>\d{4}\s\d{2}\s\d{2})");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["kind"].Value.Trim();

                    biblio.Publication.Number = match.Groups["number"].Value.Trim();

                    europeanPatent.PubNumber = match.Groups["number"].Value.Trim();

                    biblio.EuropeanPatents.Add(europeanPatent);

                    string date = match.Groups["date"].Value.Trim().Replace(" ", "/");

                    legal.Date = date;
                }
                else Console.WriteLine($"Эта запись {text} не разбилась");

                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }
            else
            if(subCode == "25")
            {
                Match match = Regex.Match(text, @"\((?<kind>[A-Z]+\d+)\)\s?(?<number>\d+)\s(?<date>\d{4}\s\d{2}\s\d{2})\s(?<info>.+)");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["kind"].Value.Trim();

                    biblio.Publication.Number = match.Groups["number"].Value.Trim();

                    europeanPatent.PubNumber = match.Groups["number"].Value.Trim();

                    biblio.EuropeanPatents.Add(europeanPatent);

                    string date = match.Groups["date"].Value.Trim().Replace(" ", ".");
                    legal.Note = "|| Datę wpisu | " + date;
                    legal.Language = "PL";

                    noteTranslation.Language = "EN";
                    noteTranslation.Tr = "|| Date of entry | " + date;
                    noteTranslation.Type = "INID";
                    legal.Translations = new List<NoteTranslation> { noteTranslation };

                    string leDate = Path.GetFileName(CurrentFileName.Replace(".tetml", ""));
                    Match leDateBuild = Regex.Match(leDate, @"(?<f1>[A-Z]{2}_)(?<date>[0-9]+)(?<f2>_.+)");
                    if (leDateBuild.Success)
                    {
                        legal.Date = leDateBuild.Groups["date"].Value.Trim().Insert(4, ".").Insert(7, ".");
                    }

                    biblio.Assignees = new List<PartyMember>();
                    Match assignerBuild = Regex.Match(match.Groups["info"].Value.Trim(), @".+?:(?<grant1>.+)\si\s.+:(?<grant2>.+)");
                    if (assignerBuild.Success)
                    {
                        List<string> assigner = new List<string>
                        {
                            assignerBuild.Groups["grant1"].Value.Trim(),
                            assignerBuild.Groups["grant2"].Value.Trim()
                        };

                        foreach (string item in assigner)
                        {
                            Match matchAssigner = Regex.Match(item, @"(?<name>.+?),\s?(?<adress>.+),\s?(?<code>.+)");

                            if (matchAssigner.Success) {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = matchAssigner.Groups["name"].Value.Trim(),
                                    Address1 = matchAssigner.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(matchAssigner.Groups["code"].Value.Trim())
                                });

                                string proverka = MakeCountryCode(matchAssigner.Groups["code"].Value.Trim());
                                if ( proverka == null)
                                {
                                    Console.WriteLine($"{matchAssigner.Groups["code"].Value.Trim()} --- {biblio.Publication.Number}");
                                }
                            }
                        }
                    }
                    else
                    if(match.Groups["info"].Value.Trim().Contains(";"))
                    {
                        List<string> assigners = Regex.Split(match.Groups["info"].Value.Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string assigner in assigners)
                        {
                            Match match1 = Regex.Match(assigner, @"(?<name>.+?),\s?(?<adress>.+),\s?(?<code>.+)\s");
                            if (match1.Success)
                            {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = match1.Groups["name"].Value.Trim(),
                                    Address1 = match1.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(match1.Groups["code"].Value.Trim())
                                });

                                string proverka = MakeCountryCode(match1.Groups["code"].Value.Trim());
                                if (proverka == null)
                                {
                                    Console.WriteLine($"{match1.Groups["code"].Value.Trim()} --- {biblio.Publication.Number}");
                                }
                            }
                        }
                    }
                    else Console.WriteLine($"Данные инвенторы не разделились {match.Groups["info"].Value.Trim()} ---- {biblio.Publication.Number}");
                }
                else Console.WriteLine($"Эта запись --- {text} --- не разбилась --- {biblio.Publication.Number}");

                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }
            else
            if(subCode == "32")
            {
                Match match = Regex.Match(text, @"\((?<kind>[A-Z]\d)\)\s?\(\d+\)\s?(?<number>\d+)\s?(?<date>[0-9]{4}\s[0-9]{2}\s[0-9]{2})\s?(?<note>.+\.)");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                    biblio.Publication.Number = match.Groups["number"].Value.Trim();

                    legal.Date = match.Groups["date"].Value.Replace(" ", ".").Trim();

                    legal.Note = "|| Zakres wygaśnięcia | " + match.Groups["note"].Value.Trim();
                    legal.Language = "PL";

                    noteTranslation.Language = "EN";
                    noteTranslation.Tr = "|| Expiry range | Patent has expired in completely.";
                    noteTranslation.Type = "INID";
                    legal.Translations = new List<NoteTranslation> { noteTranslation };
                }
                else Console.WriteLine($"Данная запись не разбилась -- {text}");

                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }
            else
            if(subCode == "46")
            {
                Match match = Regex.Match(note, @"(?<pubKind>[A-Z][0-9]).+(?<date>[0-9]{2}\s[0-9]{2}\s[0-9]{4})\s(?<note>[0-9]{4}\/[0-9]{2})\s(?<other>.+)");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["pubKind"].Value.Trim();

                    europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Replace(" ", ".").Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "-").Trim();

                    legal.Note = "|| Rok wydania i numer Europejskiego Biuletynu Patentowego, w którym ogłoszono o udzieleniu lub zmianie patentu | " + match.Groups["note"].Value.Trim();
                    legal.Language = "PL";

                    noteTranslation.Language = "EN";
                    noteTranslation.Tr = "|| Year of issue and number of the European Patent Bulletin in which grant or amendment of the patent was announced | " + match.Groups["note"].Value.Trim();
                    noteTranslation.Type = "INID";
                    legal.Translations = new List<NoteTranslation> { noteTranslation };

                    Match match1 = Regex.Match(match.Groups["other"].Value.Trim(), @"(?<kind>\D+)(?<number>\d+)\s(?<code>[A-Z]{1}[0-9]{1,2})");
                    if (match1.Success)
                    {
                        biblio.Publication.Number = match1.Groups["number"].Value.Trim();

                        europeanPatent.PubNumber = match1.Groups["kind"].Value.ToUpper() + match1.Groups["number"].Value.Trim();
                        europeanPatent.PubKind = match1.Groups["code"].Value.Trim();
                    }
                }
                else
                {
                    Console.WriteLine($"{note}");
                }

                Match match2 = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")).Trim(), @"[0-9]{8}");
                legal.Date = match2.Value.Insert(4, "-").Insert(7, "-").Trim();

                biblio.EuropeanPatents.Add(europeanPatent);
                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }

            return legalEvent;
        }
        internal Diamond.Core.Models.LegalStatusEvent SplitNoteNew(string note, string subCode, string sectionCode)
        {
            string text = note.Replace("\r", "").Replace("\n", " ").Trim();

            Diamond.Core.Models.LegalStatusEvent legalEvent = new()
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                SubCode = subCode,
                SectionCode = sectionCode,
                CountryCode = "PL",
                Id = Id++
            };

            LegalEvent legal = new ();

            Biblio biblio = new ();

            NoteTranslation noteTranslation = new ();

            biblio.EuropeanPatents = new();

            CultureInfo cultureInfo = new("RU-ru");

            if(subCode == "10")
            {

            }
            else if(subCode == "25")
            {
                Match match = Regex.Match(text, @"\([0-9]{2}\)\s(?<number>\d+)\s?.+?:(?<grant1>.+)\sW.+:\s(?<grant2>.+)");

                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["number"].Value.Trim();

                    biblio.Assignees = new List<PartyMember>();

                    List<string> assigners = new List<string>
                        {
                            match.Groups["grant1"].Value.Trim(),
                            match.Groups["grant2"].Value.Trim()
                        };

                    foreach (string assigner in assigners)
                    {
                        Match match1 = Regex.Match(assigner, @"(?<name>.+?),\s(?<adress>.+),\s(?<country>\D+)");

                        if (match1.Success)
                        {
                            biblio.Assignees.Add(new PartyMember
                            {
                                Name = match1.Groups["name"].Value.Trim(),
                                Address1 = match1.Groups["adress"].Value.Trim(),
                                Country = MakeCountryCode( match1.Groups["country"].Value.Trim())
                            });

                            string proverka = MakeCountryCode(match1.Groups["country"].Value.Trim());

                            if (proverka == null)
                            {
                                Console.WriteLine($"{match1.Groups["country"].Value.Trim()} --- {biblio.Publication.Number}");
                            }
                        }
                    }

                }
                else Console.WriteLine($"Эта запись  не разбилась ---- {text}");

                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }
            else if(subCode == "31")
            {
                Match match = Regex.Match(note.Trim().TrimEnd('.'), @"\((?<pKind>\D\d{1,2})\)\s.+?\s(?<pNum>.+?)\s(?<leDate>\d{4}\s?\d{2}\s?\d{2})\s(?<leNote>.+)");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["pKind"].Value.Trim();
                    biblio.Publication.Number = match.Groups["pNum"].Value.Trim();

                    legal.Date = match.Groups["leDate"].Value.Replace(" ", "/").Trim();
                    legal.Note = "|| Zakres wygaśnięcia | " + match.Groups["leNote"].Value.Trim();
                    legal.Language = "PL";
                    noteTranslation.Tr = "|| Expiry range | Patent has expired completely";
                    noteTranslation.Language = "EN";
                    noteTranslation.Type = "INID";

                    legal.Translations = new(){noteTranslation};
                }
                else Console.WriteLine($"{note}");

                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }
            else if (subCode == "32")
            {
                Match match = Regex.Match(text, @"\((?<kind>[A-Z]{1}\d+)\)(\s\([0-9]{2}\)\s)?(?<number>[0-9]+)\s?(?<date>[0-9]{4}\s[0-9]{2}\s[0-9]{2})\s?(?<note>.+)");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["kind"].Value.Trim();

                    biblio.Publication.Number = match.Groups["number"].Value.Trim();

                    legal.Date = match.Groups["date"].Value.Replace(" ", ".").Trim();

                    legal.Note = "|| Zakres wygaśnięcia | " + match.Groups["note"].Value.Trim();
                    legal.Language = "PL";

                    noteTranslation.Language = "EN";
                    noteTranslation.Tr = "|| Expiry range | Patent has expired in completely.";
                    noteTranslation.Type = "INID";
                    legal.Translations = new List<NoteTranslation> { noteTranslation };
                }
                else Console.WriteLine($"Данная нота не разбилась {text}");

                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }
            else if(subCode == "47")
            {
                Match match = Regex.Match(note, @"\((?<pubKind>[A-Z][0-9])\).+(?<date>[0-9]{2}\s[0-9]{2}\s[0-9]{4})\s(?<note>[0-9]{4}\/[0-9]{2})\s(?<other>.+)");

                EuropeanPatent europeanPatent = new();

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["pubKind"].Value.Trim();

                    europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Replace(" ", ".").Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "-").Trim();

                    legal.Note = "|| Rok wydania i numer Europejskiego Biuletynu Patentowego, w którym ogłoszono o udzieleniu lub zmianie patentu | " + match.Groups["note"].Value.Trim();
                    legal.Language = "PL";

                    noteTranslation.Language = "EN";
                    noteTranslation.Tr = "|| Year of issue and number of the European Patent Bulletin in which grant or amendment of the patent was announced | " + match.Groups["note"].Value.Trim();
                    noteTranslation.Type = "INID";
                    legal.Translations = new List<NoteTranslation> { noteTranslation };

                    Match match1 = Regex.Match(match.Groups["other"].Value.Trim(), @"(?<kind>\D+)(?<number>\d+)\s(?<code>[A-Z]{1}[0-9]{1,2})");
                    if (match1.Success)
                    {
                        biblio.Publication.Number = match1.Groups["number"].Value.Trim();

                        europeanPatent.PubNumber = match1.Groups["kind"].Value.ToUpper() + match1.Groups["number"].Value.Trim();
                        europeanPatent.PubKind = match1.Groups["code"].Value.Trim();
                    }
                }
                else
                {
                    Console.WriteLine($"{note}");
                }
                Match match2 = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")).Trim(), @"[0-9]{8}");
                legal.Date = match2.Value.Insert(4, "-").Insert(7, "-").Trim();

                biblio.EuropeanPatents.Add(europeanPatent);
                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }
            else if (subCode is "51")
            {
                Match match = Regex.Match(note.Replace("\r","").Replace("\n", " ").Trim(), @"\((?<kind>[A-Z]\d+)\).+\)\s(?<num>\d+)\s(?<leNote>\D.+)\.?");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                    biblio.Publication.Number = match.Groups["num"].Value.Trim();

                    legal.Note = "|| Zakres uniewaznienia | " + match.Groups["leNote"].Value.Trim();
                    legal.Language = "PL";

                    noteTranslation.Language = "EN";
                    noteTranslation.Tr = "|| Range of revocation | Annulled entirety by the European Patent Office";
                    noteTranslation.Type = "INID";
                    legal.Translations = new List<NoteTranslation> { noteTranslation };

                    Match match2 = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")).Trim(), @"[0-9]{8}");
                    legal.Date = match2.Value.Insert(4, "/").Insert(7, "/").Trim();
                }
                else
                {
                    Match matchNew = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                        @"\((?<kind>\D\d).+?(?<num>\d+)\s(?<ledate>.+?)\s(?<note>\D+)\.?");

                    if (matchNew.Success)
                    {
                        biblio.Publication.Kind = matchNew.Groups["kind"].Value.Trim();
                        biblio.Publication.Number = matchNew.Groups["num"].Value.Trim();

                        legal.Note = "|| Zakres uniewaznienia | " + matchNew.Groups["note"].Value.Trim();
                        legal.Language = "PL";

                        noteTranslation.Language = "EN";
                        noteTranslation.Tr = "|| Range of revocation | Annulled entirety by the European Patent Office";
                        noteTranslation.Type = "INID";
                        legal.Translations = new List<NoteTranslation> { noteTranslation };

                        legal.Date = matchNew.Groups["ledate"].Value.Replace(" ","/").Trim();
                    }
                    else Console.WriteLine($"{note}");
                }

              

                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }

            return legalEvent;
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
        internal string MakeCountryCode(string code)
        {
            string countryCode = code switch
            {
                "Niemcy" =>"DE",
                "Malta" => "MT",
                "Austria" => "AT",
                "Francja" => "FR",
                "Wielka Brytania" => "GB",
                "Szwecja" => "SE",
                "Stany Zjednoczone Ameryki" => "US",
                "Dania" => "DK",
                "Holandia" => "NL",
                "Izrael" => "IL",
                "Wlochy" => "IT",
                "Włochy" => "IT",
                "Czechy" => "CZ",
                "Hiszpania" => "ES",
                "Finlandia" => "FI",
                "Norwegia" => "NO",
                "Japonia" => "JP",
                "Korea Poludniowa" => "KR",
                "Korea Południowa" => "KR",
                "Meksyk" => "MX",
                "Portugalia" => "PT",
                "Szwajcaria" => "CH",
                "Polska" => "PL",
                "Belgia" => "BE",
                "Irlandia" => "IE",
                "Luksemburg" => "LU",
                "Australia" => "AU",
                "Samoa" => "WS",
                "Chiny" => "CN",
                "Hongkong" => "HK",
                "Slowenia" => "SI",
                "Słowenia" => "SI",
                "Slowacja" => "SK",
                "Słowacja" => "SK",
                "Kanada" => "CA",
                "Malezja" => "MY",
                _ => null
            };

             return countryCode;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
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
