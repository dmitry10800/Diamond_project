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

namespace BD_Diamond_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
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

                if(subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("New Application for Patents Filed"))
                        .TakeWhile(val => !val.Value.StartsWith("Deputy Registrar."))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\s[0-9]{1,4}\/\s)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith(" ")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AZ"));
                    }
                }
            }
            return statusEvents;
        }
        internal string MakeText(List<XElement> xElements)
        {
            string text = null;

            foreach (var xElement in xElements)
            {
                text += xElement.Value.Trim() + " ";
            }
            return text;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legal = new()
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                SubCode = subCode,
                SectionCode = sectionCode,
                CountryCode = "BD",
                Id = id++
            };

            Biblio biblio = new();
            CultureInfo culture = new("Ru-ru");

            biblio.Applicants = new();
            biblio.Titles = new();

            var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<appNum>[0-9]{1,5}\/\s?[0-9]{4})\s(?<appDate>[0-9]{2}.[0-9]{2}.[0-9]{4})\s(?<name>.+,\s?[A-Z]{2})\s(?<title>.+)");

            if (match.Success)
            {
                biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                biblio.Application.Date = DateTime.Parse(match.Groups["appDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".","/").Trim();

                var applicants = Regex.Match(match.Groups["name"].Value.Trim(), @"(?<name>.+)(?<code>[A-Z]{2}$)");
                if (applicants.Success)
                {
                    biblio.Applicants.Add(new PartyMember
                    {
                        Name = applicants.Groups["name"].Value.Trim().TrimEnd(','),
                        Country = applicants.Groups["code"].Value.Trim()
                    });
                }
                else Console.WriteLine($"{match.Groups["name"].Value.Trim()} не разбился");

                biblio.Titles.Add(new Title
                {
                    Language = "EN",
                    Text = match.Groups["title"].Value.Trim()
                });

                legal.Biblio = biblio;
                return legal;
            }
            else
            {
                var match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<appNum>[0-9]{1,5}\/\s?[0-9]{4})\s(?<appDate>[0-9]{2}.[0-9]{2}.[0-9]{4})\s(?<name>.+\sand\s[A-Z]{2})\s(?<title>.+)");
                if (match1.Success)
                {
                    biblio.Application.Number = match1.Groups["appNum"].Value.Trim();
                    biblio.Application.Date = DateTime.Parse(match1.Groups["appDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    biblio.Titles.Add(new Title
                    {
                        Language = "EN",
                        Text = match1.Groups["title"].Value.Trim()
                    });

                    List<string> names = new();
                    List<string> codes = new();
                    List<string> applicants = new();

                    var temps = Regex.Split(match1.Groups["name"].Value.Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var temp in temps)
                    {
                        var tmp = Regex.Match(temp, @"(?<gr>.+)and(?<gr2>.+)");
                        if (tmp.Success)
                        {
                            applicants.Add(tmp.Groups["gr"].Value.Trim());
                            applicants.Add(tmp.Groups["gr2"].Value.Trim());
                        }
                        else applicants.Add(temp.Trim());
                    }
                    foreach (var applicant in applicants)
                    {
                        if (applicant.Length > 2)
                        {
                            var applic = Regex.Match(applicant, @"(?<name>.+),\s?(?<code>[A-Z]{2})");
                            if (applic.Success)
                            {
                                names.Add(applic.Groups["name"].Value.Trim());
                                codes.Add(applic.Groups["code"].Value.Trim());
                            }
                            else names.Add(applicant);
                        }
                        else codes.Add(applicant);
                    }

                    for (var i = 0; i < codes.Count; i++)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = names[i],
                            Country = codes[i]
                        });
                    }
                    legal.Biblio = biblio;

                }
                else Console.WriteLine($"{note.Replace("\r", "").Replace("\n", " ").Trim()}");
                return legal;
            }
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                var url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
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
