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

namespace Diamond_UY_Maksim
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

                if(subCode == "8")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("RESOLUCIONES DE PATENTES, ABANDONADAS"))
                             .TakeWhile(val => !val.Value.StartsWith("RESOLUCIONES DE MODELOS DE UTILIDAD, ABANDONADAS"))
                             .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=No\.\s)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("N")).ToList();

                     foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FA"));
                    }
                }
            }

                return statusEvents;
        }


        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "UY",
                SectionCode = sectionCode,
                SubCode = subCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            CultureInfo culture = new CultureInfo("ru-RU");
            if(subCode == "8")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"\.\s?(?<appNum>\d+)\s(?<leDate>\d{2}\/\d{2}\/\d{4})\s(?<title>.+)\.\s(?<assigner>.+)\s(?<agent>\d+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["leDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "ES",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                    {
                        Name = match.Groups["assigner"].Value.Trim()
                    });

                    statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                    {
                        Name = match.Groups["agent"].Value.Trim()
                    });

                    statusEvent.LegalEvent.Note = "|| Agent number | " + match.Groups["agent"].Value.Trim();
                    statusEvent.LegalEvent.Language = "EN";
                }
                else
                {
                    Match match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"\.\s?(?<appNum>\d+)\s(?<leDate>\d{2}\/\d{2}\/\d{4})\s(?<title>.+?)\s(?<assigner>[A-Z][a-z].+)\s(?<agent>\d+)");

                    if (match1.Success)
                    {
                        statusEvent.Biblio.Application.Number = match1.Groups["appNum"].Value.Trim();
                        statusEvent.LegalEvent.Date = DateTime.Parse(match1.Groups["leDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "ES",
                            Text = match1.Groups["title"].Value.Trim()
                        });

                        statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                        {
                            Name = match1.Groups["assigner"].Value.Trim()
                        });

                        statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                        {
                            Name = match1.Groups["agent"].Value.Trim()
                        });

                        statusEvent.LegalEvent.Note = "|| Agent number | " + match1.Groups["agent"].Value.Trim();
                        statusEvent.LegalEvent.Language = "EN";
                    }
                    else 
                    {
                        Match match2 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"\.\s?(?<appNum>\d+)\s(?<leDate>\d{2}\/\d{2}\/\d{4})\s(?<title>.+?)\s(?<agent>\d+)");

                        if (match2.Success)
                        {
                            statusEvent.Biblio.Application.Number = match2.Groups["appNum"].Value.Trim();
                            statusEvent.LegalEvent.Date = DateTime.Parse(match2.Groups["leDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "ES",
                                Text = match2.Groups["title"].Value.Trim()
                            });

                            statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                            {
                                Name = match2.Groups["agent"].Value.Trim()
                            });

                            statusEvent.LegalEvent.Note = "|| Agent number | " + match2.Groups["agent"].Value.Trim();
                            statusEvent.LegalEvent.Language = "EN";
                        }
                        else {
                            Match match3 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"\.\s?(?<appNum>\d+)\s(?<leDate>\d{2}\/\d{2}\/\d{4})\s(?<title>.+)");

                            if (match3.Success)
                            {
                                statusEvent.Biblio.Application.Number = match3.Groups["appNum"].Value.Trim();
                                statusEvent.LegalEvent.Date = DateTime.Parse(match3.Groups["leDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                                statusEvent.Biblio.Titles.Add(new Integration.Title
                                {
                                    Language = "ES",
                                    Text = match3.Groups["title"].Value.Trim()
                                });
                            }
                            else Console.WriteLine(note.Replace("\r", "").Replace("\n", " ").Trim());
                        } 
                    }
                    
                } 
            }

            return statusEvent;
        }

        internal string MakeText (List<XElement> xElements, string subCode)
        {
            string text = null;

            if(subCode == "8")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + " ";
                }
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
