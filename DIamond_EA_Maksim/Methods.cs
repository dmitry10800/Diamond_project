using Integration;
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

namespace DIamond_EA_Maksim
{
    class Methods
    {

        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
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

                if(subCode == "31")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("PC4A РЕГИСТРАЦИЯ ПЕРЕДАЧИ ПРАВА НА ЕВРАЗИЙСКИЙ ПАТЕНТ ПУТЕМ УСТУПКИ ПРАВА (53)"))
                             .TakeWhile(val => !val.Value.StartsWith("ЕВРАЗИЙСКАЯ ПАТЕНТНАЯ ОРГАНИЗАЦИЯ (ЕАПО)"))
                             .ToList();

                    List<string> notes = Regex.Split(BuildText(xElements).Replace("\r","").Replace("\n"," "), @"(?=[0-9]{6}\s[A-Z])")
                        .Where(val => !string.IsNullOrEmpty(val)).Where( val => val.StartsWith("0")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "PC4A"));
                    }
                }
            }
            return statusEvents;
        }
        internal string BuildText(List<XElement> xElements)
        {
            string fullText = null;

            foreach (XElement text in xElements)
            {
                fullText += text.Value.Trim() + " ";
            }
            return fullText;
        }

        internal Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legal = new()
            {
                CountryCode = "EA",
                SectionCode = sectionCode,
                SubCode = subCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf"))
            };

            Biblio biblio = new()
            {
                Assignees = new()
            };

            LegalEvent legalEvent = new()
            {
                Translations = new()
            };
            NoteTranslation noteTranslation = new();

            if (subCode == "31")
            {
                Match match = Regex.Match(note.Trim(), @"(?<pubNum>[0-9]+)\s?(?<pubKind>[A-Z][0-9]+)\s?(?<date45>[0-9]{4}.[0-9]{2}.[0-9]{2}).+(?<note1>[N|№]\s?\d+)\s?(?<name1>.+?)\((?<code1>[A-Z]{2})\)\s?(?<name2>.+)\((?<code2>[A-Z]{2})\)\s?(?<evDate>[0-9]{4}.[0-9]{2}.[0-9]{2})\s?(?<noteNum>.+)\s(?<noteDate>[0-9]{4}.[0-9]{2}.[0-9]{2})");

                if (match.Success)
                {
                    biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();
                    biblio.Publication.Kind = match.Groups["pubKind"].Value.Trim();

                    biblio.DOfPublication = new DOfPublication
                    {
                        date_45 = match.Groups["date45"].Value.Trim()
                    };

                    biblio.Assignees.Add(new PartyMember
                    {
                        Name = match.Groups["name1"].Value.Trim(),
                        Country = match.Groups["code1"].Value.Trim()
                    });

                    biblio.Assignees.Add(new PartyMember
                    {
                        Name = match.Groups["name2"].Value.Trim(),
                        Country = match.Groups["code2"].Value.Trim()
                    });

                    legalEvent.Note = "|| № Бюллетеня, в котором был опубликован патент | " + match.Groups["note1"].Value.Replace("N", "№").Replace(" ", "").Trim() +
                        " || Номер регистрации документа об уступке права | " + match.Groups["noteNum"].Value.Trim() +
                        " || Дата публикации извещения | " + match.Groups["noteDate"].Value.Trim();
                    legalEvent.Language = "RU";

                    noteTranslation.Language = "EN";
                    noteTranslation.Type = "note";
                    noteTranslation.Tr = "|| Eurasian patent publication Bulletin No. | " + match.Groups["note1"].Value.Replace("N", "№").Replace(" ", "").Trim() +
                        " || Registration Number of the document of assignment of rights | " + match.Groups["noteNum"].Value.Trim() +
                        " || Publication date of notice | " + match.Groups["noteDate"].Value.Trim();

                    legalEvent.Translations.Add(noteTranslation);

                    legalEvent.Date = match.Groups["evDate"].Value.Trim();
                }
                else
                {
                    Console.WriteLine($"{note}");
                }
                legal.LegalEvent = legalEvent;
                legal.Biblio = biblio;
            }

            return legal;
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
