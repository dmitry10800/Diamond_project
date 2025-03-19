using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Integration;

namespace Diamond_ZM_Maksim;

public class Methods
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

            if (subCode == "5")
            {
                xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("DATE PAID"))
                       .TakeWhile(val => !val.Value.StartsWith("STATUTORY NOTICES UNDER THE TRADE MARKS ACT (CAP. 401)"))
                       .ToList();

                var notes = Regex.Split(MakeText(xElements, subCode), @"(?=AP\/.+)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("AP")).ToList();

                foreach (var note in notes)
                {
                    statusEvents.Add(MakeNotes(note, subCode, "ND"));
                }
            }
        }
        return statusEvents;
    }

    private Diamond.Core.Models.LegalStatusEvent MakeNotes(string note, string subCode, string sectionCode)
    {
        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
        {
            CountryCode = "ZM",
            SubCode = subCode,
            SectionCode = sectionCode,
            Id = _id++,
            GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
            Biblio = new Biblio(),
            LegalEvent = new LegalEvent()
        };

        if (subCode == "5")
        {
            var noteMatch = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Trim(),
                @"(?<appNum>.+)\s(?<evDate>\d{2}\.\d{2}\.\d{4})\s(?<validDate>\d{2}\.\d{2}\.\d{4})\s(?<note>\d+)");

            if (noteMatch.Success)
            {
                statusEvent.Biblio.Application.Number = noteMatch.Groups["appNum"].Value.Replace("|","").Trim();

                statusEvent.LegalEvent.Date = DateTime.Parse(noteMatch.Groups["evDate"].Value.Trim())
                    .ToString("yyyy.MM.dd").Replace(".", "/");

                statusEvent.LegalEvent.Language = "EN";
                statusEvent.LegalEvent.Note = "|| VALID UNTIL DATE | " + DateTime
                                                  .Parse(noteMatch.Groups["validDate"].Value.Trim())
                                                  .ToString("yyyy.MM.dd").Replace(".", "/") + "\n"
                                              + "|| ANNIVERSARY | " + noteMatch.Groups["note"].Value.Trim();
            }
        }
        return statusEvent;
    }

    private static string MakeText(List<XElement> xElements, string subCode)
    {
        var text = new StringBuilder();

        switch (subCode)
        {
            case "5":
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
}