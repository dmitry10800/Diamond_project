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
        private string _currentFileName;
        private int _id = 1;

        private const string I12 = "(12)";
        private const string I21 = "(21)";
        private const string I22 = "(22)";
        private const string I30 = "(30)";
        private const string I71 = "(71)";
        private const string I72 = "(72)";
        private const string I74 = "(74)";
        private const string I54 = "(54)";
        private const string I57 = "(57)";

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

            List<XElement> xElements = new();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("(12)"))
                       .TakeWhile(val => !val.Value.StartsWith("LAPSED"))
                       .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"((?=\(12\)\nM)|(?=\(12\)\sM))").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(12)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatentNewStyle(note, subCode, "FG"));
                    }
                }
                if (subCode == "9")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("18 MONTH PUBLICATION"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(12\).+)").Where(_ => !string.IsNullOrEmpty(_) && _.StartsWith("(12)") && _.Contains("(21) Application No. : PI")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatentNewStyle(note, subCode, "AA"));
                    }
                }
                if (subCode == "10")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("18 MONTH PUBLICATION"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(12\).+)").Where(_ => !string.IsNullOrEmpty(_) && _.StartsWith("(12)") && _.Contains("(21) Application No. : UI")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatentNewStyle(note, subCode, "AA"));
                    }
                }

            }
            return statusEvents;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatentNewStyle(string note, string subCode, string sectionCode)
        {
            var statusEvent = new Diamond.Core.Models.LegalStatusEvent()
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "MY",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                LegalEvent = new(),
                Biblio = new()
                {
                    DOfPublication = new()
                }
            };

            var culture = new CultureInfo("ru-RU");

            if (subCode is "1")
            {
                var match = Regex.Match(note.Trim(),
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
                    var match2 = Regex.Match(note.Trim(),
                        @".+\(47.+:\s(?<inid47>.+)\(30.+\(51.+:(?<inid51>.+)\s\(11\).+?(?<pub>\d+.[A-Z]{1})\s\(56\).+:(?<inid56>.+)\(72.+\n(?<title>.+\n\(54\).+)\(57.+?:(?<abstract>.+)",
                        RegexOptions.Singleline);

                    if (match2.Success)
                    {
                        statusEvent = PatentFor1subs(match2.Groups["inid47"].Value.Trim(),
                            match2.Groups["inid51"].Value.Trim(),
                            match2.Groups["title"].Value.Replace("(54)", "").Replace("Title", "").Replace(":", "").Trim(),
                            match2.Groups["pub"].Value.Trim(),
                            match2.Groups["inid56"].Value.Trim(),
                            match2.Groups["abstract"].Value.Trim());
                    }
                    else Console.WriteLine($"{note}");
                }
            }
            if (subCode is "9" or "10")
            {
                note = CleanText(note, subCode);

                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I12))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace(I12, "").Trim();
                    }
                    if (inid.StartsWith(I21))
                    {
                        statusEvent.Biblio.Application.Number = CleanInid(inid, subCode);
                    }
                    if (inid.StartsWith(I22))
                    {
                        var match = Regex.Match(CleanInid(inid, subCode), @"(?<Day>\d+)(?<Month>.+?)(?<Year>\d+)");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = match.Groups["Year"].Value.Trim() + "/" + MakeMonth(match.Groups["Month"].Value.Trim()) + "/" + match.Groups["Day"].Value.Trim();
                        }
                    }
                    if (inid.StartsWith(I30))
                    {
                        var match = Regex.Match(CleanInid(inid, subCode), @"(?<Day>\d+)(?<Month>.+?)(?<Year>\d+)");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Priorities.Add(new Priority()
                            {
                                Date = match.Groups["Year"].Value.Trim() + "/" + MakeMonth(match.Groups["Month"].Value.Trim()) + "/" + match.Groups["Day"].Value.Trim()
                            }); 
                        }
                    }
                    if (inid.StartsWith(I71))
                    {
                        statusEvent.Biblio.Applicants = MakePartyMembers(CleanInid(inid,subCode), subCode);
                    }
                    if (inid.StartsWith(I72))
                    {
                        statusEvent.Biblio.Inventors = MakePartyMembers(CleanInid(inid, subCode), subCode);
                    }
                    if (inid.StartsWith(I74))
                    {
                        var cleanInid = CleanInid(inid, subCode);
                        if (cleanInid != null)
                        {
                            var match = Regex.Match(cleanInid, @"(?<Name>.+)(?<Adress>C\/O.+)");
                            if (match.Success)
                            {
                                statusEvent.Biblio.Agents.Add(new PartyMember()
                                {
                                    Name = match.Groups["Name"].Value.Trim(),
                                    Address1 = match.Groups["Adress"].Value.Trim()
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Agents.Add(new PartyMember()
                                {
                                    Name = cleanInid.Trim()
                                });
                            }
                        }
                    }
                    if (inid.StartsWith(I54))
                    {
                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Text = CleanInid(inid,subCode),
                            Language = "EN"
                        });
                    }
                    if (inid.StartsWith(I57))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Abstract()
                        {
                            Language = "EN",
                            Text = CleanInid(inid, subCode)
                        });
                    }
                }
            }
            return statusEvent;
        }
        internal Diamond.Core.Models.LegalStatusEvent PatentFor1subs(string inid47, string inid51, string inid54, string inidPub, string inid56, string inidAbstr)
        {
            Diamond.Core.Models.LegalStatusEvent status = new()
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "MY",
                SubCode = "1",
                SectionCode = "FG",
                Id = _id++,
                LegalEvent = new(),
                Biblio = new()
                {
                    DOfPublication = new()
                }
            };

            var matchDate = Regex.Match(inid47.Trim(), @"(?<day47>\d+)\s(?<month47>\D+)\s(?<year47>\d{4})");
            if (matchDate.Success)
            {
                status.Biblio.DOfPublication.date_47 = matchDate.Groups["year47"].Value.Trim() + "/" +
                                                       MakeMonth(matchDate.Groups["month47"].Value.Trim()) + "/" +
                                                       matchDate.Groups["day47"].Value.Trim();

                if (MakeMonth(matchDate.Groups["month47"].Value.Trim()) is null) Console.WriteLine($"{matchDate.Groups["month47"].Value.Trim()}");
            }
            else Console.WriteLine($"<==== PatentFor1subs --- {inid47}");

            status.Biblio.Titles.Add(new Title()
            {
                Language = "EN",
                Text = inid54.Replace("\r", "").Replace("\n", " ").Trim()
            });

            var ipcs = Regex.Split(inid51.Replace("\r", "").Replace("\n", " ").Trim(), @"(?=[A-Z]\d{2}[A-Z].+)")
                .Where(val => !String.IsNullOrEmpty(val)).ToList();

            foreach (var ipc in ipcs)
            {
                status.Biblio.Ipcs.Add(new Ipc()
                {
                    Class = ipc.Trim()
                });
            }

            var matchPub = Regex.Match(inidPub.Replace("\r", "").Replace("\n", " ").Trim(),
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

            var priorArtList = Regex.Split(inid56.Replace("\r", "").Replace("\n", " ").Trim(),
                @"([A-Z]{2}\s.+\s[A-Z]\d)").Where(val => !string.IsNullOrEmpty(val)).ToList();

            foreach (var prioArt in priorArtList)
            {
                var matchPr = Regex.Match(prioArt.Trim(), @"(?<code>[A-Z]{2})\s(?<num>.+)\s(?<kind>[A-Z]\d)");

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
            "January" => "01",
            "February" => "02",
            "March" => "03",
            "April" => "04",
            "May" => "05",
            "June" => "06",
            "July" => "07",
            "August" => "08",
            "September" => "09",
            "October" => "10",
            "November" => "11",
            "December" => "12",
            _ => null
        };
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            var text = string.Empty;
            var sb = new StringBuilder();

            if (subCode == "1")
            {
                foreach (var element in xElements)
                {
                    text += element.Value + "\n";
                }
                return text;
            }

            if (subCode is "9" or "10")
            {
                foreach (var xElement in xElements)
                {
                    sb = sb.Append(xElement.Value);
                }
                return sb.ToString();
            }
            return null;
        }
        private string CleanText(string text, string subCode)
        {
            if (subCode is "9" or "10")
            {
                var match = Regex.Match(text, @"(?<Text>.+)PATENT\s?BA", RegexOptions.Singleline);
                if (match.Success)
                {
                    return match.Groups["Text"].Value.Replace("\r", "").Replace("\n", " ").Trim();
                }
            }
            return text.Replace("\r", "").Replace("\n", " ").Trim();
        }
        private List<string> MakeInids(string text, string subCode)
        {
            var inids = new List<string>();

            if (subCode is "9" or "10")
            {
                var match = Regex.Match(text, @"(?<Inids>.+)(?<Inid57>\(57\).+)");
                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["Inids"].Value.Trim(), @"(?=\(\d{2}\).+)").Where(_ => !string.IsNullOrEmpty(_)).ToList();
                    inids.Add(match.Groups["Inid57"].Value.Trim());
                }
            }

            return inids;
        }
        private string CleanInid(string inid, string subCode)
        {
            var inidClean = string.Empty;
            if (subCode is "9" or "10")
            {
                var match = Regex.Match(inid, @"\(\d{2}\).+:(?<Text>.+)");
                if (match.Success)
                {
                    inidClean = match.Groups["Text"].Value.Trim();
                }
                else
                {
                    var match2 = Regex.Match(inid, @"\(\d{2}\).+:(?<Text>)");
                    if (match2.Success && match2.Groups["Text"].Value == "")
                    {
                        inidClean = null;
                    }
                }
            }
            return inidClean;
        }

        private List<PartyMember> MakePartyMembers(string inid, string subcode)
        {
            var partyMembers = new List<PartyMember>();
            if (inid != null)
            {
                if (subcode is "9" or "10")
                {
                    var partyMembersTmp = Regex.Split(inid, @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var partyMember in partyMembersTmp)
                    {
                        var partyMemberClass = new PartyMember()
                        {
                            Name = partyMember.Trim()
                        };
                        partyMembers.Add(partyMemberClass);
                    }
                }
            }
            return partyMembers;
        } 
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
