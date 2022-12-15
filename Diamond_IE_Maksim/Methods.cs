using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_IE_Maksim
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

                if(subCode == "52")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Patents Expired"))
                        .TakeWhile(val => !val.Value.StartsWith("Request for Grant of Supplementary Protection Certificate"))
                        .TakeWhile(val => !val.Value.StartsWith("Application for Restoration of Lapsed Patents – Section 37"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=^S?\d{6,})", RegexOptions.Multiline).Where(val => !string.IsNullOrEmpty(val) && new Regex(@"^\d+.*").Match(val).Success)
                        .ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "MK"));
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
                CountryCode = "IE",
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            if(subCode == "52")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<pNum>\d+)\s(?<title>.+?)\.\s(?<assignee>.+)");
                if (match.Success)
                {
                    statusEvent.Biblio.Publication.Number = match.Groups["pNum"].Value.Trim();
                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "EN",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    foreach (string assignee in Regex.Split(match.Groups["assignee"].Value.Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList())
                    {
                        statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                        {
                            Name = assignee
                        });
                    }

                    Match match1 = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")), @"\d{8}");

                    if (match1.Success)
                    {
                        statusEvent.LegalEvent.Date = match1.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }
                }
                else Console.WriteLine($"{note}");
            }

            return statusEvent;
        }
        internal String MakeText(List<XElement> xElements, String subCode)
        {
            string text = null;

            if(subCode == "52")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + "\n";
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
