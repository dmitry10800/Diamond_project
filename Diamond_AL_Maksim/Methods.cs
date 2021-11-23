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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_AL_Maksim
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

                if (subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("PATENTA TË LËSHUARA"))
                        .TakeWhile(val => !val.Value.StartsWith("NDRYSHIMI I ADRESËS SË"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(11\)\s\d)").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note.Replace("Buletini i Pronësisë Industriale", ""), subCode, "FG"));
                    }
                }
                else if (subCode == "19")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("PATENTA TË SKADUARA"))                        
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(\s11\s\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("( 11 )")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "MK"));
                    }
                }
            }
            return statusEvents;
        }
        internal string MakeText(List<XElement> xElements)
        {
            string text = null;

            foreach (XElement xElement in xElements)
            {
                text += xElement.Value + "\n";
            }
            
            return text;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legal = new()
            {
                SectionCode = sectionCode,
                SubCode = subCode,
                Id = Id++,
                CountryCode = "AL",
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf"))
            };

            Biblio biblio = new()
            {
                DOfPublication = new DOfPublication()
            };

            CultureInfo culture = new("ru-RU");

            biblio.EuropeanPatents = new();
            EuropeanPatent europeanPatent = new();

            if (subCode == "3")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(11)"))
                    {
                        biblio.Publication.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(11)", "").Trim();
                    }
                    else if (inid.StartsWith("(97)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(97)", "").Trim(), @"(?<num>.+)\s\/\s(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            europeanPatent.PubNumber = match.Groups["num"].Value.Trim();
                            europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(96)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(96)", "").Trim(), @"(?<num>.+)\s\/\s(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            europeanPatent.AppNumber = match.Groups["num"].Value.Trim();
                            europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        biblio.Application.Date = DateTime.Parse(inid.Replace("\r", "").Replace("\n", " ").Replace("(22)","").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        biblio.Application.Number = inid.Replace("\r", "").Replace("\n", " ").Replace("(21)", "").Replace(" ","").Trim();
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Replace("(54)", "").Trim(), @"(?<title>.+)\s(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            biblio.Application.EffectiveDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            biblio.Titles.Add(new Title
                            {
                                Language = "SQ",
                                Text = match.Groups["title"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else if (inid.StartsWith("(30)"))
                    {
                        List<string> priorites = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(30)","").Trim(), @";|and").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priorite in priorites)
                        {
                            Match match = Regex.Match(priorite.Trim(), @"(?<num>.+)\s(?<date>\d{2}.\d{2}.\d{4})\s(?<code>\D{2})");

                            if (match.Success)
                            {
                                biblio.Priorities.Add(new Priority
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Number = match.Groups["num"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                });
                            }
                            else Console.WriteLine($"{priorite} -- 30");
                        }
                    }
                    else if (inid.StartsWith("(71)"))
                    {
                        List<string> applicants = Regex.Split(inid.Replace("(71)", "").Trim(), @"(?<=,\s\D{2}\n)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match match = Regex.Match(applicant.Trim(), @"(?<name>.+)\n(?<adress>.+),\s(?<code>\D{2})");

                            if (match.Success)
                            {
                                biblio.Applicants.Add(new PartyMember
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{applicant} -- 71");
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        List<string> inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(72)", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+)\s\((?<adress>.+)\)");

                            if (match.Success)
                            {
                                biblio.Inventors.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inventor} -- 72");
                        }
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        Match match = Regex.Match(inid.Replace("(74)", "").Trim(), @"(?<name>.+)\n(?<adress>.+)");

                        if (match.Success)
                        {
                            biblio.Agents.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Trim(),
                                Country = "AL"
                            });
                        }
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        int count = 1;

                        List<string> claims = Regex.Split(inid.Replace("\r","").Replace("\n"," ").Replace("(57)","").Trim(), @"(?>\d{1,3}\.\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string claim in claims)
                        {
                            biblio.Claims.Add(new DiamondProjectClasses.Claim
                            {
                                Language = "SQ",
                                Text = claim.Trim(),
                                Number = count.ToString()
                            });

                            count++;
                        }
                    }
                    else Console.WriteLine($"{inid} - not process");
                }
                Match match11 = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                if (match11.Success)
                {
                    biblio.DOfPublication.date_45 = match11.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                }

                biblio.EuropeanPatents.Add(europeanPatent);
                legal.Biblio = biblio;
            }
            else if(subCode == "19")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("( 11 )") || inid.StartsWith("(11 )") || inid.StartsWith("( 11)") || inid.StartsWith("(11)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<num>[0-9]{1,5})\s(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                        if (match.Success)
                        {
                            biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            biblio.DOfPublication = new DOfPublication()
                            {
                                date_45 = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim()
                            };
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<num>[0-9]{1,5})");

                            if (match1.Success)
                            {
                                biblio.Publication.Number = match1.Groups["num"].Value.Trim();
                            }
                            else Console.WriteLine($"{inid}  ================== 11");
                        }
                    }
                    else
                    if (inid.StartsWith("( 97 )") || inid.StartsWith("(97 )") || inid.StartsWith("( 97)") || inid.StartsWith("(97)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<num>.+)\/\s?(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                        if (match.Success)
                        {
                            europeanPatent.PubNumber = match.Groups["num"].Value.Trim();
                            europeanPatent.PubDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<num>.+)\/");
                            if (match1.Success)
                            {
                                europeanPatent.PubNumber = match1.Groups["num"].Value.Trim();
                            }
                            else Console.WriteLine($"{ inid} ======== 97");
                        }
                    }
                    else
                    if (inid.StartsWith("( 96 )") || inid.StartsWith("(96 )") || inid.StartsWith("( 96)") || inid.StartsWith("(96)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<num>.+)\/\s?(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                        if (match.Success)
                        {
                            europeanPatent.AppNumber = match.Groups["num"].Value.Trim();
                            europeanPatent.AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<num>.+)\/");
                            if (match1.Success)
                            {
                                europeanPatent.AppNumber = match1.Groups["num"].Value.Trim();
                            }
                            else Console.WriteLine($"{inid} ======== 96");
                        }
                    }
                    else
                    if (inid.StartsWith("( 21 )") || inid.StartsWith("(21 )") || inid.StartsWith("( 21)") || inid.StartsWith("(21)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)(?<num>.+)");

                        if (match.Success)
                        {
                            biblio.Application.Number = match.Groups["num"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} =========== 21");
                    }
                    else
                    if (inid.StartsWith("( 22 )") || inid.StartsWith("(22 )") || inid.StartsWith("( 22)") || inid.StartsWith("(22)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)\s?(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})");

                        if (match.Success)
                        {
                            biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim();
                        }
                        else Console.WriteLine($"{inid} ============= 22");
                    }
                    else
                    if (inid.StartsWith("( 54 )") || inid.StartsWith("(54 )") || inid.StartsWith("( 54)") || inid.StartsWith("(54)"))
                    {

                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"\)\s?(?<text>.+)");

                        if (match.Success)
                        {
                            biblio.Titles = new()
                            {
                                new Title
                                {
                                    Language = "SQ",
                                    Text = match.Groups["text"].Value.Trim()
                                }
                            };
                        }
                        else Console.WriteLine($"{inid} ================ 54");
                    }
                    else
                    if (inid.StartsWith("( 30 )") || inid.StartsWith("(30 )") || inid.StartsWith("( 30)") || inid.StartsWith("(30)"))
                    {
                        List<string> priorities = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("( 30)", "").Trim(), @"and").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        biblio.Priorities = new();

                        foreach (string priority in priorities)
                        {
                            Match match = Regex.Match(priority.Trim(), @"(?<num>.+)\s(?<date>[0-9]{2}.\s?[0-9]{2}.\s?[0-9]{4})\s(?<code>[A-Z]{2})");

                            if (match.Success)
                            {
                                biblio.Priorities.Add(new Priority
                                {
                                    Number = match.Groups["num"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "-").Trim(),
                                    Country = match.Groups["code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{priority} ================ 30");
                        }
                    }
                    else
                    if (inid.StartsWith("( 73 )") || inid.StartsWith("(73 )") || inid.StartsWith("( 73)") || inid.StartsWith("(73)"))
                    {
                        biblio.Assignees = new();

                        Match match = Regex.Match(inid.Replace("( 73 )", "").Trim(), @"(?<name>.*)\n(?<adress>.*\n?.*\n?.*\n?.*)");
                        if (match.Success)
                        {
                            Match match1 = Regex.Match(match.Groups["adress"].Value.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<adres>.+)\s(?<code>[A-Z]{2}$)");
                            if (match1.Success)
                            {
                                biblio.Assignees.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match1.Groups["adres"].Value.Trim(),
                                    Country = match1.Groups["code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inid} ========== 73/adress");
                        }
                        else Console.WriteLine($"{inid} ========== 73");
                    }
                    else
                    if (inid.StartsWith("( 72 )") || inid.StartsWith("(72 )") || inid.StartsWith("( 72)") || inid.StartsWith("(72)"))
                    {
                        biblio.Inventors = new();

                        List<string> inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("( 72 )", "").Trim(), @"\)\s?;").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+?)\s?\((?<adress>.*)\)?");

                            if (match.Success)
                            {
                                biblio.Inventors.Add(new PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.TrimEnd(')').Trim()
                                });
                            }
                            else Console.WriteLine($"{inventor} ============ 72");
                        }
                    }
                    else Console.WriteLine($"{inid} =========== не обработан");
                }

                Match date = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")), @"[0-9]{8}");
                if (date.Success)
                {
                    legal.LegalEvent = new LegalEvent()
                    {
                        Date = date.Value.Insert(4, "/").Insert(7, "/").Trim()
                    };
                }

                biblio.EuropeanPatents.Add(europeanPatent);
                legal.Biblio = biblio;
            }

            return legal;
        }
        internal List<string> MakeInids(string note, string subCode)
        {
            List<string> inids = new();

            if(subCode == "3")
            {
                Match match = Regex.Match(note.Trim(), @"(?<inids>.+?)\s(?<inid57>\(57.+)", RegexOptions.Singleline);

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["inids"].Value.Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                    inids.Add(match.Groups["inid57"].Value.Trim());
                }
                else Console.WriteLine($"{note}  --- not spliting");
            }
            else if(subCode == "19")
            {
                inids = Regex.Split(note.Trim(), @"(?=\(\s{0,2}[0-9]{2}\s{0,2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
            }

            return inids;
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
