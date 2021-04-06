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

                if (sub == "17")
                {

                    xElements = tet.Descendants().Where(value => value.Name.LocalName == "Text")
                        .SkipWhile(e => !e.Value.StartsWith("GERİ ÇEKMİŞ SAYILAN PATENT / FAYDALI MODEL BAŞVURULARI"))
                        .TakeWhile(e => !e.Value.StartsWith("MÜLGA 551 SAYILI KHK'NİN 133 ÜNCÜ MADDE HÜKMÜ UYARINCA KORUMA"))
                        .ToList();

                    foreach (string note in BuildNotes(xElements))
                    {
                        convertedPatents.Add(SplitNote(note, sub, "FA"));
                    }
                }

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

            Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();

            legalEvent.GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf"));

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
        public void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
