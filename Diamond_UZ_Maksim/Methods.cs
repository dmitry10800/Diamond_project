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
                       .SkipWhile(val => !val.Value.StartsWith("I. ИХТИРОЛАР"))
                       .TakeWhile(val => !val.Value.StartsWith("1.1. Z1A"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(13\)\s[А-Я])").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(13)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "BZ1A"));
                    }
                }
                else if (subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("1.2. FG4A"))
                       .TakeWhile(val => !val.Value.StartsWith("FG4A"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(11\)\s)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG4A"));
                    }
                }
                else if (subCode == "4")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("I. ФОЙДАЛ МОДЕЛЛАР"))
                       .TakeWhile(val => !val.Value.StartsWith("2.2. G4K"))
                       .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(11\)\s[A-Z])").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG4K"));
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

        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
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

            if (subCode == "1")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(13)"))
                    {
                        var tmpKind = inid.Replace("(13)", "")
                            .Replace("В", "B")
                            .Replace("А", "A")
                            .Replace("С", "C")
                            .Trim();
                        
                        statusEvent.Biblio.Publication.Kind = tmpKind;
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(71)") && !inid.StartsWith("(71)(72)(73)") && !inid.StartsWith("(71)(73)") && !inid.StartsWith("(71)(72)"))
                    {
                        List<string> applicantsAll = Regex.Split(inid.Replace("(71)", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string item in applicantsAll)
                        {
                            List<string> applicants = Regex.Split(item.Trim(), @"(?<=,\s[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            if (applicants.Count == 1)
                            {
                                Match match = Regex.Match(applicants[0].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

                                if (match.Success)
                                {
                                    List<string> appls = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    string country = match.Groups["country"].Value.Trim();

                                    foreach (string appl in appls)
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = appl.Trim(),
                                            Language = "RU",
                                            Country = country
                                        });
                                    }
                                }
                                else Console.WriteLine($"{item} --- 71");
                            }
                            else
                            {
                                for (int i = 0; i < applicants.Count; i++)
                                {
                                    Match match1 = Regex.Match(applicants[i].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");
                                    Match match2 = Regex.Match(applicants[++i].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

                                    if (match1.Success && match2.Success)
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = match1.Groups["names"].Value.Trim(),
                                            Language = "UZ",
                                            Country = match1.Groups["country"].Value.Trim(),
                                            Translations = new List<Integration.Translation>
                                                    {
                                                        new Integration.Translation
                                                        {
                                                            Type = "71",
                                                            Language = "RU",
                                                            TrName = match2.Groups["names"].Value.Trim()
                                                        }
                                                    }
                                        });
                                    }
                                    else Console.WriteLine($"{applicants[i]}");
                                }
                            }
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        List<string> inventorsAll = Regex.Split(inid.Replace("(72)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string item in inventorsAll)
                        {
                            Match match = Regex.Match(item.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                string country = match.Groups["country"].Value.Trim();

                                if (country == "UZ" || country == "BY" || country == "RU")
                                {
                                    List<string> inventorsUz = Regex.Split(match.Groups["names"].Value.Trim(), ",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    foreach (string inventor in inventorsUz)
                                    {
                                        statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                        {
                                            Name = inventor.Trim(),
                                            Language = "RU",
                                            Country = country
                                        });
                                    }
                                }
                                else
                                {
                                    List<string> inventorsOther = Regex.Split(match.Groups["names"].Value.Trim(), @"(.+?,.+?,)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    foreach (string inventor in inventorsOther)
                                    {
                                        statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                        {
                                            Name = inventor.TrimEnd(',').Trim(),
                                            Language = "RU",
                                            Country = country
                                        });
                                    }
                                }
                            }
                            else Console.WriteLine($"{item} --- 72");
                        }
                    }
                    else if (inid.StartsWith("(71)(72)"))
                    {
                        List<string> people = Regex.Split(inid.Replace("(71)(72)", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string item in people)
                        {
                            List<string> men = Regex.Split(item.Trim(), @"(?<=[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string man in men)
                            {
                                Match match = Regex.Match(man.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

                                if (match.Success)
                                {
                                    string country = match.Groups["country"].Value.Trim();

                                    List<string> appls = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    foreach (string appl in appls)
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Country = country,
                                            Name = appl.Trim(),
                                            Language = "RU"
                                        });

                                        statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                        {
                                            Country = country,
                                            Name = appl.Trim(),
                                            Language = "RU"
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["version"].Value.Trim()
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                {
                                    Class = ipc
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        Match match = Regex.Match(inid.Replace("(54)", "").Trim(), @"(?<uz>.+?)\s(?<ru>[А-Я].+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "UZ",
                                Text = match.Groups["uz"].Value.Trim()                                
                            });

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "RU",
                                Text = match.Groups["ru"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} - 54");
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        Match match = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>.+)\s(?<ru>Использование:\s?.+)_");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "UZ",
                                Text = match.Groups["uz"].Value.Replace("_","").Trim()
                            });

                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "RU",
                                Text = match.Groups["ru"].Value.Replace("_", "").Trim()
                            });
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>.+)\s(?<ru>Использование:\s?.+)");

                            if (match1.Success)
                            {
                                statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                                {
                                    Language = "UZ",
                                    Text = match1.Groups["uz"].Value.Trim()
                                });

                                statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                                {
                                    Language = "RU",
                                    Text = match1.Groups["ru"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inid} - 57");
                        }
                    }
                    else if (inid.StartsWith("(31)(32)(33)"))
                    {
                        Match match = Regex.Match(inid.Replace("(31)(32)(33)", "").Trim(), @"(?<num>.+),?\s(?<date>\d{2}\.\d{2}\.\d{4}),?\s?(?<country>\D{2})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Priorities.Add(new Integration.Priority
                            {
                                Number = match.Groups["num"].Value.TrimEnd(',').Trim(),
                                Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
                                Country = match.Groups["country"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 33");
                    }
                    else if (inid.StartsWith("(85)"))
                    {
                        statusEvent.Biblio.IntConvention.PctNationalDate = DateTime.Parse(inid.Replace("(85)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(86)"))
                    {
                        Match match = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<num>.+),\s(?<date>\d{2}\.\d{2}\.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            statusEvent.Biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<date>\d{2}\.\d{2}\.\d{4}),?\s(?<num>.+)");
                            if (match1.Success)
                            {
                                statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                statusEvent.Biblio.IntConvention.PctApplNumber = match1.Groups["num"].Value.Trim();
                            }
                            else Console.WriteLine($"{inid} --- 86");
                        }                     
                    }
                    else if (inid.StartsWith("(87)"))
                    {
                        Match match = Regex.Match(inid.Replace("(87)", "").Trim(), @"(?<num>.+),\s(?<date>\d{2}\.\d{2}\.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            statusEvent.Biblio.IntConvention.PctPublNumber = match.Groups["num"].Value.Trim();
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("(87)", "").Trim(), @"(?<date>\d{2}\.\d{2}\.\d{4}),?\s(?<num>.+)");
                            if (match1.Success)
                            {
                                statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                statusEvent.Biblio.IntConvention.PctPublNumber = match1.Groups["num"].Value.Trim();
                            }
                            else Console.WriteLine($"{inid} --- 87");
                        }
                    }
                    else Console.WriteLine($"{inid} - not processing");
                }
            }
            else if (subCode == "3")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(11)"))
                    {
                        statusEvent.Biblio.Publication.Number = inid.Replace("(11)", "").Trim();
                    }
                    else if (inid.StartsWith("(13)"))
                    {
                        statusEvent.Biblio.Publication.Kind = inid.Replace("(13)", "").Trim();
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["version"].Value.Trim()
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                {
                                    Class = ipc
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        Match match = Regex.Match(inid.Replace("(54)", "").Trim(), @"(?<uz>.+?)\s(?<ru>[А-Я].+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "UZ",
                                Text = match.Groups["uz"].Value.Trim()
                            });

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "RU",
                                Text = match.Groups["ru"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} - 54");
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        Match match = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>1\.\s.+?)_(?<ru>.+)");

                        if (match.Success)
                        {
                            List<string> claimsUz = Regex.Split(match.Groups["uz"].Value.Replace("_","").Trim(), @"(?>\d{1,2}\.\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();
                            List<string> claimsRu = Regex.Split(match.Groups["ru"].Value.Replace("_","").Trim(), @"(?>\d{1,2}\.\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            int num = 1;
                            for (int i = 0; i < claimsUz.Count; i++)
                            {
                                statusEvent.Biblio.Claims.Add(new DiamondProjectClasses.Claim
                                {
                                    Language = "UZ",
                                    Text = claimsUz[i],
                                    Number = num.ToString(),
                                    Translations = new List<Integration.Translation>
                                    {
                                       new Integration.Translation
                                       {
                                           Type = "57",
                                           Language = "RU",
                                           Tr = claimsRu[i]
                                       }
                                    }
                                });

                                num++;
                            }

                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>.+?)_(?<ru>.+)");

                            if (match1.Success)
                            {
                                statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                                {
                                    Language = "UZ",
                                    Text = match1.Groups["uz"].Value.Replace("_","").Trim()
                                });

                                statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                                {
                                    Language = "RU",
                                    Text = match1.Groups["ru"].Value.Replace("_", "").Trim()
                                });
                            }
                            else Console.WriteLine($"{inid}  ---- 57");
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        List<string> inventorsAll = Regex.Split(inid.Replace("(72)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string item in inventorsAll)
                        {
                            Match match = Regex.Match(item.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                string country = match.Groups["country"].Value.Trim();

                                List<string> inventors = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (string inventor in inventors)
                                {
                                    statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Country = country,
                                        Name = inventor.Trim(),
                                        Language = "RU"
                                    });
                                }
                            }
                            else Console.WriteLine($"{item}   ---- 72");
                        }
                    }
                    else if (inid.StartsWith("(73)"))
                    {
                        List<string> assignessAll = Regex.Split(inid.Replace("(73)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string item in assignessAll)
                            {
                                List<string> assigness = Regex.Split(item.Trim(), @"(?<=[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (string assigner in assigness)
                                {
                                    Match match = Regex.Match(assigner, @"(?<names>.+),\s(?<country>[A-Z]{2})");

                                    if (match.Success)
                                    {
                                        string country = match.Groups["country"].Value.Trim();

                                        List<string> assigns = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                        foreach (string assig in assigns)
                                        {
                                            statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                                            {
                                                Country = country,
                                                Name = assig.Trim(),
                                                Language = "RU"
                                            });
                                        }
                                    }
                                }
                            }
                        
                    }
                    else if (inid.StartsWith("(71)") && !inid.StartsWith("(71)(72)(73)") && !inid.StartsWith("(71)(73)"))
                    {
                        List<string> applicantsAll = Regex.Split(inid.Replace("(71)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        if (applicantsAll != null)
                        {
                            foreach (string item in applicantsAll)
                            {
                                List<string> applicants = Regex.Split(item.Trim(), @"(?<=[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (string applicant in applicants)
                                {
                                    Match match = Regex.Match(applicant.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

                                    if (match.Success)
                                    {
                                        string country = match.Groups["country"].Value.Trim();

                                        List<string> appls = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                        foreach (string appl in appls)
                                        {
                                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                            {
                                                Country = country,
                                                Name = appl.Trim(),
                                                Language = "RU"
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (inid.StartsWith("(71)(73)"))
                    {
                        List<string> people = Regex.Split(inid.Replace("(71)(73)", "").Trim(), @"(?<=\s[A-Z]{2}\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string men in people)
                        {
                            Match match = Regex.Match(men.Trim(), @"(?<name>.+),\s(?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Language = "RU",
                                    Country = match.Groups["country"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim()
                                });

                                statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                                {
                                    Language = "RU",
                                    Country = match.Groups["country"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{men} --- 71/73");
                        }
                    }
                    else if (inid.StartsWith("(71)(72)(73)"))
                    {
                        List<string> people = Regex.Split(inid.Replace("(71)(72)(73)", "").Trim(), @"(?<=\s[A-Z]{2}\s)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string men in people)
                        {
                            Match match = Regex.Match(men.Trim(), @"(?<name>.+),\s(?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                string country = match.Groups["country"].Value.Trim();

                                List<string> man = Regex.Split(match.Groups["name"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                foreach (string item in man)
                                {
                                    statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                    {
                                        Language = "RU",
                                        Country = country,
                                        Name = item
                                    });

                                    statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                                    {
                                        Language = "RU",
                                        Country = country,
                                        Name = item
                                    });

                                    statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                    {
                                        Language = "RU",
                                        Country = country,
                                        Name = item
                                    });
                                }                              
                            }
                            else Console.WriteLine($"{men} --- 71/73");
                        }
                    }
                    else if (inid.StartsWith("(85)"))
                    {
                        statusEvent.Biblio.IntConvention.PctNationalDate = DateTime.Parse(inid.Replace("(85)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(86)"))
                    {
                        Match match = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<num>.+),\s(?<date>\d{2}\.\d{2}\.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            statusEvent.Biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("(86)", "").Trim(), @"(?<date>\d{2}\.\d{2}\.\d{4}),?\s(?<num>.+)");
                            if (match1.Success)
                            {
                                statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                statusEvent.Biblio.IntConvention.PctApplNumber = match1.Groups["num"].Value.Trim();
                            }
                            else Console.WriteLine($"{inid} --- 86");
                        }
                    }
                    else if (inid.StartsWith("(87)"))
                    {
                        Match match = Regex.Match(inid.Replace("(87)", "").Trim(), @"(?<num>.+),\s(?<date>\d{2}\.\d{2}\.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            statusEvent.Biblio.IntConvention.PctPublNumber = match.Groups["num"].Value.Trim();
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("(87)", "").Trim(), @"(?<date>\d{2}\.\d{2}\.\d{4}),?\s(?<num>.+)");
                            if (match1.Success)
                            {
                                statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                statusEvent.Biblio.IntConvention.PctPublNumber = match1.Groups["num"].Value.Trim();
                            }
                            else Console.WriteLine($"{inid} --- 87");
                        }
                    }
                    else if (inid.StartsWith("(31)(32)(33)"))
                    {
                        Match match = Regex.Match(inid.Replace("(31)(32)(33)", "").Trim(), @"(?<num>.+),?\s(?<date>\d{2}\.\d{2}\.\d{4}),?\s?(?<country>\D{2})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Priorities.Add(new Integration.Priority
                            {
                                Number = match.Groups["num"].Value.TrimEnd(',').Trim(),
                                Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
                                Country = match.Groups["country"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} --- 33");
                    }
                    else Console.WriteLine($"{inid} - not processing");
                }
            }
            else if (subCode == "4")
            {
                foreach (string inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith("(11)"))
                    {
                        statusEvent.Biblio.Publication.Number = inid.Replace("(11)", "").Trim();
                    }
                    else if (inid.StartsWith("(13)"))
                    {
                        statusEvent.Biblio.Publication.Kind = inid.Replace("(13)", "").Trim();
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s\((?<version>[0-9]{4}.[0-9]{2})\)?");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                {
                                    Class = match.Groups["class"].Value.Trim(),
                                    Date = match.Groups["version"].Value.Trim()
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                                {
                                    Class = ipc
                                });
                            }
                        }
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        Match match = Regex.Match(inid.Replace("(54)", "").Trim(), @"(?<uz>.+?)\s(?<ru>[А-Я].+)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "UZ",
                                Text = match.Groups["uz"].Value.Trim()
                            });

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "RU",
                                Text = match.Groups["ru"].Value.Trim()
                            });
                        }
                        else Console.WriteLine($"{inid} - 54");
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        Match match = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>.+)\s(?<ru>Использование:\s?.+)_");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "UZ",
                                Text = match.Groups["uz"].Value.Replace("_", "").Trim()
                            });

                            statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Language = "RU",
                                Text = match.Groups["ru"].Value.Replace("_", "").Trim()
                            });
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("(57)", "").Trim(), @"(?<uz>.+)\s(?<ru>Использование:\s?.+)");

                            if (match1.Success)
                            {
                                statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                                {
                                    Language = "UZ",
                                    Text = match1.Groups["uz"].Value.Trim()
                                });

                                statusEvent.Biblio.Abstracts.Add(new Integration.Abstract
                                {
                                    Language = "RU",
                                    Text = match1.Groups["ru"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inid} - 57");
                        }
                    }
                    else if (inid.StartsWith("(71)(73)"))
                    {
                        List<string> applicantsAll = Regex.Split(inid.Replace("(71)(73)", "").Trim(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string item in applicantsAll)
                        {
                            List<string> applicants = Regex.Split(item.Trim(), @"(?<=,\s[A-Z]{2})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            if (applicants.Count == 1)
                            {
                                Match match = Regex.Match(applicants[0].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

                                if (match.Success)
                                {
                                    List<string> appls = Regex.Split(match.Groups["names"].Value.Trim(), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    string country = match.Groups["country"].Value.Trim();

                                    foreach (string appl in appls)
                                    {
                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = appl.Trim(),
                                            Language = "RU",
                                            Country = country
                                        });

                                        statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                                        {
                                            Name = appl.Trim(),
                                            Language = "RU",
                                            Country = country
                                        });
                                    }
                                }
                                else Console.WriteLine($"{item} --- 71");
                            }
                            else
                            {
                                for (int i = 0; i < applicants.Count; i++)
                                {
                                    try
                                    {
                                        Match match1 = Regex.Match(applicants[i].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");
                                        Match match2 = Regex.Match(applicants[++i].Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

                                        if (match1.Success && match2.Success)
                                        {
                                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                            {
                                                Name = match1.Groups["names"].Value.Trim(),
                                                Language = "UZ",
                                                Country = match1.Groups["country"].Value.Trim(),
                                                Translations = new List<Integration.Translation>
                                                    {
                                                        new Integration.Translation
                                                        {
                                                            Type = "71",
                                                            Language = "RU",
                                                            TrName = match2.Groups["names"].Value.Trim()
                                                        }
                                                    }
                                            });

                                            statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                                            {
                                                Name = match1.Groups["names"].Value.Trim(),
                                                Language = "UZ",
                                                Country = match1.Groups["country"].Value.Trim(),
                                                Translations = new List<Integration.Translation>
                                                    {
                                                        new Integration.Translation
                                                        {
                                                            Type = "73",
                                                            Language = "RU",
                                                            TrName = match2.Groups["names"].Value.Trim()
                                                        }
                                                    }
                                            });
                                        }
                                        else
                                            Console.WriteLine($"{applicants[i]}");
                                    }
                                    catch (Exception e)
                                    {
                                        //todo : fix the issue with Exception and create a proper handler
                                        Console.WriteLine($"EXCEPTION: {applicants[i]}");
                                    }
                                }
                            }
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        List<string> inventorsAll = Regex.Split(inid.Replace("(72)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string item in inventorsAll)
                        {
                            Match match = Regex.Match(item.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                string country = match.Groups["country"].Value.Trim();

                                if (country == "UZ" || country == "BY" || country == "RU")
                                {
                                    List<string> inventorsUz = Regex.Split(match.Groups["names"].Value.Trim(), ",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    foreach (string inventor in inventorsUz)
                                    {
                                        statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                        {
                                            Name = inventor.Trim(),
                                            Language = "RU",
                                            Country = country
                                        });
                                    }
                                }
                                else
                                {
                                    List<string> inventorsOther = Regex.Split(match.Groups["names"].Value.Trim(), @"(.+?,.+?,)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    foreach (string inventor in inventorsOther)
                                    {
                                        statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                        {
                                            Name = inventor.TrimEnd(',').Trim(),
                                            Language = "RU",
                                            Country = country
                                        });
                                    }
                                }
                            }
                            else Console.WriteLine($"{item} --- 72");
                        }
                    }
                    else if (inid.StartsWith("(71)(72)(73)"))
                    {
                        List<string> inventorsAll = Regex.Split(inid.Replace("(71)(72)(73)", ""), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string item in inventorsAll)
                        {
                            Match match = Regex.Match(item.Trim(), @"(?<names>.+),\s(?<country>[A-Z]{2})");

                            if (match.Success)
                            {
                                string country = match.Groups["country"].Value.Trim();

                                if (country == "UZ" || country == "BY" || country == "RU")
                                {
                                    List<string> inventorsUz = Regex.Split(match.Groups["names"].Value.Trim(), ",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    foreach (string inventor in inventorsUz)
                                    {
                                        statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                        {
                                            Name = inventor.Trim(),
                                            Language = "RU",
                                            Country = country
                                        });

                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = inventor.Trim(),
                                            Language = "RU",
                                            Country = country
                                        });

                                        statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                                        {
                                            Name = inventor.Trim(),
                                            Language = "RU",
                                            Country = country
                                        });
                                    }
                                }
                                else
                                {
                                    List<string> inventorsOther = Regex.Split(match.Groups["names"].Value.Trim(), @"(.+?,.+?,)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                                    foreach (string inventor in inventorsOther)
                                    {
                                        statusEvent.Biblio.Inventors.Add(new Integration.PartyMember
                                        {
                                            Name = inventor.TrimEnd(',').Trim(),
                                            Language = "RU",
                                            Country = country
                                        });

                                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                        {
                                            Name = inventor.TrimEnd(',').Trim(),
                                            Language = "RU",
                                            Country = country
                                        });

                                        statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                                        {
                                            Name = inventor.TrimEnd(',').Trim(),
                                            Language = "RU",
                                            Country = country
                                        });
                                    }
                                }
                            }
                            else Console.WriteLine($"{item} --- 72");
                        }
                    }
                    else Console.WriteLine($"{inid} -- not processed");
                }
            }

            return statusEvent;
        }

        internal List<string> MakeInids (string note, string subCode)
        {
            List<string> inids = new();

            if (subCode == "1")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<start>.+?)\s(?<tripleInid>\(\d{2}\)\(\d{2}\)\(.+?)\s(?<cont>\(\d{2}\)\s.+)\s(?<inid57>\(57\).+)");

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["start"].Value.Trim() + " " + match.Groups["cont"].Value.Trim(), @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    List<string> tripleInids = Regex.Split(match.Groups["tripleInid"].Value.Trim(), @"(?=\(\d{2}\)\(\d{2}\)\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string inid in tripleInids)
                    {
                        inids.Add(inid);
                    }

                    inids.Add(match.Groups["inid57"].Value.Trim());

                    return inids;
                }
                else
                {
                    Match match1 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<start>.+?)\s(?<doubleInid>\(\d{2}\)\(\d{2}\).+?)\s(?<cont>\(\d{2}\)\s.+)\s(?<inid57>\(57\).+)");

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
            else if (subCode == "3")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<start>.+)\s(?<tripleInid>\(\d{2}\)\(\d{2}\)\(.+?)\s(?<cont>\(\d{2}\).+)\s(?<inid57>\(57\).+)");

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["start"].Value.Trim() + " " + match.Groups["cont"].Value.Trim(), @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    inids.Add(match.Groups["tripleInid"].Value.Trim());

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
            else if (subCode == "4")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(), @"(?<start>.+)\s(?<triple>\(\d{2}\)\(\d{2}\)\(\d{2}\).+?)\s(?<cont>\(\d{2}\).+)\s(?<inid57>\(57\).+)");

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["start"].Value.Trim() + " " + match.Groups["cont"].Value.Trim(), @"(?=\(\d{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    inids.Add(match.Groups["triple"].Value.Trim());

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
