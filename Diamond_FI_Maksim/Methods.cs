using Integration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_FI_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            var statusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
            var directory = new DirectoryInfo(path);
            var files = directory.GetFiles("*.tetml", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

            XElement tet;
            var xElements = new List<XElement>();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;
                tet = XElement.Load(tetml);

                if (subCode == "23")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Lapsed patents"))
                        .TakeWhile(val => !val.Value.StartsWith("Expired patents"))
                        .ToList();

                    var text = MakeText(xElements, subCode);

                    var notes = Regex.Split(text, @"(?=Public\snotification.+)", RegexOptions.Singleline)
                                             .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Pub")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(CreateRecord(note
                            .Replace("Application data", "")
                            .Replace("Read more in the Patent Information Service", ""), subCode, "MK"));
                    }
                }

                if (subCode == "24")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Expired patents"))
                        .TakeWhile(val => !val.Value.StartsWith("Reinstated patents"))
                        .ToList();

                    var text = MakeText(xElements, subCode);

                    var notes = Regex.Split(text, @"(?=Public\snotification.+)", RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Pub")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(CreateRecord(note
                            .Replace("Application data", "")
                            .Replace("Read more in the Patent Information Service", ""), subCode, "MK"));
                    }
                }
            }
            return statusEvents;
        }

        private static string MakeText(List<XElement> xElements, string subCode)
        {
            var text = new StringBuilder();

            switch (subCode)
            {
                case "23":
                    {
                        foreach (var xElement in xElements)
                        {
                            text = text.AppendLine(xElement.Value + "\n");
                        }
                        break;
                    }
                case "24":
                {
                    foreach (var xElement in xElements)
                    {
                        text = text.AppendLine(xElement.Value + "\n");
                    }
                    break;
                }
            }
            return text.ToString();
        }

        private Diamond.Core.Models.LegalStatusEvent CreateRecord(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalEvent = new()
            {
                CountryCode = "FI",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new Biblio(),
                LegalEvent = new LegalEvent()
            };

            var euPatent = new EuropeanPatent();

            if (subCode == "23")
            {
                var cleanedText = Regex.Replace(note, @"^\s*$[\r\n]*", string.Empty, RegexOptions.Multiline);

                var pattern =
                    """
                    Public\s+notification\s+date\s*(?<PublicNotificationDate>[^\n]*)\n?(?(PublicNotificationDate)|(?<PublicNotificationDate>[^\n]+))
                    International\s+patent\s+class\s*(?<InternationalPatentClassLine>[^\n]*)\n(?<InternationalPatentClassBlock>(?:(?!^\w.*).*\n)*)
                    Application\s+number\s*(?<ApplicationNumber>[^\n]*)\n?(?(ApplicationNumber)|(?<ApplicationNumber>[^\n]+))
                    Patent\s+number\s*(?<PatentNumber>[^\n]*)\n?(?(PatentNumber)|(?<PatentNumber>[^\n]+))
                    Date\s+of\s+receipt\s*(?<DateOfReceipt>[^\n]*)\n?(?(DateOfReceipt)|(?<DateOfReceipt>[^\n]+))Filing\s+date\s*(?<FilingDate>[^\n]*)\n?(?(FilingDate)|(?<FilingDate>[^\n]+))
                    Applicant\(s\)\s*(?<ApplicantsLine>[^\n]*)\n(?<ApplicantsBlock>(?:(?!^\w.*).*\n)*)
                    Inventor\(s\)\s*(?<InventorsLine>(?:\d+\.\s+[^\n]+)?)\n(?<InventorsBlock>(?:(?!^\w.*).*\n)*)
                    Agent\s*(?<AgentLine>[^\n]*)\n(?<AgentBlock>(?:(?!^Title\s+of\s+invention).*\n)*)
                    Title\s+of\s+invention\n
                    (?<TitleOfInvention>(?:.|\n)+)
                    """;
                var match = Regex.Match(cleanedText, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                if (match.Success)
                {
                    legalEvent.LegalEvent.Date = DateTime
                        .ParseExact(match.Groups["PublicNotificationDate"].Value.Trim(), "dd.MM.yyyy",
                            CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");

                    var ipcs = Regex.Split(match.Groups["InternationalPatentClass"].Value.Trim(), @"\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();

                    foreach (var ipc in ipcs)
                    {
                        legalEvent.Biblio.Ipcs.Add(new Ipc()
                        {
                            Class = ipc.Trim()
                        });
                    }

                    euPatent.AppNumber = match.Groups["ApplicationNumber"].Value.Trim();
                    euPatent.PubNumber = match.Groups["PatentNumber"].Value.Trim();

                    legalEvent.Biblio.Publication.Number = "FI/" + match.Groups["PatentNumber"].Value.Trim();

                    euPatent.AppDate = DateTime
                        .ParseExact(match.Groups["DateOfReceipt"].Value.Trim(), "dd.MM.yyyy",
                            CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");

                    legalEvent.Biblio.Application.Date = DateTime
                        .ParseExact(match.Groups["FilingDate"].Value.Trim(), "dd.MM.yyyy",
                            CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");

                    var applicants = Regex.Split(match.Groups["Applicants"].Value.Trim(), @"\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();

                    foreach (var applicant in applicants)
                    {
                        legalEvent.Biblio.Applicants.Add(new PartyMember()
                        {
                            Name = applicant.Trim()
                        });
                    }

                    var inventors = Regex.Split(match.Groups["Inventors"].Value.Trim(), @"\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();

                    foreach (var inventor in inventors)
                    {
                        var cleanInventor = Regex.Replace(inventor.Trim(), @"^\d+\.\s*", "");
                        legalEvent.Biblio.Inventors.Add(new PartyMember()
                        {
                            Name = cleanInventor.Trim()
                        });
                    }

                    var agents = Regex.Split(match.Groups["Agent"].Value.Trim(), @"\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();

                    foreach (var agent in agents)
                    {
                        legalEvent.Biblio.Agents.Add(new PartyMember()
                        {
                            Name = agent.Trim()
                        });
                    }

                    var titlesMatch = Regex.Match(
                        match.Groups["TitleOfInvention"].Value.Replace("\r", "").Replace("\n", " ").Trim(),
                        @"FI:(?<fi>.+)SV:(?<se>.+)EN:(?<en>.+)");

                    if (titlesMatch.Success)
                    {
                        legalEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "FI",
                            Text = titlesMatch.Groups["fi"].Value.Trim()
                        });
                        legalEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "SV",
                            Text = titlesMatch.Groups["se"].Value.Trim()
                        });
                        legalEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "EN",
                            Text = titlesMatch.Groups["en"].Value.Trim()
                        });
                    }
                    else
                    {
                        var titlesMatch1 = Regex.Match(
                            match.Groups["TitleOfInvention"].Value.Replace("\r", "").Replace("\n", " ").Trim(),
                            @"FI:(?<fi>.+)EN:(?<en>.+)");
                        if (titlesMatch1.Success)
                        {
                            legalEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "FI",
                                Text = titlesMatch1.Groups["fi"].Value.Trim()
                            });
                            legalEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "EN",
                                Text = titlesMatch1.Groups["en"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{match.Groups["TitleOfInvention"].Value} ------------------ titles");
                    }
                }
                else
                {
                    Console.WriteLine(cleanedText);
                }
                legalEvent.Biblio.EuropeanPatents.Add(euPatent);
            }

            if (subCode == "24")
            {
                var cleanedText = Regex.Replace(note, @"^\s*$[\r\n]*", string.Empty, RegexOptions.Multiline);

                var pattern =
                    """
                    Public\s+notification\s+date\s*(?<PublicNotificationDate>[^\n]*)\s*
                    Application\s+number\s*(?<ApplicationNumber>[^\n]*)\s*
                    Patent\s+number\s*(?<PatentNumber>[^\n]*)\s*
                    Applicant\(s\)\s*(?<Applicants>(?:.*\n)+?)Title\s+of\s+invention\s*
                    (?<TitleOfInvention>(?:.|\n)+)
                    """;
                var match = Regex.Match(cleanedText, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                if (match.Success)
                {
                    legalEvent.LegalEvent.Date = DateTime
                        .ParseExact(match.Groups["PublicNotificationDate"].Value.Trim(), "dd.MM.yyyy",
                            CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");

                    euPatent.AppNumber = match.Groups["ApplicationNumber"].Value.Trim();
                    euPatent.PubNumber = match.Groups["PatentNumber"].Value.Trim();

                    legalEvent.Biblio.Publication.Number = "FI/" + match.Groups["PatentNumber"].Value.Trim();

                    var applicants = Regex.Split(match.Groups["Applicants"].Value.Trim(), @"\n")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();

                    foreach (var applicant in applicants)
                    {
                        legalEvent.Biblio.Applicants.Add(new PartyMember()
                        {
                            Name = applicant.Trim()
                        });
                    }

                    var titlesMatch = Regex.Match(
                        match.Groups["TitleOfInvention"].Value.Replace("\r", "").Replace("\n", " ").Trim(),
                        @"FI:(?<fi>.+)SV:(?<se>.+)EN:(?<en>.+)");

                    if (titlesMatch.Success)
                    {
                        legalEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "FI",
                            Text = titlesMatch.Groups["fi"].Value.Trim()
                        });
                        legalEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "SV",
                            Text = titlesMatch.Groups["se"].Value.Trim()
                        });
                        legalEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "EN",
                            Text = titlesMatch.Groups["en"].Value.Trim()
                        });
                    }
                    else
                    {
                        var titlesMatch1 = Regex.Match(
                            match.Groups["TitleOfInvention"].Value.Replace("\r", "").Replace("\n", " ").Trim(),
                            @"FI:(?<fi>.+)EN:(?<en>.+)");
                        if (titlesMatch1.Success)
                        {
                            legalEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "FI",
                                Text = titlesMatch1.Groups["fi"].Value.Trim()
                            });
                            legalEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "EN",
                                Text = titlesMatch1.Groups["en"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{match.Groups["TitleOfInvention"].Value} ------------------ titles");
                    }
                }
                else
                {
                    Console.WriteLine(cleanedText);
                }
                legalEvent.Biblio.EuropeanPatents.Add(euPatent);
            }
            return legalEvent;
        }
    }
}
