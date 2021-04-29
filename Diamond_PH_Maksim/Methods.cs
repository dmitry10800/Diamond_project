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

namespace Diamond_PH_Maksim
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

                if(subCode == "22")
                {
                    //xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                    //      .SkipWhile(val => !val.Value.StartsWith("1.1 FORFEITED INVENTION APPLICATIONS"))
                    //      .TakeWhile(val => !val.Value.StartsWith("INTELLECTUAL PROPERTY PHILIPPINES"))
                    //      .ToList();

                    StreamReader streamReader = new(CurrentFileName.Replace(".tetml", ".txt"));

                    List<string> notes = Regex.Split(streamReader.ReadToEnd().Replace("\r","").Replace("\n"," ").Trim(), @"(?=\d\/\d{4}\/\d{6,7})")
                        .Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith(@"1/")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "FA"));
                    }
                }
            }


            return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                Id = Id++,
                SectionCode = sectionCode,
                SubCode = subCode,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "PH",
                Biblio = new(),
                LegalEvent = new()
            };

            if(subCode == "22")
            {
                Match match = Regex.Match(note.Replace("\r","").Replace("\n", " ").Trim(), @"(?<appNum>\d\/\d{4}\/\d{6,7})\s?(?<day>\d+)\s(?<month>.+)\s(?<year>\d{4})\s?(?<title>.+)\s?\[(?<code>\D{2})");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                    statusEvent.Biblio.Application.Date = match.Groups["year"].Value.Trim() + "-" + MakeMonth( match.Groups["month"].Value.Replace(" ","").ToLower().Trim()) + "-" + match.Groups["day"].Value.Trim();

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "EN",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                    {
                        Country = match.Groups["code"].Value.Trim()
                    });

                    Match match1 = Regex.Match(CurrentFileName.Replace(".tetml", "").Trim(), @"\d{8}");

                    if (match1.Success)
                    {
                        statusEvent.LegalEvent.Date = match1.Value.Insert(4, @"-").Insert(7, @"-").Trim();
                    }

                }
                else Console.WriteLine($"{note} - wrong match");
            }

            return statusEvent;
        }

        internal string MakeMonth(string month) => month switch
        {
            "january" => "01",
            "february" => "02",
            "march" => "03",
            "april" => "04",
            "may" => "05",
            "june" => "06",
            "july" => "07",
            "august" => "08",
            "september" => "09",
            "october" => "10",
            "november" => "11",
            "december" => "12",
            _ => null
        };       
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
