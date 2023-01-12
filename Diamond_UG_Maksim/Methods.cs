using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Integration;

namespace Diamond_UG_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalStatusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = new();

            foreach (var item in files)
            {
                _currentFileName = item;

                tet = XElement.Load(item);

                if (subCode is "3" or "4")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text").ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(20\) Filing.+)", RegexOptions.Singleline)
                       .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(20)")).ToList();

                    foreach (var note in notes)
                    {
                        legalStatusEvents.Add(MakeNote(note, subCode, "BA"));
                    }
                }
            }
            return legalStatusEvents;
        }
        private Diamond.Core.Models.LegalStatusEvent MakeNote(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatus = new()
            {
                Id = _id++,
                SectionCode = sectionCode,
                SubCode = subCode,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "UG",
                Biblio = new Biblio(),
                LegalEvent = new LegalEvent()
            };

            if (subCode is "3" or "4")
            {
                foreach (var inid in MakeInids(note))
                {
                    if (inid.StartsWith("(20)"))
                    {
                        var match = Regex.Match(
                            inid.Replace("(20) Filing Number and Date","").Replace("\r", "").Replace("\n", " ").Trim(),
                            @"(?<appNum>.+)\s(?<appDate>\d{4}\.\d{2}\.\d{2})");

                        if (match.Success)
                        {
                            legalStatus.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                            legalStatus.Biblio.Application.Date = match.Groups["appDate"].Value.Replace(".","/").Trim();
                        }
                        else Console.WriteLine($"{inid} -- 20");
                    }
                    else if (inid.StartsWith("(30)"))
                    {
                        var priorities = Regex.Split(inid.Replace("(30) Priority Details","").Replace("\r", "").Replace("\n", " ").Trim(),
                            @"(?<=\d{4}\.\d{2}\.\d{2})", RegexOptions.Singleline).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();

                        if (priorities.Count != 0)
                        {
                            foreach (var priority in priorities)
                            {
                                var match = Regex.Match(priority.Trim(), @"(?<code>\D{2})\s(?<num>.+)\s(?<date>\d{4}\.\d{2}\.\d{2})", RegexOptions.Singleline);

                                if (match.Success)
                                {
                                    legalStatus.Biblio.Priorities.Add(new Priority()
                                    {
                                        Country = match.Groups["code"].Value.Trim(),
                                        Number = match.Groups["num"].Value.Trim(),
                                        Date = match.Groups["date"].Value.Replace(".", "/").Trim()
                                    });
                                }
                                else Console.WriteLine($"{priority} -- 30");
                            }
                        }
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"(?<inid51>.+)(?<inid7173>\(71\/73\).+)", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            var ipcs = Regex.Split(match.Groups["inid51"].Value.Replace("(51) IPC Class(es)", "").Trim(), "\n")
                                .Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
                            if (ipcs.Count != 0)
                            {
                                foreach (var ipc in ipcs)
                                {
                                    var match51inid = Regex.Match(ipc.Trim(), @"(?<class>.+)\((?<year>.+)\)");

                                    if (match51inid.Success)
                                    {
                                        legalStatus.Biblio.Ipcs.Add(new Ipc()
                                        {
                                            Date = match51inid.Groups["year"].Value.Trim(),
                                            Class = match51inid.Groups["class"].Value.Trim()
                                        });
                                    }
                                    else Console.WriteLine($"{ipc} -- 51");
                                }
                            }

                            var applicants = Regex.Split(match.Groups["inid7173"].Value.Replace("(71/73) Applicant", "").Trim(), "\n")
                                .Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
                            if (applicants.Count != 0)
                            {
                                foreach (var applicant in applicants)
                                {
                                    var matchApplicant = Regex.Match(applicant.Trim(), @"(?<name>.+):(?<adress>.+),\s(?<code>\D{2}$)");
                                    if (matchApplicant.Success)
                                    {
                                        legalStatus.Biblio.Applicants.Add(new PartyMember()
                                        {
                                            Country = matchApplicant.Groups["code"].Value.Trim(),
                                            Name = matchApplicant.Groups["name"].Value.Trim(),
                                            Address1 = matchApplicant.Groups["adress"].Value.Trim()
                                        });

                                        legalStatus.Biblio.Assignees.Add(new PartyMember()
                                        {
                                            Country = matchApplicant.Groups["code"].Value.Trim(),
                                            Name = matchApplicant.Groups["name"].Value.Trim(),
                                            Address1 = matchApplicant.Groups["adress"].Value.Trim()
                                        });
                                    }
                                    else
                                    {
                                        var matchApplicant1 = Regex.Match(applicant.Trim(), @"(?<name>.+):(?<adress>.+)");
                                        if (matchApplicant1.Success)
                                        {
                                            legalStatus.Biblio.Applicants.Add(new PartyMember()
                                            {
                                                Name = matchApplicant1.Groups["name"].Value.Trim(),
                                                Address1 = matchApplicant1.Groups["adress"].Value.Trim()
                                            });

                                            legalStatus.Biblio.Assignees.Add(new PartyMember()
                                            {
                                                Name = matchApplicant1.Groups["name"].Value.Trim(),
                                                Address1 = matchApplicant1.Groups["adress"].Value.Trim()
                                            });
                                        }
                                        else
                                        {
                                            legalStatus.Biblio.Applicants.Add(new PartyMember()
                                            {
                                                Name = applicant
                                            });

                                            legalStatus.Biblio.Assignees.Add(new PartyMember()
                                            {
                                                Name = applicant
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        else Console.WriteLine($"{inid} - 51/71/73");
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        var inventors = Regex.Split(inid.Replace("(72) Inventor", "").Trim(), "\n")
                                .Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();

                        if (inventors.Count != 0)
                        {
                            foreach (var inventor in inventors)
                            {
                                var match = Regex.Match(inventor.Trim(), @"(?<name>.+):(?<adress>.+),\s(?<code>\D{2}$)");
                                if (match.Success)
                                {
                                    legalStatus.Biblio.Inventors.Add(new PartyMember()
                                    {
                                        Country = match.Groups["code"].Value.Trim(),
                                        Name = match.Groups["name"].Value.Trim(),
                                        Address1 = match.Groups["adress"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    var match1 = Regex.Match(inventor.Trim(), @"(?<name>.+):(?<adress>.+)");
                                    if (match1.Success)
                                    {
                                        legalStatus.Biblio.Inventors.Add(new PartyMember()
                                        {
                                            Name = match1.Groups["name"].Value.Trim(),
                                            Address1 = match1.Groups["adress"].Value.Trim()
                                        });
                                    }
                                    else
                                    {
                                        legalStatus.Biblio.Inventors.Add(new PartyMember()
                                        {
                                            Name = inventor
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        var match = Regex.Match(inid.Replace("(74) Representative Name","").Replace("\r","").Replace("\n"," ").Trim(), 
                            @"(?<name>.+)\s:(?<adress>.+)");

                        if (match.Success)
                        {
                            legalStatus.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim()
                            });
                        }
                        else
                        {
                            var name = inid.Replace("(74) Representative Name ", "").Replace("\r", "").Replace("\n", "").Trim();

                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                legalStatus.Biblio.Agents.Add(new PartyMember()
                                {
                                    Name = name
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        var titles = Regex.Split(inid.Replace("(54) Title", "").Trim(), @"\n")
                            .Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();

                        legalStatus.Biblio.Titles.Add(new Title()
                        {
                            Text = titles[0],
                            Language = "EN"
                        });
                    }
                    else Console.WriteLine($"{inid}");
                }
            }
            return legalStatus;
        }
        private List<string> MakeInids(string note)
        {
            var inids = new List<string>();

            inids = Regex.Split(note, @"(?=\(\d{2}\))", RegexOptions.Singleline).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            return inids;
        }
        private string MakeText(List<XElement> xElements)
        {
            var sb = new StringBuilder();

            foreach (var xElement in xElements)
            {
                sb = sb.Append(xElement.Value + " " + '\r' + '\n');
            }
            return sb.ToString();
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                var url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                _ = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
