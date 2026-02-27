using Integration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Xml.Linq;

namespace Diamond_BN_Maksim
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
            var xElements = new List<XElement>();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                var tet = XElement.Load(tetml);
                if (subCode == "2")
                {
                    xElements = tet.Descendants()
                        .Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("PATENT RENEWAL APPLICATION", StringComparison.Ordinal))
                        .TakeWhile(val => !val.Value.StartsWith("PATENT GRANTED UNDER SECTION 30", StringComparison.Ordinal))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=Patent\sNo\.)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Patent")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "ND"));
                    }
                }
            }
            return statusEvents;
        }

        private Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            var statusEvent = new Diamond.Core.Models.LegalStatusEvent()
            {
                CountryCode = "BN",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new Biblio(),
                LegalEvent = new LegalEvent()
            };

            if (subCode == "2")
            {
                var match = Regex.Match(note.Trim(),
                    @"Patent\sNo.:(?<inid11>.+)Date\sof\sGrant:\s?(?<dayNote>\d+)\s(?<monthNote>.+),\s(?<yearNote>\d{4})\sAnnuity:(?<note>.+)Applicant\(s\)\s?/\s?Proprietor\(s\):(?<inid71>.+)Title\sof\sInvention:(?<title>.+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Publication.Number = match.Groups["inid11"].Value.Trim();

                    var month = MakeMonth(match.Groups["monthNote"].Value.Trim());
                    if (month != null)
                    {
                        statusEvent.LegalEvent.Note =
                            $"|| Date of grant | {match.Groups["yearNote"].Value.Trim()}/{month}/{match.Groups["dayNote"].Value.Trim()}" +
                            "\n"
                            + $"|| Term | {match.Groups["note"].Value.Trim()}";
                    }
                    else
                    {
                        Console.WriteLine($"{match.Groups["monthNote"].Value.Trim()}");
                    }

                    statusEvent.Biblio.Applicants.Add(new PartyMember()
                    {
                        Name = $"{match.Groups["inid71"].Value.Trim()}"
                    });
                    statusEvent.Biblio.Assignees.Add(new PartyMember()
                    {
                        Name = $"{match.Groups["inid71"].Value.Trim()}"
                    });

                    statusEvent.Biblio.Titles.Add(new Title()
                    {
                        Language = "EN",
                        Text = $"{match.Groups["title"].Value.Trim()}"
                    });

                    var match1 = Regex.Match(Path.GetFileName(_currentFileName.Replace(".tetml", "").Trim()), @"[0-9]{8}");

                    if (match1.Success)
                    {
                        statusEvent.LegalEvent = new LegalEvent
                        {
                            Date = match1.Value.Insert(4, @"/").Insert(7, @"/").Trim()
                        };
                    }
                }
                else
                {
                    Console.WriteLine($"{note.Trim()}");
                }
            }

            return statusEvent;
        }

        private string MakeText(List<XElement> xElements, string subCode)
        {
            StringBuilder sb = new();
            foreach (var xElement in xElements)
            {
                sb.Append(xElement.Value.Replace("\r", "").Replace("\n", " ") + " ");
            }
            
            return sb.ToString();
        }

        private string? MakeMonth(string month) => month switch
        {
            "May" => "05",
            "October" => "10",
            "November" => "11",
            "April" => "04",
            "December" => "12",
            "June" => "06",
            "July" => "07",
            "September" => "09",
            "March" => "03",
            "February" => "02",
            "August" => "08",
            "January" => "01",
            _ => null
        };
    }
}
