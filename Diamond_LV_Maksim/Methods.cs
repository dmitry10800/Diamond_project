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

namespace Diamond_LV_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        private string I51 = "(51)";
        private string I11 = "(11)";
        private string I21 = "(21)";
        private string I22 = "(22)";
        private string I45 = "(45)";
        private string I31 = "(31)";
        private string I32 = "(32)";
        private string I33 = "(33)";
        private string I86 = "(86)";
        private string I87 = "(87)";
        private string I73 = "(73)";
        private string I72 = "(72)";
        private string I74 = "(74)";
        private string I54 = "(54)";

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

                if (subCode == "4")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("(Patentu likuma 71. panta otrā un piektā daļa)"))
                        .TakeWhile(val => !val.Value.StartsWith("Latvijā apstiprināto Eiropas patentu"))
                        .ToList();

                    var notes = Regex.Split(FullText(xElements, subCode), @"(?=\(51\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_) && _.StartsWith("(51)")).ToList();

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
                CountryCode = "LV",
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new()
                {
                    DOfPublication = new()
                }
            };

            var culture = new CultureInfo("ru-RU");

            if (subCode == "4")
            {
                var intConvention = new IntConvention();

                foreach (var inid in MakeInids(note, subCode))
                {
                    if (inid.StartsWith(I21))
                    {
                        patent.Biblio.Application.Number = CleanInid(inid);
                    }
                    else if (inid.StartsWith(I22))
                    {
                        patent.Biblio.Application.Date = DateTime.Parse(CleanInid(inid), culture).ToString("yyyy.MM.dd").Replace(".","/").Trim();
                    }
                    else if (inid.StartsWith(I45))
                    {
                        patent.Biblio.DOfPublication.date_45 = DateTime.Parse(CleanInid(inid), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith(I86))
                    {
                        var match = Regex.Match(CleanInid(inid), @"(?<Number>.+)\s(?<Date>\d{2}.\d{2}.\d{4})",  RegexOptions.Singleline);
                        if (match.Success)
                        {
                            intConvention.PctApplNumber = match.Groups["Number"].Value.Trim();
                            intConvention.PctApplDate = DateTime.Parse(match.Groups["Date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine(inid + " - 86");
                    }
                    else if (inid.StartsWith(I87))
                    {
                        var match = Regex.Match(CleanInid(inid), @"(?<Number>.+)\s(?<Date>\d{2}.\d{2}.\d{4})", RegexOptions.Singleline);
                        if (match.Success)
                        {
                            intConvention.PctPublNumber = match.Groups["Number"].Value.Trim();
                            intConvention.PctPublDate = DateTime.Parse(match.Groups["Date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine(inid + " - 87");
                    }
                    else if (inid.StartsWith(I73))
                    {
                        var assignees = Regex.Split(CleanInid(inid).Replace("\r",""), @"(?<=,\s[A-Z]{2}\n)", RegexOptions.Singleline).Where(_ =>!string.IsNullOrEmpty(_)).ToList();

                        foreach (var assignee in assignees)
                        {
                            var match = Regex.Match(assignee, @"(?<Name>.+);(?<Adress>.+),\s(?<Code>[A-Z]{2})", RegexOptions.Singleline);
                            if (match.Success)
                            {
                                patent.Biblio.Assignees.Add(new PartyMember()
                                {
                                    Name = match.Groups["Name"].Value.Trim(),
                                    Address1 = match.Groups["Adress"].Value.Trim(),
                                    Country = match.Groups["Code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine(assignee + " - 73");
                        }
                    }
                    else if (inid.StartsWith(I72))
                    {
                        var inventors = Regex.Split(CleanInid(inid), @"(?<=\([A-Z]{2}\),)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        foreach (var inventor in inventors)
                        {
                            var match = Regex.Match(inventor, @"(?<Name>.+)\((?<Code>[A-Z]{2})\)", RegexOptions.Singleline);
                            if (match.Success)
                            {
                                patent.Biblio.Inventors.Add(new PartyMember()
                                {
                                    Name = match.Groups["Name"].Value.Trim(),
                                    Country = match.Groups["Code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine(inventor + " - 72");
                        }
                    }
                    else if (inid.StartsWith(I74))
                    {
                        var agents = Regex.Split(CleanInid(inid), @"(?<=,\s[A-Z]{2}\n)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        foreach (var agent in agents)
                        {
                            var match = Regex.Match(agent, @"(?<Name>.+);(?<Adress>.+),\s(?<Code>[A-Z]{2})", RegexOptions.Singleline);
                            if (match.Success)
                            {
                                patent.Biblio.Agents.Add(new PartyMember()
                                {
                                    Name = match.Groups["Name"].Value.Trim(),
                                    Address1 = match.Groups["Adress"].Value.Trim(),
                                    Country = match.Groups["Code"].Value.Trim()
                                });
                            }
                            else Console.WriteLine(agent + " - 74");
                        }
                    }
                    else if (inid.StartsWith(I54))
                    {
                        var sbLV = new StringBuilder();
                        var sbEN = new StringBuilder();

                        var text = Regex.Split(CleanInid(inid), @"\n").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        if (text.Count % 2 == 0)
                        {
                            for (int i = 0; i < text.Count/2; i++)
                            {
                                sbLV.Append(text[i] + " ");
                            }

                            for (int i = text.Count/2; i < text.Count; i++)
                            {
                                sbEN.Append(text[i] + " ");
                            }

                            patent.Biblio.Titles.Add(new Title()
                            {
                                Text = sbLV.ToString(),
                                Language = "LV",
                            });

                            patent.Biblio.Titles.Add(new Title()
                            {
                                Text = sbEN.ToString(),
                                Language = "EN",
                            });
                        }
                        else
                        {
                            for (int i = 0; i < (text.Count/2)+1; i++)
                            {
                                sbLV.Append(text[i] + " ");
                            }

                            for (int i = (text.Count / 2)+1; i < text.Count; i++)
                            {
                                sbEN.Append(text[i] + " ");
                            }

                            patent.Biblio.Titles.Add(new Title()
                            {
                                Text = sbLV.ToString(),
                                Language = "LV",
                            });

                            patent.Biblio.Titles.Add(new Title()
                            {
                                Text = sbEN.ToString(),
                                Language = "EN",
                            });
                        }
                    }
                    else if (inid.StartsWith(I51))
                    {
                        var match = Regex.Match(inid, @"\(51\)(?<Inid51>.+)\(11\)(?<Inid11>.+\s)(?<Inid13>[A-Z]{1}\d+)\s(?<Inid51Next>.+)", RegexOptions.Singleline);
                        if (match.Success)
                        {
                            patent.Biblio.Publication.Number = match.Groups["Inid11"].Value.Trim();
                            patent.Biblio.Publication.Kind = match.Groups["Inid13"].Value.Trim();

                            var ipcs = Regex.Split(match.Groups["Inid51"].Value + " " + match.Groups["Inid51Next"].Value, @"\n").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                            foreach (var ipc in ipcs)
                            {
                                var ipcMatch = Regex.Match(ipc, @"(?<Class>.+)\s\((?<Version>.+)\)");
                                if (ipcMatch.Success)
                                {
                                    patent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Class = ipcMatch.Groups["Class"].Value.Trim(),
                                        Date = ipcMatch.Groups["Version"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    patent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Class = ipc
                                    });
                                }
                            }
                        }
                        else
                        {
                            var match2 = Regex.Match(inid, @"\(51\)(?<Inid51>.+)\(11\)(?<Inid11>.+\s)(?<Inid13>[A-Z]{1}\d+)", RegexOptions.Singleline);
                            if (match2.Success)
                            {
                                patent.Biblio.Publication.Number = match2.Groups["Inid11"].Value.Trim();
                                patent.Biblio.Publication.Kind = match2.Groups["Inid13"].Value.Trim();

                                var ipcs = Regex.Split(match2.Groups["Inid51"].Value, @"\n").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                                foreach (var ipc in ipcs)
                                {
                                    var ipcMatch = Regex.Match(ipc, @"(?<Class>.+)\s\((?<Version>.+)\)");
                                    if (ipcMatch.Success)
                                    {
                                        patent.Biblio.Ipcs.Add(new Ipc()
                                        {
                                            Class = ipcMatch.Groups["Class"].Value.Trim(),
                                            Date = ipcMatch.Groups["Version"].Value.Trim()
                                        });
                                    }
                                    else
                                    {
                                        patent.Biblio.Ipcs.Add(new Ipc()
                                        {
                                            Class = ipc
                                        });
                                    }
                                }
                            }
                            else Console.WriteLine(inid + " - 51");
                        }
                    }
                    else if (inid.StartsWith(I31))
                    {
                        var priorities = Regex
                            .Split(
                                inid.Replace(I31, "").Replace(I32, "").Replace(I33, "").Replace("\r", "")
                                    .Replace("\n", " ").Replace("  ", " "), @"(?<=\.\d{4}\s[A-Z]{2})",
                                RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        foreach (var prior in priorities)
                        {
                            var priorMatch = Regex.Match(prior,
                                @"(?<Num>.+)\s(?<Date>\d{2}.\d{2}.\d{4})\s(?<Country>[A-Z]{2})");
                            if (priorMatch.Success)
                            {
                                patent.Biblio.Priorities.Add(new Priority()
                                {
                                    Number = priorMatch.Groups["Num"].Value.Trim(),
                                    Date = priorMatch.Groups["Date"].Value.Trim(),
                                    Country = priorMatch.Groups["Country"].Value.Trim(),
                                });
                            }
                            else Console.WriteLine(prior + " --31");
                        }
                    }
                    else Console.WriteLine(inid);
                }
                patent.Biblio.IntConvention = intConvention;
            }

            return patent;
        }

        private string CleanInid(string inid)
        {
            return Regex.Replace(inid, @"^\(\d{2}\)", "", RegexOptions.Singleline).Trim();
        }
        private List<string> MakeInids(string note, string subCode)
        {
            var inids = new List<string>();

            if (subCode == "4")
            {
                var match = Regex.Match(note, @"(?<Inid51>.+)(?<OtherInids1>\(21\).+)(?<Priorities>\(31\).+)(?<OtherInids2>\(86\).+)", RegexOptions.Singleline);
                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["OtherInids1"].Value + match.Groups["OtherInids2"].Value, @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    inids.Add(match.Groups["Inid51"].Value);
                    inids.Add(match.Groups["Priorities"].Value);
                }
                else
                {
                    var match2 = Regex.Match(note, @"(?<Inid51>.+)(?<OtherInids1>\(21\).+)(?<Priorities>\(31\).+)(?<OtherInids2>\(73\).+)", RegexOptions.Singleline);
                    if (match2.Success)
                    {
                        inids = Regex.Split(match2.Groups["OtherInids1"].Value + match2.Groups["OtherInids2"].Value, @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        inids.Add(match2.Groups["Inid51"].Value);
                        inids.Add(match2.Groups["Priorities"].Value);
                    }
                    else
                    {
                        var match3 = Regex.Match(note, @"(?<Inid51>.+)(?<OtherInids1>\(21\).+)", RegexOptions.Singleline);
                        if (match3.Success)
                        {
                            inids = Regex.Split(match3.Groups["OtherInids1"].Value, @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                            inids.Add(match3.Groups["Inid51"].Value);
                        }
                        else Console.WriteLine(note);
                    }
                }
            }
            return inids;
        }
        private string FullText(List<XElement> xElements, string subCode)
        {
            var sb = new StringBuilder();

            if (subCode == "4")
            {
                foreach (var xElement in xElements)
                {
                    sb = sb.AppendLine(xElement.Value);
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
