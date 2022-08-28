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

namespace Diamond_TR_Maksim
{
    class Methods
    {
        public static string CurrentFileName;

        int id = 1;
        public List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string sub)
        {
            List<Diamond.Core.Models.LegalStatusEvent> convertedPatents = new List<Diamond.Core.Models.LegalStatusEvent>();

            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            List<string> files = new List<string>();

            foreach (FileInfo file in directoryInfo.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            foreach (string tetFile in files)
            {
                CurrentFileName = tetFile;

                tet = XElement.Load(tetFile);

                if (sub == "10")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("YILLIK ÜCRETLERİN ÖDENMEMESİ NEDENİYLE GEÇERSİZ SAYILAN BAŞVURULAR"))
                        .TakeWhile(e => !e.Value.StartsWith("YENİDEN GEÇERLİLİK KAZANAN PATENT/FAYDALI MODELLER"))
                        .ToList();

                    foreach (string note in BuildNotes(xElements))
                    {                        
                        convertedPatents.Add(SplitNote(note, sub, "FD"));
                    }
                }
                else
                if (sub == "13")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("YILLIK ÜCRETLERİN ÖDENMEMESİ NEDENİYLE HAKKI SONA EREN PATENT"))
                        .TakeWhile(e => !e.Value.StartsWith("YENİDEN GEÇERLİLİK KAZANAN PATENT / FAYDALI MODEL BAŞVURULARI"))
                        .ToList();

                    foreach (string note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "MM"));
                    }
                }
                else
                if (sub == "16")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("GEÇERSİZ SAYILAN / REDDEDİLEN VE GERİ ÇEKİLMİŞ SAYILAN BAŞVURULAR")) 
                        .TakeWhile(e => !e.Value.StartsWith("GERİ ÇEKİLEN PATENT / FAYDALI MODEL BAŞVURULARI (6769 SMK)") )
                        .ToList();

                    foreach (string note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "FD"));
                    }
                }
                else
                if (sub == "17")
                {

                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("GERİ ÇEKMİŞ SAYILAN PATENT / FAYDALI MODEL BAŞVURULARI"))
                        .TakeWhile(e => !e.Value.StartsWith("REDDEDİLEN PATENT / FAYDALI MODEL BAŞVURULARI (6769 SMK)"))
                        .TakeWhile(e => !e.Value.StartsWith("MÜLGA 551 SAYILI KHK'NİN 133 ÜNCÜ MADDE HÜKMÜ UYARINCA KORUMA"))
                        .ToList();

                    foreach (string note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "FA"));
                    }
                }
                else
                if (sub == "19")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("MÜLGA 551 SAYILI KHK'NİN 133 ÜNCÜ MADDE HÜKMÜ UYARINCA KORUMA"))
                        .TakeWhile(e => !e.Value.StartsWith("Mülga 551 Sayılı KHK'nin 129 uncu veya 165 inci Maddeleri Hükmü Uyarınca Hükümsüzlüğüne"))
                        .ToList();

                    foreach (string note in Regex.Split(MakeText(xElements), @"(?=\d{4}\/)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("2")).ToList())
                    {
                        convertedPatents.Add(MakePatent(note, sub, "MK"));
                    }
                }
                else
                if (sub == "27")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("SUBCODE 27"))
                        .TakeWhile(e => !e.Value.StartsWith("6769 SAYILI SMK"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(12\)\s[A-Z])").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(12)")).ToList();

                    foreach (string note in notes)
                    {
                        convertedPatents.Add(MakePatent(note, sub, "HH/TH"));
                    }
                }
                else
                if (sub == "30")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("6769 SAYILI SMK'NIN 96 NCI MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU"))
                        .TakeWhile(e => !e.Value.StartsWith("6769 SAYILI SMK'NIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU") && !e.Value.StartsWith("6769 SAYILI SM'KNIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU") &&
                        !e.Value.StartsWith("6769 SAYILI SMK NIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU"))
                        .ToList();

                    foreach (string note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "EZ"));
                    }
                }
                else
                if (sub == "31")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("6769 SAYILI SMK'NIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU") && !e.Value.StartsWith("6769 SAYILI SM'KNIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU") &&
                        !e.Value.StartsWith("6769 SAYILI SMK NIN 143 NCÜ MADDE HÜKMÜ UYARINCA ARAŞTIRMA RAPORU"))
                        .TakeWhile(e => !e.Value.StartsWith("MÜLGA 551 SAYILI KHK'NİN 57 NCİ MADDE HÜKMÜ UYARINCA İNCELEMELİ PATENT"))
                        .ToList();

                    foreach (string note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "EZ"));
                    }
                }
                else
                if (sub == "39")
                {
                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("6769 SAYILI SMK'NIN UYGULANMASINA DAİR YÖNETMENLİĞİN 117 NCİ MADDESİ 7"+"\n"+ "NCİ, VE 8 İNCİ FIKRALARI UYARINCA KULLANMA/KULLANMAMA BEYANI"))
                        .TakeWhile(e => !e.Value.StartsWith("6769 SAYILI SMK&apos;NIN UYGULANMASINA DAİR YÖNETMENLİĞİN 117 NCİ MADDESİ 7" + "\n" + "NCİ, VE 8 İNCİ FIKRALARI UYARINCA KULLANMA/KULLANMAMA BEYANI") && 
                        !e.Value.StartsWith("YILLIK ÜCRETLERİN ÖDENMEMESİ NEDENİYLE GEÇERSİZ SAYILAN BAŞVURULAR"))
                        .ToList();

                    foreach (string note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "EZ"));
                    }
                }
            }
            return convertedPatents;
        }

        public Diamond.Core.Models.LegalStatusEvent SplitNote (string note, string subCode, string sectionCode)  
        {
            string workNote = note.Replace("\r", "").Replace("\n", " ");

            Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf"))
            };

            Match dateMatch = Regex.Match(Path.GetFileName(CurrentFileName), @"\d{8}");

            legalEvent.LegalEvent = new LegalEvent
            {
                Date = dateMatch.Value.Insert(4, "-").Insert(7, "-")
            };

            legalEvent.SubCode = subCode;

            legalEvent.SectionCode = sectionCode;

            legalEvent.CountryCode = "TR";

            legalEvent.Id = id++;

            Biblio biblio = new Biblio();

            Regex optionOne = new Regex(@"(?<number>\d+\/\d+)\s(?<title>\D[a-z|ö|ğ|ü|ç].*?)\s(?<name>\D[A-Z].+?)(?<contTitle>[a-z|ö|ğ|ü|ç].+)");

            Match matchOne = optionOne.Match(workNote);
            if (matchOne.Success)
            {
                biblio.Application.Number = matchOne.Groups["number"].Value.Trim();

                Title title = new Title()
                {
                    Text = matchOne.Groups["title"].Value.Trim() + " " + matchOne.Groups["contTitle"].Value.Trim(),
                    Language = "TR"
                };
                biblio.Titles.Add(title);

                string name = matchOne.Groups["name"].Value.Trim();

                List<string> applicants = Regex.Split(name, @" , ").ToList();

                biblio.Applicants = new List<PartyMember>();

                foreach (string applicant in applicants)
                {
                    biblio.Applicants.Add(new PartyMember
                    {
                        Name = applicant
                    });
                }
            }
            else
            {
                Regex optionTwo = new Regex(@"(?<number>\d+\/\d+)\s(?<title>\D[a-z|ö|ğ|ü|ç].*?)\s(?<name>\D[A-Z].+)");

                Match matchTwo = optionTwo.Match(workNote);

                if (matchTwo.Success)
                {
                    biblio.Application.Number = matchTwo.Groups["number"].Value.Trim();

                    Title title = new Title()
                    {
                        Text = matchTwo.Groups["title"].Value.Trim(),
                        Language = "TR"
                    };
                    biblio.Titles.Add(title);

                    string name = matchTwo.Groups["name"].Value.Trim();

                    List<string> applicants = Regex.Split(name, @" , ").ToList();

                    biblio.Applicants = new List<PartyMember>();

                    foreach (string applicant in applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant
                        });
                    }
                }
                else
                {
                    Regex optionThree = new Regex(@"(?<number>\d+\/\d+)\s(?<name>.*)");

                    Match matchThree = optionThree.Match(workNote);

                    if (matchThree.Success)
                    {
                        biblio.Application.Number = matchThree.Groups["number"].Value.Trim();

                        string name = matchThree.Groups["name"].Value.Trim();

                        List<string> applicants = Regex.Split(name, @" , ").ToList();

                        biblio.Applicants = new List<PartyMember>();

                        foreach (string applicant in applicants)
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

        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new Diamond.Core.Models.LegalStatusEvent()
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                SectionCode = sectionCode,
                CountryCode = "TR",
                Id = id++,
                SubCode = subCode,
                Biblio = new Biblio(),
                LegalEvent = new LegalEvent()
            };

            CultureInfo cultureInfo = new CultureInfo("ru-Ru");

            if(subCode == "19")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<aNum>\d+\/\d+)\s(?<title>.+?)\s(?<applicant>[A-Z].+)\s(?<eDate>\d{2}.\d{2}.\d{4})");

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
            else
            if(subCode == "27")
            {
                EuropeanPatent europeanPatent = new EuropeanPatent();

                foreach (string inid in MakeInids(note))
                {
                    if (inid.StartsWith("(11)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(11)", "").Trim(), @"(?<pNum>.+)\s(?<pKind>[A-Z]\d+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["pNum"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["pKind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} --- 11");

                    }
                    else
                    if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(21) Başvuru Numarası", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(22)"))
                    {
                        statusEvent.Biblio.Application.Date = inid.Replace("\r", "").Replace("\n", " ").Replace("(22) Başvuru Tarihi", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(51)"))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace("(51) Buluşun tasnif sınıfları", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();
                        
                        foreach (string ipc in ipcs)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Ipc
                            {
                                Class = ipc.TrimStart('/').TrimStart('/').Insert(4," ").Trim()
                            });
                        }
                    }
                    else
                    if (inid.StartsWith("(86)") || inid.StartsWith("(96) EP Başvuru No"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(86) EP Başvuru No", "").Replace("(96) EP Başvuru No", "").Replace("//"," ").Trim(),
                            @"(?<num>.+\.\d)\s(?<ipcs>.+?)\s[A-Z]{2}?");
                        if (match.Success)
                        {
                            europeanPatent.AppNumber = match.Groups["num"].Value.Trim();

                            List<string> ipcs = Regex.Split(match.Groups["ipcs"].Value.Trim(), @"\s").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string ipc in ipcs)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Ipc
                                {
                                    Class = ipc.Insert(4, " ").Trim()
                                });
                            }
                        }
                        else 
                        {
                            Match match11 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(86) EP Başvuru No", "").Replace("(96) EP Başvuru No", "").Replace("//", " ").Trim(),
                            @"(?<num>.+\.\d)\s(\s)?(?<ipcs>.+)");
                            if (match11.Success)
                            {
                                europeanPatent.AppNumber = match11.Groups["num"].Value.Trim();

                                List<string> ipcs = Regex.Split(match11.Groups["ipcs"].Value.Trim(), @"\s").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (string ipc in ipcs)
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
                    else
                    if (inid.StartsWith("(96) Başvuru Tarihi"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @".+(?<date>\d{4}\/\d{2}\/\d{2})");
                        if (match.Success)
                        {
                            europeanPatent.AppDate = match.Groups["date"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} --- 96");

                    }
                    else if (inid.StartsWith("(73)"))
                    {
                        Match match = Regex.Match(inid.Replace("(73) Patent Sahibi", "").Trim(), @"(?<name>.+?)\n(?<adress>.+)\s(?<country>.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Assignees.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
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
                    else
                    if (inid.StartsWith("(72)"))
                    {
                        List<string> inventors = Regex.Split(inid.Replace("(72) Buluşu Yapanlar", "").Replace("(72) Buluşu Yapan", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            statusEvent.Biblio.Inventors.Add(new PartyMember
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    else
                    if (inid.StartsWith("(54)"))
                    {
                        statusEvent.Biblio.Titles.Add(new Title
                        {
                            Language = "TR",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(54) Buluş Başlığı", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith("(57)"))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Abstract
                        {
                            Language = "TR",
                            Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(57) Özet", "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith("(97) EP Yayın No"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(97) EP Yayın No", "").Trim(), @"(?<eNum>.+)(?<eKind>[A-Z]\d+)");

                        if (match.Success)
                        {
                            europeanPatent.PubNumber = match.Groups["eNum"].Value.Trim();
                            europeanPatent.PubKind = match.Groups["eKind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} --- 97 number");
                    }
                    else
                    if (inid.StartsWith("(97) EP Yayın Tarihi"))
                    {
                        europeanPatent.PubDate = inid.Replace("\r", "").Replace("\n", " ").Replace("(97) EP Yayın Tarihi", "").Trim();
                    }
                    else
                    if (inid.StartsWith("(43)"))
                    {
                        statusEvent.Biblio.Publication.Date = inid.Replace("\r", "").Replace("\n", " ").Replace("(43) Basvuru Yay?n Tarihi)","").Trim();
                    }
                    else
                    if (inid.StartsWith("(71)"))
                    {
                        List<string> applicants = Regex.Split(inid.Replace("(71) Başvuru Sahibi", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            statusEvent.Biblio.Applicants.Add(new PartyMember
                            {
                                Name = applicant.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(31)"))
                    {
                        List<string> priorities = Regex.Split(inid.Replace("(31)", "").Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priority in priorities)
                        {
                            Match match = Regex.Match(priority.Trim(), @"(?<date>\d{4}\/\d{2}\/\d{2})\s(?<code>[A-Z]{2})\s(?<num>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Priority
                                {
                                    Date = match.Groups["date"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim(),
                                    Number = match.Groups["num"].Value.Replace(" ", "").Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        Match match = Regex.Match(inid.Replace("(74) Vekil", "").Trim(), @"(?<inid74>.+\))\s(?<inid72>.+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember
                            {
                                Name = match.Groups["inid74"].Value.Replace("\r", "").Replace("\n", " ").Trim()
                            });

                            List<string> inventors = Regex.Split(match.Groups["inid72"].Value.Trim(), @"\n").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string inventor in inventors)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember
                                {
                                    Name = inventor.Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(12)"))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(12)", "").Replace("\r","").Replace("\n"," ").Trim();
                    }
                    else Console.WriteLine($"{inid}");
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

                Match match1 = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")), @"\d{8}");
                if (match1.Success)
                {
                    statusEvent.LegalEvent.Date = match1.Value.Insert(4, "/").Insert(7, "/").Trim();
                }

            }

            return statusEvent;
        }

        public string MakeText(List<XElement> xElements)
        {
            string fullText = null;

            foreach (XElement item in xElements)
            {
                fullText += item.Value + "\n";
            }

            return fullText;
        }
        internal List<string> MakeInids (string note)
        {
            List<string> inids = Regex.Split(note.Substring(0, note.IndexOf("(57)")).Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

            inids.Add(note.Substring(note.IndexOf("(57)")));

            return inids;
        }
        internal string MakeCountry(string country) => country switch
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
        public List<string> BuildNotes (List<XElement> xElements )
        {
            string fullText = null;

            foreach (XElement item in xElements)
            {
                fullText += item.Value + "\n";
            }

            Regex splitRegex = new Regex(@"(?=\d{4}\/\d{5})", RegexOptions.Multiline);

            List<string> notes = splitRegex.Split(fullText).ToList();
            notes.RemoveAt(0);

            return notes;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = httpClient.PostAsync("", content).Result;
                string answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
