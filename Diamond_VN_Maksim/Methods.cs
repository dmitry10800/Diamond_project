using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Integration;

namespace Diamond_VN_Maksim
{
    class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = directory.GetFiles("*.tetml", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

            XElement tet;

            List<XElement> xElements = new();

            foreach (string tetml in files)
            {
                CurrentFileName = tetml;

                tet = XElement.Load(tetml);

                if(subCode == "6")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("thay ®æi chñ ®¬n"))
                          //   .TakeWhile(val => !val.Value.StartsWith(""))
                             .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Th.ng\s?b.o\s?s.:\s?)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Thông")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "GB"));
                    }
                }
                else if (subCode == "12")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("§¬n yªu cÇu cÊp b»ng ®éc quyÒn S¸NG CHÕ"))
                        .TakeWhile(val => !val.Value.StartsWith("§¬n yªu cÇu cÊp b»ng ®éc quyÒn GI¶I PH¸P H÷U ÝCH"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s\d)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note,subCode,"AA"));
                    }
                }
                else if (subCode == "13")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("§¬n yªu cÇu cÊp b»ng ®éc quyÒn GI¶I PH¸P H÷U ÝCH"))
                        .TakeWhile(val => !val.Value.StartsWith("Y£U CÇU thÈm ®Þnh NéI DUNG"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s\d)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "AA"));
                    }
                }
                else if (subCode == "14")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("S¸ng chÕ ®−îc cÊp B»NG ®éC QUYÒN"))
                        .TakeWhile(val => !val.Value.StartsWith("Gi¶i ph¸p h÷u Ých ®−îc cÊp B»NG ®éC QUYÒN"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s\d)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
                else if (subCode == "15")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Gi¶i ph¸p h÷u Ých ®−îc cÊp B»NG ®éC QUYÒN"))
                        .TakeWhile(val => !val.Value.StartsWith("söa ®æi, duy tr×, cÊp l¹i, chÊm døt, huû bá v¨n b»ng b¶o hé,"))
                        .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s\d)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "FG"));
                    }
                }
                else if(subCode == "16")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("2 - DUY TRÌ HIỆU LỰC VĂN BẰNG BẢO HỘ"))
                            .TakeWhile(val => !val.Value.StartsWith("b - Duy trì hiệu lực Bằng độc quyền gi¶i ph¸p h÷u Ých"))
                            .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Th.ng\s?b.o\s?s.:\s?)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Thông")).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "NZ"));
                    }
                }
                else if(subCode == "23")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("Do ng−êi nép ®¬n yªu cÇu"))
                            .TakeWhile(val => !val.Value.StartsWith("Söa ®æi ®¬n"))
                            .ToList();

                    List<string> notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\d+-2\d{3}.+)").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"\d+-\d{4}.+").Match(val).Success).ToList();

                    foreach (string note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "EE"));
                    }
                }
            }
            return statusEvents;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "VN",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = Id++,
                GazetteName = Path.GetFileName(CurrentFileName.Replace(".tetml", ".pdf")),
                Biblio = new()
                {
                    InvOrApps = new(),
                    DOfPublication = new(),
                    InvAppGrants = new()
                },
                NewBiblio = new(),
                LegalEvent = new()
                {
                    Translations = new()
                }
            };

            CultureInfo culture = new("ru-RU");

            if(subCode == "6")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"nộp\sđơn\s(?<application>.+)\sBên\s?chuyển\s?nhượng:(?<applicants>.+)\sBên\s?được\s?chuyển\s?nhượng:(?<applicantsNew>.+)");

                if (match.Success)
                {
                    List<string> application = Regex.Split(match.Groups["application"].Value.Trim(), @"(?<=\d{2}\/\d{2}\/\d{4})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (string item in application)
                    {
                        Match matchApp = Regex.Match(item.Trim(), @"(?<appNum>.+)\s(?<appDate>\d{2}\/\d{2}\/\d{4})");

                        if (matchApp.Success)
                        {
                            statusEvent.Biblio.Application.Number = matchApp.Groups["appNum"].Value.Trim();
                            statusEvent.Biblio.Application.Date = DateTime.Parse(matchApp.Groups["appDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{item} --- application");
                    }
                    
                    Match match71 = Regex.Match(match.Groups["applicants"].Value.Trim(), @"(?<name>.+)\s\((?<code>\D{2})\)\s(?<adress>.+),\s");
                    if (match71.Success)
                    {
                        statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                        {
                            Country = match71.Groups["code"].Value.Trim(),
                            Name = match71.Groups["name"].Value.Trim(),
                            Address1 = match71.Groups["adress"].Value.Trim(),
                        });
                    }
                    else Console.WriteLine($"{match.Groups["applicants"].Value.Trim()} ---- 71");

                    Match match71new = Regex.Match(match.Groups["applicantsNew"].Value.Trim(), @"(?<name>.+)\s\((?<code>\D{2})\)\s(?<adress>.+),\s");
                    if (match71.Success)
                    {
                        statusEvent.NewBiblio.Applicants.Add(new Integration.PartyMember
                        {
                            Country = match71new.Groups["code"].Value.Trim(),
                            Name = match71new.Groups["name"].Value.Trim(),
                            Address1 = match71new.Groups["adress"].Value.Trim(),
                        });
                    }
                    else Console.WriteLine($"{match.Groups["applicantsNew"].Value.Trim()} ---- 71 new");
                }
                else Console.WriteLine($"{note}");
            }
            else if(subCode is "12" or "13")
            {
                List<string> inids = new();
                Match splitTextAndField57 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<allinids>.+)(?<inid57>\(57\)\s.+)");

                if (splitTextAndField57.Success)
                {
                    Match match = Regex.Match(splitTextAndField57.Groups["allinids"].Value.Trim(), @"(?<allI>.+)(?<i30>\(30\).+)(?<i87>\(87\).+?\d{2}\/\d{2}\/\d{4})(?<i30c>.+)(?<allIc>\(51\).+)");

                    if (match.Success)
                    {
                        inids = Regex.Split(match.Groups["allI"].Value + match.Groups["allIc"].Value.Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                        inids.Add(match.Groups["i87"].Value.Trim());
                        inids.Add(match.Groups["i30"].Value + match.Groups["i30c"].Value.Trim());
                        inids.Add(splitTextAndField57.Groups["inid57"].Value.Trim());
                    }
                    else
                    {
                        inids = Regex.Split(splitTextAndField57.Groups["allinids"].Value.Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                        inids.Add(splitTextAndField57.Groups["inid57"].Value.Trim());
                    }
                }

                foreach (string inid in inids)
                {
                    if (inid.StartsWith("(11)"))
                    {
                        Match match = Regex.Match(inid.Replace("(11)", "").Trim(), @"(?<num>\d+)\s(?<kind>\D)");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} -- 11");
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        statusEvent.Biblio.Publication.Date = DateTime.Parse(inid.Replace("(43)", "").Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    else if (inid.StartsWith("(85)"))
                    {
                        statusEvent.Biblio.IntConvention.PctNationalDate = DateTime.Parse(inid.Replace("(85)", "").Trim(), culture)
                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        Match match = Regex.Match(inid.Replace("(22)", "").Trim(),
                            @"(?<appdate>\d{2}\/\d{2}\/\d{4})\s(?<note>.+)\s(?<noteDate>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["appdate"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            statusEvent.LegalEvent.Note = "|| " + match.Groups["note"].Value.Trim() + " | " +
                                                          match.Groups["noteDate"].Value.Trim();
                        }
                        else
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                    }
                    else if (inid.StartsWith("(86)"))
                    {
                        Match match = Regex.Match(inid.Replace("(86)", "").Trim(),
                            @"(?<num>.+)\s?(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 86");
                    }
                    else if (inid.StartsWith("(87)"))
                    {
                        Match match = Regex.Match(inid.Replace("(87)", "").Trim(),
                            @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})\s(?<note>.+)\s(?<date1>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctPublNumber = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            statusEvent.LegalEvent.Note = "|| " + match.Groups["note"].Value.Trim() + " | " + DateTime.Parse(match.Groups["date1"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n";
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("(87)", "").Trim(),
                                @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (match1.Success)
                            {
                                statusEvent.Biblio.IntConvention.PctPublNumber = match1.Groups["num"].Value.Trim();
                                statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else Console.WriteLine($"{inid} --- 87");
                        }
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @";")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Ipc()
                            {
                                Class = ipc.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(71)"))
                    {
                        List<string> applicants = Regex.Split(inid.Replace("(71)", "").Trim(), @"(?=\d{1,3}\..+)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicants)
                        {
                            Match match = Regex.Match(applicant.Trim(),
                                @"(?<name>.+)\((?<code>\D{2})\)\s(?<adress>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{applicant} --- 71");
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        List<string> inventors = Regex.Split(inid.Replace("(72)", "").Trim(), @";")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+)\((?<code>\D{2})\)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inventor} --- 72");
                        }
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        Match match = Regex.Match(inid.Replace("(74)", "").Trim(), @"(?<name>.+)\((?<english>\D+)\)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Language = "VI",
                                Name = match.Groups["name"].Value.Trim(),
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        Language = "EN",
                                        TrName = match.Groups["english"].Value.Trim(),
                                        Type = "INID"
                                    }
                                }
                            });
                        }
                        else Console.WriteLine($"{inid} --- 74");
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "VI",
                            Text = inid.Replace("(54)","").Trim()
                        });
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Abstract()
                        {
                            Language = "VI",
                            Text = inid.Replace("(57)", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(30)"))
                    {
                        Match match = Regex.Match(inid.Replace("(30)", "").Trim(),
                            @"(?<inid30>.+)\s(?<note>Ngày.+)\s(?<noteDate>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.LegalEvent.Note = "|| " + match.Groups["note"].Value.Trim() + " | " +
                                                          match.Groups["noteDate"].Value.Trim();

                            List <string> priorities = Regex.Split(match.Groups["inid30"].Value.Trim(), @"(?<=\s[A-Z]{2}\s)")
                                .Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string priority in priorities)
                            {
                                Match prior = Regex.Match(priority,
                                    @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})\s(?<code>\D{2})");

                                if (prior.Success)
                                {
                                    statusEvent.Biblio.Priorities.Add(new Priority()
                                    {
                                        Country = prior.Groups["code"].Value.Trim(),
                                        Number = prior.Groups["num"].Value.Trim(),
                                        Date = DateTime.Parse(prior.Groups["date"].Value.Trim(), culture)
                                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                });
                                }
                                else
                                {
                                    Match match1 = Regex.Match(priority.Trim(),
                                        @"(?<note>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");
                                    if (match1.Success)
                                    {
                                        statusEvent.LegalEvent.Note = "|| " + match1.Groups["note"].Value.Trim() + " | " +
                                                                      match1.Groups["date"].Value.Trim();
                                    }
                                    else Console.WriteLine($"{priority} --- 30");}
                            }
                        }
                    }
                    else if (inid.StartsWith("(62)"))
                    {
                        statusEvent.Biblio.Related.Add(new RelatedDocument()
                        {
                            Inid = "62",
                            Number = inid.Replace("(62)","").Trim()
                        });
                    }
                    else if (inid.StartsWith("(75)"))
                    {
                        List<string> invOrApps = Regex.Split(inid.Replace("(75)", "").Trim(), @"(?=\d{1,3}\..+)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string invOrApp in invOrApps)
                        {
                            Match match = Regex.Match(invOrApp.Trim(),
                                @"(?<name>.+)\((?<code>\D{2})\)\s(?<adress>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.InvOrApps.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{invOrApp} --- 71");
                        }
                    }
                    else Console.WriteLine($"{inid}");
                }
            }
            else if (subCode is "14" or "15")
            {
                List<string> inids = new();
                Match splitTextAndField57 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<allinids>.+)(?<inid57>\(57\)\s.+)");

                if (splitTextAndField57.Success)
                {
                    Match match = Regex.Match(splitTextAndField57.Groups["allinids"].Value.Trim(), @"(?<allI>.+)(?<i30>\(30\).+)(?<i87>\(87\).+?\d{2}\/\d{2}\/\d{4})(?<i30c>.+)(?<allIc>\(51\).+)");

                    if (match.Success)
                    {
                        inids = Regex.Split(match.Groups["allI"].Value + match.Groups["allIc"].Value.Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                        inids.Add(match.Groups["i87"].Value.Trim());
                        inids.Add(match.Groups["i30"].Value + match.Groups["i30c"].Value.Trim());
                        inids.Add(splitTextAndField57.Groups["inid57"].Value.Trim());
                    }
                    else
                    {
                        inids = Regex.Split(splitTextAndField57.Groups["allinids"].Value.Trim(), @"(?=\(\d{2}\))").Where(val => !string.IsNullOrEmpty(val)).ToList();
                        inids.Add(splitTextAndField57.Groups["inid57"].Value.Trim());
                    }
                }

                foreach (string inid in inids)
                {
                    if (inid.StartsWith("(11)"))
                    {
                        Match match = Regex.Match(inid.Replace("(11)", "").Trim(), @"(?<num>.+)\s(?<kind>\D)");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} -- 11");
                    }
                    else if (inid.StartsWith("(15)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @"(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.LegalEvent.Note = "|| (15) Ngày cấp | " + match.Groups["date"].Value.Trim() + "\n";
                            statusEvent.LegalEvent.Language = "VI";
                            statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                            {
                                Language = "EN",
                                Type = "INID",
                                Tr = "(15) || Date of grant | " + match.Groups["date"].Value.Trim() + "\n"
                        });
                        }
                        else Console.WriteLine($"{inid} ---- 15");
                    }
                    else if (inid.StartsWith("(21)"))
                    {
                        statusEvent.Biblio.Application.Number = inid.Replace("(21)", "").Trim();
                    }
                    else if (inid.StartsWith("(85)"))
                    {
                        statusEvent.Biblio.IntConvention.PctNationalDate = DateTime.Parse(inid.Replace("(85)", "").Trim(), culture)
                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(22)"))
                    {
                        statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture)
                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                    }
                    else if (inid.StartsWith("(45)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @"(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.DOfPublication.date_45 = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} -- 45");
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        Match match = Regex.Match(inid.Trim(), @"(?<date>\d{4}\-\d{2}\-\d{2})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Date =match.Groups["date"].Value.Trim().Replace("-", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} -- 43");
                    }
                    else if (inid.StartsWith("(54)"))
                    {
                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "VI",
                            Text = inid.Replace("(54)", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(57)"))
                    {
                        statusEvent.Biblio.Abstracts.Add(new Abstract()
                        {
                            Language = "VI",
                            Text = inid.Replace("(57)", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(86)"))
                    {
                        Match match = Regex.Match(inid.Replace("(86)", "").Trim(),
                            @"(?<num>.+)\s?(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} --- 86");
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        Match match = Regex.Match(inid.Replace("(74)", "").Trim(), @"(?<name>.+)\((?<english>\D+)\)");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Language = "VI",
                                Name = match.Groups["name"].Value.Trim(),
                                Translations = new List<Translation>
                                {
                                    new Translation
                                    {
                                        Language = "EN",
                                        TrName = match.Groups["english"].Value.Trim(),
                                        Type = "INID"
                                    }
                                }
                            });
                        }
                        else Console.WriteLine($"{inid} --- 74");
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        List<string> ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @";")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string ipc in ipcs)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Ipc()
                            {
                                Class = ipc.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(30)"))
                    {
                        List<string> priorities = Regex.Split(inid.Replace("(30)","").Trim(), @"(?<=\s[A-Z]{2}\s)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string priority in priorities)
                        {
                            Match prior = Regex.Match(priority,
                                @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})\s(?<code>\D{2})");

                            if (prior.Success)
                            {
                                statusEvent.Biblio.Priorities.Add(new Priority()
                                {
                                    Country = prior.Groups["code"].Value.Trim(),
                                    Number = prior.Groups["num"].Value.Trim(),
                                    Date = DateTime.Parse(prior.Groups["date"].Value.Trim(), culture)
                                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                                });
                            }
                            else Console.WriteLine($"{priority} --- 30");
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        List<string> inventors = Regex.Split(inid.Replace("(72)", "").Trim(), @";")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inventor in inventors)
                        {
                            Match match = Regex.Match(inventor.Trim(), @"(?<name>.+)\((?<code>\D{2})\)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inventor} --- 72");
                        }
                    }
                    else if (inid.StartsWith("(87)"))
                    {
                        Match match = Regex.Match(inid.Replace("(87)", "").Trim(),
                            @"(?<num>.+)\s?(?<kind>[A-Z]\d{1,2})\s(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctPublNumber = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.IntConvention.PctPublKind = match.Groups["kind"].Value.Trim();
                            statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else
                        {
                            Match match1 = Regex.Match(inid.Replace("(87)", "").Trim(),
                                @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (match1.Success)
                            {
                                statusEvent.Biblio.IntConvention.PctPublNumber = match1.Groups["num"].Value.Trim();
                                statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else Console.WriteLine($"{inid} --- 87");
                        }
                    }
                    else if (inid.StartsWith("(62)"))
                    {
                        statusEvent.Biblio.Related.Add(new RelatedDocument()
                        {
                            Inid = "62",
                            Number = inid.Replace("(62)", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(67)"))
                    {
                        statusEvent.Biblio.Related.Add(new RelatedDocument()
                        {
                            Inid = "67",
                            Number = inid.Replace("(67)", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(73)"))
                    {
                        List<string> assignees = Regex.Split(inid.Replace("(73)", "").Trim(), @"(?=\d{1,3}\..+)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string assignee in assignees)
                        {
                            Match match = Regex.Match(assignee.Trim(),
                                @"(?<name>.+)\((?<code>\D{2})\)\s(?<adress>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Assignees.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{assignee} --- 73");
                        }
                    }
                    else if (inid.StartsWith("(76)"))
                    {
                        List<string> invAppGrants = Regex.Split(inid.Replace("(76)", "").Trim(), @"(?=\d{1,3}\..+)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string inv in invAppGrants)
                        {
                            Match match = Regex.Match(inv.Trim(),
                                @"(?<name>.+)\((?<code>\D{2})\)\s(?<adress>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.InvAppGrants.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim()
                                });
                            }
                            else Console.WriteLine($"{inv} --- 76");
                        }
                    }

                    else Console.WriteLine($"{inid}");
                }
            }
            else if(subCode == "16")
            {
                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"Ng.y\sn.p:\s?(?<dateNote>\d{2}\/\d{2}\/\d{4}).+tr.\s..n\s(?<pubNum>\d+).+(?<noteDate>\d{2}\/\d{2}\/\d{4}).+.ng\sb.o\sh.:\s?(?<field73>.+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();
                    statusEvent.LegalEvent.Note = "|| Hiệu lực Bằng độc quyền sáng chế số " + match.Groups["pubNum"].Value.Trim() + " được duy trì đến " +
                         DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n" +
                         "|| Ngày nộp | " + DateTime.Parse(match.Groups["dateNote"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                    statusEvent.LegalEvent.Language = "VI";

                    statusEvent.LegalEvent.Translations.Add(new Integration.NoteTranslation
                    {
                        Language = "EN",
                        Type = "note",
                        Tr = "|| Expiration date | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n" +
                        "|| Extension filing date | " + DateTime.Parse(match.Groups["dateNote"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim()
                    });

                        Match match1 = Regex.Match(match.Groups["field73"].Value.Trim(), @"(?<name>.+)\s\((?<code>\D{2})\)\s(?<adress>.+)");

                        if (match1.Success)
                        {
                            statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                            {
                                Country = match1.Groups["code"].Value.Trim(),
                                Name = match1.Groups["name"].Value.Trim(),
                                Address1 = match1.Groups["adress"].Value.Trim(),
                            });
                        }
                        else Console.WriteLine($"{match.Groups["field73"].Value.Trim()}");
                    

                }
                else Console.WriteLine($"{note.Replace("\r", "").Replace("\n", " ").Trim()}");
            }
            else if(subCode == "23")
            {
                

                Match match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<appNum>.+?)\s(?<pubNum>.+)\s(?<pubDate>\d{2}\/\d{2}\/\d{4})\s(?<evDate>\d{2}\/\d{2}\/\d{4})\s(?<ipcs>.+)\s");

                if (match.Success)
                {
                    statusEvent.Biblio.Application.Number = match.Groups["appNum"].Value.Trim();
                    statusEvent.Biblio.Publication.Number = match.Groups["pubNum"].Value.Trim();

                    statusEvent.Biblio.Ipcs.Add(new Integration.Ipc
                    {
                        Class = match.Groups["ipcs"].Value.Trim()
                    });

                    statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Groups["pubDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["evDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                }
                else Console.WriteLine($"{note}");
            }

            return statusEvent;
        }
        internal string MakeText(List<XElement> xElements, string subCode)
        {
            string text = null;

            if (subCode is "6" or "23" or "16" or "12" or "13" or "14" or "15")
            {
                foreach (XElement xElement in xElements)
                {
                    text += xElement.Value + " ";
                }

                return text.Trim();
            }

            else return text.Trim();
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
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
