using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_PK_Maksim
{
    internal class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        public List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            foreach (var tetml in files)
            {
                CurrentFileName = tetml;

                tet = XElement.Load(tetml);

                if(subCode == "13")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("Expiry List of 2021"))
                       .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\d{6}\s)").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"(^\d)").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "MK"));
                    }  
                }
            }
            return statusEvents;
        }

        internal string MakeText(List<XElement> xElement, string subCode)
        {
            var text = "";

            if(subCode == "13")
            {
                foreach (var element in xElement)
                {
                    text += element.Value + "\n";
                }

                return text;
            }

            return null;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "PK",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                LegalEvent = new(),
                Biblio = new()
            };

            CultureInfo culture = new("ru-RU");

            if(subCode == "13")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<num>\d{6})\s(?<appDate>\d{2}.\d{2}.\d{4})\s(?<prDate>\d{2}.\d{2}.\d{4})\s(?<code>\D\s?\D)\s(?<evDate>\d{2}.\d{2}.\d{4})");

                if (match.Success)
                {
                    statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                    statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["appDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    var country = match.Groups["code"].Value.Replace(" ", "").Trim();
                    if(country == "UK")
                    {
                        country = "GB";
                    }

                    statusEvent.Biblio.Priorities.Add(new Integration.Priority
                    {
                        Country = country,
                        Date = DateTime.Parse(match.Groups["prDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                    });
                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["evDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                }
                else 
                {
                    var match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                        @"(?<num>\d{6})\s(?<appDate>\d{2}.\d{2}.\d{4})\s(?<evDate>\d{2}.\d{2}.\d{4})");

                    if (match1.Success)
                    {
                        statusEvent.Biblio.Publication.Number = match1.Groups["num"].Value.Trim();
                        statusEvent.Biblio.Application.Date = DateTime.Parse(match1.Groups["appDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        statusEvent.LegalEvent.Date = DateTime.Parse(match1.Groups["evDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else Console.WriteLine($"{note} --- not process");
                } 
            }


            return statusEvent;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
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
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
