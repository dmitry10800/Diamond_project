using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Integration;

namespace Diamond_MY_Maksim
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

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("(12)"))
                       .TakeWhile(val => !val.Value.StartsWith("LAPSED"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"((?=\(12\)\nM)|(?=\(12\)\sM))").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(12)")).ToList();

                    foreach (string note in notes)
                    {
                       statusEvents.Add(MakePatentNewStyle(note, subCode, "FG"));
                    }
                }
            }
            return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatentNewStyle(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "MY",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                LegalEvent = new(),
                Biblio = new()
                {
                    DOfPublication = new()
                }
            };

            CultureInfo culture = new("ru-RU");

            if (subCode is "1")
            {
                Match match = Regex.Match(note.Trim(),
                    @".+\(47.+:\s(?<inid47>.+)\(30.+\(51.+:(?<inid51>.+)\s\(54.+:\s(?<title>.+?)\(57.+?\(11\).+?(?<pub>\d+.[A-Z]{1})\s\(56\).+:(?<inid56>.+)\(72.+:.+?\n(?<abstract>.+)",
                    RegexOptions.Singleline);

                if (match.Success)
                {
                    statusEvent = PatentFor1subs(match.Groups["inid47"].Value.Trim(),
                        match.Groups["inid51"].Value.Trim(),
                        match.Groups["title"].Value.Trim(),
                        match.Groups["pub"].Value.Trim(),
                        match.Groups["inid56"].Value.Trim(),
                        match.Groups["abstract"].Value.Trim());
                }
                else
                { 
                    Match match2 = Regex.Match(note.Trim(),
                        @".+\(47.+:\s(?<inid47>.+)\(30.+\(51.+:(?<inid51>.+)\s\(11\).+?(?<pub>\d+.[A-Z]{1})\s\(56\).+:(?<inid56>.+)\(72.+\n(?<title>.+\n\(54\).+)\(57.+?:(?<abstract>.+)",
                        RegexOptions.Singleline);

                    if (match2.Success)
                    {
                        statusEvent = PatentFor1subs(match2.Groups["inid47"].Value.Trim(),
                            match2.Groups["inid51"].Value.Trim(),
                            match2.Groups["title"].Value.Replace("(54)","").Replace("Title","").Replace(":","").Trim(),
                            match2.Groups["pub"].Value.Trim(),
                            match2.Groups["inid56"].Value.Trim(),
                            match2.Groups["abstract"].Value.Trim());
                    }
                    else Console.WriteLine($"{note}");
                }
            }

            return statusEvent;
        }
        internal Diamond.Core.Models.LegalStatusEvent PatentFor1subs(string inid47, string inid51,
            string inid54, string inidPub, string inid56, string inidAbstr)
        {
            Diamond.Core.Models.LegalStatusEvent status = new()
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "MY",
                SubCode = "1",
                SectionCode = "FG",
                Id = Id++,
                LegalEvent = new(),
                Biblio = new()
                {
                    DOfPublication = new()
                }
            };

            Match matchDate = Regex.Match(inid47.Trim(), @"(?<day47>\d+)\s(?<month47>\D+)\s(?<year47>\d{4})");
            if (matchDate.Success)
            {
                status.Biblio.DOfPublication.date_47 = matchDate.Groups["year47"].Value.Trim() + "/" + 
                                                       MakeMonth(matchDate.Groups["month47"].Value.Trim()) + "/" + 
                                                       matchDate.Groups["day47"].Value.Trim();

                if(MakeMonth(matchDate.Groups["month47"].Value.Trim()) is null) Console.WriteLine($"{matchDate.Groups["month47"].Value.Trim()}");
            }
            else Console.WriteLine($"<==== PatentFor1subs --- {inid47}");

            status.Biblio.Titles.Add(new Title()
            {
                Language = "EN",
                Text = inid54.Replace("\r", "").Replace("\n", " ").Trim()
            });

            List<string> ipcs = Regex.Split(inid51.Replace("\r","").Replace("\n"," ").Trim(), @"(?=[A-Z]\d{2}[A-Z].+)")
                .Where(val => !String.IsNullOrEmpty(val)).ToList();

            foreach (string ipc in ipcs)
            {
                status.Biblio.Ipcs.Add(new Ipc()
                {
                    Class = ipc.Trim()
                });
            }

            Match matchPub = Regex.Match(inidPub.Replace("\r", "").Replace("\n", " ").Trim(),
                @"(?<pubNum>\d+).(?<pubKind>[A-Z]{1})");

            if (matchPub.Success)
            {
                status.Biblio.Publication.Number = matchPub.Groups["pubNum"].Value.Trim();
                status.Biblio.Publication.Kind = matchPub.Groups["pubKind"].Value.Trim();
            }
            else Console.WriteLine($"<==== PatentFor1subs --- {inidPub}");

            status.Biblio.Abstracts.Add(new Abstract()
            {
                Language = "EN",
                Text = inidAbstr.Replace("\r", "").Replace("\n", " ").Trim()
            });

            List<string> priorArtList = Regex.Split(inid56.Replace("\r", "").Replace("\n", " ").Trim(),
                @"([A-Z]{2}\s.+\s[A-Z]\d)").Where(val => !string.IsNullOrEmpty(val)).ToList();

            foreach (string prioArt in priorArtList)
            {
                Match matchPr = Regex.Match(prioArt.Trim(), @"(?<code>[A-Z]{2})\s(?<num>.+)\s(?<kind>[A-Z]\d)");

                if (matchPr.Success)
                {
                    status.Biblio.PatentCitations.Add(new PatentCitation()
                    {
                        Kind = matchPr.Groups["kind"].Value.Trim(),
                        Number = matchPr.Groups["num"].Value.Trim(),
                        Authority = matchPr.Groups["code"].Value.Trim()
                    });
                }
            }
            return status;
        }
        internal string? MakeMonth(string month) => month switch
        {
            "January" => 01.ToString(),
            "February" => 02.ToString(),
            "March" => 03.ToString(),
            "April" => 04.ToString(),
            "May" => 05.ToString(),
            "June" => 06.ToString(),
            "July" => 07.ToString(),
            "August" => 08.ToString(),
            "September" => 09.ToString(),
            "October" => 10.ToString(),
            "November" => 11.ToString(),
            "December" => 12.ToString(),
            _ => null
        };
        internal string MakeText(List<XElement> xElement, string subCode)
        {
            string text = "";

            if (subCode == "1")
            {
                foreach (XElement element in xElement)
                {
                    text += element.Value + "\n";
                }
                return text;
            }
            return null;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
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
