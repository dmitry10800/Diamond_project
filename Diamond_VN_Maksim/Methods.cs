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

namespace Diamond_VN_Maksim
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

                if(subCode == "6")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("thay ®æi chñ ®¬n"))
                          //   .TakeWhile(val => !val.Value.StartsWith(""))
                             .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Th.ng\s?b.o\s?s.:\s?)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Thông")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "GB"));
                    }

                }
                else
                if(subCode == "16")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("2 - DUY TRÌ HIỆU LỰC VĂN BẰNG BẢO HỘ"))
                            .TakeWhile(val => !val.Value.StartsWith("b - Duy trì hiệu lực Bằng độc quyền gi¶i ph¸p h÷u Ých"))
                            .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Th.ng\s?b.o\s?s.:\s?)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Thông")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "NZ"));
                    }
                }
                else
                if(subCode == "23")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("Do ng−êi nép ®¬n yªu cÇu"))
                            .TakeWhile(val => !val.Value.StartsWith("Söa ®æi ®¬n"))
                            .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\d+-2\d{3}.+)").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d+-\d{4}.+").Match(val).Success).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "EE"));
                    }
                }
            }

                return statusEvents;

        }



        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "VN",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                NewBiblio = new(),
                LegalEvent = new()
                {
                    Translations = new()
                }
            };

            CultureInfo culture = new("ru-RU");

            if(subCode == "6")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"nộp\sđơn\s(?<application>.+)\sBên\s?chuyển\s?nhượng:(?<applicants>.+)\sBên\s?được\s?chuyển\s?nhượng:(?<applicantsNew>.+)");

                if (match.Success)
                {
                    List<string> application = Regex.Split(match.Groups["application"].Value.Trim(), @"(?<=\d{2}\/\d{2}\/\d{4})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string item in application)
                    {
                        Match matchApp = Regex.Match(item.Trim(), @"(?<appNum>.+)\s(?<appDate>\d{2}\/\d{2}\/\d{4})");

                        if (matchApp.Success)
                        {
                            statusEvent.Biblio.Application.Number = matchApp.Groups["appNum"].Value.Trim();
                            statusEvent.Biblio.Application.Date = DateTime.Parse(matchApp.Groups["appDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{item} --- application");
                    }
                    
                    Match match71 = Regex.Match(match.Groups["applicants"].Value.Trim(), @"(?<name>.+)\s\((?<code>\D{2})\)\s(?<adress>.+),\s");
                    if (match71.Success)
                    {
                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                        {
                            Country = match71.Groups["code"].Value.Trim(),
                            Name = match71.Groups["name"].Value.Trim(),
                            Address1 = match71.Groups["adress"].Value.Trim(),
                        });
                    }
                    else Console.WriteLine($"{match.Groups["applicants"].Value.Trim()} ---- 71");

                    Match match71new = Regex.Match(match.Groups["applicantsNew"].Value.Trim(), @"(?<name>.+)\s\((?<code>\D{2})\)\s(?<adress>.+),\s");
                    if (match71.Success)
                    {
                        statusEvent.NewBiblio.Applicants.Add(new Integration.PartyMember
                        {
                            Country = match71new.Groups["code"].Value.Trim(),
                            Name = match71new.Groups["name"].Value.Trim(),
                            Address1 = match71new.Groups["adress"].Value.Trim(),
                        });
                    }
                    else Console.WriteLine($"{match.Groups["applicantsNew"].Value.Trim()} ---- 71 new");
                }
                else Console.WriteLine($"{note}");
            }
            else
            if(subCode == "16")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"Ng.y\sn.p:\s?(?<dateNote>\d{2}\/\d{2}\/\d{4}).+tr.\s..n\s(?<pubNum>\d+).+(?<noteDate>\d{2}\/\d{2}\/\d{4}).+.ng\sb.o\sh.:\s?(?<field73>.+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();
                    statusEvent.LegalEvent.Note = "|| Hiệu lực Bằng độc quyền sáng chế số " + match.Groups["pubNum"].Value.Trim() + " được duy trì đến " +
                         DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n" +
                         "|| Ngày nộp | " + DateTime.Parse(match.Groups["dateNote"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                    statusEvent.LegalEvent.Language = "VI";

                    statusEvent.LegalEvent.Translations.Add(new Integration.NoteTranslation
                    {
                        Language = "EN",
                        Type = "note",
                        Tr = "|| Expiration date | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n" +
                        "|| Extension filing date | " + DateTime.Parse(match.Groups["dateNote"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim()
                    });

                        Match match1 = Regex.Match(match.Groups["field73"].Value.Trim(), @"(?<name>.+)\s\((?<code>\D{2})\)\s(?<adress>.+)");

                        if (match1.Success)
                        {
                            statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                            {
                                Country = match1.Groups["code"].Value.Trim(),
                                Name = match1.Groups["name"].Value.Trim(),
                                Address1 = match1.Groups["adress"].Value.Trim(),
                            });
                        }
                        else Console.WriteLine($"{match.Groups["field73"].Value.Trim()}");
                    

                }
                else Console.WriteLine($"{note.Replace("\r", "").Replace("\n", " ").Trim()}");
            }
            else
            if(subCode == "23")
            {
                

                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<appNum>.+?)\s(?<pubNum>.+)\s(?<pubDate>\d{2}\/\d{2}\/\d{4})\s(?<evDate>\d{2}\/\d{2}\/\d{4})\s(?<ipcs>.+)\s");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                    statusEvent.Biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();

                    statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                    {
                        Class = match.Groups["ipcs"].Value.Trim()
                    });

                    statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Groups["pubDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["evDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                }
                else Console.WriteLine($"{note}");
            }

            return statusEvent;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

            if (subCode == "6" || subCode == "23" || subCode == "16")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + " ";
                }

                return text.Trim();
            }

            else return text.Trim();
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
