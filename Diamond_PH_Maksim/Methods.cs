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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_PH_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
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
                if(subCode == "5")
                {
                    //xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                    //      .SkipWhile(val => !val.Value.StartsWith("LAPSED PATENTS"))
                    //      //.TakeWhile(val => !val.Value.StartsWith("INTELLECTUAL PROPERTY PHILIPPINES"))
                    //      .ToList();

                    StreamReader streamReader = new(CurrentFileName.Replace(".tetml", ".txt"));

                    foreach (string note in Regex.Split(streamReader.ReadToEnd().Replace("\r", "").Replace("\n", " ").Trim(), @"(?=\d\/\d{4}\/\d+)")
                        .Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("1/")).ToList())
                    {
                        statusEvents.Add(SplitNote(note, subCode, "MM"));
                    }
                }
                else
                if(subCode == "22")
                {
                    //xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                    //      .SkipWhile(val => !val.Value.StartsWith("1.1 FORFEITED INVENTION APPLICATIONS"))
                    //      .TakeWhile(val => !val.Value.StartsWith("INTELLECTUAL PROPERTY PHILIPPINES"))
                    //      .ToList();

                    StreamReader streamReader = new(CurrentFileName.Replace(".tetml", ".txt"));

                    List<string> notes = Regex.Split(streamReader.ReadToEnd().Replace("\r","").Replace("\n"," ").Trim(), @"(?=\d\/\d{4}\/\d{6,7})")
                        .Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith(@"1/")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "FA"));
                    }
                }
                else
                if(subCode == "29")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                         .SkipWhile(val => !val.Value.StartsWith("1.1 PUBLICATION OF INVENTION APPLICATIONS (PCT"))
                         .TakeWhile(val => !val.Value.StartsWith("1.1 PUBLICATION OF DIVISIONAL INVENTION APPLICATIONS (PCT)"))
                         .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=[0-9]{1,3}\sPCT\/.+)", RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d{1,3}\sPCT\/.+").Match(val).Success).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "BA"));
                    }
                }
                else
                if (subCode == "35")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                          .SkipWhile(val => !val.Value.StartsWith("REGISTERED UTILITY MODELS"))
                          .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\d\/\d{4}\/\d{6})")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("2")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(SplitNote(note, subCode, "FG"));
                    }
                }
            }


            return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent SplitNote(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                Id = Id++,
                SectionCode = sectionCode,
                SubCode = subCode,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "PH",
                Biblio = new(),
                LegalEvent = new()
            };

            CultureInfo culture = new("ru-Ru");
            if (subCode == "5")
            {
                Match match = Regex.Match(note.Trim(), @"(?<aNum>\d+\/\d+\/\d+).+?(?<owner>[A-Z].+)\s(?<d43>\d{2}\/\d{2}\/\d{4})\s(?<title>([A-Z]|\d)\D+)");

             
                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                    foreach (string owner in Regex.Split(match.Groups["owner"].Value.Trim(), @";|\sand\s").Where(val => !string.IsNullOrEmpty(val)).ToList())
                    {
                        Match match1 = Regex.Match(owner.Trim(), @"(?<name>\D+)\s\[(?<code>[A-Z]{2})");
                        if (match1.Success)
                        {
                            statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                            {
                                Name = match1.Groups["name"].Value.Trim(),
                                Country = match1.Groups["code"].Value.Trim()
                            });
                        }
                        else
                        {
                            statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                            {
                                Name = owner.Trim()
                            });
                        }
                    }

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "EN",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Groups["d43"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    Match match2 = Regex.Match(CurrentFileName.Replace(".txt", ""), @"\d{8}");

                    if (match2.Success)
                    {
                        statusEvent.LegalEvent.Date = match2.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }
                }
                else Console.WriteLine($"{note}");
            }
            else
            if(subCode == "22")
            {
                Match match = Regex.Match(note.Replace("\r","").Replace("\n", " ").Trim(), @"(?<appNum>\d\/\d{4}\/\d{6,7})\s?(?<day>\d+)\s(?<month>.+)\s(?<year>\d{4})\s?(?<title>.+)\s?\[(?<code>\D{2})");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                    statusEvent.Biblio.Application.Date = match.Groups["year"].Value.Trim() + "-" + MakeMonth( match.Groups["month"].Value.Replace(" ","").ToLower().Trim()) + "-" + match.Groups["day"].Value.Trim();

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "EN",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                    {
                        Country = match.Groups["code"].Value.Trim()
                    });

                    Match match1 = Regex.Match(CurrentFileName.Replace(".tetml", "").Trim(), @"\d{8}");

                    if (match1.Success)
                    {
                        statusEvent.LegalEvent.Date = match1.Value.Insert(4, @"-").Insert(7, @"-").Trim();
                    }

                }
                else Console.WriteLine($"{note} - wrong match");
            }
            else
            if(subCode == "29")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<PCTAppNum>PCT.+?)\s(?<appNum>.+)\s(?<PCTdate>\d{2}\/\d{2}\/\d{4})\s(?<title>.+?)\s(?<ipcs>[A-Z]\d.+)");

                if (match.Success)
                {
                    statusEvent.Biblio.IntConvention.PctApplNumber = match.Groups["PCTAppNum"].Value.Trim();
                    statusEvent.Biblio.IntConvention.PctNationalDate = DateTime.Parse(match.Groups["PCTdate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();

                    List<string> ipcs = Regex.Split(match.Groups["ipcs"].Value.Trim(), @";").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"[A-Z]\d{2}[A-Z]\s\d{1,2}\/\d+").Match(val).Success).ToList();

                    foreach (string ipc in ipcs)
                    {
                        statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                        {
                            Class = ipc.Trim()
                        });
                    }

                    List<string> applicants = Regex.Split(match.Groups["title"].Value.Trim(), @"(?<=])").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    for (int i = 0; i < applicants.Count; i++)
                    {
                        if(i == 0)
                        {
                            Match match1 = Regex.Match(applicants[i].Trim(), @"(?<title>.+)\s\[(?<code>\D{2})");

                            if (match1.Success)
                            {
                                statusEvent.Biblio.Titles.Add(new Integration.Title
                                {
                                    Language = "EN",
                                    Text = match1.Groups["title"].Value.Trim()
                                });

                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Country = match1.Groups["code"].Value.Trim()
                                });
                            }
                            else 
                            {
                                statusEvent.Biblio.Titles.Add(new Integration.Title
                                {
                                    Language = "EN",
                                    Text = applicants[i].Trim()
                                });
                            }                           
                        }
                        else
                        {
                            Match match1 = Regex.Match(applicants[i].Replace("and", "").Replace(";","").Replace(",","").Trim(), @"(?<name>.+)\s\[(?<code>\D{2})");

                            if (match1.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Country = match1.Groups["code"].Value.Trim(),
                                    Name = match1.Groups["name"].Value.Trim()
                                });
                            }
                        }

                    }

                }
                else Console.WriteLine($"{note}");
            }
            else
            if(subCode == "35")
            {
                CultureInfo cultureInfo = new("ru-RU");

                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<num>.+?)\s(?<assigner>.+)]\s(?<title>.+)\s(?<appDate>\d{2}.\d{2}.\d{4})");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["num"].Value.Trim();
                    statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["appDate"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    statusEvent.Biblio.Titles.Add(new Integration.Title
                    {
                        Language = "EN",
                        Text = match.Groups["title"].Value.Trim()
                    });

                    Match match1 = Regex.Match(match.Groups["assigner"].Value.Trim(), @"(?<name>.+)\s\[(?<code>[A-Z]{2})");

                    if (match1.Success)
                    {
                        statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                        {
                            Name = match1.Groups["name"].Value.Trim(),
                            Country = match1.Groups["code"].Value.Trim()
                        });
                    }
                    else Console.WriteLine($"{match.Groups["assigner"].Value.Trim()}");
                }
                else Console.WriteLine($"{note}");
            }

            return statusEvent;
        }

        internal string MakeMonth(string month) => month switch
        {
            "january" => "01",
            "february" => "02",
            "march" => "03",
            "april" => "04",
            "may" => "05",
            "june" => "06",
            "july" => "07",
            "august" => "08",
            "september" => "09",
            "october" => "10",
            "november" => "11",
            "december" => "12",
            _ => null
        }; 
        internal string MakeText(List<XElement> xElements)
        {
            string text = null;
            
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + " ";
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
