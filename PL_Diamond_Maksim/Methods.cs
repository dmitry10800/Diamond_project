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

namespace PL_Diamond_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode, string newOrOld)
        {
            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = new();

            DirectoryInfo directoryInfo = new(path);

            List<string> files = new();

            foreach (var file in directoryInfo.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            if (newOrOld == "new")
            {
                foreach (var tetml in files)
                {
                    _currentFileName = tetml;

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

                        var notes = Regex.Split(BuildText(xElements), @"(?=\([0-9]{2})").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(")).ToList();

                        foreach (var note in notes)
                        {
                            convertedPatents.Add(SplitNoteNew(note, subCode, "PD"));
                        }
                    }
                    else if(subCode == "31")
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                           .SkipWhile(val => !val.Value.StartsWith("WYGAŚNIĘCIE PRAWA"))
                           .TakeWhile(val => !val.Value.StartsWith("WPISY I ZMIANY W REJESTRZE NIEUWZGLĘDNIONE"))
                           .ToList();

                        var notes = Regex.Split(BuildText(xElements), @"(?=\([A-Z])").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\(\D\d{1,2}\)").Match(val).Success).ToList();

                        foreach (var note in notes)
                        {
                            convertedPatents.Add(SplitNoteNew(note, subCode, "MZ"));
                        }
                    }
                    else if (subCode == "32")
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("Poniższe zestawienie zawiera kolejno: numer wygasłego patentu" + "\n" + "europejskiego, datę wygaśnięcia oraz zakres wygaśnięcia."))
                            .TakeWhile(val => !val.Value.StartsWith("WPISY I ZMIANY W REJESTRZE NIEUWZGLĘDNIONE"))
                            .ToList();

                        var notes = Regex.Split(BuildText(xElements), @"(?=\([A-Z])").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(")).ToList();

                        foreach (var note in notes)
                        {
                            convertedPatents.Add(SplitNoteNew(note, subCode, "MZ"));
                        }
                    }
                    else if (subCode == "33")
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("Poniższe zestawienie zawiera kolejno: numer wygasłego prawa" + "\n" + "ochronnego, datę wygaśnięcia oraz zakres wygaśnięcia."))
                            .TakeWhile(val => !val.Value.StartsWith("Wpisy i zmiany w rejestrze nieuwzględnione"))
                            .ToList();

                        var notes = Regex.Split(BuildText(xElements), @"(?=\([A-Z]\d\).+)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(")).ToList();

                        foreach (var note in notes)
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

                        var notes = Regex.Split(BuildText(xElements), @"(?=\([A-Z][0-9]\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var note in notes)
                        {
                            convertedPatents.Add(SplitNoteNew(note.Trim(), subCode, "RH"));
                        }

                    }
                    else if (subCode is "51")
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("Poniższe zestawienie zawiera kolejno: numer unieważnionego patentu"))
                            .TakeWhile(val => !val.Value.StartsWith("WYGAŚNIĘCIE PRAWA"))
                            .ToList();

                        var notesList = Regex.Split(BuildText(xElements), @"(?=\(T\d\)\s)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(T")).ToList();

                        foreach (var note in notesList)
                        {
                            convertedPatents.Add(SplitNoteNew(note.Trim(), subCode, "MF"));
                        }
                    }
                }
            }
            else if (newOrOld == "old")
            {
                foreach (var tetml in files)
                {
                    _currentFileName = tetml;

                    tet = XElement.Load(tetml);

                    Diamond.Core.Models.LegalStatusEvent legalEvent = new();

                    if (subCode == "10")
                    {
                        xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                       .SkipWhile(e => !e.Value.StartsWith("PATENTU EUROPEJSKIEGO"))
                       .TakeWhile(e => !e.Value.StartsWith("DECYZJE O ZMIANIE LUB UCHYLENIU DECYZJI") && !e.Value.StartsWith("B. WZORY ŻYTKOWE"))
                       .ToList();

                        var notes = Regex.Split(BuildText(xElements), @"(?=\()").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        if (!notes[0].StartsWith(@"("))
                        {
                            notes.RemoveAt(0);
                        }

                        foreach (var note in notes)
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

                        var notes = Regex.Split(BuildText(xElements), @"(?=\([A-Z]\d)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        if (!notes[0].StartsWith(@"("))
                        {
                            notes.RemoveAt(0);
                        }

                        foreach (var note in notes)
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

                        var notes = Regex.Split(BuildText(xElements), @"(?=\([A-Z]\d)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        if (!notes[0].StartsWith(@"("))
                        {
                            notes.RemoveAt(0);
                        }
                        foreach (var note in notes)
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

                        var notes = Regex.Split(BuildText(xElements), @"(?=T[0-9])").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var note in notes)
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
            var text = note.Replace("\r", "").Replace("\n", " ").Trim();

            Diamond.Core.Models.LegalStatusEvent legalEvent = new()
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),

                SubCode = subCode,

                SectionCode = sectionCode,

                CountryCode = "PL",

                Id = _id++
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
                var match = Regex.Match(text, @"\((?<kind>[A-Z]+\d+)\)\s?(?<number>\d+)\s(?<date>\d{4}\s\d{2}\s\d{2})");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["kind"].Value.Trim();

                    biblio.Publication.Number = match.Groups["number"].Value.Trim();

                    europeanPatent.PubNumber = match.Groups["number"].Value.Trim();

                    biblio.EuropeanPatents.Add(europeanPatent);

                    var date = match.Groups["date"].Value.Trim().Replace(" ", "/");

                    legal.Date = date;
                }
                else Console.WriteLine($"Эта запись {text} не разбилась");

                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }
            else
            if(subCode == "25")
            {
                var match = Regex.Match(text, @"\((?<kind>[A-Z]+\d+)\)\s?(?<number>\d+)\s(?<date>\d{4}\s\d{2}\s\d{2})\s(?<info>.+)");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["kind"].Value.Trim();

                    biblio.Publication.Number = match.Groups["number"].Value.Trim();

                    europeanPatent.PubNumber = match.Groups["number"].Value.Trim();

                    biblio.EuropeanPatents.Add(europeanPatent);

                    var date = match.Groups["date"].Value.Trim().Replace(" ", ".");
                    legal.Note = "|| Datę wpisu | " + date;
                    legal.Language = "PL";

                    noteTranslation.Language = "EN";
                    noteTranslation.Tr = "|| Date of entry | " + date;
                    noteTranslation.Type = "INID";
                    legal.Translations = new List<NoteTranslation> { noteTranslation };

                    var leDate = Path.GetFileName(_currentFileName.Replace(".tetml", ""));
                    var leDateBuild = Regex.Match(leDate, @"(?<f1>[A-Z]{2}_)(?<date>[0-9]+)(?<f2>_.+)");
                    if (leDateBuild.Success)
                    {
                        legal.Date = leDateBuild.Groups["date"].Value.Trim().Insert(4, ".").Insert(7, ".");
                    }

                    biblio.Assignees = new List<PartyMember>();
                    var assignerBuild = Regex.Match(match.Groups["info"].Value.Trim(), @".+?:(?<grant1>.+)\si\s.+:(?<grant2>.+)");
                    if (assignerBuild.Success)
                    {
                        var assigner = new List<string>
                        {
                            assignerBuild.Groups["grant1"].Value.Trim(),
                            assignerBuild.Groups["grant2"].Value.Trim()
                        };

                        foreach (var item in assigner)
                        {
                            var matchAssigner = Regex.Match(item, @"(?<name>.+?),\s?(?<adress>.+),\s?(?<code>.+)");

                            if (matchAssigner.Success) {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = matchAssigner.Groups["name"].Value.Trim(),
                                    Address1 = matchAssigner.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(matchAssigner.Groups["code"].Value.Trim())
                                });

                                var proverka = MakeCountryCode(matchAssigner.Groups["code"].Value.Trim());
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
                        var assigners = Regex.Split(match.Groups["info"].Value.Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var assigner in assigners)
                        {
                            var match1 = Regex.Match(assigner, @"(?<name>.+?),\s?(?<adress>.+),\s?(?<code>.+)\s");
                            if (match1.Success)
                            {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = match1.Groups["name"].Value.Trim(),
                                    Address1 = match1.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(match1.Groups["code"].Value.Trim())
                                });

                                var proverka = MakeCountryCode(match1.Groups["code"].Value.Trim());
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
                var match = Regex.Match(text, @"\((?<kind>[A-Z]\d)\)\s?\(\d+\)\s?(?<number>\d+)\s?(?<date>[0-9]{4}\s[0-9]{2}\s[0-9]{2})\s?(?<note>.+\.)");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                    biblio.Publication.Number = match.Groups["number"].Value.Trim();

                    legal.Date = match.Groups["date"].Value.Replace(" ", ".").Trim();

                    legal.Note = "|| Zakres wygaśnięcia | " + match.Groups["note"].Value.Trim();
                    legal.Language = "PL";

                    noteTranslation.Language = "EN";
                    noteTranslation.Tr = "|| Expiry range | Patent has expired completely.";
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
                var match = Regex.Match(note, @"(?<pubKind>[A-Z][0-9]).+(?<date>[0-9]{2}\s[0-9]{2}\s[0-9]{4})\s(?<note>[0-9]{4}\/[0-9]{2})\s(?<other>.+)");

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

                    var match1 = Regex.Match(match.Groups["other"].Value.Trim(), @"(?<kind>\D+)(?<number>\d+)\s(?<code>[A-Z]{1}[0-9]{1,2})");
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

                var match2 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")).Trim(), @"[0-9]{8}");
                legal.Date = match2.Value.Insert(4, "-").Insert(7, "-").Trim();

                biblio.EuropeanPatents.Add(europeanPatent);
                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }

            return legalEvent;
        }
        internal Diamond.Core.Models.LegalStatusEvent SplitNoteNew(string note, string subCode, string sectionCode)
        {
            var text = note.Replace("\r", "").Replace("\n", " ").Trim();

            Diamond.Core.Models.LegalStatusEvent legalEvent = new()
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                SubCode = subCode,
                SectionCode = sectionCode,
                CountryCode = "PL",
                Id = _id++,
                NewBiblio = new Biblio()
            };

            LegalEvent legal = new ();

            Biblio biblio = new ();

            NoteTranslation noteTranslation = new ();

            biblio.EuropeanPatents = new();

            CultureInfo cultureInfo = new("ru-RU");

            if(subCode == "10")
            {

            }
            else if(subCode == "25")
            {
                var match = Regex.Match(text, @"\)\s(?<number>\d+).+:(?<grantOld>.+);.+:(?<grantNew>.+)", RegexOptions.Singleline);

                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["number"].Value.Trim();

                    var assigneersOld = Regex.Split(match.Groups["grantOld"].Value.Trim(), @";",
                        RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var assigneer in assigneersOld)
                    {
                        var matchOld = Regex.Match(assigneer, @"(?<name>.+),(?<adress>.+),(?<country>.+)", RegexOptions.Singleline);

                        if (matchOld.Success)
                        {
                            var country = MakeCountryCode(matchOld.Groups["country"].Value.Trim());
                            if (country != null)
                            {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = matchOld.Groups["name"].Value.Trim(),
                                    Address1 = matchOld.Groups["adress"].Value.Trim(),
                                    Country = country
                                });
                            }
                            else
                                Console.WriteLine(matchOld.Groups["country"].Value.Trim());
                        }
                        else
                            Console.WriteLine(assigneer + "--- old");
                    }

                    var assigneersNew = Regex.Split(match.Groups["grantNew"].Value.Trim(), @";",
                        RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var assigneer in assigneersNew)
                    {
                        var matchNew = Regex.Match(assigneer, @"(?<name>.+),(?<adress>.+),(?<country>.+)", RegexOptions.Singleline);

                        if (matchNew.Success)
                        {
                            var country = MakeCountryCode(matchNew.Groups["country"].Value.Trim());
                            if (country != null)
                            {
                                legalEvent.NewBiblio.Assignees.Add(new PartyMember
                                {
                                    Name = matchNew.Groups["name"].Value.Trim(),
                                    Address1 = matchNew.Groups["adress"].Value.Trim(),
                                    Country = country
                                });
                            }
                            else
                                Console.WriteLine(matchNew.Groups["country"].Value.Trim());
                        }
                        else
                            Console.WriteLine(assigneer + "--- new");
                    }

                    biblio.EuropeanPatents.Add(new EuropeanPatent()
                    {
                        PubNumber = match.Groups["number"].Value.Trim()
                    });

                    var leDate = Regex.Match(_currentFileName.Replace(".tetml", ""), @"\d{8}");
                    if (leDate.Success)
                    {
                        legal.Date = leDate.Value.Insert(4,"/").Insert(7,"/");
                    }
                }
                else 
                    Console.WriteLine($"{text} --- not split");

                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }
            else if(subCode == "31")
            {
                var match = Regex.Match(note.Trim().TrimEnd('.'), @"\((?<pKind>\D\d{1,2})\)\s.+?\s(?<pNum>.+?)\s(?<leDate>\d{4}\s?\d{2}\s?\d{2})\s(?<leNote>.+)");

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
                var match = Regex.Match(text, @"\((?<kind>[A-Z]{1}\d+)\)(\s\([0-9]{2}\)\s)?(?<number>[0-9]+)\s?(?<date>[0-9]{4}\s[0-9]{2}\s[0-9]{2})\s?(?<note>.+)");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["kind"].Value.Trim();

                    biblio.Publication.Number = match.Groups["number"].Value.Trim();

                    legal.Date = match.Groups["date"].Value.Replace(" ", "/").Trim();

                    legal.Note = "|| Zakres wygaśnięcia | " + match.Groups["note"].Value.Trim();
                    legal.Language = "PL";

                    noteTranslation.Language = "EN";
                    noteTranslation.Tr = "|| Expiry range | Patent has expired completely.";
                    noteTranslation.Type = "INID";
                    legal.Translations = new List<NoteTranslation> { noteTranslation };
                }
                else Console.WriteLine($"This note not split {text}");

                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }
            else if (subCode == "33")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), 
                    @"\((?<pKind>\D\d).+?(?<pNum>\d+)\s(?<evDate>\d{4}\s\d{2}\s\d{2})\s(?<note>.+)");

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["pKind"].Value.Trim();
                    biblio.Publication.Number = match.Groups["pNum"].Value.Trim();

                    legal.Date = match.Groups["evDate"].Value.Replace(" ", "/").Trim();

                    legal.Note = "|| Zakres wygaśnięcia | " + match.Groups["note"].Value.Trim();
                    legal.Language = "PL";

                    noteTranslation.Language = "EN";
                    noteTranslation.Tr = "|| Expiry range | The right has expired completely.";
                    noteTranslation.Type = "INID";
                    legal.Translations = new List<NoteTranslation> { noteTranslation };

                    legalEvent.LegalEvent = legal;
                    legalEvent.Biblio = biblio;
                }
                else Console.WriteLine(note + " -- not matched");
            }
            else if(subCode == "47")
            {
                var match = Regex.Match(note, @"\((?<pubKind>[A-Z][0-9])\).+(?<date>[0-9]{2}\s[0-9]{2}\s[0-9]{4})\s(?<note>[0-9]{4}\/[0-9]{2})\s(?<other>.+)");

                EuropeanPatent europeanPatent = new();

                if (match.Success)
                {
                    biblio.Publication.Kind = match.Groups["pubKind"].Value.Trim();

                    europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Replace(" ", ".").Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "-").Trim();

                    legal.Note = "|| Rok wydania i numer Europejskiego Biuletynu Patentowego, w którym ogłoszono o udzieleniu patentu | " + match.Groups["note"].Value.Trim();
                    legal.Language = "PL";

                    noteTranslation.Language = "EN";
                    noteTranslation.Tr = "|| Year of issue and number of the European Patent Bulletin, in which the patent was announced | " + match.Groups["note"].Value.Trim();
                    noteTranslation.Type = "INID";
                    legal.Translations = new List<NoteTranslation> { noteTranslation };

                    var match1 = Regex.Match(match.Groups["other"].Value.Trim(), @"(?<kind>\D+)(?<number>\d+)\s(?<code>[A-Z]{1}[0-9]{1,2})");
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
                var match2 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")).Trim(), @"[0-9]{8}");
                legal.Date = match2.Value.Insert(4, "-").Insert(7, "-").Trim();

                biblio.EuropeanPatents.Add(europeanPatent);
                legalEvent.LegalEvent = legal;
                legalEvent.Biblio = biblio;
            }
            else if (subCode is "51")
            {
                var match = Regex.Match(note.Replace("\r","").Replace("\n", " ").Trim(), @"\((?<kind>[A-Z]\d+)\).+\)\s(?<num>\d+)\s(?<leNote>\D.+)\.?");

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

                    var match2 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")).Trim(), @"[0-9]{8}");
                    legal.Date = match2.Value.Insert(4, "/").Insert(7, "/").Trim();
                }
                else
                {
                    var matchNew = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
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

            foreach (var text in xElements)
            {
                fullText += text.Value.Trim() + " ";
            }
            return fullText;
        }
        internal string MakeCountryCode(string code)
        {
            var countryCode = code switch
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
                "Polska 000042352" => "PL",
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
                "Nowa Zelandia" => "NZ",
                "CURAÇAO" => "CW",
                _ => null
            };

             return countryCode;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
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
