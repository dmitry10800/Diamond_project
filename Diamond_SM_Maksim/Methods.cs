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

namespace Diamond_SM_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            var patents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directory = new DirectoryInfo(path);

            var files = new List<string>();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            var xElements = new List<XElement>();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Brevetti Europei Convalidati"))
                        .TakeWhile(val => !val.Value.StartsWith("BREVETTI/PATENTS"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(219\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_) && _.StartsWith("(219)")).ToList();

                    foreach (var note in notes)
                    {
                        patents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
            }
            return patents;
        }

        private Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            var patent = new Diamond.Core.Models.LegalStatusEvent()
            {
                SectionCode = sectionCode,
                SubCode = subCode,
                CountryCode = "SM",
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new Biblio()
            };

            var euroPatent = new EuropeanPatent();
            var culture = new CultureInfo("ru-RU");

            if (subCode == "3")
            {
                var appNumMatch = Regex.Match(note, @"Domanda:(?<appNum>.+?)\s", RegexOptions.Singleline);

                if (appNumMatch.Success)
                {
                    patent.Biblio.Application.Number = appNumMatch.Groups["appNum"].Value.Trim();
                }
                else
                    Console.WriteLine($"{note} --- applicationNum");

                var appDateMatch = Regex.Match(note, @"Deposito:\s?(?<appDate>\d{2}\/\d{2}\/\d{4})",
                    RegexOptions.Singleline);
                if (appDateMatch.Success)
                {
                    patent.Biblio.Application.Date = DateTime.Parse(appDateMatch.Groups["appDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                }
                else
                    Console.WriteLine($"{note} --- applicationDate");

                var epAppNumberMatch = Regex.Match(note, @"Domanda Internazionale:(?<epAppNum>.+?)\s", RegexOptions.Singleline);
                if (epAppNumberMatch.Success)
                {
                    euroPatent.AppNumber = epAppNumberMatch.Groups["epAppNum"].Value.Trim();
                }
                else
                    Console.WriteLine($"{note} --- epApplicationNumber");

                var epAppDateMatch = Regex.Match(note, @"Internazionale:\s?(?<epAppDate>\d{2}\/\d{2}\/\d{4})", RegexOptions.Singleline);
                if (epAppDateMatch.Success)
                {
                    euroPatent.AppDate = DateTime.Parse(epAppDateMatch.Groups["epAppDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                }
                else
                    Console.WriteLine($"{note} --- epApplicationDate");

                var epPubNumberMatch = Regex.Match(note, @"Pubblicazione\s(?<epPubNum>EP:?\s?\d+?)\s",
                    RegexOptions.Singleline);
                if (epPubNumberMatch.Success)
                {
                    euroPatent.PubNumber = epPubNumberMatch.Groups["epPubNum"].Value.Replace(":", "").Trim();
                }
                else
                    Console.WriteLine($"{note} --- epPublicationNumber");

                var prioritMatch = Regex.Match(note, @"Priorità:(?<priority>.+?)\(", RegexOptions.Singleline);
                if (prioritMatch.Success)
                {
                    var priorityList = Regex.Split(prioritMatch.Groups["priority"].Value.Trim(), @";|and", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var priority in priorityList)
                    {
                        var priorityMatch = Regex.Match(priority.Trim(),
                            @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})\s(?<kind>\D{2})", RegexOptions.Singleline);

                        if (priorityMatch.Success)
                        {
                            patent.Biblio.Priorities.Add(new Priority()
                            {
                                Number = priorityMatch.Groups["num"].Value.Trim(),
                                Date = DateTime.Parse(priorityMatch.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
                                Country = priorityMatch.Groups["kind"].Value.Trim()
                            });
                        }
                        else
                            Console.WriteLine($"{priority} ---- priority");
                    }
                }
                else
                    Console.WriteLine($"{note} --- priority");

                var assignerMatch = Regex.Match(note, @"Applicants:(?<assigneers>.+?)\(\d", RegexOptions.Singleline);
                if (assignerMatch.Success)
                {
                    var assigneersList = Regex.Split(assignerMatch.Groups["assigneers"].Value.Replace("\r", "").Replace("\n", " ").Trim(), @";|\sand\s", RegexOptions.Singleline).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();

                    foreach (var assignner in assigneersList)
                    {
                        var assignerrsMatch = Regex.Match(assignner, @"(?<name>.+?),(?<adress>.+),(?<country>.+)");

                        if (assignerrsMatch.Success)
                        {
                            var country = MakeCountryCode(assignerrsMatch.Groups["country"].Value.Trim());

                            if (country != null)
                            {
                                patent.Biblio.Assignees.Add(new PartyMember()
                                {
                                    Name = assignerrsMatch.Groups["name"].Value.Replace("- ","").Trim(),
                                    Address1 = assignerrsMatch.Groups["adress"].Value.Trim(),
                                    Country = country
                                });
                            }
                            else
                                Console.WriteLine($"{assignerrsMatch.Groups["country"].Value.Trim()} --- country");
                        }
                    }
                }
                else
                    Console.WriteLine($"{note} --- assigneers");

                var agentsMatch = Regex.Match(note, @"Agent:(?<agents>.+?)(\(|Titolo)", RegexOptions.Singleline);
                if (agentsMatch.Success)
                {
                    var agentMatch =
                        Regex.Match(agentsMatch.Groups["agents"].Value.Replace("\r", "").Replace("\n", " ").Trim(),
                            @"(?<name>.+?),(?<adress>.+)");
                    if (agentMatch.Success)
                    {
                        patent.Biblio.Agents.Add(new PartyMember()
                        {
                            Name = agentMatch.Groups["name"].Value.Replace("- ", "").Trim(),
                            Address1 = agentMatch.Groups["adress"].Value.Trim(),
                            Country = "SM"
                        });
                    }
                }
                else
                    Console.WriteLine($"{note} --- agents");

                var inventorsMatch = Regex.Match(note, @"Inventors:(?<inventors>.+?)(\(|Titolo)", RegexOptions.Singleline);
                if (inventorsMatch.Success)
                {
                    var inventorsLIst = Regex.Split(inventorsMatch.Groups["inventors"].Value.Replace("\r","").Replace("\n"," ").Trim(), @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    foreach (var inventor in inventorsLIst)
                    {
                        patent.Biblio.Inventors.Add(new PartyMember()
                        {
                            Name = inventor.Replace("- ", "").Trim()
                        });
                    }
                }
                else
                    Console.WriteLine($"{note} --- inventors");

                var titleMatch = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"Titolo(?<title>.+)");
                if (titleMatch.Success)
                {
                    patent.Biblio.Titles.Add(new Title()
                    {
                        Language = "IT",
                        Text = titleMatch.Groups["title"].Value.Trim()
                    });
                }
                else
                    Console.WriteLine($"{note} --- title");

                patent.Biblio.EuropeanPatents.Add(euroPatent);
            }
            return patent;
        }

        private string? MakeCountryCode(string countryCode) => countryCode switch
        {
            "U.S.A." => "US",
            "Germany Fed.Rep" => "DE",
            "Finland" => "FI",
            "Canada" => "CA",
            "Netherlands" => "NL",
            "08192 Sant Quirze del Vallès" => "ES",
            "Great Britain" => "GB",
            "France" => "FR",
            "Rep.of Korea" => "KR",
            "Denmark" => "DK",
            "Japan" => "JP",
            "Belgium" => "BE",
            "Spain" => "ES",
            "Peoples China" => "CN",
            "Czech Republic" => "CZ",
            "Austria" => "AT",
            "Italy" => "IT",
            _ => null
        };

        private string MakeText(List<XElement> xElements, string subCode)
        {
            var sb = new StringBuilder();

            if (subCode == "3")
            {
                foreach (var xElement in xElements)
                {
                        sb = sb.Append(xElement.Value + "\n");
                }
            }
            return sb.ToString();
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
                _ = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
