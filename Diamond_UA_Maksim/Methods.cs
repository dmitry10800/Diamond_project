using Diamond.Core.Models;
using Integration;
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
using System.Xml.Linq;

namespace Diamond_UA_Maksim
{
    class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            var legalStatusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directoryInfo = new DirectoryInfo(path);

            var files = new List<string>();

            foreach (var file in directoryInfo.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            var xElements = new List<XElement>();

            foreach (var tetFile in files)
            {
                _currentFileName = tetFile;

                tet = XElement.Load(tetFile);

                if (subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Припинення чинності майнових прав інтелектуальної власності на винахід" + "\n" + "у разі несплати річного збору"))
                        .TakeWhile(val => !val.Value.StartsWith("Визнання прав на винахід недійсними в судовому порядку повністю")
                        && !val.Value.StartsWith("Заява володільця патенту про готовність надання будь-якій особі"))
                        .ToList();

                    foreach (var note in BuildNotes(xElements))
                    {
                        var legalStatusEvent = SplitNote(note, subCode, "MM");

                        if (legalStatusEvent.Biblio.Publication.Number != null) legalStatusEvents.Add(legalStatusEvent);

                    }
                }

                if (subCode == "8")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("Припинення чинності майнових прав інтелектуальної власності на" + "\n" + "корисну модель у разі несплати річного збору"))
                       .TakeWhile(val => !val.Value.StartsWith("Передача виключних майнових прав інтелектуальної власності на" + "\n" + "корисну модель"))
                       .ToList();

                    foreach (var note in BuildNotes(xElements))
                    {
                        var legalStatusEvent = SplitNote(note, subCode, "MK");

                        if (legalStatusEvent.Biblio.Publication.Number != null) legalStatusEvents.Add(legalStatusEvent);
                    }
                }

                if (subCode == "7")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Припинення чинності майнових прав інтелектуальної власності на" + "\n" + "корисну модель у зв'язку із закінч нням строку чинності"))
                        .TakeWhile(val => !val.Value.StartsWith("Передача виключних майнових прав інтелектуальної власності на" + "\n" + "корисну модель"))
                        .ToList();

                    var notes = BuildNotes(xElements);

                    foreach (var note in notes)
                    {
                        var legalStatusEvent = SplitNoteFor7Sub(note, subCode, "MK");

                        if (legalStatusEvent.Biblio.Publication.Number != null) legalStatusEvents.Add(legalStatusEvent);
                    }
                }
            }
            return legalStatusEvents;
        }

        internal string BuildText(List<XElement> xElements)
        {
            string fullText = null;

            foreach (var text in xElements)
            {
                fullText += text.Value.Trim() + "\n";
            }
            return fullText;
        }

        internal List<string> BuildNotes(List<XElement> xElements) => Regex.Split(BuildText(xElements), @"(?<=\d{2}.\d{2}.\d{4})", RegexOptions.Multiline).ToList();

        internal Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string sub, string sectionCode)
        {
            var legalStatus = new Diamond.Core.Models.LegalStatusEvent
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),

                CountryCode = "UA",

                SectionCode = sectionCode,

                SubCode = sub,

                Id = _id++
            };

            var biblioData = new Biblio();

            var cultureInfo = new CultureInfo("ru-RU");

            foreach (var inid in SplitNoteToInid(note))
            {
                if (inid.StartsWith("(1)"))
                {
                    var text = inid.Replace("(1)", "").Trim();

                    biblioData.Publication.Number = text;
                }
                if (inid.StartsWith("(2)"))
                {
                    var text = inid.Replace("(2)", "").Trim();

                    legalStatus.LegalEvent = new LegalEvent
                    {
                        Date = DateTime.Parse(text, cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim()
                    };
                }
            }

            legalStatus.Biblio = biblioData;
            return legalStatus;
        }

        internal Diamond.Core.Models.LegalStatusEvent SplitNoteFor7Sub(string note, string sub, string sectionCode)
        {
            var legalStatusEvent = new Diamond.Core.Models.LegalStatusEvent()
            {
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "UA",
                SectionCode = sectionCode,
                SubCode = sub,
                Id = _id++,
                Biblio = new Biblio(),
                LegalEvent = new LegalEvent()
            };

            var culture = new CultureInfo("ru-RU");
            var noteMatch = Regex.Match(note.Trim(), @"(?<num>\d+)\s(?<date>\d{2}.\d{2}.\d{4})", RegexOptions.Singleline);
            if (noteMatch.Success)
            {
                legalStatusEvent.Biblio.Publication.Number = noteMatch.Groups["num"].Value.Trim();
                legalStatusEvent.LegalEvent.Date = DateTime.Parse(noteMatch.Groups["date"].Value.Trim(), culture)
                    .ToString("yyyy/MM/dd").Replace(".", "/").Trim();
            }

            return legalStatusEvent;
        }

        internal List<string> SplitNoteToInid(string note)
        {
            var text = note.Replace("\r", "").Replace("\n", " ").Trim();

            var match = Regex.Match(text, @"(?<number>[0-9]+)\s(?<date>\d{2}.\d{2}.\d{4})");

            var notes = new List<string>();

            if (match.Success)
            {
                var number = "(1) " + match.Groups["number"].Value.Trim();
                var date = "(2) " + match.Groups["date"].Value.Trim();
                notes.Add(number);
                notes.Add(date);
            }
            else
            {
                notes.Add("-");
            }

            return notes;
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
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
