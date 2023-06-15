﻿using Integration;
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
using static System.Net.Mime.MediaTypeNames;

namespace Diamond_MD_Maksim
{
    class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        private const string _i21 = "(21)";
        private const string _i13 = "(13)";
        private const string _i51 = "(51)";
        private const string _i96 = "(96)";
        private const string _i97 = "(97)";
        private const string _i87 = "(87)";
        private const string _i31 = "(31)";
        private const string _i32 = "(32)";
        private const string _i33 = "(33)";
        private const string _i71 = "(71)";
        private const string _i712 = "(712)";
        private const string _i72 = "(72)";
        private const string _i54 = "(54)";

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            var statusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directory = new DirectoryInfo(path);

            var files = new List<string>();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if(subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("GEPI publică fiecare solicitare de validare după informarea de către OEB privind achitarea taxei"))
                        .TakeWhile(val => !val.Value.StartsWith("FF4A Brevete de invenţie acordate /"))
                        .ToList();

                    if (xElements.Count == 0)
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("GEPI shall publish any request for validation after it has been informed by the EPO that the"))
                            .TakeWhile(val => !val.Value.StartsWith("FF4A Brevete de invenţie acordate /"))
                            .ToList();
                    }

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(21\))", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(21)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakeConvertedPatent(note, subCode, "BB2A"));
                    }
                }
            }
            return statusEvents;
        }

        internal string MakeText(List<XElement> xElements, string subCode)
        {
            var sb = new StringBuilder();

            if (subCode == "2")
            {
                foreach (var xElement in xElements)
                {
                    sb = sb.Append(xElement.Value + "\n");
                }
            }
            return sb.ToString();
        }

        internal Diamond.Core.Models.LegalStatusEvent MakeConvertedPatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatus = new()
            {
                SubCode = subCode,
                SectionCode = sectionCode,
                CountryCode = "MD",
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Id = _id++,
                Biblio = new Biblio()
            };

            var euroPatent = new EuropeanPatent();

            var priorityNumbers = new List<string>();
            var priorityDates = new List<string>();
            var priorityCountries = new List<string>();

            foreach (var inid in MakeInids(note))
            {
                if (inid.StartsWith(_i21))
                {
                    legalStatus.Biblio.Application.Number = inid.Replace(_i21, "").Trim();
                }
                else if (inid.StartsWith(_i13))
                {
                    legalStatus.Biblio.Publication.Kind = inid.Replace(_i13, "").Trim();
                }
                else if (inid.StartsWith(_i51))
                {
                    var inid51 = Regex.Replace(inid, @"(\(51\)\s?Int.\s?Cl.:)", "", RegexOptions.Singleline).Trim();
                    var inids = Regex.Split(inid51, @"(?<=\))", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var iinid in inids)
                    {
                        var match = Regex.Match(iinid.Trim(), @"(?<class>.+)\((?<date>.+)\)");
                        if (match.Success)
                        {
                            legalStatus.Biblio.Ipcs.Add(new Ipc
                            {
                                Class = match.Groups["class"].Value.Trim(),
                                Date = match.Groups["date"].Value.Replace(".", "/").Trim()
                            });
                        }
                        else 
                            Console.WriteLine($"{iinid} in 51");
                    }
                }
                else if (inid.StartsWith(_i96))
                {
                    var match = Regex.Match(inid.Replace(_i96, "").Trim(), @"(?<number>.+),\s(?<date>[0-9]{4}.[0-9]{2}.[0-9]{2})", RegexOptions.Singleline);

                    if (match.Success)
                    {
                        euroPatent.AppNumber = match.Groups["number"].Value.Trim();
                        euroPatent.AppDate = match.Groups["date"].Value.Replace(".", "/").Trim();
                    }
                    else 
                        Console.WriteLine($"{inid} in 96");
                }
                else if (inid.StartsWith(_i97))
                {
                    var match = Regex.Match(inid.Replace(_i97, "").Trim(), @"(?<number>.+),\s(?<date>[0-9]{4}.[0-9]{2}.[0-9]{2})", RegexOptions.Singleline);

                    if (match.Success)
                    {
                        euroPatent.PubNumber = match.Groups["number"].Value.Trim();
                        euroPatent.PubDate = match.Groups["date"].Value.Replace(".", "/").Trim();
                    }
                    else 
                        Console.WriteLine($"{inid} in 97");
                }
                else if (inid.StartsWith(_i87))
                {
                    var match = Regex.Match(inid.Replace(_i87, "").Trim(), @"(?<number>.+),\s(?<date>[0-9]{4}.[0-9]{2}.[0-9]{2})", RegexOptions.Singleline);

                    if (match.Success)
                    {
                        legalStatus.Biblio.IntConvention.PctPublNumber = match.Groups["number"].Value.Trim();
                        legalStatus.Biblio.IntConvention.PctPublDate = match.Groups["date"].Value.Replace(".", "/").Trim();
                    }
                    else 
                        Console.WriteLine($"{inid} in 87");
                }
                else if (inid.StartsWith(_i31))
                {
                    priorityNumbers = Regex.Split(inid.Replace(_i31, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();
                }
                else if (inid.StartsWith(_i32))
                {
                    priorityDates = Regex.Split(inid.Replace(_i32, "").Replace(".", "/").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();
                }
                else if (inid.StartsWith(_i33))
                {
                    priorityCountries = Regex.Split(inid.Replace(_i33, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();
                }
                else if (inid.StartsWith(_i71))
                {
                    var applicants = Regex.Split(inid.Replace(_i71, "").Trim(), ";", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var applicant in applicants)
                    {
                        var match = Regex.Match(applicant.Trim(), @"(?<name>.+)(?<country>[A-Z]{2})", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            legalStatus.Biblio.Applicants.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Replace("- ","").Trim().TrimEnd(','),
                                Country = match.Groups["country"].Value.Trim()
                            });
                        }
                        else 
                            Console.WriteLine($"{applicant} in 71");
                    }
                }
                else if (inid.StartsWith(_i72))
                {
                    var inventors = Regex.Split(inid.Replace(_i72, "").Trim(), ";", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var inventor in inventors)
                    {
                        var match = Regex.Match(inventor.Trim(), @"(?<name>.+)(?<country>[A-Z]{2})", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            legalStatus.Biblio.Inventors.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Replace("- ", "").Trim().TrimEnd(','),
                                Country = match.Groups["country"].Value.Trim()
                            });
                        }
                        else 
                            Console.WriteLine($"{inventor} in 72");
                    }
                }
                else if (inid.StartsWith(_i712))
                {
                    var inventorsApplicants = Regex.Split(inid.Replace(_i712, "").Trim(), ";", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var inventorsApplicant in inventorsApplicants)
                    {
                        var match = Regex.Match(inventorsApplicant.Trim(), @"(?<name>.+)(?<country>[A-Z]{2})", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            legalStatus.Biblio.Inventors.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Replace("- ", "").Trim().TrimEnd(','),
                                Country = match.Groups["country"].Value.Trim()
                            });
                            legalStatus.Biblio.Applicants.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Replace("- ", "").Trim().TrimEnd(','),
                                Country = match.Groups["country"].Value.Trim()
                            });
                        }
                        else
                            Console.WriteLine($"{inventorsApplicant} in 712");
                    }

                }
                else if (inid.StartsWith(_i54))
                {
                    var titleMatchList = Regex.Split(inid.Replace(_i54, "").Trim(), @"(?=\n[A-Z][a-z]|\n[А-Я][а-я])").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    if (titleMatchList.Count == 3)
                    {
                        for (int i = 0; i < titleMatchList.Count; i++)
                        {
                            if (i == 0)
                            {
                                legalStatus.Biblio.Titles.Add(new Title
                                {
                                    Language = "RO",
                                    Text = titleMatchList[i].Replace("\r","").Replace("\n", " ").Trim()
                                });
                            }
                            if (i == 1)
                            {
                                legalStatus.Biblio.Titles.Add(new Title
                                {
                                    Language = "EN",
                                    Text = titleMatchList[i].Replace("\r", "").Replace("\n", " ").Trim()
                                });
                            }
                            if (i == 2)
                            {
                                legalStatus.Biblio.Titles.Add(new Title
                                {
                                    Language = "RU",
                                    Text = titleMatchList[i].Replace("\r", "").Replace("\n", " ").Trim()
                                });
                            }
                        }
                    }
                    else
                    {
                        titleMatchList = Regex.Split(inid.Replace(_i54, "").Trim(), @"(?=\n[A-Z]{2,3}|\n[А-Я][а-я])")
                            .Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        if (titleMatchList.Count == 3)
                        {
                            for (int i = 0; i < titleMatchList.Count; i++)
                            {
                                if (i == 0)
                                {
                                    legalStatus.Biblio.Titles.Add(new Title
                                    {
                                        Language = "RO",
                                        Text = titleMatchList[i].Replace("\r", "").Replace("\n", " ").Trim()
                                    });
                                }

                                if (i == 1)
                                {
                                    legalStatus.Biblio.Titles.Add(new Title
                                    {
                                        Language = "EN",
                                        Text = titleMatchList[i].Replace("\r", "").Replace("\n", " ").Trim()
                                    });
                                }

                                if (i == 2)
                                {
                                    legalStatus.Biblio.Titles.Add(new Title
                                    {
                                        Language = "RU",
                                        Text = titleMatchList[i].Replace("\r", "").Replace("\n", " ").Trim()
                                    });
                                }
                            }
                        }
                        else
                            Console.WriteLine($"{inid} --- 54");
                    }
                }
                else 
                    Console.WriteLine($"{inid} don't processed");
            }

            for (var i = 0; i < priorityNumbers.Count; i++)
            {
                legalStatus.Biblio.Priorities.Add(new Priority
                {
                    Number = priorityNumbers[i].Trim(),
                    Date = priorityDates[i].Trim(),
                    Country = priorityCountries[i].Trim()
                });
            }

            legalStatus.Biblio.EuropeanPatents.Add(euroPatent);

            return legalStatus;
        }
        internal List<string> MakeInids(string note)
        {
            var inids = new List<string>();
            if (note.Contains("(71)(72)"))
            {
                var match = Regex.Match(note.Trim(), @"(?<part1>.+)\s(?<inid7172>\(71.+)\s(?<part2>\(\d.+)",
                    RegexOptions.Singleline);

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["part1"].Value.Trim(), @"(?=\([0-9]{2}\)\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();
                    inids.Add(match.Groups["inid7172"].Value.Replace("(71)(72)","(712)").Trim());
                    inids.Add(match.Groups["part2"].Value.Trim());
                }
                else
                    Console.WriteLine($"{note} --- not split");
            }
            else
            {
                inids = Regex.Split(note.Trim(), @"(?=\([0-9]{2}\)\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();
            }
            return inids;
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
