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
        private string CurrentFileName;
        private int id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalStatusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            List<string> files = new List<string>();

            foreach (FileInfo file in directoryInfo.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = new List<XElement>();

            foreach (string tetFile in files)
            {
                CurrentFileName = tetFile;

                tet = XElement.Load(tetFile);

                if(subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Припинення чинності майнових прав інтелектуальної власності на винахід" + "\n" + "у разі несплати річного збору"))
                        .TakeWhile(val => !val.Value.StartsWith("Визнання прав на винахід недійсними в судовому порядку повністю") 
                        && !val.Value.StartsWith("Заява володільця патенту про готовність надання будь-якій особі"))
                        .ToList();

                    foreach (string note in BuildNotes(xElements))
                    {
                        Diamond.Core.Models.LegalStatusEvent legalStatusEvent = SplitNote(note, subCode, "MM");

                        if(legalStatusEvent.Biblio.Publication.Number !=null) legalStatusEvents.Add(legalStatusEvent);

                    }
                }

                if(subCode == "8")
                {
                     xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Припинення чинності майнових прав інтелектуальної власності на" + "\n" + "корисну модель у разі несплати річного збору"))
                        .TakeWhile(val => !val.Value.StartsWith("Передача виключних майнових прав інтелектуальної власності на" + "\n" + "корисну модель"))
                        .ToList();

                    foreach (string note in BuildNotes(xElements))
                    {
                        Diamond.Core.Models.LegalStatusEvent legalStatusEvent = SplitNote(note, subCode, "MM");

                        if (legalStatusEvent.Biblio.Publication.Number != null) legalStatusEvents.Add(legalStatusEvent);
                    }
                }            
            }
                return legalStatusEvents;
        }

        internal string BuildText(List<XElement> xElements)
        {
            string fullText = null;

            foreach (XElement text in xElements)
            {
                fullText += text.Value.Trim() + "\n";
            }
            return fullText;
        }

        internal List<string> BuildNotes(List<XElement> xElements) => Regex.Split(BuildText(xElements), @"(?<=\d{2}.\d{2}.\d{4})", RegexOptions.Multiline).ToList();

        internal Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string sub, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatus = new Diamond.Core.Models.LegalStatusEvent
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),

                CountryCode = "UA",

                SectionCode = sectionCode,

                SubCode = sub,

                Id = id++
            };

            Biblio biblioData = new Biblio();

            CultureInfo cultureInfo = new CultureInfo("ru-RU");

            foreach (string inid in SplitNoteToInid(note))
            {
                if (inid.StartsWith("(1)"))
                {
                    string text = inid.Replace("(1)", "").Trim();

                    biblioData.Publication.Number = text;
                }
                if (inid.StartsWith("(2)"))
                {
                    string text = inid.Replace("(2)", "").Trim();

                    legalStatus.LegalEvent = new LegalEvent
                    {
                        Date = DateTime.Parse(text, cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim()
                    };
                }
            }

            legalStatus.Biblio = biblioData;
            return legalStatus;
        }

        internal List<string> SplitNoteToInid(string note)
        {
            string text = note.Replace("\r", "").Replace("\n", " ").Trim();

            Match match = Regex.Match(text, @"(?<number>[0-9]+)\s(?<date>\d{2}.\d{2}.\d{4})");

            List<string> notes = new List<string>();

            if (match.Success)
            {
                string number = "(1) " + match.Groups["number"].Value.Trim();
                string date = "(2) " + match.Groups["date"].Value.Trim();
                notes.Add(number);
                notes.Add(date);
            }
            else
            {
                notes.Add("-");              
            }

            return notes;
        }

        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
