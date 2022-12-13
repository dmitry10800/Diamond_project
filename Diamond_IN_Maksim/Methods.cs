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

namespace Diamond_IN_Maksim
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

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("Early Publication:"))
                            .TakeWhile(val => !val.Value.StartsWith("Publication After 18 Months:"))
                            .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(12\)\sPA)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(12) PA")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BZ"));
                    }
                }
                else if(subCode == "10")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("WEEKLY ISSUED FER (DELHI)"))
                            .TakeWhile(val => !val.Value.StartsWith("PUBLICATION UNDER SECTION 57 AND UNDER RULE 81(3)"))
                            .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?>(\d{1,3}\sKOLKATA)|(\d{1,3}\sDELHI)|(\d{1,3}\sMUMBAI)|(\d{1,3}\sCHENNAI))")
                        .Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d").Match(val).Success).ToList();

                    List<string> works = new();
                    for (int i = 0; i < notes.Count; i+=2)
                    {
                        works.Add(notes[i] + " " + notes[i+1]);
                    }

                    foreach (string note in works)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "EC"));
                    }

                }
            }

            return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "IN",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new(),
                LegalEvent = new()
            };

            CultureInfo culture = new("ru-RU");
            if(subCode == "1")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(12)"))
                    {
                        statusEvent.Biblio.Publication.LanguageDesignation = inid.Replace("(12)", "").Trim();
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @"(?<num>\d+)\s(?<kind>\D)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Number = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid}  --- 21");
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 22");
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 43");
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"invention :\s?(?<title>.+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "EN",
                                Text = match.Groups["title"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 54");
                    }
                    else if (inid.StartsWith("(71)"))
                    {
                        // todo: сделать нормальную регулярку и предусмотреть символы --------- и формат, в которой есть NA
                        List<string> applicants = Regex.Split(inid.Replace("\r","").Replace("\n"," ").Replace("(71)Name of Applicant :","").Trim(),
                            @"(?>\d\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match match = Regex.Match(applicant.Trim(), @"(?<name>.+)\sAddress\sof\sApplicant\s?:(?<adress>.+),\s?(?<code>.+?)\s");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(match.Groups["code"].Value.Trim())
                                });

                                if(MakeCountryCode(match.Groups["code"].Value.Trim()) == null)
                                {
                                    Console.WriteLine($"<=================== {match.Groups["code"].Value}");
                                }
                            }
                            else
                            {
                                Match match1 = Regex.Match(applicant.Trim(), @"(?<name>.+)\sName\sof\sApplicant.+");

                                if (match1.Success)
                                {
                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                    {
                                        Name = match1.Groups["name"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                    {
                                        Name = applicant.Trim()
                                    });
                                }
                            }
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        List<string> inventors = Regex.Split(inid.Replace("\r", "").Replace("\n", " ").Replace("(72)Name of Inventor :", "").Trim(),
                            @"(?>\d\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+)\sAddress\sof\sApplicant\s?:(?<adress>.+),\s?(?<code>.+?)\s");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = MakeCountryCode(match.Groups["code"].Value.Trim())
                                });

                                if (MakeCountryCode(match.Groups["code"].Value.Trim()) == null)
                                {
                                    Console.WriteLine($"<=================== {match.Groups["code"].Value}");
                                }
                            }
                            else
                            {
                                Match match1 = Regex.Match(inventor.Trim(), @"(?<name>.+)\sName\sof\sApplicant.+");

                                if (match1.Success)
                                {
                                    statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = match1.Groups["name"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Name = inventor.Trim()
                                    });
                                }
                            }
                        }
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        Match match = Regex.Match(inid.Replace("\r", "").Replace("\n", " ").Trim(), @"act\s?:(?<text>.+?)\s(?<note>No\..+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "EN",
                                Text = match.Groups["text"].Value.Trim()
                            });

                            Match match1 = Regex.Match(match.Groups["note"].Value.Trim(), @"(?<a1>No\..+?)\s?:\s?(?<p>\d+)\s(?<a2>No\..+)\s?:\s?(?<c>\d+)");

                            if (match1.Success)
                            {
                                statusEvent.LegalEvent.Note = "|| " + match1.Groups["a1"].Value.Trim() + " | " + match1.Groups["p"].Value.Trim() + " || " + match1.Groups["a2"].Value.Trim()
                                    + " | " + match1.Groups["c"].Value.Trim();
                            }
                            else Console.WriteLine($"{match.Groups["note"].Value.Trim()} --- note57");
                        }
                        else {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "EN",
                                Text = inid.Replace("\r", "").Replace("\n", " ").Replace("(57)","").Replace("Abstract :","").Trim()
                            });

                        } 
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        var ipcs = GetInternationalClassification(inid);
                        if (ipcs.Any())
                        {
                            statusEvent.Biblio.Ipcs = ipcs;
                        }
                    }

                }
            }
            else if(subCode == "10")
            {
                Match match = Regex.Match(note.Trim(), @"\d{1,3}?\s(?<city>\D.+?)\s\s?(?<aNum>\d.+)\s(?<eDate>\d{2}\/\d{2}\/\d{4}).+?(?<f74>[A-Z].+)\s(?<note>.+@.+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["aNum"].Value.Trim();

                    statusEvent.LegalEvent.Note = "|| LOCATION | " + match.Groups["city"].Value.Trim() + " || EMAIL | " + match.Groups["note"].Value.Trim();

                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["eDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    statusEvent.Biblio.Agents.Add(new Integration.PartyMember
                    {
                        Address1 = match.Groups["f74"].Value.Trim()
                    });
                }
                else
                {
                    Console.WriteLine($"{note}");
                }
            }

            return statusEvent;
        }

        private static List<Ipc> GetInternationalClassification(string inid)
        {
            inid = new Regex(@"\s").Replace(inid, "");
            var results = new List<Ipc>();
            try
            {
                var pattern = new Regex(@"(?<FirstPart>[A-Z]{1}\d+[A-Z]{1})(?<SecondPart>(?<FirstNumber>\d+)(\/(?<SecondNumber>\d+))*)"); //example: C07D0239420000, C12P0013000000, C22C 1/04, C22C 14/00
                var matches = pattern.Matches(inid).Where(x => !string.IsNullOrWhiteSpace(x.Value)).ToList();
                if (!matches.Any())
                {
                    return results.Any() ? results : null;
                }
                foreach (var match in matches)
                {
                    var firstPart = match.Groups["FirstPart"].Value;
                    var secondPart = match.Groups["SecondPart"].Value;
                    if (secondPart.Contains("/"))
                    {
                        var firstNumber = match.Groups["FirstNumber"].Value.TrimStart('0');
                        var secondNumber = match.Groups["SecondNumber"].Value.TrimEnd('0');
                        if (firstNumber.Length == 0)
                        {
                            firstNumber = "00";
                        }
                        if (secondNumber.Length == 0)
                        {
                            secondNumber = "00";
                        }
                        if (!string.IsNullOrWhiteSpace(firstNumber) && !string.IsNullOrWhiteSpace(secondNumber))
                        {
                            if (secondNumber.Length == 1)
                            {
                                secondNumber = $"{secondNumber}0";
                            }
                            secondPart = $"{firstNumber}/{secondNumber}";
                        }

                        if (!string.IsNullOrWhiteSpace(firstPart) && !string.IsNullOrWhiteSpace(secondPart))
                        {
                            results.Add(new Ipc
                            {
                                Class = $"{firstPart} {secondPart}"
                            });
                        }
                    }
                    else
                    {
                        var pat = new Regex(@"(?<FirstNumber>\d{4})(?<SecondNumber>\d+)");
                        var patMatch = pat.Match(secondPart);
                        if (!patMatch.Success)
                        {
                            continue;
                        }
                        var firstNumber = patMatch.Groups["FirstNumber"].Value.TrimStart('0');
                        var secondNumber = patMatch.Groups["SecondNumber"].Value.TrimEnd('0');
                        if (firstNumber.Length == 0)
                        {
                            firstNumber = "00";
                        }
                        if (secondNumber.Length == 0)
                        {
                            secondNumber = "00";
                        }
                        if (!string.IsNullOrWhiteSpace(firstNumber) && !string.IsNullOrWhiteSpace(secondNumber))
                        {
                            if (secondNumber.Length == 1)
                            {
                                secondNumber = $"{secondNumber}0";
                            }
                            secondPart = $"{firstNumber}/{secondNumber}";
                        }

                        if (!string.IsNullOrWhiteSpace(firstPart) && !string.IsNullOrWhiteSpace(secondPart))
                        {
                            results.Add(new Ipc
                            {
                                Class = $"{firstPart} {secondPart}"
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Getting (51) International classification failed. Ex:{e.Message}");
            }
            return results.Any() ? results : null;
        }

        internal string MakeCountryCode(string code) => code switch
        {
            "Tamilnadu" => "IN",
            "India." => "IN",
            "India" => "IN",
            "Haryana" => "IN",
            "Gurugram" => "IN",
            "Tirunelveli." => "IN",
            "INDIA" => "IN",
            "UTTAR" => "IN",
            "China" => "CN",
            "DELHI" => "IN",
            "INDIA." => "IN",
            "MAHARASHTRA." => "IN",
            "Dehradun-" => "IN",
            "CHHATTISGARH-" => "IN",
            "Uttarakhand." => "IN",
            "Gajraula" => "IN",
            "Amroha(U.P)" => "IN",
            "Maharashtra" => "IN",
            "Thiruvallur" => "IN",
            "Mathura" => "IN",
            "Jharkhand" => "IN",
            "TAMILNADU" => "IN",
            "Tuljapur" => "IN",
            "ANDHRA" => "IN",
            "Bangalore." => "IN",
            "Andhra" => "IN",
            "Uttar" => "IN",
            "Tamil" => "IN",
            "TAMIL" => "IN",
            "COIMBATORE" => "IN",
            "Coimbatore" => "IN",
            "KARNATAKA" => "IN",
            "Karnataka" => "IN",
            "Noida" => "IN",
            "Delhi" => "IN",
            "Indore" => "IN",
            "Kompall" => "IN",
            "Kompally" => "IN",
            "Bangalore" => "IN",
            "Hyderabad" => "IN",
            "Chennai" => "IN",
            "Bengaluru" => "IN",
            "Telangana" => "IN",
            "TELANGANA-" => "IN",
            "TAMILNADU-" => "IN",
            "Ireland" => "IE",
            "Japan" => "JP",
            "Tokyo" => "JP",
            "Osaka" => "JP",
            "Utah" => "US",
            "USA" => "US",
            "California" => "US",
            "Washington" => "US",
            "NC" => "US",
            "Massachusetts" => "US",
            "Florida" => "US",
            "Pennsylvania" => "US",
            "Ontario" => "CA",
            "Colorado" => "US",
            "Seoul" => "KR",
            "Switzerland" => "CH",
            "Saudi" => "SA",
            _ => null
        };
        internal List<string> MakeInids(string note, string subCode)
        {
            List<string> inids = new();

            if(subCode == "1")
            {
                Match match = Regex.Match(note.Trim(),
                    @"(?<inid12>\(12.+)\s(?<inid21>\(21.+)\(19.+(?<inid22>\(22.+)\s(?<inid43>\(43.+)\s(?<inid54>\(54.+)\s(?<inids51all>\(51.+)\s(?<inid71>\(71.+)\s(?<inid72>\(72.+)\s(?<inid57>\(57.+)",
                    RegexOptions.Singleline);

                if (match.Success)
                {
                    inids.Add(match.Groups["inid12"].Value.Trim());
                    inids.Add(match.Groups["inid21"].Value.Trim());
                    inids.Add(match.Groups["inid22"].Value.Trim());
                    inids.Add(match.Groups["inid43"].Value.Trim());
                    inids.Add(match.Groups["inid54"].Value.Trim());
                    inids.Add(match.Groups["inids51all"].Value.Trim());
                    inids.Add(match.Groups["inid71"].Value.Trim());
                    inids.Add(match.Groups["inid72"].Value.Trim());
                    inids.Add(match.Groups["inid57"].Value.Trim());
                }
                else Console.WriteLine(note);
            }
            return inids;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;
            if(subCode == "1")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + "\n";
                }

                return text;
            }
            else if (subCode == "10")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + " ";
                }

                return text.Replace("\r", "").Replace("\n", " ").Trim();
            }

            return null;
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
