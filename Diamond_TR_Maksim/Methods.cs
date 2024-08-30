using Integration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_TR_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        int _id = 1;

        private const string I11 = "(11)";
        private const string I12 = "(12)";
        private const string I43 = "(43)";
        private const string I10 = "(10)";
        private const string I21 = "(21)";
        private const string I22 = "(22)";
        private const string I45 = "(45)";
        private const string I51 = "(51)";
        private const string I30 = "(30)";
        private const string I31 = "(31)";
        private const string I32 = "(32)";
        private const string I33 = "(33)";
        private const string I74 = "(74)";
        private const string I73 = "(73)";
        private const string I72 = "(72)";
        private const string I54 = "(54)";
        private const string I57 = "(57)";
        
        public List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string sub)
        {
            var convertedPatents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directoryInfo = new DirectoryInfo(path);

            var files = new List<string>();

            foreach (var file in directoryInfo.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            foreach (var tetFile in files)
            {
                _currentFileName = tetFile;

                tet = XElement.Load(tetFile);

                if (sub == "10")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("YILLIK ÜCRETLERİN ÖDENMEMESİ NEDENİYLE GEÇERSİZ SAYILAN BAŞVURULAR"))
                        .TakeWhile(e => !e.Value.StartsWith("YENİDEN GEÇERLİLİK KAZANAN PATENT/FAYDALI MODELLER"))
                        .ToList();

                    foreach (var note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "FD"));
                    }
                }
                if (sub == "13")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("YILLIK ÜCRETLERİN ÖDENMEMESİ NEDENİYLE HAKKI SONA EREN PATENT"))
                        .TakeWhile(e => !e.Value.StartsWith("YENİDEN GEÇERLİLİK KAZANAN PATENT / FAYDALI MODEL BAŞVURULARI"))
                        .ToList();

                    foreach (var note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "MM"));
                    }
                }
                if (sub == "16")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("GEÇERSİZ SAYILAN / REDDEDİLEN VE GERİ ÇEKİLMİŞ SAYILAN BAŞVURULAR"))
                        .TakeWhile(e => !e.Value.StartsWith("GERİ ÇEKİLEN PATENT / FAYDALI MODEL BAŞVURULARI (6769 SMK)"))
                        .ToList();

                    foreach (var note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "FD"));
                    }
                }
                if (sub == "17")
                {

                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("GERİ ÇEKMİŞ SAYILAN PATENT / FAYDALI MODEL BAŞVURULARI"))
                        .TakeWhile(e => !e.Value.StartsWith("REDDEDİLEN PATENT / FAYDALI MODEL BAŞVURULARI (6769 SMK)"))
                        .TakeWhile(e => !e.Value.StartsWith("MÜLGA 551 SAYILI KHK'NİN 133 ÜNCÜ MADDE HÜKMÜ UYARINCA KORUMA"))
                        .ToList();

                    foreach (var note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "FA"));
                    }
                }
                if (sub == "19")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("MÜLGA 551 SAYILI KHK'NİN 133 ÜNCÜ MADDE HÜKMÜ UYARINCA KORUMA"))
                        .TakeWhile(e => !e.Value.StartsWith("Mülga 551 Sayılı KHK'nin 129 uncu veya 165 inci Maddeleri Hükmü Uyarınca Hükümsüzlüğüne"))
                        .ToList();

                    foreach (var note in Regex.Split(MakeText(xElements), @"(?=\d{4}\/)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("2")).ToList())
                    {
                        convertedPatents.Add(MakePatent(note, sub, "MK"));
                    }
                }
                if (sub == "27")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("SUBCODE 27"))
                        .TakeWhile(e => !e.Value.StartsWith("End subcode27"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(11\)\s[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (var note in notes)
                    {
                        convertedPatents.Add(MakePatent(note, sub, "HH/TH"));
                    }
                }
                if (sub == "30")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("6769 SAYILI SMK'NIN 96 NCI MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU"))
                        .TakeWhile(e => !e.Value.StartsWith("6769 SAYILI SMK'NIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU") && !e.Value.StartsWith("6769 SAYILI SM'KNIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU") &&
                        !e.Value.StartsWith("6769 SAYILI SMK NIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU"))
                        .ToList();

                    foreach (var note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "EZ"));
                    }
                }
                if (sub == "31")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("6769 SAYILI SMK'NIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU") && !e.Value.StartsWith("6769 SAYILI SM'KNIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU") &&
                        !e.Value.StartsWith("6769 SAYILI SMK NIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU"))
                        .TakeWhile(e => !e.Value.StartsWith("MÜLGA 551 SAYILI KHK'NİN 57 NCİ MADDE HÜKMÜ UYARINCA İNCELEMELİ PATENT"))
                        .ToList();

                    foreach (var note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "EZ"));
                    }
                }
                if (sub == "39")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("6769 SAYILI SMK'NIN UYGULANMASINA DAİR YÖNETMENLİĞİN 117 NCİ MADDESİ 7" + "\n" + "NCİ, VE 8 İNCİ FIKRALARI UYARINCA KULLANMA/KULLANMAMA BEYANI"))
                        .TakeWhile(e => !e.Value.StartsWith("6769 SAYILI SMK&apos;NIN UYGULANMASINA DAİR YÖNETMENLİĞİN 117 NCİ MADDESİ 7" + "\n" + "NCİ, VE 8 İNCİ FIKRALARI UYARINCA KULLANMA/KULLANMAMA BEYANI") &&
                        !e.Value.StartsWith("YILLIK ÜCRETLERİN ÖDENMEMESİ NEDENİYLE GEÇERSİZ SAYILAN BAŞVURULAR"))
                        .ToList();

                    foreach (var note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "EZ"));
                    }
                }

                if (sub == "54")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("Patent ile ilgili dosyalar, yapılan"))
                        .TakeWhile(e => !e.Value.StartsWith("6769 SAYILI SMK"))
                        .ToList();


                    var notes = Regex.Split(MakeText(xElements), @"(?=\(11\)\sTR.+)", RegexOptions.Singleline)
                        .Where(x => !string.IsNullOrEmpty(x) && x.StartsWith("(11)")).ToList();

                    foreach (var note in notes)
                    {
                        convertedPatents.Add(MakePatent(note, sub, "FG"));
                    }
                }
            }
            return convertedPatents;
        }
        public Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string subCode, string sectionCode)
        {
            var workNote = note.Replace("\r", "").Replace("\n", " ");

            var legalEvent = new Diamond.Core.Models.LegalStatusEvent
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf"))
            };

            var dateMatch = Regex.Match(Path.GetFileName(_currentFileName), @"\d{8}");

            legalEvent.LegalEvent = new LegalEvent
            {
                Date = dateMatch.Value.Insert(4, "-").Insert(7, "-")
            };

            legalEvent.SubCode = subCode;

            legalEvent.SectionCode = sectionCode;

            legalEvent.CountryCode = "TR";

            legalEvent.Id = _id++;

            var biblio = new Biblio();

            var optionOne = new Regex(@"(?<number>\d+\/\d+)\s(?<title>\D[a-z|ö|ğ|ü|ç].*?)\s(?<name>\D[A-Z].+?)(?<contTitle>[a-z|ö|ğ|ü|ç].+)");

            var matchOne = optionOne.Match(workNote);
            if (matchOne.Success)
            {
                biblio.Application.Number = matchOne.Groups["number"].Value.Trim();

                var title = new Title()
                {
                    Text = matchOne.Groups["title"].Value.Trim() + " " + matchOne.Groups["contTitle"].Value.Trim(),
                    Language = "TR"
                };
                biblio.Titles.Add(title);

                var name = matchOne.Groups["name"].Value.Trim();

                var applicants = Regex.Split(name, @" , ").ToList();

                biblio.Applicants = new List<PartyMember>();

                foreach (var applicant in applicants)
                {
                    biblio.Applicants.Add(new PartyMember
                    {
                        Name = applicant
                    });
                }
            }
            else
            {
                var optionTwo = new Regex(@"(?<number>\d+\/\d+)\s(?<title>\D[a-z|ö|ğ|ü|ç].*?)\s(?<name>\D[A-Z].+)");

                var matchTwo = optionTwo.Match(workNote);

                if (matchTwo.Success)
                {
                    biblio.Application.Number = matchTwo.Groups["number"].Value.Trim();

                    var title = new Title()
                    {
                        Text = matchTwo.Groups["title"].Value.Trim(),
                        Language = "TR"
                    };
                    biblio.Titles.Add(title);

                    var name = matchTwo.Groups["name"].Value.Trim();

                    var applicants = Regex.Split(name, @" , ").ToList();

                    biblio.Applicants = new List<PartyMember>();

                    foreach (var applicant in applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant
                        });
                    }
                }
                else
                {
                    var optionThree = new Regex(@"(?<number>\d+\/\d+)\s(?<name>.*)");

                    var matchThree = optionThree.Match(workNote);

                    if (matchThree.Success)
                    {
                        biblio.Application.Number = matchThree.Groups["number"].Value.Trim();

                        var name = matchThree.Groups["name"].Value.Trim();

                        var applicants = Regex.Split(name, @" , ").ToList();

                        biblio.Applicants = new List<PartyMember>();

                        foreach (var applicant in applicants)
                        {
                            biblio.Applicants.Add(new PartyMember
                            {
                                Name = applicant
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine(workNote);
                    }
                }
            }
            legalEvent.Biblio = biblio;
            return legalEvent;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            var statusEvent = new Diamond.Core.Models.LegalStatusEvent()
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                SectionCode = sectionCode,
                CountryCode = "TR",
                Id = _id++,
                SubCode = subCode,
                Biblio = new Biblio(),
                LegalEvent = new LegalEvent()
            };

            var cultureInfo = new CultureInfo("ru-Ru");

            if (subCode == "19")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<aNum>\d+\/\d+)\s(?<title>.+?)\s(?<applicant>[A-Z].+)\s(?<eDate>\d{2}.\d{2}.\d{4})");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["aNum"].Value.Trim();
                    statusEvent.Biblio.Titles.Add(new Title
                    {
                        Language = "TR",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    statusEvent.Biblio.Applicants.Add(new PartyMember
                    {
                        Name = match.Groups["applicant"].Value.Trim()
                    });

                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["eDate"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Trim();
                }

                else Console.WriteLine($"{note.Replace("\r", "").Replace("\n", " ").Trim()}");
            }
            if (subCode == "27")
            {
                var europeanPatent = new EuropeanPatent();

                foreach (var inid in MakeInids(note))
                {
                    if (inid.StartsWith("(11)"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(11)", "").Trim(), @"(?<pNum>.+)\s(?<pKind>[A-Z]\d+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["pNum"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["pKind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} --- 11");
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(21) Başvuru Numarası", "").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        statusEvent.Biblio.Application.Date = inid.Replace("\r", "").Replace("\n", " ").Replace("(22) Başvuru Tarihi", "").Trim();
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        var ipcs = Regex.Split(inid.Replace("(51) Buluşun tasnif sınıfları", "").Replace("(51) Buluşun tasnif sınıfı", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            var match = Regex.Match(ipc, @"^\D\d{2}\D\s?\d{1,4}\/\d{1,4}$");

                            if (match.Success)
                            {
                                var tmp = match.Value.Replace(" ", "").Trim();

                                if (tmp.Contains(@"//"))
                                {
                                    tmp = ipc.Replace("//", "").Trim();
                                }

                                statusEvent.Biblio.Ipcs.Add(new Ipc
                                {
                                    Class = tmp.Insert(4, " ").Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(86)") || inid.StartsWith("(96) EP Başvuru No"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(86) EP Başvuru No", "").Replace("(96) EP Başvuru No", "").Replace("//", " ").Trim(),
                            @"(?<num>.+\.\d)\s(?<ipcs>.+?)\s[A-Z]{2}?");
                        if (match.Success)
                        {
                            europeanPatent.AppNumber = match.Groups["num"].Value.Trim();

                            var ipcs = Regex.Split(match.Groups["ipcs"].Value.Trim(), @"\s").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var ipc in ipcs)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc
                                {
                                    Class = ipc.Insert(4, " ").Trim()
                                });
                            }
                        }
                        else
                        {
                            var match11 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(86) EP Başvuru No", "").Replace("(96) EP Başvuru No", "").Replace("//", " ").Trim(),
                            @"(?<num>.+\.\d)\s(\s)?(?<ipcs>.+)");
                            if (match11.Success)
                            {
                                europeanPatent.AppNumber = match11.Groups["num"].Value.Trim();

                                var ipcs = Regex.Split(match11.Groups["ipcs"].Value.Trim(), @"\s").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (var ipc in ipcs)
                                {
                                    statusEvent.Biblio.Ipcs.Add(new Ipc
                                    {
                                        Class = ipc.Insert(4, " ").Trim()
                                    });
                                }
                            }
                            else europeanPatent.AppNumber = inid.Replace("\r", "").Replace("\n", " ").Replace("(86) EP Başvuru No", "").Replace("(96) EP Başvuru No", "").Trim();
                        }
                    }
                    else if (inid.StartsWith("(96) Başvuru Tarihi"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @".+(?<date>\d{4}\/\d{2}\/\d{2})");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = match.Groups["date"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} --- 96");

                    }
                    else if (inid.StartsWith("(73)"))
                    {
                        var match = Regex.Match(inid.Replace("(73) Patent Sahibi", "").Replace("(73) Patent Sahipleri", "").Trim(), @"(?<name>.+?)\n(?<adress>.+)\s(?<country>.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Assignees.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Replace("BİRLEŞİK", "").Trim(),
                                Country = MakeCountry(match.Groups["country"].Value.Trim())
                            });
                        }
                        else
                        {
                            statusEvent.Biblio.Assignees.Add(new PartyMember
                            {
                                Name = inid.Replace("(73) Patent Sahibi", "").Trim()
                            });
                        }

                        if (MakeCountry(match.Groups["country"].Value.Trim()) == null)
                        {
                            Console.WriteLine(match.Groups["country"].Value.Trim());
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        var inventors = Regex.Split(inid.Replace("(72) Buluşu Yapanlar", "").Replace("(72) Buluşu Yapan", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            statusEvent.Biblio.Inventors.Add(new PartyMember
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        statusEvent.Biblio.Titles.Add(new Title
                        {
                            Language = "TR",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(54) Buluş Başlığı", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Abstract
                        {
                            Language = "TR",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(57) Özet", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(97) EP Yayın No"))
                    {
                        var match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(97) EP Yayın No", "").Trim(), @"(?<eNum>.+)(?<eKind>[A-Z]\d+)");

                        if (match.Success)
                        {
                            europeanPatent.PubNumber = match.Groups["eNum"].Value.Trim();
                            europeanPatent.PubKind = match.Groups["eKind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} --- 97 number");
                    }
                    else if (inid.StartsWith("(97) EP Yayın Tarihi"))
                    {
                        europeanPatent.PubDate = inid.Replace("\r", "").Replace("\n", " ").Replace("(97) EP Yayın Tarihi", "").Trim();
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        statusEvent.Biblio.Publication.Date = inid.Replace("\r", "").Replace("\n", " ").Replace("(43) Basvuru Yay?n Tarihi)", "").Trim();
                    }
                    else if (inid.StartsWith("(71)"))
                    {
                        var applicants = Regex.Split(inid.Replace("(71) Başvuru Sahibi", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            statusEvent.Biblio.Applicants.Add(new PartyMember
                            {
                                Name = applicant.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(31)"))
                    {
                        var priorities = Regex.Split(inid.Replace("(31)", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priority in priorities)
                        {
                            var match = Regex.Match(priority.Trim(), @"(?<date>\d{4}\/\d{2}\/\d{2})\s(?<code>[A-Z]{2})\s(?<num>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Priority
                                {
                                    Date = match.Groups["date"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim(),
                                    Number = match.Groups["num"].Value.Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        var match = Regex.Match(inid.Replace("(74) Vekil", "").Trim(), @"(?<inid74>.+\))\s(?<inid72>.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember
                            {
                                Name = match.Groups["inid74"].Value.Replace("\r", "").Replace("\n", " ").Trim()
                            });

                            var inventors = Regex.Split(match.Groups["inid72"].Value.Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var inventor in inventors)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember
                                {
                                    Name = inventor.Trim()
                                });
                            }
                        }
                        else
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember
                            {
                                Name = inid.Replace("(74) Vekil", "").Replace("\r", "").Replace("\n", " ").Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(12)"))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(12)", "").Replace("\r", "").Replace("\n", " ").Trim();
                    }
                    else Console.WriteLine($"{inid} --- not process");
                }

                statusEvent.Biblio.EuropeanPatents.Add(europeanPatent);

                statusEvent.LegalEvent.Note = "|| Düzeltilen Bilgi | null";
                statusEvent.LegalEvent.Language = "TR";
                statusEvent.LegalEvent.Translations = new List<NoteTranslation>() { new NoteTranslation
                {
                    Language = "EN",
                    Type = "note",
                    Tr = "|| Corrected Information | null"
                }};

                var match1 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "")), @"\d{8}");
                if (match1.Success)
                {
                    statusEvent.LegalEvent.Date = match1.Value.Insert(4, "/").Insert(7, "/").Trim();
                }

            }
            if (subCode == "54")
            {
                var dOfPublication = new DOfPublication();

                var matchMain = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<group1>.+)(?<inid57>\(57\).+)", RegexOptions.Singleline);

                if (matchMain.Success)
                {
                    var items = Regex.Split(matchMain.Groups["group1"].Value.Trim(), @"(?=\(\d{2}\).+)",
                            RegexOptions.Singleline)
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();

                    items.Add(matchMain.Groups["inid57"].Value.Trim());

                    foreach (var item in items)
                    {
                        var cleanInid = CleanInid(item);

                        if (item.StartsWith(I11))
                        {
                            var inid = Regex.Match(cleanInid.Replace("\r", "").Replace("\n", " ").Trim(), @"TR\s(?<num>.+)\s(?<kind>\D.?)");
                            if (!inid.Success) continue;
                            statusEvent.Biblio.Publication.Number = inid.Groups["num"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = inid.Groups["kind"].Value.Trim();
                        }

                        if (item.StartsWith(I12))
                        {
                            statusEvent.Biblio.Publication.LanguageDesignation = cleanInid.Replace("\r", "").Replace("\n", " ").Trim();
                        }

                        if (item.StartsWith(I43))
                        {
                            var match = Regex.Match(cleanInid.Replace("\r", "").Replace("\n", " ").Trim(),
                                @"(?<date>\d{4}\/\d{2}\/\d{2})", RegexOptions.Singleline);

                            if (match.Success)
                            {
                                statusEvent.Biblio.Publication.Date = match.Groups["date"].Value.Trim();
                            }
                        }

                        if (item.StartsWith(I21))
                        {
                            statusEvent.Biblio.Application.Number = cleanInid.Replace("\r", "").Replace("\n", " ").Trim();
                        }
                        if (item.StartsWith(I22))
                        {
                            statusEvent.Biblio.Application.Date = cleanInid.Replace("\r", "").Replace("\n", " ").Trim();
                        }

                        if (item.StartsWith(I45))
                        {
                            dOfPublication.date_45 = cleanInid.Replace("\r", "").Replace("\n", " ").Trim();
                        }

                        if (item.StartsWith(I51))
                        {
                            var ipcs = Regex.Split(cleanInid.Trim(), @"(?<=\d+\/\d+\s)", RegexOptions.Singleline)
                                .Where(x => !string.IsNullOrEmpty(x)).ToList();

                            foreach (var ipc in ipcs)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = ipc
                                });
                            }
                        }

                        if (item.StartsWith(I74))
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = cleanInid.Replace("\r", "").Replace("\n", " ").Trim()
                            });
                        }

                        if (item.StartsWith(I72))
                        {
                            var inventors = Regex.Split(cleanInid, @"\n").Where(x => !string.IsNullOrEmpty(x)).ToList();

                            foreach (var inventor in inventors)
                            {
                                statusEvent.Biblio.Agents.Add(new PartyMember()
                                {
                                    Name = inventor.Trim()
                                });
                            }
                        }

                        if (item.StartsWith(I54))
                        {
                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "TR",
                                Text = cleanInid.Replace("\r", "").Replace("\n", " ").Trim()
                            });
                        }

                        if (item.StartsWith(I57))
                        {
                            statusEvent.Biblio.Abstracts.Add(new Abstract()
                            {
                                Language = "TR",
                                Text = cleanInid.Replace("\r", "").Replace("\n", " ").Trim()
                            });
                        }

                        if (item.StartsWith(I73))
                        {
                            //statusEvent.Biblio.Assignees.Add(new PartyMember()
                            //{
                            //    Name = cleanInid.Replace("\r", "").Replace("\n", " ").Trim()
                            //});
                        }

                        statusEvent.Biblio.DOfPublication = dOfPublication;
                    }
                }
            }
            return statusEvent;
        }
        public string MakeText(List<XElement> xElements)
        {
            var sb = new StringBuilder();

            foreach (var item in xElements)
            {
                sb = sb.Append(item.Value + "\n");
            }
            return sb.ToString();
        }
        internal List<string> MakeInids(string note)
        {
            var inids = new List<string>();

            var matchInids = Regex.Match(note.Trim(), @"(?<fPart>.+)(?<sPart>\(57\).+)", RegexOptions.Singleline);

            if (matchInids.Success)
            {
                inids = Regex.Split(matchInids.Groups["fPart"].Value.Trim(), @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();
                inids.Add(matchInids.Groups["sPart"].Value.Replace("\r", "").Replace("\n", " ").Trim());
            }
            return inids;
        }
        internal string? MakeCountry(string country) => country switch
        {
            "İSVİÇRE" => "CH",
            "ÇİN" => "CN",
            "AVUSTURYA" => "AT",
            "CUMHURİYETİ" => "CZ",
            "İTALYA" => "IT",
            "HOLLANDA" => "NL",
            "NORVEÇ" => "NO",
            "İSRAİL" => "IL",
            "KRALLIK" => "GB",
            "ALMANYA" => "DE",
            "KANADA" => "CA",
            "FRANSA" => "FR",
            "İSVEÇ" => "SE",
            "JAPONYA" => "JP",
            "KORE" => "KR",
            "FİNLANDİYA" => "FI",
            "TÜRKİYE" => "TR",
            "BELÇİKA" => "BE",
            "İSPANYA" => "ES",
            "YENİZELANDA" => "NZ",
            "İRLANDA" => "IR",
            "DANİMARKA" => "DK",
            "LÜKSEMBURG" => "LU",
            "ARJANTİN" => "AR",
            "MILANO" => "IT",
            "MEKSİKA" => "MX",
            "BREZİLYA" => "BR",
            "PORTEKİZ" => "PT",
            "POLONYA" => "PL",
            "AVUSTRALYA" => "AU",
            "MACARİSTAN" => "HU",
            "SLOVENYA" => "SI",
            "ŞİLİ" => "CL",
            "A.B.D." => "US",
            _ => null
        };
        public List<string> BuildNotes(List<XElement> xElements)
        {
            var fullText = string.Empty;

            foreach (var item in xElements)
            {
                fullText += item.Value + "\n";
            }

            var splitRegex = new Regex(@"(?=\d{4}\/\d{5})", RegexOptions.Multiline);

            var notes = splitRegex.Split(fullText).ToList();
            notes.RemoveAt(0);

            return notes;
        }

        private static string CleanInid(string inid)
        {
            return inid
                .Replace(I11, "")
                .Replace(I12, "")
                .Replace("(43) Başvuru Yayın Tarihi", "")
                .Replace("(10) Başvuru Yayın No", "")
                .Replace("(21) Başvuru Numarası", "")
                .Replace("(22) Başvuru Tarihi", "")
                .Replace("(45) Patent Belgesinin Veriliş Tarihi", "")
                .Replace("(51) Buluşun tasnif sınıfları", "")
                .Replace("(51) Buluşun tasnif sınıfı", "")
                .Replace("(30) Rüçhan Bilgileri", "")
                .Replace(I31, "")
                .Replace(I32, "")
                .Replace(I33, "")
                .Replace("(74) Vekil", "")
                .Replace("(73) Patent Sahibi", "")
                .Replace("(72) Buluşu Yapan", "")
                .Replace("(54) Buluş Başlığı", "")
                .Replace("(57) Özet", "");
        }
    }
}
