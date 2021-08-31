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

namespace Diamond_IN_Maksim
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

                if(subCode == "10")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("WEEKLY ISSUED FER (DELHI)"))
                            .TakeWhile(val => !val.Value.StartsWith("PUBLICATION UNDER SECTION 57 AND UNDER RULE 81(3)"))
                            .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?>(\d{1,3}\sKOLKATA)|(\d{1,3}\sDELHI)|(\d{1,3}\sMUMBAI)|(\d{1,3}\sCHENNAI))")
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d").Match(val).Success).ToList();

                    List<string> works = new();
                    for (int i = 0; i < notes.Count; i+=2)
                    {
                        works.Add(notes[i] + " " + notes[i+1]);
                    }

                    foreach (string note in works)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "EC"));
                    }

                }
            }

                return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "IN",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            CultureInfo culture = new("ru-RU");

            if(subCode == "10")
            {
                Match match = Regex.Match(note.Trim(), @"\d{1,3}?\s(?<city>\D.+?)\s\s?(?<aNum>\d.+)\s(?<eDate>\d{2}\/\d{2}\/\d{4}).+?(?<f74>[A-Z].+)\s(?<note>.+@.+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                    statusEvent.LegalEvent.Note = "|| LOCATION | " + match.Groups["city"].Value.Trim() + " || EMAIL | " + match.Groups["note"].Value.Trim();

                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["eDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                    {
                        Address1 = match.Groups["f74"].Value.Trim()
                    });
                }
               else Console.WriteLine($"{note}");
            }

            return statusEvent;
        }


        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

            if (subCode == "10")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + " ";
                }
            }

            return text.Replace("\r","").Replace("\n"," ").Trim();
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
