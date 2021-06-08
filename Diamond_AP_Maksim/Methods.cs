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

namespace Diamond_AP_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        private readonly string I21 = "(21)";
        private readonly string I22 = "(22)";
        private readonly string I23 = "(23)";
        private readonly string I31 = "(31)";
        private readonly string I32 = "(32)";
        private readonly string I33 = "(33)";
        private readonly string I54 = "(54)";
        private readonly string I71 = "(71)";
        private readonly string I72 = "(72)";
        private readonly string I74 = "(74)";
        private readonly string I84 = "(84)";
        private readonly string I51 = "(51)";
        private readonly string I75 = "(75)";
        private readonly string I86 = "(86)";
        private readonly string I96 = "(96)";

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

                if(subCode == "1")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                       .SkipWhile(val => !val.Value.StartsWith("Patent\n"+ "Applications\n" + "Filed"))
                       .TakeWhile(val => !val.Value.StartsWith("■")).ToList();


                    List<string> notes = Regex.Split(MakeText(xElements).Trim(), @"(?=\(21\))").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(21)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AF"));
                    }

                }
                else
                if(subCode == "20")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("PATENT APPLICATIONS LAPSED/WITHDRAWN"))
                        .TakeWhile(val => !val.Value.StartsWith("Utility Model")).ToList();

                    List<string> notes = Regex.Split(MakeText(xElements), @"(?=\(21\))").Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(21)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FA"));
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
                text += xElement.Value + " ";
            }

            text = text.Replace("\r", "").Replace("\n", " ").Replace("●●", " ").Replace("Patent Applications Lapsed/Withdrawn (Contd.)", " ").Replace("Patent Applications Filed (Contd.)"," ").Trim();

            return text;
        }
        internal Diamond.Core.Models.LegalStatusEvent MakePatent (string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legal = new()
            {
                CountryCode = "AP",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf"))
            };

            Biblio biblio = new();

            CultureInfo culture = new("RU-ru");

            if(subCode == "1")
            {

                Priority priority = new();

                foreach (string inid in MakeInids(note))
                {
                    if (inid.StartsWith(I21))
                    {
                        biblio.Application.Number = inid.Replace(I21, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I22))
                    {
                        biblio.Application.Date = DateTime.Parse(inid.Replace(I22, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith(I23))
                    {
                        biblio.Application.OtherDate = DateTime.Parse(inid.Replace(I23, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith(I31))
                    {
                        priority.Number = inid.Replace(I31, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I32))
                    {
                        priority.Date = DateTime.Parse(inid.Replace(I32, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else
                    if (inid.StartsWith(I33))
                    {
                        priority.Country = inid.Replace(I33, "").Trim();
                    }
                    else
                    if (inid.StartsWith(I54))
                    {
                        biblio.Titles.Add(new Title
                        {
                            Language = "EN",
                            Text = inid.Replace(I54, "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith(I71))
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = inid.Replace(I71, "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith(I72))
                    {
                        List<string> inventors = Regex.Split(inid.Replace(I72, "").Replace("et al","").Trim(), @",|\sand\s").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            biblio.Inventors.Add(new PartyMember
                            {
                                Name = inventor.Trim()
                            });
                        }
                    }
                    else
                    if (inid.StartsWith(I74))
                    {
                        biblio.Agents.Add(new PartyMember
                        {
                            Name = inid.Replace(I74, "").Trim()
                        });
                    }
                    else
                    if (inid.StartsWith(I84))
                    {
                        List<string> conventions = Regex.Split(inid.Replace(I84, ""), @",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        biblio.IntConvention.DesignatedStates = new();

                        foreach (string convention in conventions)
                        {
                            biblio.IntConvention.DesignatedStates.Add(convention.Trim());
                        }
                    }
                    else
                    if (inid.StartsWith(I96))
                    {
                        Match match = Regex.Match(inid.Replace(I96, "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4})\s(?<num>.+)");

                        if (match.Success)
                        {
                            biblio.EuropeanPatents.Add(new EuropeanPatent
                            {
                                AppDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
                                AppNumber = match.Groups["num"].Value.Trim()
                            });
                        }
                    }
                    else 
                    if (inid.StartsWith(I51))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        
                        foreach (string ipc in ipcs)
                        {
                            Match match = Regex.Match(ipc.Trim(), @"(?<num>.+)\s\((?<date>.+)\)");

                            if (match.Success)
                            {
                                biblio.Ipcs.Add(new Ipc
                                {
                                    Class = match.Groups["num"].Value.Trim(),
                                    Date = match.Groups["date"].Value.Trim()
                                });
                            }
                        }
                    }
                    else
                    if (inid.StartsWith(I75))
                    {

                        biblio.InvOrApps=new()
                        {
                            new PartyMember
                            {
                                Name = inid.Replace(I75, "").Trim()
                            }
                        };
                    }
                    else
                    if (inid.StartsWith(I86))
                    {
                        Match match = Regex.Match(inid.Replace(I86, "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4})\s(?<num>.+)");

                        if (match.Success)
                        {
                            biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                            biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                    }

                    else Console.WriteLine($"{inid}");

                }
                biblio.Priorities.Add(priority);
                legal.Biblio = biblio;
            }
            else
            if(subCode == "20")
            {
                foreach (string inid in MakeInids(note))
            {
                if (inid.StartsWith(I21))
                {
                    biblio.Application.Number = inid.Replace(I21, "").Trim();
                }
                else
                if (inid.StartsWith(I23))
                {
                    biblio.Application.OtherDate = DateTime.Parse(inid.Replace(I23, "").Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                }
                else
                if (inid.StartsWith(I54))
                {
                    biblio.Titles = new()
                    {
                        new Title
                        {
                            Language = "EN",
                            Text = inid.Replace(I54, "").Trim()
                        }
                    };
                }
                else
                if (inid.StartsWith(I71))
                {
                    biblio.Applicants = new();

                    List<string> temp = Regex.Split(inid.Replace(I71, "").Trim(), ",").Where(val => !string.IsNullOrEmpty(val))
                        .Where(val => !val.StartsWith(" et al") && !val.StartsWith("et al"))
                        .ToList();

                    List<string> applicants = new();

                    foreach (string tmp in temp)
                    {
                        if (tmp.Contains("and "))
                        {
                            Match match = Regex.Match(tmp.Trim(), @"(?<name1>.+)\s?and\s?(?<name2>.+)");

                            if (match.Success)
                            {
                                applicants.Add(match.Groups["name1"].Value.Trim());
                                applicants.Add(match.Groups["name2"].Value.Trim());
                            }
                            else
                            {
                                applicants.Add(tmp.Replace("and", "").Trim());
                            }

                        }
                        else applicants.Add(tmp);
                    }

                    foreach (string applicant in applicants)
                    {
                        biblio.Applicants.Add(new PartyMember
                        {
                            Name = applicant.Trim()
                        });
                    }
                }
                else
                if (inid.StartsWith(I72))
                {
                    biblio.Inventors = new();

                    List<string> temp = Regex.Split(inid.Replace(I72, "").Trim(), ",").Where(val => !string.IsNullOrEmpty(val))
                        .Where(val => !val.StartsWith(" et al") && !val.StartsWith("et al"))
                        .ToList();

                    List<string> inventors = new();

                    foreach (string tmp in temp)
                    {
                        if (tmp.Contains("and "))
                        {
                            Match match = Regex.Match(tmp.Trim(), @"(?<name1>.+)\s?and\s?(?<name2>.+)");

                            if (match.Success)
                            {
                                inventors.Add(match.Groups["name1"].Value.Trim());
                                inventors.Add(match.Groups["name2"].Value.Trim());
                            }
                            else
                            {
                                inventors.Add(tmp.Replace("and", "").Trim());
                            }
                        }
                        else inventors.Add(tmp);
                    }

                    foreach (string inventor in inventors)
                    {
                        biblio.Inventors.Add(new PartyMember
                        {
                            Name = inventor.Trim()
                        });
                    }
                }
                else
                if (inid.StartsWith(I74))
                {
                    biblio.Agents = new()
                    {
                        new PartyMember
                        {
                            Name = inid.Replace(I74, "").Trim()
                        }
                    };                   
                }
                else
                if (inid.StartsWith(I84))
                {
                    biblio.IntConvention = new();

                    biblio.IntConvention.DesignatedStates = new();

                    List<string> designatedStates = Regex.Split(inid.Replace(I84, "").Trim(), ",").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string designatedState in designatedStates)
                    {
                        biblio.IntConvention.DesignatedStates.Add(designatedState.Trim());
                    }
                }
                else
                if (inid.StartsWith(I51))
                {
                    biblio.Ipcs = new();

                    List<string> ipcs = Regex.Split(inid.Replace(I51, "").Trim(), @"(?<=\))").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string ipc in ipcs)
                    {
                        Match match = Regex.Match(ipc.Trim(), @"(?<class>.+)\s?\((?<date>.+)\)");
                        if (match.Success)
                        {
                            biblio.Ipcs.Add(new Ipc
                            {
                                Class = match.Groups["class"].Value.Trim(),
                                Date = match.Groups["date"].Value.Trim()
                            });
                        }
                    }
                }
                else
                if (inid.StartsWith(I75))
                {
                    biblio.InvOrApps = new();

                    List<string> temp = Regex.Split(inid.Replace(I75, "").Trim(), ",").Where(val => !string.IsNullOrEmpty(val))
                        .Where(val => !val.StartsWith(" et al") && !val.StartsWith("et al"))
                        .ToList();

                    List<string> invOrApps = new();

                    foreach (string tmp in temp)
                    {
                        if (tmp.Contains("and "))
                        {
                            Match match = Regex.Match(tmp.Trim(), @"(?<name1>.+)\s?and\s?(?<name2>.+)");

                            if (match.Success)
                            {
                                invOrApps.Add(match.Groups["name1"].Value.Trim());
                                invOrApps.Add(match.Groups["name2"].Value.Trim());
                            }
                            else Console.WriteLine($"{tmp}  72field");
                        }
                        else invOrApps.Add(tmp);
                    }
                    foreach (string invOrApp in invOrApps)
                    {
                        biblio.InvOrApps.Add(new PartyMember
                        {
                            Name = invOrApp.Trim()
                        });
                    }
                }
                else Console.WriteLine($"{inid}");    
            }

                Match date = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".tetml", "")), @"[0-9]{8}");

                if (date.Success)
                {
                    legal.LegalEvent = new LegalEvent()
                    {
                        Date = date.Value.Insert(4, "/").Insert(7, "/").Trim()
                    };
                }

                legal.Biblio = biblio;
            }

            return legal;
        }





        internal List<string> MakeInids(string note) => Regex.Split(note.Trim(), @"(?=\([0-9]{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                //string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event";
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