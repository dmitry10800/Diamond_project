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
using System.Xml.Linq;

namespace Diamond_MD_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int id = 1;

        private readonly string I21 = "(21)";
        private readonly string I13 = "(13)";
        private readonly string I51 = "(51)";
        private readonly string I96 = "(96)";
        private readonly string I97 = "(97)";
        private readonly string I87 = "(87)";
        private readonly string I31 = "(31)";
        private readonly string I32 = "(32)";
        private readonly string I33 = "(33)";
        private readonly string I71 = "(71)";
        private readonly string I72 = "(72)";
        private readonly string I54 = "(54)";

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
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
                        .SkipWhile(val => !val.Value.StartsWith("GEPI publică fiecare solicitare de validare după informarea de către OEB privind achitarea taxei"))
                        .TakeWhile(val => !val.Value.StartsWith("FF4A Brevete de invenţie acordate /"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements), @"(?=\(21\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(21)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakeConvertedPatent(note, subCode, "BB2A"));
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
                text += xElement.Value + " ";
            }
            return text;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakeConvertedPatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatus = new()
            {
                SubCode = subCode,
                SectionCode = sectionCode,
                CountryCode = "MD",
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Id = id++,
            };

            Biblio biblio = new();
            biblio.Priorities = new();
            biblio.EuropeanPatents = new();

            List<string> priorityNumbers = new();
            List<string> priorityDates = new();
            List<string> priorityCountries = new();

            foreach (var inid in MakeInids(note))
            {
                if (inid.StartsWith(I21))
                {
                    biblio.Application.Number = inid.Replace(I21, "").Trim();
                }
                else
                if (inid.StartsWith(I13))
                {
                    biblio.Publication.Kind = inid.Replace(I13, "").Trim();
                }
                else
                if (inid.StartsWith(I51))
                {
                    var inid51 = Regex.Replace(inid, @"(\(51\)\s?Int.\s?Cl.:)", "").Trim();
                    var inids = Regex.Split(inid51, @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblio.Ipcs = new();

                    foreach (var iinid in inids)
                    {
                        var match = Regex.Match(iinid, @"(?<class>.+)\((?<date>.+)\)");
                        if (match.Success)
                        {
                            biblio.Ipcs.Add(new Ipc
                            {
                                Class = match.Groups["class"].Value.Trim(),
                                Date = match.Groups["date"].Value.Replace(".", "/").Trim()
                            });
                        }
                        else Console.WriteLine($"{iinid} in 51");
                    }
                }
                else
                if (inid.StartsWith(I96))
                {
                    var match = Regex.Match(inid.Replace(I96, "").Trim(), @"(?<number>.+),\s(?<date>[0-9]{4}.[0-9]{2}.[0-9]{2})");

                    if (match.Success)
                    {
                        biblio.EuropeanPatents.Add(new EuropeanPatent
                        {
                            AppNumber = match.Groups["number"].Value.Trim(),
                            AppDate = match.Groups["date"].Value.Replace(".", "/").Trim()
                        });
                    }
                    else Console.WriteLine($"{inid} in 96");
                }
                else
                if (inid.StartsWith(I97))
                {
                    var match = Regex.Match(inid.Replace(I97, "").Trim(), @"(?<number>.+),\s(?<date>[0-9]{4}.[0-9]{2}.[0-9]{2})");

                    if (match.Success)
                    {
                        biblio.EuropeanPatents.Add(new EuropeanPatent
                        {
                            PubNumber = match.Groups["number"].Value.Trim(),
                            PubDate = match.Groups["date"].Value.Replace(".", "/").Trim()
                        });
                    }
                    else Console.WriteLine($"{inid} in 97");
                }
                else
                if (inid.StartsWith(I87))
                {
                    var match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<number>.+),\s(?<date>[0-9]{4}.[0-9]{2}.[0-9]{2})");

                    biblio.IntConvention = new();

                    if (match.Success)
                    {
                        biblio.IntConvention.PctPublNumber = match.Groups["number"].Value.Trim();
                        biblio.IntConvention.PctPublDate = match.Groups["date"].Value.Replace(".", "/").Trim();
                    }
                    else Console.WriteLine($"{inid} in 87");
                }
                else
                if (inid.StartsWith(I31))
                {
                    priorityNumbers = Regex.Split(inid.Replace(I31, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                }
                else
                if (inid.StartsWith(I32))
                {
                    priorityDates = Regex.Split(inid.Replace(I32, "").Replace(".", "/").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                }
                else
                if (inid.StartsWith(I33))
                {
                    priorityCountries = Regex.Split(inid.Replace(I33, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                }
                else
                if (inid.StartsWith(I71))
                {
                    var applicants = Regex.Split(inid.Replace(I71, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblio.Applicants = new();

                    foreach (var applicant in applicants)
                    {
                        var match = Regex.Match(applicant, @"(?<name>.+)(?<country>[A-Z]{2})");

                        if (match.Success)
                        {
                            biblio.Applicants.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim().TrimEnd(','),
                                Country = match.Groups["country"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{applicant} in 71");
                    }
                }
                else
                if (inid.StartsWith(I72))
                {
                    var inventors = Regex.Split(inid.Replace(I72, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    biblio.Inventors = new();

                    foreach (var inventor in inventors)
                    {
                        var match = Regex.Match(inventor, @"(?<name>.+)(?<country>[A-Z]{2})");

                        if (match.Success)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim().TrimEnd(','),
                                Country = match.Groups["country"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inventor} in 72");
                    }
                }
                else
                if (inid.StartsWith(I54))
                {
                    var match = Regex.Match(inid.Replace(I54, "").Trim(), @"(?<md>.+)\s(?<en>[A-Z].+)\s(?<ru>[А-Я].+)");

                    biblio.Titles = new();
                    if (match.Success)
                    {
                        biblio.Titles.Add(new Title
                        {
                            Language = "RO",
                            Text = match.Groups["md"].Value.Trim()
                        });
                        biblio.Titles.Add(new Title
                        {
                            Language = "EN",
                            Text = match.Groups["en"].Value.Trim()
                        });
                        biblio.Titles.Add(new Title
                        {
                            Language = "RU",
                            Text = match.Groups["ru"].Value.Trim()
                        });
                    }
                    else
                    {
                        var match1 = Regex.Match(inid.Replace(I54, "").Trim(), @"(?<en>.+)\s(?<ru>[А-Я].+)");

                        if (match1.Success)
                        {
                            biblio.Titles.Add(new Title
                            {
                                Language = "EN",
                                Text = match.Groups["en"].Value.Trim()
                            });
                            biblio.Titles.Add(new Title
                            {
                                Language = "RU",
                                Text = match.Groups["ru"].Value.Trim()
                            });
                        }
                        else
                        {
                            biblio.Titles.Add(new Title
                            {
                                Language = "RU",
                                Text = inid.Replace(I54, "").Trim()
                            });
                        }
                    }
                }
                else Console.WriteLine($"{inid} don't processed");
            }

            for (var i = 0; i < priorityNumbers.Count; i++)
            {
                biblio.Priorities.Add(new Priority
                {
                    Number = priorityNumbers[i].Trim(),
                    Date = priorityDates[i].Trim(),
                    Country = priorityCountries[i].Trim()
                });
            }

            legalStatus.Biblio = biblio;

            return legalStatus;
        }
        internal List<string> MakeInids(string note) => Regex.Split(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?=\([0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                var url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
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
