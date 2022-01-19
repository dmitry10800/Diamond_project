using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Integration;
using Newtonsoft.Json;

namespace Diamond_JE_Maksim
{
    internal class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalStatus = new();

            DirectoryInfo directory = new(path);

            List<string> files = directory.GetFiles("*.tetml", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

            XElement tet;

            List<XElement> xElements = new();

            foreach (string tetml in files)
            {
                CurrentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Jersey Registration"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Jersey\s?Registration.+)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Jersey")).ToList();

                    foreach (string note in notes)
                    {
                       legalStatus.Add(MakePatent(note, subCode, "FG"));
                    }
                }
            }

            return legalStatus;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legal = new()
            {
                CountryCode = "JE",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new()
                {
                    DOfPublication = new ()
                },
                LegalEvent = new()
            };

            CultureInfo culture = new("ru-RU");

            if (subCode == "1")
            {
                Match match = Regex.Match(note,
                    @"(?<kind>[A-Z]\s?\d+)\s?(?<UkReg>\d+)\s?(?<firstReg>\d{2}\/\d{2}\/\d{4})\s?(?<field45>\d{2}\/\d{2}\/\d{4})\s?Invention(?<title>.+)\s?Agent(?<field74>.+)\s?Proprietor(?<field73>.+)\s?Remarks(?<note>.+)\s?Updates");

                if (match.Success)
                {
                    legal.Biblio.Publication.Number = match.Groups["kind"].Value.Trim();

                    legal.Biblio.DOfPublication.date_45 = DateTime.Parse(match.Groups["field45"].Value.Trim(), culture)
                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    legal.Biblio.Titles.Add(new Title()
                    {
                        Text = match.Groups["title"].Value.Trim(),
                        Language = "EN"
                    });

                    legal.Biblio.Agents.Add(new PartyMember()
                    {
                        Name = match.Groups["field74"].Value.Trim()
                    });

                    legal.Biblio.Assignees.Add(new PartyMember()
                    {
                        Name = match.Groups["field73"].Value.Trim()
                    });

                    legal.LegalEvent.Note = "|| UK Registration | " + match.Groups["UkReg"].Value.Trim() +
                                            " || First Registration Date | "
                                            + DateTime.Parse(match.Groups["firstReg"].Value.Trim(), culture)
                                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                            + " || Remarks | " + match.Groups["note"].Value.Trim();
                                            ;
                    legal.LegalEvent.Language = "EN";
                }
                else
                {
                    Match match2 = Regex.Match(note,
                        @"(?<kind>[A-Z]\s?\d+)\s?(?<UkReg>\d+)\s?Invention(?<title>.+)\s?Agent(?<field74>.+)\s?Proprietor(?<field73>.+)\s?Remarks(?<note>.+)\s?Updates");

                    if (match2.Success)
                    {
                        legal.Biblio.Publication.Number = match2.Groups["kind"].Value.Trim();

                        legal.Biblio.Titles.Add(new Title()
                        {
                            Text = match2.Groups["title"].Value.Trim(),
                            Language = "EN"
                        });

                        legal.Biblio.Agents.Add(new PartyMember()
                        {
                            Name = match2.Groups["field74"].Value.Trim()
                        });

                        legal.Biblio.Assignees.Add(new PartyMember()
                        {
                            Name = match2.Groups["field73"].Value.Trim()
                        });

                        legal.LegalEvent.Note = "|| UK Registration | " + match2.Groups["UkReg"].Value.Trim()
                                                                        + " || Remarks | " + match2.Groups["note"].Value.Trim();
                        ;
                        legal.LegalEvent.Language = "EN";
                    }
                    else Console.WriteLine(note);
                }
            }

            return legal;
        }

        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

            if (subCode == "1")
            {
                foreach (XElement item in xElements)
                {
                    text += item.Value + " ";
                }
                return text.Replace("\r","").Replace("\n"," ").Trim();
            }
            return text;
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
