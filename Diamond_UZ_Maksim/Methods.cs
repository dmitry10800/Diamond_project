﻿using Newtonsoft.Json;
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
using DiamondProjectClasses;
using Integration;

namespace Diamond_UZ_Maksim
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

            List<XElement> xElements = null;

            foreach (string tetml in files)
            {
                CurrentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("Раздел А"))
                       .TakeWhile(val => !val.Value.StartsWith("Индекс МПК Номер заявки"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(13\)\s)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(13)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatentNewStyle(note, subCode, "BZ1A"));
                    }
                }
                else if (subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("1.2. 4A"))
                       .TakeWhile(val => !val.Value.StartsWith("FG A"))
                       .TakeWhile(val => !val.Value.StartsWith("FG4A"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(11\)\s[A-Z])").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatentNewStyle(note, subCode, "FG4A"));
                    }
                }
                else if (subCode == "4")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("I. FOYDАLI ODELLАR"))
                       .TakeWhile(val => !val.Value.StartsWith("2.2. 4K"))
                       .TakeWhile(val => !val.Value.StartsWith("2.2. G4K"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(11\)\s[A-Z])").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatentNewStyle(note, subCode, "FG4K"));
                    }
                }
            }
            return statusEvents;
        }
        internal string MakeText (List<XElement> xElement)
        {
            string text = "";

            foreach (XElement element in xElement)
            {
                text += element.Value + "\n";
            }

            return text;
        }

        //internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        //{
        //    Diamond.Core.Models.LegalStatusEvent statusEvent = new()
        //    {
        //        GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
        //        CountryCode = "UZ",
        //        SubCode = subCode,
        //        SectionCode = sectionCode,
        //        Id = Id++,
        //        LegalEvent = new(),
        //        Biblio = new()
        //    };

        //    CultureInfo culture = new("ru-RU");

        //    if (subCode == "1")
        //    {
        //        foreach (string inid in MakeInids(note, subCode))
        //        {
        //            if (inid.StartsWith("(13)"))
        //            {
        //                var tmpKind = inid.Replace("(13)", "")
        //                    .Replace("В", "B")
        //                    .Replace("А", "A")
        //                    .Replace("С", "C")
        //                    .Trim();
                        
        //                statusEvent.Biblio.Publication.Kind = tmpKind;
        //            }
        //            else if (inid.StartsWith("(21)"))
        //            {
        //                statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
        //            }
        //            else if (inid.StartsWith("(22)"))
        //            {
        //                statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //            }
        //            else if (inid.StartsWith("(71)") && !inid.StartsWith("(71)(72)(73)") && !inid.StartsWith("(71)(73)") && !inid.StartsWith("(71)(72)"))
        //            {
        //                List<string> applicantsAll = Regex.Split(inid.Replace("(71)", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string item in applicantsAll)
        //                {
        //                    List<string> applicants = Regex.Split(item.Trim(), @"(?<=,\s[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                    if (applicants.Count == 1)
        //                    {
        //                        Match match = Regex.Match(applicants[0].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

        //                        if (match.Success)
        //                        {
        //                            List<string> appls = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                            string country = match.Groups["country"].Value.Trim();

        //                            foreach (string appl in appls)
        //                            {
        //                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
        //                                {
        //                                    Name = appl.Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });
        //                            }
        //                        }
        //                        else Console.WriteLine($"{item} --- 71");
        //                    }
        //                    else
        //                    {
        //                        for (int i = 0; i < applicants.Count; i++)
        //                        {
        //                            Match match1 = Regex.Match(applicants[i].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");
        //                            Match match2 = Regex.Match(applicants[++i].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

        //                            if (match1.Success && match2.Success)
        //                            {
        //                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
        //                                {
        //                                    Name = match1.Groups["names"].Value.Trim(),
        //                                    Language = "UZ",
        //                                    Country = match1.Groups["country"].Value.Trim(),
        //                                    Translations = new List<Integration.Translation>
        //                                            {
        //                                                new Integration.Translation
        //                                                {
        //                                                    Type = "71",
        //                                                    Language = "RU",
        //                                                    TrName = match2.Groups["names"].Value.Trim()
        //                                                }
        //                                            }
        //                                });
        //                            }
        //                            else Console.WriteLine($"{applicants[i]}");
        //                        }
        //                    }
        //                }
        //            }
        //            else if (inid.StartsWith("(72)"))
        //            {
        //                List<string> inventorsAll = Regex.Split(inid.Replace("(72)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string item in inventorsAll)
        //                {
        //                    Match match = Regex.Match(item.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

        //                    if (match.Success)
        //                    {
        //                        string country = match.Groups["country"].Value.Trim();

        //                        if (country == "UZ" || country == "BY" || country == "RU")
        //                        {
        //                            List<string> inventorsUz = Regex.Split(match.Groups["names"].Value.Trim(), ",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                            foreach (string inventor in inventorsUz)
        //                            {
        //                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
        //                                {
        //                                    Name = inventor.Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });
        //                            }
        //                        }
        //                        else
        //                        {
        //                            List<string> inventorsOther = Regex.Split(match.Groups["names"].Value.Trim(), @"(.+?,.+?,)").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                            foreach (string inventor in inventorsOther)
        //                            {
        //                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
        //                                {
        //                                    Name = inventor.TrimEnd(',').Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });
        //                            }
        //                        }
        //                    }
        //                    else Console.WriteLine($"{item} --- 72");
        //                }
        //            }
        //            else if (inid.StartsWith("(71)(72)"))
        //            {
        //                List<string> people = Regex.Split(inid.Replace("(71)(72)", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string item in people)
        //                {
        //                    List<string> men = Regex.Split(item.Trim(), @"(?<=[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                    foreach (string man in men)
        //                    {
        //                        Match match = Regex.Match(man.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

        //                        if (match.Success)
        //                        {
        //                            string country = match.Groups["country"].Value.Trim();

        //                            List<string> appls = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                            foreach (string appl in appls)
        //                            {
        //                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
        //                                {
        //                                    Country = country,
        //                                    Name = appl.Trim(),
        //                                    Language = "RU"
        //                                });

        //                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
        //                                {
        //                                    Country = country,
        //                                    Name = appl.Trim(),
        //                                    Language = "RU"
        //                                });
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (inid.StartsWith("(51)"))
        //            {
        //                List<string> ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string ipc in ipcs)
        //                {
        //                    Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

        //                    if (match.Success)
        //                    {
        //                        statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
        //                        {
        //                            Class = match.Groups["class"].Value.Trim(),
        //                            Date = match.Groups["version"].Value.Trim()
        //                        });
        //                    }
        //                    else
        //                    {
        //                        statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
        //                        {
        //                            Class = ipc
        //                        });
        //                    }
        //                }
        //            }
        //            else if (inid.StartsWith("(54)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(54)", "").Trim(), @"(?<uz>.+?)\s(?<ru>[А-Я].+)");

        //                if (match.Success)
        //                {
        //                    statusEvent.Biblio.Titles.Add(new Integration.Title
        //                    {
        //                        Language = "UZ",
        //                        Text = match.Groups["uz"].Value.Trim()                                
        //                    });

        //                    statusEvent.Biblio.Titles.Add(new Integration.Title
        //                    {
        //                        Language = "RU",
        //                        Text = match.Groups["ru"].Value.Trim()
        //                    });
        //                }
        //                else Console.WriteLine($"{inid} - 54");
        //            }
        //            else if (inid.StartsWith("(57)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>.+)\s(?<ru>Использование:\s?.+)_");

        //                if (match.Success)
        //                {
        //                    statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
        //                    {
        //                        Language = "UZ",
        //                        Text = match.Groups["uz"].Value.Replace("_","").Trim()
        //                    });

        //                    statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
        //                    {
        //                        Language = "RU",
        //                        Text = match.Groups["ru"].Value.Replace("_", "").Trim()
        //                    });
        //                }
        //                else
        //                {
        //                    Match match1 = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>.+)\s(?<ru>Использование:\s?.+)");

        //                    if (match1.Success)
        //                    {
        //                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
        //                        {
        //                            Language = "UZ",
        //                            Text = match1.Groups["uz"].Value.Trim()
        //                        });

        //                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
        //                        {
        //                            Language = "RU",
        //                            Text = match1.Groups["ru"].Value.Trim()
        //                        });
        //                    }
        //                    else Console.WriteLine($"{inid} - 57");
        //                }
        //            }
        //            else if (inid.StartsWith("(31)(32)(33)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(31)(32)(33)", "").Trim(), @"(?<num>.+),?\s(?<date>\d{2}\.\d{2}\.\d{4}),?\s?(?<country>\D{2})");

        //                if (match.Success)
        //                {
        //                    statusEvent.Biblio.Priorities.Add(new Integration.Priority
        //                    {
        //                        Number = match.Groups["num"].Value.TrimEnd(',').Trim(),
        //                        Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
        //                        Country = match.Groups["country"].Value.Trim()
        //                    });
        //                }
        //                else Console.WriteLine($"{inid} --- 33");
        //            }
        //            else if (inid.StartsWith("(85)"))
        //            {
        //                statusEvent.Biblio.IntConvention.PctNationalDate = DateTime.Parse(inid.Replace("(85)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //            }
        //            else if (inid.StartsWith("(86)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<num>.+),\s(?<date>\d{2}\.\d{2}\.\d{4})");

        //                if (match.Success)
        //                {
        //                    statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //                    statusEvent.Biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.Trim();
        //                }
        //                else
        //                {
        //                    Match match1 = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<date>\d{2}\.\d{2}\.\d{4}),?\s(?<num>.+)");
        //                    if (match1.Success)
        //                    {
        //                        statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //                        statusEvent.Biblio.IntConvention.PctApplNumber = match1.Groups["num"].Value.Trim();
        //                    }
        //                    else Console.WriteLine($"{inid} --- 86");
        //                }                     
        //            }
        //            else if (inid.StartsWith("(87)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(87)", "").Trim(), @"(?<num>.+),\s(?<date>\d{2}\.\d{2}\.\d{4})");

        //                if (match.Success)
        //                {
        //                    statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //                    statusEvent.Biblio.IntConvention.PctPublNumber = match.Groups["num"].Value.Trim();
        //                }
        //                else
        //                {
        //                    Match match1 = Regex.Match(inid.Replace("(87)", "").Trim(), @"(?<date>\d{2}\.\d{2}\.\d{4}),?\s(?<num>.+)");
        //                    if (match1.Success)
        //                    {
        //                        statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //                        statusEvent.Biblio.IntConvention.PctPublNumber = match1.Groups["num"].Value.Trim();
        //                    }
        //                    else Console.WriteLine($"{inid} --- 87");
        //                }
        //            }
        //            else Console.WriteLine($"{inid} - not processing");
        //        }
        //    }
        //    else if (subCode == "3")
        //    {
        //        foreach (string inid in MakeInids(note, subCode))
        //        {
        //            if (inid.StartsWith("(11)"))
        //            {
        //                statusEvent.Biblio.Publication.Number = inid.Replace("(11)", "").Trim();
        //            }
        //            else if (inid.StartsWith("(13)"))
        //            {
        //                statusEvent.Biblio.Publication.Kind = inid.Replace("(13)", "").Trim();
        //            }
        //            else if (inid.StartsWith("(21)"))
        //            {
        //                statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
        //            }
        //            else if (inid.StartsWith("(22)"))
        //            {
        //                statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //            }
        //            else if (inid.StartsWith("(51)"))
        //            {
        //                List<string> ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string ipc in ipcs)
        //                {
        //                    Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

        //                    if (match.Success)
        //                    {
        //                        statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
        //                        {
        //                            Class = match.Groups["class"].Value.Trim(),
        //                            Date = match.Groups["version"].Value.Trim()
        //                        });
        //                    }
        //                    else
        //                    {
        //                        statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
        //                        {
        //                            Class = ipc
        //                        });
        //                    }
        //                }
        //            }
        //            else if (inid.StartsWith("(54)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(54)", "").Trim(), @"(?<uz>.+?)\s(?<ru>[А-Я].+)");

        //                if (match.Success)
        //                {
        //                    statusEvent.Biblio.Titles.Add(new Integration.Title
        //                    {
        //                        Language = "UZ",
        //                        Text = match.Groups["uz"].Value.Trim()
        //                    });

        //                    statusEvent.Biblio.Titles.Add(new Integration.Title
        //                    {
        //                        Language = "RU",
        //                        Text = match.Groups["ru"].Value.Trim()
        //                    });
        //                }
        //                else Console.WriteLine($"{inid} - 54");
        //            }
        //            else if (inid.StartsWith("(57)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>1\.\s.+?)_(?<ru>.+)");

        //                if (match.Success)
        //                {
        //                    List<string> claimsUz = Regex.Split(match.Groups["uz"].Value.Replace("_","").Trim(), @"(?>\d{1,2}\.\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();
        //                    List<string> claimsRu = Regex.Split(match.Groups["ru"].Value.Replace("_","").Trim(), @"(?>\d{1,2}\.\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                    int num = 1;
        //                    for (int i = 0; i < claimsUz.Count; i++)
        //                    {
        //                        statusEvent.Biblio.Claims.Add(new DiamondProjectClasses.Claim
        //                        {
        //                            Language = "UZ",
        //                            Text = claimsUz[i],
        //                            Number = num.ToString(),
        //                            Translations = new List<Integration.Translation>
        //                            {
        //                               new Integration.Translation
        //                               {
        //                                   Type = "57",
        //                                   Language = "RU",
        //                                   Tr = claimsRu[i]
        //                               }
        //                            }
        //                        });

        //                        num++;
        //                    }

        //                }
        //                else
        //                {
        //                    Match match1 = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>.+?)_(?<ru>.+)");

        //                    if (match1.Success)
        //                    {
        //                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
        //                        {
        //                            Language = "UZ",
        //                            Text = match1.Groups["uz"].Value.Replace("_","").Trim()
        //                        });

        //                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
        //                        {
        //                            Language = "RU",
        //                            Text = match1.Groups["ru"].Value.Replace("_", "").Trim()
        //                        });
        //                    }
        //                    else Console.WriteLine($"{inid}  ---- 57");
        //                }
        //            }
        //            else if (inid.StartsWith("(72)"))
        //            {
        //                List<string> inventorsAll = Regex.Split(inid.Replace("(72)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string item in inventorsAll)
        //                {
        //                    Match match = Regex.Match(item.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

        //                    if (match.Success)
        //                    {
        //                        string country = match.Groups["country"].Value.Trim();

        //                        List<string> inventors = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                        foreach (string inventor in inventors)
        //                        {
        //                            statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
        //                            {
        //                                Country = country,
        //                                Name = inventor.Trim(),
        //                                Language = "RU"
        //                            });
        //                        }
        //                    }
        //                    else Console.WriteLine($"{item}   ---- 72");
        //                }
        //            }
        //            else if (inid.StartsWith("(73)"))
        //            {
        //                List<string> assignessAll = Regex.Split(inid.Replace("(73)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                    foreach (string item in assignessAll)
        //                    {
        //                        List<string> assigness = Regex.Split(item.Trim(), @"(?<=[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                        foreach (string assigner in assigness)
        //                        {
        //                            Match match = Regex.Match(assigner, @"(?<names>.+),\s(?<country>[A-Z]{2})");

        //                            if (match.Success)
        //                            {
        //                                string country = match.Groups["country"].Value.Trim();

        //                                List<string> assigns = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                                foreach (string assig in assigns)
        //                                {
        //                                    statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
        //                                    {
        //                                        Country = country,
        //                                        Name = assig.Trim(),
        //                                        Language = "RU"
        //                                    });
        //                                }
        //                            }
        //                        }
        //                    }
                        
        //            }
        //            else if (inid.StartsWith("(71)") && !inid.StartsWith("(71)(72)(73)") && !inid.StartsWith("(71)(73)"))
        //            {
        //                List<string> applicantsAll = Regex.Split(inid.Replace("(71)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                if (applicantsAll != null)
        //                {
        //                    foreach (string item in applicantsAll)
        //                    {
        //                        List<string> applicants = Regex.Split(item.Trim(), @"(?<=[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                        foreach (string applicant in applicants)
        //                        {
        //                            Match match = Regex.Match(applicant.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

        //                            if (match.Success)
        //                            {
        //                                string country = match.Groups["country"].Value.Trim();

        //                                List<string> appls = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                                foreach (string appl in appls)
        //                                {
        //                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
        //                                    {
        //                                        Country = country,
        //                                        Name = appl.Trim(),
        //                                        Language = "RU"
        //                                    });
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (inid.StartsWith("(71)(73)"))
        //            {
        //                List<string> people = Regex.Split(inid.Replace("(71)(73)", "").Trim(), @"(?<=\s[A-Z]{2}\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string men in people)
        //                {
        //                    Match match = Regex.Match(men.Trim(), @"(?<name>.+),\s(?<country>[A-Z]{2})");

        //                    if (match.Success)
        //                    {
        //                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
        //                        {
        //                            Language = "RU",
        //                            Country = match.Groups["country"].Value.Trim(),
        //                            Name = match.Groups["name"].Value.Trim()
        //                        });

        //                        statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
        //                        {
        //                            Language = "RU",
        //                            Country = match.Groups["country"].Value.Trim(),
        //                            Name = match.Groups["name"].Value.Trim()
        //                        });
        //                    }
        //                    else Console.WriteLine($"{men} --- 71/73");
        //                }
        //            }
        //            else if (inid.StartsWith("(71)(72)(73)"))
        //            {
        //                List<string> people = Regex.Split(inid.Replace("(71)(72)(73)", "").Trim(), @"(?<=\s[A-Z]{2}\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string men in people)
        //                {
        //                    Match match = Regex.Match(men.Trim(), @"(?<name>.+),\s(?<country>[A-Z]{2})");

        //                    if (match.Success)
        //                    {
        //                        string country = match.Groups["country"].Value.Trim();

        //                        List<string> man = Regex.Split(match.Groups["name"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                        foreach (string item in man)
        //                        {
        //                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
        //                            {
        //                                Language = "RU",
        //                                Country = country,
        //                                Name = item
        //                            });

        //                            statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
        //                            {
        //                                Language = "RU",
        //                                Country = country,
        //                                Name = item
        //                            });

        //                            statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
        //                            {
        //                                Language = "RU",
        //                                Country = country,
        //                                Name = item
        //                            });
        //                        }                              
        //                    }
        //                    else Console.WriteLine($"{men} --- 71/73");
        //                }
        //            }
        //            else if (inid.StartsWith("(85)"))
        //            {
        //                statusEvent.Biblio.IntConvention.PctNationalDate = DateTime.Parse(inid.Replace("(85)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //            }
        //            else if (inid.StartsWith("(86)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<num>.+),\s(?<date>\d{2}\.\d{2}\.\d{4})");

        //                if (match.Success)
        //                {
        //                    statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //                    statusEvent.Biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.Trim();
        //                }
        //                else
        //                {
        //                    Match match1 = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<date>\d{2}\.\d{2}\.\d{4}),?\s(?<num>.+)");
        //                    if (match1.Success)
        //                    {
        //                        statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //                        statusEvent.Biblio.IntConvention.PctApplNumber = match1.Groups["num"].Value.Trim();
        //                    }
        //                    else Console.WriteLine($"{inid} --- 86");
        //                }
        //            }
        //            else if (inid.StartsWith("(87)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(87)", "").Trim(), @"(?<num>.+),\s(?<date>\d{2}\.\d{2}\.\d{4})");

        //                if (match.Success)
        //                {
        //                    statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //                    statusEvent.Biblio.IntConvention.PctPublNumber = match.Groups["num"].Value.Trim();
        //                }
        //                else
        //                {
        //                    Match match1 = Regex.Match(inid.Replace("(87)", "").Trim(), @"(?<date>\d{2}\.\d{2}\.\d{4}),?\s(?<num>.+)");
        //                    if (match1.Success)
        //                    {
        //                        statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //                        statusEvent.Biblio.IntConvention.PctPublNumber = match1.Groups["num"].Value.Trim();
        //                    }
        //                    else Console.WriteLine($"{inid} --- 87");
        //                }
        //            }
        //            else if (inid.StartsWith("(31)(32)(33)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(31)(32)(33)", "").Trim(), @"(?<num>.+),?\s(?<date>\d{2}\.\d{2}\.\d{4}),?\s?(?<country>\D{2})");

        //                if (match.Success)
        //                {
        //                    statusEvent.Biblio.Priorities.Add(new Integration.Priority
        //                    {
        //                        Number = match.Groups["num"].Value.TrimEnd(',').Trim(),
        //                        Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
        //                        Country = match.Groups["country"].Value.Trim()
        //                    });
        //                }
        //                else Console.WriteLine($"{inid} --- 33");
        //            }
        //            else Console.WriteLine($"{inid} - not processing");
        //        }
        //    }
        //    else if (subCode == "4")
        //    {
        //        foreach (string inid in MakeInids(note, subCode))
        //        {
        //            if (inid.StartsWith("(11)"))
        //            {
        //                statusEvent.Biblio.Publication.Number = inid.Replace("(11)", "").Trim();
        //            }
        //            else if (inid.StartsWith("(13)"))
        //            {
        //                statusEvent.Biblio.Publication.Kind = inid.Replace("(13)", "").Trim();
        //            }
        //            else if (inid.StartsWith("(21)"))
        //            {
        //                statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
        //            }
        //            else if (inid.StartsWith("(22)"))
        //            {
        //                statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
        //            }
        //            else if (inid.StartsWith("(51)"))
        //            {
        //                List<string> ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string ipc in ipcs)
        //                {
        //                    Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

        //                    if (match.Success)
        //                    {
        //                        statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
        //                        {
        //                            Class = match.Groups["class"].Value.Trim(),
        //                            Date = match.Groups["version"].Value.Trim()
        //                        });
        //                    }
        //                    else
        //                    {
        //                        statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
        //                        {
        //                            Class = ipc
        //                        });
        //                    }
        //                }
        //            }
        //            else if (inid.StartsWith("(54)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(54)", "").Trim(), @"(?<uz>.+?)\s(?<ru>[А-Я].+)");

        //                if (match.Success)
        //                {
        //                    statusEvent.Biblio.Titles.Add(new Integration.Title
        //                    {
        //                        Language = "UZ",
        //                        Text = match.Groups["uz"].Value.Trim()
        //                    });

        //                    statusEvent.Biblio.Titles.Add(new Integration.Title
        //                    {
        //                        Language = "RU",
        //                        Text = match.Groups["ru"].Value.Trim()
        //                    });
        //                }
        //                else Console.WriteLine($"{inid} - 54");
        //            }
        //            else if (inid.StartsWith("(57)"))
        //            {
        //                Match match = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>.+)\s(?<ru>Использование:\s?.+)_");

        //                if (match.Success)
        //                {
        //                    statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
        //                    {
        //                        Language = "UZ",
        //                        Text = match.Groups["uz"].Value.Replace("_", "").Trim()
        //                    });

        //                    statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
        //                    {
        //                        Language = "RU",
        //                        Text = match.Groups["ru"].Value.Replace("_", "").Trim()
        //                    });
        //                }
        //                else
        //                {
        //                    Match match1 = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>.+)\s(?<ru>Использование:\s?.+)");

        //                    if (match1.Success)
        //                    {
        //                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
        //                        {
        //                            Language = "UZ",
        //                            Text = match1.Groups["uz"].Value.Trim()
        //                        });

        //                        statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
        //                        {
        //                            Language = "RU",
        //                            Text = match1.Groups["ru"].Value.Trim()
        //                        });
        //                    }
        //                    else Console.WriteLine($"{inid} - 57");
        //                }
        //            }
        //            else if (inid.StartsWith("(71)(73)"))
        //            {
        //                List<string> applicantsAll = Regex.Split(inid.Replace("(71)(73)", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string item in applicantsAll)
        //                {
        //                    List<string> applicants = Regex.Split(item.Trim(), @"(?<=,\s[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                    if (applicants.Count == 1)
        //                    {
        //                        Match match = Regex.Match(applicants[0].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

        //                        if (match.Success)
        //                        {
        //                            List<string> appls = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                            string country = match.Groups["country"].Value.Trim();

        //                            foreach (string appl in appls)
        //                            {
        //                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
        //                                {
        //                                    Name = appl.Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });

        //                                statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
        //                                {
        //                                    Name = appl.Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });
        //                            }
        //                        }
        //                        else Console.WriteLine($"{item} --- 71");
        //                    }
        //                    else
        //                    {
        //                        for (int i = 0; i < applicants.Count; i++)
        //                        {
        //                            try
        //                            {
        //                                Match match1 = Regex.Match(applicants[i].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");
        //                                Match match2 = Regex.Match(applicants[++i].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

        //                                if (match1.Success && match2.Success)
        //                                {
        //                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
        //                                    {
        //                                        Name = match1.Groups["names"].Value.Trim(),
        //                                        Language = "UZ",
        //                                        Country = match1.Groups["country"].Value.Trim(),
        //                                        Translations = new List<Integration.Translation>
        //                                            {
        //                                                new Integration.Translation
        //                                                {
        //                                                    Type = "71",
        //                                                    Language = "RU",
        //                                                    TrName = match2.Groups["names"].Value.Trim()
        //                                                }
        //                                            }
        //                                    });

        //                                    statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
        //                                    {
        //                                        Name = match1.Groups["names"].Value.Trim(),
        //                                        Language = "UZ",
        //                                        Country = match1.Groups["country"].Value.Trim(),
        //                                        Translations = new List<Integration.Translation>
        //                                            {
        //                                                new Integration.Translation
        //                                                {
        //                                                    Type = "73",
        //                                                    Language = "RU",
        //                                                    TrName = match2.Groups["names"].Value.Trim()
        //                                                }
        //                                            }
        //                                    });
        //                                }
        //                                else
        //                                    Console.WriteLine($"{applicants[i]}");
        //                            }
        //                            catch (Exception e)
        //                            {
        //                                //todo : fix the issue with Exception and create a proper handler
        //                                Console.WriteLine($"EXCEPTION: {applicants[i]}");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (inid.StartsWith("(72)"))
        //            {
        //                List<string> inventorsAll = Regex.Split(inid.Replace("(72)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string item in inventorsAll)
        //                {
        //                    Match match = Regex.Match(item.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

        //                    if (match.Success)
        //                    {
        //                        string country = match.Groups["country"].Value.Trim();

        //                        if (country == "UZ" || country == "BY" || country == "RU")
        //                        {
        //                            List<string> inventorsUz = Regex.Split(match.Groups["names"].Value.Trim(), ",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                            foreach (string inventor in inventorsUz)
        //                            {
        //                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
        //                                {
        //                                    Name = inventor.Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });
        //                            }
        //                        }
        //                        else
        //                        {
        //                            List<string> inventorsOther = Regex.Split(match.Groups["names"].Value.Trim(), @"(.+?,.+?,)").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                            foreach (string inventor in inventorsOther)
        //                            {
        //                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
        //                                {
        //                                    Name = inventor.TrimEnd(',').Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });
        //                            }
        //                        }
        //                    }
        //                    else Console.WriteLine($"{item} --- 72");
        //                }
        //            }
        //            else if (inid.StartsWith("(71)(72)(73)"))
        //            {
        //                List<string> inventorsAll = Regex.Split(inid.Replace("(71)(72)(73)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                foreach (string item in inventorsAll)
        //                {
        //                    Match match = Regex.Match(item.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

        //                    if (match.Success)
        //                    {
        //                        string country = match.Groups["country"].Value.Trim();

        //                        if (country == "UZ" || country == "BY" || country == "RU")
        //                        {
        //                            List<string> inventorsUz = Regex.Split(match.Groups["names"].Value.Trim(), ",").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                            foreach (string inventor in inventorsUz)
        //                            {
        //                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
        //                                {
        //                                    Name = inventor.Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });

        //                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
        //                                {
        //                                    Name = inventor.Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });

        //                                statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
        //                                {
        //                                    Name = inventor.Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });
        //                            }
        //                        }
        //                        else
        //                        {
        //                            List<string> inventorsOther = Regex.Split(match.Groups["names"].Value.Trim(), @"(.+?,.+?,)").Where(val => !string.IsNullOrEmpty(val)).ToList();

        //                            foreach (string inventor in inventorsOther)
        //                            {
        //                                statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
        //                                {
        //                                    Name = inventor.TrimEnd(',').Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });

        //                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
        //                                {
        //                                    Name = inventor.TrimEnd(',').Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });

        //                                statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
        //                                {
        //                                    Name = inventor.TrimEnd(',').Trim(),
        //                                    Language = "RU",
        //                                    Country = country
        //                                });
        //                            }
        //                        }
        //                    }
        //                    else Console.WriteLine($"{item} --- 72");
        //                }
        //            }
        //            else Console.WriteLine($"{inid} -- not processed");
        //        }
        //    }

        //    return statusEvent;
        //}
        internal Diamond.Core.Models.LegalStatusEvent MakePatentNewStyle(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatus = new()
            {
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                CountryCode = "UZ",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                LegalEvent = new(),
                Biblio = new()
            };

            CultureInfo culture = new("ru-RU");

            if (subCode is "1")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(13)"))
                    {
                        legalStatus.Biblio.Publication.Kind = inid.Replace("(13)", "").Trim() switch
                        {
                            "А" => "A",
                            "В" => "B",
                            "С" => "C"
                        };
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        legalStatus.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        legalStatus.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture)
                            .ToString("yyyy.MM.dd").Replace(".","/").Trim();
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @",")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<ed>.+)\)");

                            if (match.Success)
                            {
                                legalStatus.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["ed"].Value.Trim()
                                });
                            }
                            else
                            {
                                legalStatus.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = ipc.Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(71)") && !inid.StartsWith("(71)(72)"))
                    {
                        List<string> applicantsAll = Regex.Split(inid.Replace("(71)", "").Trim(), @"(?<=,\s[A-Z]{2}\s)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        List<string> applicantsEn = new();
                        List<string> applicantsRu = new();

                        for (int i = 0; i < applicantsAll.Count; i++)
                        {
                            if (i % 2 == 0)
                            {
                                applicantsEn.Add(applicantsAll[i]);
                            }
                            else applicantsRu.Add(applicantsAll[i]);
                        }

                        for (int i = 0; i < applicantsRu.Count; i++)
                        {
                            Match match = Regex.Match(applicantsRu[i], @"(?<name>.+),\s(?<code>[A-Z]{2})");
                            Match match1 = Regex.Match(applicantsEn[i], @"(?<name>.+),\s(?<code>[A-Z]{2})");

                            if (match.Success && match1.Success)
                            {
                                legalStatus.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Language = "RU",
                                    Country = match.Groups["code"].Value.Trim(),
                                    Translations = new List<Translation>()
                                    {
                                        new()
                                        {
                                            TrName = match1.Groups["name"].Value.Trim(),
                                            Language = "EN",
                                            Type = "71"
                                        }
                                    }
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        List<string> inventorsAll = Regex.Split(inid.Replace("(72)", "").Trim(), @"(?<=,\s[A-Z]{2}\s)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        List<string> inventorsAllRu = new();
                        List<string> inventorsAllEn = new();
                        
                        for (int i = 0; i < inventorsAll.Count; i++)
                        {
                            inventorsAllEn.Add(inventorsAll[i]);
                            inventorsAllRu.Add(inventorsAll[i + inventorsAll.Count/2]);
                            i++;
                        }

                        List<string> inventorsEn = new();
                        List<string> inventorsRu = new();

                        for (int i = 0; i < inventorsAllEn.Count; i++)
                        {
                            inventorsEn = Regex.Split(inventorsAllEn[i], @";").Where(val => !string.IsNullOrEmpty(val)).ToList();
                            inventorsRu = Regex.Split(inventorsAllRu[i], @";").Where(val => !string.IsNullOrEmpty(val)).ToList();
                        }

                        for (int i = 0; i < inventorsRu.Count; i++)
                        {
                            Match match = Regex.Match(inventorsRu[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");
                            Match matchEn = Regex.Match(inventorsEn[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");

                            if (match.Success && matchEn.Success)
                            {
                                if (match.Groups["code"].Value.Trim() == "UZ")
                                {
                                    List<string> inventorsNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @",")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> inventorsNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @",")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < inventorsNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Inventors.Add(new PartyMember()
                                        {
                                            Name = inventorsNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new()
                                                {
                                                    TrName = inventorsNamesEn[j],
                                                    Language = "EN",
                                                    Type = "72"
                                                }
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    List<string> inventorsNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @"(?=\,\s[А-Я]{2})")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> inventorsNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @"(?=\,\s[A-Z]{2})")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < inventorsNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Inventors.Add(new PartyMember()
                                        {
                                            Name = inventorsNamesRu[j].Trim().TrimStart(',').Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new()
                                                {
                                                    TrName = inventorsNamesEn[j].Trim().TrimStart(',').Trim(),
                                                    Language = "EN",
                                                    Type = "72"
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                            else Console.WriteLine($"{inid} ----------72 field");
                        }
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        Match match = Regex.Match(inid.Replace("(54)", "").Trim(), @"(?<en>.+)\s(?<ru>[А-Я].+)");

                        if (match.Success)
                        {
                            legalStatus.Biblio.Titles.Add(new Title()
                            {
                                Language = "RU",
                                Text = match.Groups["ru"].Value.Trim(),
                            });

                            legalStatus.Biblio.Titles.Add(new Title()
                            {
                                Language = "UZ",
                                Text = match.Groups["en"].Value.Trim(),
                            });
                        }
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        Match match = Regex.Match(inid.Replace("(57)", "").Trim(),
                            @"(?<en>.+\.).+\s(?<ru>Использование:.+)");

                        if (match.Success)
                        {
                            legalStatus.Biblio.Abstracts.Add(new Abstract()
                            {
                                Language = "RU",
                                Text = match.Groups["ru"].Value.Replace("_","").Trim()
                            });

                            legalStatus.Biblio.Abstracts.Add(new Abstract()
                            {
                                Language = "UZ",
                                Text = match.Groups["en"].Value.Replace("_","").Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} -- 57");
                    }
                    else if (inid.StartsWith("(85)"))
                    {
                        legalStatus.Biblio.IntConvention.PctNationalDate = DateTime.Parse(inid.Replace("(85)", "").Trim(), culture)
                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(86)"))
                    {
                        Match match = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<num>.+),?\s(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            legalStatus.Biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            legalStatus.Biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.TrimEnd(',').Trim();
                        }
                        else
                        {
                            Match match2 = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4}),?\s(?<num>.+)");

                            if (match2.Success)
                            {
                                legalStatus.Biblio.IntConvention.PctApplDate = DateTime.Parse(match2.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                                legalStatus.Biblio.IntConvention.PctApplNumber = match2.Groups["num"].Value.TrimEnd(',').Trim();
                            }
                            else Console.WriteLine($"{inid} -- 86");
                        }
                    }
                    else if (inid.StartsWith("(87)"))
                    {
                        Match match = Regex.Match(inid.Replace("(87)", "").Trim(), @"(?<num>.+),?\s(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            legalStatus.Biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            legalStatus.Biblio.IntConvention.PctPublNumber = match.Groups["num"].Value.TrimEnd(',').Trim();
                        }
                        else
                        {
                            Match match2 = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4}),?\s(?<num>.+)");

                            if (match2.Success)
                            {
                                legalStatus.Biblio.IntConvention.PctPublDate = DateTime.Parse(match2.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                                legalStatus.Biblio.IntConvention.PctPublNumber = match2.Groups["num"].Value.TrimEnd(',').Trim();
                            }
                            else Console.WriteLine($"{inid} -- 87");
                        }
                    }
                    else if (inid.StartsWith("(31)(32)(33)"))
                    {
                        List<string> priorities = Regex
                            .Split(inid.Replace("(31)(32)(33)", "").Trim(), @"(?<=\s[A-Z]{2})")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priority in priorities)
                        {
                            Match match = Regex.Match(priority.Trim(), @"(?<num>.+),\s(?<date>.+),\s(?<code>\D{2})");

                            if (match.Success)
                            {
                                legalStatus.Biblio.Priorities.Add(new Priority
                                {
                                    Number = match.Groups["num"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(71)(72)"))
                    {
                        List<string> people = Regex.Split(inid.Replace("(71)(72)", "").Trim(), @"(?<=,\s[A-Z]{2}\s)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        List<string> peopleRu = new();
                        List<string> peopleEn = new();

                        for (int i = 0; i < people.Count; i++)
                        {
                            peopleEn.Add(people[i]);
                            peopleRu.Add(people[i + people.Count / 2]);
                            i++;
                        }

                        for (int i = 0; i < peopleRu.Count; i++)
                        {
                            Match match = Regex.Match(peopleRu[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");
                            Match matchEn = Regex.Match(peopleEn[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");

                            if (match.Success && matchEn.Success)
                            {
                                if (match.Groups["code"].Value.Trim() == "UZ")
                                {
                                    List<string> peopleNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @",")
                                    .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> peopleNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @",")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < peopleNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Inventors.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "72"
                                                }
                                            }
                                        });

                                        legalStatus.Biblio.Applicants.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new ()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "71"
                                                }
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    List<string> peopleNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                    .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> peopleNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < peopleNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Inventors.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new ()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "72"
                                                }
                                            }
                                        });

                                        legalStatus.Biblio.Applicants.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new Translation()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "71"
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                            else Console.WriteLine($"{inid} ----------71/72 field");
                        }
                    }
                    else Console.WriteLine($"{inid}");
                }
            }
            else if (subCode is "3" or "4")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(11)"))
                    {
                        legalStatus.Biblio.Publication.Number = inid.Replace("(11)", "").Trim();
                    }
                    else if (inid.StartsWith("(13)"))
                    {
                        legalStatus.Biblio.Publication.Kind = inid.Replace("(13)", "").Trim();
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @",")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<ed>.+)\)");

                            if (match.Success)
                            {
                                legalStatus.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["ed"].Value.Trim()
                                });
                            }
                            else
                            {
                                legalStatus.Biblio.Ipcs.Add(new Ipc()
                                {
                                    Class = ipc.Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        legalStatus.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        Match match = Regex.Match(inid.Replace("(22)", "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");
                        if (match.Success)
                        {
                            legalStatus.Biblio.Application.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        List<string> inventorsAll = Regex.Split(inid.Replace("(72)", "").Trim(), @"(?<=,\s[A-Z]{2}\s)")
                           .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        List<string> inventorsRu = new();
                        List<string> inventorsEn = new();

                        for (int i = 0; i < inventorsAll.Count; i++)
                        {
                            inventorsEn.Add(inventorsAll[i]);
                            inventorsRu.Add(inventorsAll[i + inventorsAll.Count / 2]);
                            i++;
                        }

                        for (int i = 0; i < inventorsRu.Count; i++)
                        {
                            Match match = Regex.Match(inventorsRu[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");
                            Match matchEn = Regex.Match(inventorsEn[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");

                            if (match.Success && matchEn.Success)
                            {
                                if (match.Groups["code"].Value.Trim() == "UZ")
                                {
                                    List<string> inventorsNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @",")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> inventorsNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @",")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < inventorsNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Inventors.Add(new PartyMember()
                                        {
                                            Name = inventorsNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new()
                                                {
                                                    TrName = inventorsNamesEn[j],
                                                    Language = "EN",
                                                    Type = "72"
                                                }
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    List<string> inventorsNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> inventorsNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < inventorsNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Inventors.Add(new PartyMember()
                                        {
                                            Name = inventorsNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new Translation()
                                                {
                                                    TrName = inventorsNamesEn[j],
                                                    TrAddress1 = matchEn.Groups["code"].Value.Trim(),
                                                    Language = "EN",
                                                    Type = "INID"
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                            else Console.WriteLine($"{inid} ----------72 field");
                        }
                    }
                    else if (inid.StartsWith("(73)"))
                    {
                        List<string> assignessAll = Regex.Split(inid.Replace("(73)", "").Trim(), @"(?<=,\s[A-Z]{2}\s)")
                           .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        List<string> assignessRu = new();
                        List<string> assignessEn = new();
                        
                        for (int i = 0; i < assignessAll.Count; i++)
                        {
                            assignessEn.Add(assignessAll[i]);
                            assignessRu.Add(assignessAll[++i]);
                        }

                        for (int i = 0; i < assignessRu.Count; i++)
                        {
                            Match match = Regex.Match(assignessRu[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");
                            Match matchEn = Regex.Match(assignessEn[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");

                            if (match.Success && matchEn.Success)
                            {
                                if (match.Groups["code"].Value.Trim() == "UZ")
                                {
                                    List<string> assignesNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @",")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> assignesNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @",")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < assignesNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Assignees.Add(new PartyMember()
                                        {
                                            Name = assignesNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new ()
                                                {
                                                    TrName = assignesNamesEn[j],
                                                    Language = "EN",
                                                    Type = "73"
                                                }
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    List<string> assignesNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> assignesNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < assignesNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Assignees.Add(new PartyMember()
                                        {
                                            Name = assignesNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new ()
                                                {
                                                    TrName = assignesNamesEn[j],
                                                    Language = "EN",
                                                    Type = "73"
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                            else Console.WriteLine($"{inid} ----------73 field");
                        }
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        Match match = Regex.Match(inid.Replace("(54)", "").Trim(), @"(?<en>.+)\s(?<ru>[А-Я].+)");

                        if (match.Success)
                        {
                            legalStatus.Biblio.Titles.Add(new Title()
                            {
                                Language = "RU",
                                Text = match.Groups["ru"].Value.Trim(),
                            });

                            legalStatus.Biblio.Titles.Add(new Title()
                            {
                                Language = "UZ",
                                Text = match.Groups["en"].Value.Trim(),
                            });
                        }
                    }
                    else if (inid.StartsWith("(71)(73)"))
                    {
                        List<string> people = Regex.Split(inid.Replace("(71)(73)", "").Trim(), @"(?<=,\s[A-Z]{2}\s)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        List<string> peopleRu = new();
                        List<string> peopleEn = new();

                        for (int i = 0; i < people.Count; i++)
                        {
                            peopleEn.Add(people[i]);
                            peopleRu.Add(people[++i]);
                        }

                        for (int i = 0; i < peopleRu.Count; i++)
                        {
                            Match match = Regex.Match(peopleRu[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");
                            Match matchEn = Regex.Match(peopleEn[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");

                            if (match.Success && matchEn.Success)
                            {
                                if (match.Groups["code"].Value.Trim() == "UZ")
                                {
                                    List<string> peopleNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @",")
                                    .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> peopleNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @",")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < peopleNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Assignees.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new ()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "73"
                                                }
                                            }
                                        });

                                        legalStatus.Biblio.Applicants.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new ()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "71"
                                                }
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    List<string> peopleNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                    .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> peopleNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < peopleNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Assignees.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new Translation()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "73"
                                                }
                                            }
                                        });

                                        legalStatus.Biblio.Applicants.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new Translation()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "71"
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                            else Console.WriteLine($"{inid} ----------71/73 field");
                        }
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        Match match = Regex.Match(inid.Replace("(57)", "").Trim(),
                            @"(?<en>.+?\.?)\s(?<ru>1?\.?\s?[А-Я].+)");

                        if (match.Success)
                        {
                            if (match.Groups["en"].Value.Trim().StartsWith("1."))
                            {
                                List<string> claimUz = Regex.Split(match.Groups["en"].Value.Trim(), @"(?=\d{1,2}\.\s)")
                                    .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                List<string> claimRu = Regex.Split(match.Groups["ru"].Value.Trim(), @"(?=\d{1,2}\.\s)")
                                    .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                int number = 1;

                                for (int i = 0; i < claimUz.Count; i++)
                                {
                                    Match matchCl = Regex.Match(claimUz[i].Trim(), @"\d{1,2}\.\s(?<text>.+)");
                                    Match matchClRu = Regex.Match(claimRu[i].Trim(), @"\d{1,2}\.\s(?<text>.+)");
                                    legalStatus.Biblio.Claims.Add(new Claim()
                                    {
                                        Number = number.ToString(),
                                        Text = matchCl.Groups["text"].Value.Trim().Replace("_",""),
                                        Language = "UZ",
                                        Translations = new(){new Translation()
                                        {
                                            Type = "INID",
                                            Language = "RU",
                                            Tr = matchClRu.Groups["text"].Value.Trim().Replace("_", "")
                                        }}
                                    });
                                }
                            }
                            else
                            {
                                legalStatus.Biblio.Abstracts.Add(new Abstract()
                                {
                                    Language = "RU",
                                    Text = match.Groups["ru"].Value.Replace("_", "").Trim()
                                });

                                legalStatus.Biblio.Abstracts.Add(new Abstract()
                                {
                                    Language = "UZ",
                                    Text = match.Groups["en"].Value.Replace("_", "").Trim()
                                });
                            }
                        }
                        else Console.WriteLine($"{inid} -- 57");
                    }
                    else if (inid.StartsWith("(71)(72)") && !inid.Contains("(71)(72)(73)"))
                    {

                        List<string> people = Regex.Split(inid.Replace("(71)(72)", "").Trim(), @"(?<=,\s[A-Z]{2}\s)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        List<string> peopleRu = new();
                        List<string> peopleEn = new();

                        for (int i = 0; i < people.Count; i++)
                        {
                            peopleEn.Add(people[i]);
                            peopleRu.Add(people[i + people.Count / 2]);
                            i++;
                        }

                        for (int i = 0; i < peopleRu.Count; i++)
                        {
                            Match match = Regex.Match(peopleRu[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");
                            Match matchEn = Regex.Match(peopleEn[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");

                            if (match.Success && matchEn.Success)
                            {
                                if (match.Groups["code"].Value.Trim() == "UZ")
                                {
                                    List<string> peopleNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @",")
                                    .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> peopleNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @",")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < peopleNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Inventors.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new Translation()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "72"
                                                }
                                            }
                                        });

                                        legalStatus.Biblio.Applicants.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new Translation()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "71"
                                                }
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    List<string> peopleNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                    .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    List<string> peopleNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    for (int j = 0; j < peopleNamesRu.Count; j++)
                                    {
                                        legalStatus.Biblio.Inventors.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new Translation()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "72"
                                                }
                                            }
                                        });

                                        legalStatus.Biblio.Applicants.Add(new PartyMember()
                                        {
                                            Name = peopleNamesRu[j].Trim(),
                                            Country = match.Groups["code"].Value.Trim(),
                                            Language = "RU",
                                            Translations = new()
                                            {
                                                new Translation()
                                                {
                                                    TrName = peopleNamesEn[j],
                                                    Language = "EN",
                                                    Type = "71"
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                            else Console.WriteLine($"{inid} ----------71/72 field");
                        }
                    }
                    else if (inid.Contains("triple"))
                    {
                        if (inid.Contains("(31)(32)(33)"))
                        {
                            List<string> priorities = Regex
                                .Split(inid.Replace("triple (31)(32)(33)", "").Trim(), @"(?<=\s[A-Z]{2})")
                                .Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string priority in priorities)
                            {
                                Match match = Regex.Match(priority.Trim(), @"(?<num>.+),\s(?<date>.+),\s(?<code>\D{2})");

                                if (match.Success)
                                {
                                    legalStatus.Biblio.Priorities.Add(new Priority
                                    {
                                        Number = match.Groups["num"].Value.Trim(),
                                        Country = match.Groups["code"].Value.Trim(),
                                        Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                    });
                                }
                            }
                        }
                        else
                        {
                            List<string> people = Regex.Split(inid.Replace("triple (71)(72)(73)", "").Trim(), @"(?<=,\s[A-Z]{2}\s)")
                          .Where(val => !string.IsNullOrEmpty(val)).ToList();

                            List<string> peopleRu = new();
                            List<string> peopleEn = new();

                            for (int i = 0; i < people.Count; i++)
                            {
                                peopleEn.Add(people[i]);
                                peopleRu.Add(people[i + people.Count / 2]);
                                i++;
                            }

                            for (int i = 0; i < peopleRu.Count; i++)
                            {
                                Match match = Regex.Match(peopleRu[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");
                                Match matchEn = Regex.Match(peopleEn[i].Trim(), @"(?<names>.+),\s(?<code>[A-Z]{2})");

                                if (match.Success && matchEn.Success)
                                {
                                    if (match.Groups["code"].Value.Trim() == "UZ")
                                    {
                                        List<string> peopleNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @",")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                        List<string> peopleNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @",")
                                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                        for (int j = 0; j < peopleNamesRu.Count; j++)
                                        {
                                            legalStatus.Biblio.Inventors.Add(new PartyMember()
                                            {
                                                Name = peopleNamesRu[j].Trim(),
                                                Country = match.Groups["code"].Value.Trim(),
                                                Language = "RU",
                                                Translations = new()
                                                {
                                                    new Translation()
                                                    {
                                                        TrName = peopleNamesEn[j],
                                                        Language = "EN",
                                                        Type = "72"
                                                    }
                                                }
                                            });

                                            legalStatus.Biblio.Applicants.Add(new PartyMember()
                                            {
                                                Name = peopleNamesRu[j].Trim(),
                                                Country = match.Groups["code"].Value.Trim(),
                                                Language = "RU",
                                                Translations = new()
                                                {
                                                    new Translation()
                                                    {
                                                        TrName = peopleNamesEn[j],
                                                        Language = "EN",
                                                        Type = "71"
                                                    }
                                                }
                                            });

                                            legalStatus.Biblio.Assignees.Add(new PartyMember()
                                            {
                                                Name = peopleNamesRu[j].Trim(),
                                                Country = match.Groups["code"].Value.Trim(),
                                                Language = "RU",
                                                Translations = new()
                                                {
                                                    new Translation()
                                                    {
                                                        TrName = peopleNamesEn[j],
                                                        Language = "EN",
                                                        Type = "73"
                                                    }
                                                }
                                            });
                                        }
                                    }
                                    else
                                    {
                                        List<string> peopleNamesRu = Regex.Split(match.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                        List<string> peopleNamesEn = Regex.Split(matchEn.Groups["names"].Value.Trim(), @"(\D+?,\s\D+?),")
                                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                        for (int j = 0; j < peopleNamesRu.Count; j++)
                                        {
                                            legalStatus.Biblio.Inventors.Add(new PartyMember()
                                            {
                                                Name = peopleNamesRu[j].Trim(),
                                                Country = match.Groups["code"].Value.Trim(),
                                                Language = "RU",
                                                Translations = new()
                                                {
                                                    new Translation()
                                                    {
                                                        TrName = peopleNamesEn[j],
                                                        Language = "EN",
                                                        Type = "72"
                                                    }
                                                }
                                            });

                                            legalStatus.Biblio.Applicants.Add(new PartyMember()
                                            {
                                                Name = peopleNamesRu[j].Trim(),
                                                Country = match.Groups["code"].Value.Trim(),
                                                Language = "RU",
                                                Translations = new()
                                                {
                                                    new Translation()
                                                    {
                                                        TrName = peopleNamesEn[j],
                                                        Language = "EN",
                                                        Type = "71"
                                                    }
                                                }
                                            });

                                            legalStatus.Biblio.Assignees.Add(new PartyMember()
                                            {
                                                Name = peopleNamesRu[j].Trim(),
                                                Country = match.Groups["code"].Value.Trim(),
                                                Language = "RU",
                                                Translations = new()
                                                {
                                                    new Translation()
                                                    {
                                                        TrName = peopleNamesEn[j],
                                                        Language = "EN",
                                                        Type = "73"
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                                else Console.WriteLine($"{inid} ----------71/72/73 field");
                            }
                        }
                    }
                    else if (inid.StartsWith("(85)"))
                    {
                        legalStatus.Biblio.IntConvention.PctNationalDate = DateTime.Parse(inid.Replace("(85)", "").Trim(), culture)
                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(86)"))
                    {
                        Match match = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<num>.+)\s(?<date>\d{2}\.\d{2}\.\d{4})");

                        if (match.Success)
                        {
                            legalStatus.Biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            legalStatus.Biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.TrimEnd(',').Trim();
                        }
                        else
                        {
                            Match match2 = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4}),?\s(?<num>.+)");

                            if (match2.Success)
                            {
                                legalStatus.Biblio.IntConvention.PctApplDate = DateTime.Parse(match2.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                                legalStatus.Biblio.IntConvention.PctApplNumber = match2.Groups["num"].Value.TrimEnd(',').Trim();
                            }
                            else Console.WriteLine($"{inid} -- 86");
                        }
                    }
                    else if (inid.StartsWith("(87)"))
                    {
                        Match match = Regex.Match(inid.Replace("(87)", "").Trim(), @"(?<num>.+)\s(?<date>\d{2}\.\d{2}\.\d{4})");

                        if (match.Success)
                        {
                            legalStatus.Biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            legalStatus.Biblio.IntConvention.PctPublNumber = match.Groups["num"].Value.TrimEnd(',').Trim();
                        }
                        else
                        {
                            Match match2 = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4}),?\s(?<num>.+)");

                            if (match2.Success)
                            {
                                legalStatus.Biblio.IntConvention.PctPublDate = DateTime.Parse(match2.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                                legalStatus.Biblio.IntConvention.PctPublNumber = match2.Groups["num"].Value.TrimEnd(',').Trim();
                            }
                            else Console.WriteLine($"{inid} -- 87");
                        }
                    }
                    else if (inid.StartsWith("(31)(32)(33)"))
                    {
                        List<string> priorities = Regex
                            .Split(inid.Replace("(31)(32)(33)", "").Trim(), @"(?<=\s[A-Z]{2})")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priority in priorities)
                        {
                            Match match = Regex.Match(priority.Trim(), @"(?<num>.+),\s(?<date>.+),\s(?<code>\D{2})");

                            if (match.Success)
                            {
                                legalStatus.Biblio.Priorities.Add(new Priority
                                {
                                    Number = match.Groups["num"].Value.Trim(),
                                    Country = match.Groups["code"].Value.Trim(),
                                    Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(63)"))
                    {
                        Match match = Regex.Match(inid.Replace("(63)", ""), @"(?<num>.+),\s(?<date>.+)");

                        if (match.Success)
                        {
                            legalStatus.Biblio.Related.Add(new RelatedDocument()
                            {
                                Number = match.Groups["num"].Value.Trim(),
                                Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                            });
                        }
                        else Console.WriteLine($"{inid}");
                    }
                    else Console.WriteLine($"{inid}");
                }
            }

            return legalStatus;
        }
        internal List<string> MakeInids (string note, string subCode)
        {
            List<string> inids = new();

            if (subCode == "1")
            {
                Match starts = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<start>.+?)\s(?<tripleInid>\(\d{2}\)\(\d{2}\)\(.+?)\s(?<doubleInid>\(\d{2}\)\(\d{2}\)\s.+)\s(?<cont>\(\d{2}\)\s.+)\s(?<inid57>\(57\).+)");

                if (starts.Success)
                {
                    inids = Regex
                        .Split(starts.Groups["start"].Value.Trim() + " " + starts.Groups["cont"].Value.Trim(),
                            @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    List<string> tripleInids = Regex
                        .Split(starts.Groups["tripleInid"].Value.Trim(), @"(?=\(\d{2}\)\(\d{2}\)\(\d{2}\).+)")
                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string inid in tripleInids)
                    {
                        inids.Add(inid);
                    }

                    inids.Add(starts.Groups["doubleInid"].Value.Trim());

                    inids.Add(starts.Groups["inid57"].Value.Trim());

                    return inids;
                }
                else
                {

                    Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                        @"(?<start>.+?)\s(?<tripleInid>\(\d{2}\)\(\d{2}\)\(.+?)\s(?<cont>\(\d{2}\)\s.+)\s(?<inid57>\(57\).+)");

                    if (match.Success)
                    {
                        inids = Regex
                            .Split(match.Groups["start"].Value.Trim() + " " + match.Groups["cont"].Value.Trim(),
                                @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        List<string> tripleInids = Regex
                            .Split(match.Groups["tripleInid"].Value.Trim(), @"(?=\(\d{2}\)\(\d{2}\)\(\d{2}\).+)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inid in tripleInids)
                        {
                            inids.Add(inid);
                        }

                        inids.Add(match.Groups["inid57"].Value.Trim());

                        return inids;
                    }
                    else
                    {
                        Match match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                            @"(?<start>.+?)\s(?<doubleInid>\(\d{2}\)\(\d{2}\).+?)\s(?<cont>\(\d{2}\)\s.+)\s(?<inid57>\(57\).+)");

                        if (match1.Success)
                        {
                            inids = Regex
                                .Split(match1.Groups["start"].Value.Trim() + " " + match1.Groups["cont"].Value.Trim(),
                                    @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            inids.Add(match1.Groups["doubleInid"].Value.Trim());

                            inids.Add(match1.Groups["inid57"].Value.Trim());

                            return inids;
                        }
                        else
                        {
                            Match match2 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                                @"(?<start>.+)\s(?<inid57>\(57\).+)");

                            if (match2.Success)
                            {
                                inids = Regex.Split(match2.Groups["start"].Value.Trim(), @"(?=\(\d{2}\)\s.+)")
                                    .Where(val => !string.IsNullOrEmpty(val)).ToList();

                                inids.Add(match2.Groups["inid57"].Value.Trim());

                                return inids;
                            }
                            else Console.WriteLine($"{note} - don't procces with inids");
                        }
                    }
                }
            }
            else if (subCode == "3")
            {
                Match matchFirst = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<start>.+)\s(?<tripleInid>\(\d{2}\)\(\d{2}\)\(.+)\s(?<doubleInid>\(\d{2}\)\(\d{2}\).+?)\s(?<cont>\(\d{2}.+)\s(?<inid57>\(57\).+)");
                if (matchFirst.Success)
                {
                    inids = Regex.Split(matchFirst.Groups["start"].Value.Trim() + " " + matchFirst.Groups["cont"].Value.Trim(), @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    inids.Add("triple " + matchFirst.Groups["tripleInid"].Value.Trim());

                    inids.Add(matchFirst.Groups["doubleInid"].Value.Trim());

                    inids.Add(matchFirst.Groups["inid57"].Value.Trim());

                    return inids;
                }
                else
                {
                    Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<start>.+)\s(?<tripleInid>\(\d{2}\)\(\d{2}\)\(.+?)\s(?<cont>\(\d{2}\).+)\s(?<inid57>\(57\).+)");

                    if (match.Success)
                    {
                        inids = Regex.Split(match.Groups["start"].Value.Trim() + " " + match.Groups["cont"].Value.Trim(), @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        inids.Add("triple " + match.Groups["tripleInid"].Value.Trim());

                        inids.Add(match.Groups["inid57"].Value.Trim());

                        return inids;
                    }
                    else
                    {
                        Match match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<start>.+)\s(?<doubleInid>\(\d{2}\)\(.+?)\s(?<cont>\(\d{2}\).+)\s(?<inid57>\(57\).+)");

                        if (match1.Success)
                        {
                            inids = Regex.Split(match1.Groups["start"].Value.Trim() + " " + match1.Groups["cont"].Value.Trim(), @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            inids.Add(match1.Groups["doubleInid"].Value.Trim());

                            inids.Add(match1.Groups["inid57"].Value.Trim());

                            return inids;
                        }
                        else
                        {
                            Match match2 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<start>.+)\s(?<inid57>\(57\).+)");

                            if (match2.Success)
                            {
                                inids = Regex.Split(match2.Groups["start"].Value.Trim(), @"(?=\(\d{2}\)\s.+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                inids.Add(match2.Groups["inid57"].Value.Trim());

                                return inids;
                            }
                            else Console.WriteLine($"{ note} - don't procces with inids");
                        }
                    }
                }
            }
            else if (subCode == "4")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<start>.+)\s(?<triple>\(\d{2}\)\(\d{2}\)\(\d{2}\).+?)\s(?<cont>\(\d{2}\).+)\s(?<inid57>\(57\).+)");

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["start"].Value.Trim() + " " + match.Groups["cont"].Value.Trim(), @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    inids.Add("triple " + match.Groups["triple"].Value.Trim());

                    inids.Add(match.Groups["inid57"].Value.Trim());

                    return inids;
                }
                else
                {
                   Match match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<start>.+)\s(?<double>\(\d{2}\)\(\d{2}\).+?)\s(?<cont>\(\d{2}\).+)\s(?<inid57>\(57\).+)");

                    if (match1.Success)
                    {
                        inids = Regex.Split(match1.Groups["start"].Value.Trim() + " " + match1.Groups["cont"].Value.Trim(), @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        inids.Add(match1.Groups["double"].Value.Trim());

                        inids.Add(match1.Groups["inid57"].Value.Trim());

                        return inids;
                    }
                    else
                    {
                        Match match2 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<start>.+)\s(?<inid57>\(57\).+)");

                        if (match2.Success)
                        {
                            inids = Regex.Split(match2.Groups["start"].Value.Trim(), @"(?=\(\d{2}\)\s.+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            inids.Add(match2.Groups["inid57"].Value.Trim());

                            return inids;
                        }
                        else Console.WriteLine($"{ note} - don't procces with inids");
                    }
                }
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
