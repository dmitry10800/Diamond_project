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
using Diamond.Core.Models;
using Integration;
using SixLabors.ImageSharp;

namespace Diamond_VN_Maksim
{
    class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            var files = directory.GetFiles("*.tetml", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

            XElement tet;

            List<XElement> xElements = new();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "3")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("a - Chuyển nhượng quyền sở hữu Bằng độc quyền sáng chế"))
                        .TakeWhile(val => !val.Value.StartsWith("b - Chuyển nhượng quyền sở hữu Bằng độc quyền gi¶i ph¸p h÷u Ých"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Quy.t\s?..nh\s?)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Quy")).ToList();

                    foreach (var note in notes)
                    {
                        foreach (var patent in MakeListPatent(note, subCode, "PC"))
                        {
                            statusEvents.Add(patent);
                        }
                    }
                }
                else if (subCode == "4")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("b - Chuyển nhượng quyền sở hữu Bằng độc quyền gi¶i ph¸p h÷u Ých"))
                        .TakeWhile(val => !val.Value.StartsWith("2- CHUYÓN GIAO QUYÒN Sö DôNG ®èI TƯîNG Së H÷U C«NG NGHIÖP"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Quy.t\s?..nh\s?)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Quy")).ToList();

                    foreach (var note in notes)
                    {
                        foreach (var patent in MakeListPatent(note, subCode, "PC"))
                        {
                            statusEvents.Add(patent);
                        }
                    }
                }
                else if (subCode == "6")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                             .SkipWhile(val => !val.Value.StartsWith("thay ®æi chñ ®¬n"))
                             //   .TakeWhile(val => !val.Value.StartsWith(""))
                             .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Th.ng\s?b.o\s?s.:\s?)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Thông")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "GB"));
                    }
                }
                else if (subCode == "7")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("b- Ghi nhËn thay ®æi chñ ®¬n yªu cÇu cÊp B»ng ®éc quyÒn Gi¶i ph¸p h÷u Ých"))
                        // .TakeWhile(val => !val.Value.StartsWith(""))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Th.ng\s?b.o\s?s.:\s?)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Thông")).ToList();

                    foreach (var note in notes)
                    {
                        foreach (var patent in MakeListPatent(note, subCode, "GB"))
                        {
                            statusEvents.Add(patent);
                        }
                    }
                }
                else if (subCode == "12")
                {
                    var imageFiles = GetImages(tet);
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.EndsWith("ĐƠN YÊU CẦU CẤP BẰNG ĐỘC QUYỀN SÁNG CHẾ"))
                        .TakeWhile(val => !val.Value.StartsWith("PHẦN II"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s\d)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    statusEvents.AddRange(notes.Select(note => MakePatent(note, subCode, "AA", imageFiles)));
                }
                else if (subCode == "13")
                {
                    var imageFiles = GetImages(tet);
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("ĐƠN YÊU CẦU CẤP BẰNG ĐỘC QUYỀN GIẢI PHÁP HỮU ÍCH"))
                        .TakeWhile(val => !val.Value.StartsWith("Y£U CÇU thÈm ®Þnh NéI DUNG"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s\d)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    statusEvents.AddRange(notes.Select(note => MakePatent(note, subCode, "AA", imageFiles)));
                }
                else if (subCode == "14")
                {
                    var imageFiles = GetImages(tet);
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("SÁNG CHẾ ĐƯỢC CẤP BẰNG ĐỘC QUYỀN"))
                        .TakeWhile(val => !val.Value.StartsWith("PHẦN II"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s\d)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    statusEvents.AddRange(notes.Select(note => MakePatent(note, subCode, "FG", imageFiles)));
                }
                else if (subCode == "15")
                {
                    var imageFiles = GetImages(tet);
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("GIẢI PHÁP HỮU ÍCH ĐƯỢC CẤP BẰNG ĐỘC QUYỀN"))
                        .TakeWhile(val => !val.Value.StartsWith("PhÇn iII"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(11\)\s\d)")
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    statusEvents.AddRange(notes.Select(note => MakePatent(note, subCode, "FG", imageFiles)));
                }
                else if (subCode == "16")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("2 - DUY TRÌ HIỆU LỰC VĂN BẰNG BẢO HỘ"))
                            .TakeWhile(val => !val.Value.StartsWith("b - Duy trì hiệu lực Bằng độc quyền gi¶i ph¸p h÷u Ých"))
                            .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Th.ng\s?b.o\s?s.:\s?)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Thông")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "NZ"));
                    }
                }
                else if (subCode == "17")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("b - Duy trì hiệu lực Bằng độc quyền gi¶i ph¸p h÷u Ých"))
                        .TakeWhile(val => !val.Value.StartsWith("3 - CẤP LẠI VĂN BẰNG BẢO HỘ"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Th.ng\s?b.o\s?s.:\s?)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Thông")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "NZ"));
                    }
                }
                else if (subCode == "18")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("3 - CẤP LẠI VĂN BẰNG BẢO HỘ"))
                        .TakeWhile(val => !val.Value.StartsWith("PHÇN iV"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Quy.t\s..nh\s?)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Quy")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "NB"));
                    }
                }
                else if (subCode is "19")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("b - Cấp lại Bằng độc quyền Giải pháp hữu ích"))
                        .TakeWhile(val => !val.Value.StartsWith("PHẨN IV"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Quy.t\s..nh.+)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Quy")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "NB"));
                    }
                }
                else if (subCode is "20")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("5- Ghi nhËn ®¹i diÖn së h÷u c«ng nghiÖp"))
                        .TakeWhile(val => !val.Value.StartsWith("581"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Quy.t\s..nh.+)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Quy")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "PC"));
                    }
                }
                else if (subCode is "22")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("4 – CẤP PHÓ BẢN BẰNG ĐỘC QUYỀN SÁNG CHẾ"))
                        .TakeWhile(val => !val.Value.StartsWith("5- Ghi nhËn ®¹i"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Quy.t\s..nh\s\d.+)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Quy")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "SC"));
                    }
                }
                else if (subCode == "23")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("Do ng−êi nép ®¬n yªu cÇu"))
                            .TakeWhile(val => !val.Value.StartsWith("Söa ®æi ®¬n"))
                            .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\d+-2\d{3}.+)").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"1-2\d{3}.+").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "EE"));
                    }
                }
                else if (subCode == "24")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Y£U CÇU thÈm ®Þnh NéI DUNG"))
                        .TakeWhile(val => !val.Value.StartsWith("PhÇn iV"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=\d+-2\d{3}.+)").Where(val => !string.IsNullOrEmpty(val) && new Regex(@"2-2\d{3}.+").Match(val).Success).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "EE"));
                    }
                }
                else if (subCode == "29")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("b- cấp phó bản Bằng độc quyền Giải pháp hữu ích"))
                        .TakeWhile(val => !val.Value.StartsWith("PHẨN IV"))
                        .ToList();

                    var notes = Regex.Split(MakeText(xElements, subCode).Trim(), @"(?=Quy.t.+)").Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("Quy")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakePatent(note, subCode, "SC"));
                    }
                }
            }
            return statusEvents;
        }

        private Dictionary<string, string> GetImages(XElement tet)
        {
            var result = new Dictionary<string, string>();
            try
            {
                var iDAndFilenameInfo = new Dictionary<string, string>();
                var imagesList = tet.Descendants()
                    .Where(x => x.Name.LocalName == "Image")
                    .Select(x => (x.Attribute("id")?.Value, x.Attribute("filename")?.Value)).ToList();
                foreach (var i in imagesList.Where(i => !iDAndFilenameInfo.ContainsKey(i.Item1)))
                {
                    iDAndFilenameInfo.Add(i.Item1, i.Item2);
                }
                var iDAndPatentKeyInfo = new Dictionary<string, string>();
                var pages = tet.Descendants().Elements().Where(x => x.Name.LocalName == "Page").ToList();
                foreach (var page in pages)
                {
                    var numberPattern = new Regex(@"(?=\(11\)\s(?<Number>\d[^\(]+))");
                    var pageImage = page.Descendants()
                        .Where(x => x.Name.LocalName == "PlacedImage")
                        .ToList();
                    if (!pageImage.Any())
                    {
                        continue;
                    }
                    var pageElements = page.Descendants().Where(x => x.Name.LocalName == "Text").ToList();
                    var patentNumber = pageElements.Where(x => numberPattern.Match(x.Value).Success).ToList();
                    if (patentNumber.Any() && patentNumber.Count == 1)
                    {
                        var patentNumberValue = numberPattern.Match(patentNumber.FirstOrDefault()?.Value).Groups["Number"]
                            .Value.Trim();

                        if (!string.IsNullOrWhiteSpace(patentNumberValue))
                        {
                            foreach (var item in pageImage)
                            {
                                var value = item.Attribute("image")?.Value;
                                if (value != null)
                                {
                                    iDAndPatentKeyInfo.Add(value, patentNumberValue);
                                }
                            }
                        }
                    }
                }
                foreach (var pair in iDAndPatentKeyInfo.Where(pair => iDAndFilenameInfo.ContainsKey(pair.Key)))
                {
                    result.Add(iDAndFilenameInfo[pair.Key], iDAndPatentKeyInfo[pair.Key]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Getting images failed. {e.Message}");
                throw;
            }

            return result;
        }

        internal Diamond.Core.Models.LegalStatusEvent MakePatent(string note, string subCode, string sectionCode, Dictionary<string, string> imagesDictionary = null)
        {
            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
            {
                CountryCode = "VN",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new Biblio
                {
                    InvOrApps = new List<PartyMember>(),
                    DOfPublication = new DOfPublication(),
                    InvAppGrants = new List<PartyMember>()
                },
                NewBiblio = new Biblio(),
                LegalEvent = new LegalEvent
                {
                    Translations = new List<NoteTranslation>()
                }
            };

            CultureInfo culture = new("ru-RU");

            if (subCode is "6")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"nộp\sđơn\s(?<application>.+)\sBên\s?chuyển\s?nhượng:(?<applicants>.+)\sBên\s?được\s?chuyển\s?nhượng:(?<applicantsNew>.+)");

                if (match.Success)
                {
                    var application = Regex.Split(match.Groups["application"].Value.Trim(), @"(?<=\d{2}\/\d{2}\/\d{4})").Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var item in application)
                    {
                        var matchApp = Regex.Match(item.Trim(), @"(?<appNum>.+)\s(?<appDate>\d{2}\/\d{2}\/\d{4})");

                        if (matchApp.Success)
                        {
                            statusEvent.Biblio.Application.Number = matchApp.Groups["appNum"].Value.Trim();
                            statusEvent.Biblio.Application.Date = DateTime.Parse(matchApp.Groups["appDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{item} --- application");
                    }

                    var match71 = Regex.Match(match.Groups["applicants"].Value.Trim(), @"(?<name>.+)\s\((?<code>\D{2})\)\s(?<adress>.+),\s");
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

                    var match71new = Regex.Match(match.Groups["applicantsNew"].Value.Trim(), @"(?<name>.+)\s\((?<code>\D{2})\)\s(?<adress>.+),\s");
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
            else if (subCode is "12" or "13")
            {
                List<string> inids = new();
                var splitTextAndField57 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<allinids>.+)(?<inid57>\(57\)\s.+)");

                if (splitTextAndField57.Success)
                {
                    var match = Regex.Match(splitTextAndField57.Groups["allinids"].Value.Trim(), @"(?<allI>.+)(?<i30>\(30\).+)(?<i87>\(87\).+?\d{2}\/\d{2}\/\d{4})(?<i30c>.+)(?<allIc>\(51\).+)");

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

                foreach (var inid in inids)
                {
                    if (inid.StartsWith("(11)"))
                    {
                        var match = Regex.Match(inid.Replace("(11)", "").Trim(), @"(?<num>\d+)\s(?<kind>\D)");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            if (subCode is "13")
                            {
                                if (match.Groups["kind"].Value.Trim() is "A")
                                {
                                    statusEvent.Biblio.Publication.Kind = "U";
                                }
                                else statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                            }
                            else statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} -- 11");
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        var match = Regex.Match(inid.Replace("(43)", "").Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid}");
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
                        var match = Regex.Match(inid.Replace("(22)", "").Trim(),
                            @"(?<appdate>\d{2}\/\d{2}\/\d{4})\s(?<note>.+)\s(?<noteDate>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(match.Groups["appdate"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            var matchTmp = Regex.Match(
                                match.Groups["note"].Value,
                                @"(?<note1>.+)\s(?<date1>\d{2}\/\d{2}\/\d{4})\s(?<note2>.+)");

                            if (matchTmp.Success)
                            {
                                statusEvent.LegalEvent.Note =
                                    "|| " + matchTmp.Groups["note1"].Value.Trim().TrimEnd(':').Trim() + " | " +
                                    DateTime.Parse(matchTmp.Groups["date1"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n" +
                                    "|| " + matchTmp.Groups["note2"].Value.Trim().TrimEnd(':').Trim() + " | " +
                                    DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n";
                                statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                                {
                                    Language = "EN",
                                    Type = "INID",
                                    Tr = "|| Date of request for substantive examination | " + DateTime.Parse(matchTmp.Groups["date1"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n" +
                                    "|| Date of request for early publication | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n"
                                });
                            }
                            else
                            {
                                statusEvent.LegalEvent.Note = "|| " + match.Groups["note"].Value.Trim().TrimEnd(':').Trim() + " | " +
                                                              DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n";

                                statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                                {
                                    Language = "EN",
                                    Type = "INID",
                                    Tr = "|| Date of request for substantive examination | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n"
                                });
                            }
                        }
                        else
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(inid.Replace("(22)", "").Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                    }
                    else if (inid.StartsWith("(86)"))
                    {
                        var match = Regex.Match(inid.Replace("(86)", "").Trim(),
                          @"(?<num>.+)\s?(?<kind>[A-Z]\d{1,2})\s(?<date>\d{2}\/\d{2}\/\d{4})\s(?<note>.+)\s(?<date1>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctApplNumber = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.IntConvention.PctApplKind = match.Groups["kind"].Value.Trim();
                            statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            statusEvent.LegalEvent.Note = "|| " + match.Groups["note"].Value.Trim().TrimEnd(':').Trim() + " | " + DateTime.Parse(match.Groups["date1"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n";

                            statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                            {
                                Language = "EN",
                                Type = "INID",
                                Tr = "|| Date of request for substantive examination | " + DateTime.Parse(match.Groups["date1"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n"
                            });
                        }
                        else
                        {
                            var matchKind = Regex.Match(inid.Replace("(86)", "").Trim(),
                                @"(?<num>.+)\s?(?<kind>[A-Z]\d{1,2})\s(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (matchKind.Success)
                            {
                                statusEvent.Biblio.IntConvention.PctApplNumber = matchKind.Groups["num"].Value.Trim();
                                statusEvent.Biblio.IntConvention.PctApplKind = matchKind.Groups["kind"].Value.Trim();
                                statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(matchKind.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else
                            {
                                var match1 = Regex.Match(inid.Replace("(86)", "").Trim(),
                                    @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})\s(?<note>.+)\s(?<date1>\d{2}\/\d{2}\/\d{4})");

                                if (match1.Success)
                                {
                                    statusEvent.Biblio.IntConvention.PctApplNumber = match1.Groups["num"].Value.Trim();
                                    statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture)
                                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                                    statusEvent.LegalEvent.Note = "|| " + match1.Groups["note"].Value.Trim().TrimEnd(':').Trim() + " | " + DateTime.Parse(match1.Groups["date1"].Value.Trim(), culture)
                                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n";

                                    statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                                    {
                                        Language = "EN",
                                        Type = "INID",
                                        Tr = "|| Date of request for substantive examination | " + DateTime.Parse(match1.Groups["date1"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n"
                                    });
                                }
                                else
                                {
                                    var match2 = Regex.Match(inid.Replace("(86)", "").Trim(),
                                        @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                                    if (match2.Success)
                                    {
                                        statusEvent.Biblio.IntConvention.PctApplNumber = match2.Groups["num"].Value.Trim();
                                        statusEvent.Biblio.IntConvention.PctApplDate = DateTime.Parse(match2.Groups["date"].Value.Trim(), culture)
                                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                    }
                                    else Console.WriteLine($"{inid} --- 86");
                                }
                            }
                        }
                    }
                    else if (inid.StartsWith("(87)"))
                    {
                        var match = Regex.Match(inid.Replace("(87)", "").Trim(),
                            @"(?<num>.+)\s?(?<kind>[A-Z]\d{1,2})\s(?<date>\d{2}\/\d{2}\/\d{4})\s(?<note>.+)\s(?<date1>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.IntConvention.PctPublNumber = match.Groups["num"].Value.Trim();
                            statusEvent.Biblio.IntConvention.PctPublKind = match.Groups["kind"].Value.Trim();
                            statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                            statusEvent.LegalEvent.Note = "|| " + match.Groups["note"].Value.Trim().TrimEnd(':').Trim() + " | " + DateTime.Parse(match.Groups["date1"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n";

                            statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                            {
                                Language = "EN",
                                Type = "INID",
                                Tr = "|| Date of request for substantive examination | " + DateTime.Parse(match.Groups["date1"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n"
                            });
                        }
                        else
                        {
                            var matchKind = Regex.Match(inid.Replace("(87)", "").Trim(),
                                @"(?<num>.+)\s?(?<kind>[A-Z]\d{1,2})\s(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (matchKind.Success)
                            {
                                statusEvent.Biblio.IntConvention.PctPublNumber = matchKind.Groups["num"].Value.Trim();
                                statusEvent.Biblio.IntConvention.PctPublKind = matchKind.Groups["kind"].Value.Trim();
                                statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(matchKind.Groups["date"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else
                            {
                                var match1 = Regex.Match(inid.Replace("(87)", "").Trim(),
                                    @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})\s(?<note>.+)\s(?<date1>\d{2}\/\d{2}\/\d{4})");

                                if (match1.Success)
                                {
                                    statusEvent.Biblio.IntConvention.PctPublNumber = match1.Groups["num"].Value.Trim();
                                    statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match1.Groups["date"].Value.Trim(), culture)
                                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                                    statusEvent.LegalEvent.Note = "|| " + match1.Groups["note"].Value.Trim().TrimEnd(':').Trim() + " | " + DateTime.Parse(match1.Groups["date1"].Value.Trim(), culture)
                                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n";
                                    statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                                    {
                                        Language = "EN",
                                        Type = "INID",
                                        Tr = "|| Date of request for substantive examination | " + DateTime.Parse(match1.Groups["date1"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n"
                                    });
                                }
                                else
                                {
                                    var match2 = Regex.Match(inid.Replace("(87)", "").Trim(),
                                        @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                                    if (match2.Success)
                                    {
                                        statusEvent.Biblio.IntConvention.PctPublNumber = match2.Groups["num"].Value.Trim();
                                        statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(match2.Groups["date"].Value.Trim(), culture)
                                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                    }
                                    else Console.WriteLine($"{inid} --- 87");
                                }
                            }
                        }
                    }
                    else if (inid.StartsWith("(51)"))
                    {
                        var ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @";")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Ipc()
                            {
                                Class = ipc.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(71)"))
                    {
                        var applicants = Regex.Split(inid.Replace("(71)", "").Trim(), @"(?=\d{1,3}\..+)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var match = Regex.Match(applicant.Trim(),
                                @"(\d{1,3}\.\s)?(?<name>.+)\((?<code>\D{2})\)\s(?<adress>.+),");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim(),
                                    Language = match.Groups["code"].Value.Trim() switch
                                    {
                                        "VN" => "VI",
                                        _ => "EN"
                                    }
                                });
                            }
                            else Console.WriteLine($"{applicant} --- 71");
                        }
                    }
                    else if (inid.StartsWith("(72)"))
                    {
                        var inventors = Regex.Split(inid.Replace("(72)", "").Trim(), @";")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            var match = Regex.Match(inventor.Trim(), @"(?<name>.+)\((?<code>\D{2})\)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim(),
                                    Language = match.Groups["code"].Value.Trim() switch
                                    {
                                        "VN" => "VI",
                                        _ => "EN"
                                    }
                                });
                            }
                            else Console.WriteLine($"{inventor} --- 72");
                        }
                    }
                    else if (inid.StartsWith("(74)"))
                    {
                        var match = Regex.Match(inid.Replace("(74)", "").Trim(), @"(?<name>.+)\((?<english>.+)\)");

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
                    else if (inid.StartsWith("(30)"))
                    {
                        var match = Regex.Match(inid.Replace("(30)", "").Trim(),
                            @"(?<inid30>.+)\s(?<note>Ngày.+)\s(?<noteDate>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.LegalEvent.Note = "|| " + match.Groups["note"].Value.Trim().TrimEnd(':').Trim() + " | " +
                                                          DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            statusEvent.LegalEvent.Language = "VI";
                            statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                            {
                                Language = "EN",
                                Type = "INID",
                                Tr = "|| Date of request for substantive examination | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n"
                            });


                            var priorities = Regex.Split(match.Groups["inid30"].Value.Trim(), @"(?<=\s[A-Z]{2}\s)")
                                .Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var priority in priorities)
                            {
                                var prior = Regex.Match(priority,
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
                                    var match1 = Regex.Match(priority.Trim(),
                                        @"(?<note>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");
                                    if (match1.Success)
                                    {
                                        statusEvent.LegalEvent.Note = "|| " + match1.Groups["note"].Value.Trim().TrimEnd(':').Trim() + " | " +
                                                                      DateTime.Parse(match1.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                                    }
                                    else Console.WriteLine($"{priority} --- 30");
                                }
                            }
                        }
                        else
                        {
                            var priorities = Regex.Split(inid.Replace("(30)", "").Trim(), @"(?<=\s[A-Z]{2}\s)")
                                .Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var priority in priorities)
                            {
                                var prior = Regex.Match(priority,
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
                            }
                        }
                    }
                    else if (inid.StartsWith("(62)"))
                    {
                        statusEvent.Biblio.Related.Add(new RelatedDocument()
                        {
                            Source = "62",
                            Number = inid.Replace("(62)", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(67)"))
                    {
                        statusEvent.Biblio.Related.Add(new RelatedDocument()
                        {
                            Source = "67",
                            Number = inid.Replace("(67)", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(75)"))
                    {
                        var invOrApps = Regex.Split(inid.Replace("(75)", "").Trim(), @"(?=\d{1,3}\..+)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var invOrApp in invOrApps)
                        {
                            var match = Regex.Match(invOrApp.Trim(),
                                @"(\d{1,3}\.\s)?(?<name>.+)\((?<code>\D{2})\)\s(?<adress>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.InvOrApps.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim(),
                                    Language = match.Groups["code"].Value.Trim() switch
                                    {
                                        "VN" => "VI",
                                        _ => "EN"
                                    }
                                });
                            }
                            else Console.WriteLine($"{invOrApp} --- 71");
                        }
                    }
                    else Console.WriteLine($"{inid}");

                    statusEvent.LegalEvent.Language = "VI";
                }
            }
            else if (subCode is "14" or "15")
            {
                List<string> inids = new();
                var splitTextAndField57 = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<allinids>.+)(?<inid57>\(57\)\s.+)");

                if (splitTextAndField57.Success)
                {
                    var match = Regex.Match(splitTextAndField57.Groups["allinids"].Value.Trim(), @"(?<allI>.+)(?<i30>\(30\).+)(?<i87>\(87\).+?\d{2}\/\d{2}\/\d{4})(?<i30c>.+)(?<allIc>\(51\).+)");

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

                foreach (var inid in inids)
                {
                    if (inid.StartsWith("(11)"))
                    {
                        var match = Regex.Match(inid.Replace("(11)", "").Trim(), @"(?<num>.+)\s(?<kind>\D)");
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["num"].Value.Trim();
                            if (subCode is "15")
                            {
                                if (match.Groups["kind"].Value.Trim() is "B")
                                {
                                    statusEvent.Biblio.Publication.Kind = "Y";
                                }
                                else statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                            }
                            else statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else Console.WriteLine($"{inid} -- 11");
                    }
                    else if (inid.StartsWith("(15)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.LegalEvent.Note = "|| (15) Ngày cấp | " + match.Groups["date"].Value.Trim() + "\n";
                            statusEvent.LegalEvent.Language = "VI";
                            statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                            {
                                Language = "EN",
                                Type = "INID",
                                Tr = "|| (15) Date of grant | " + match.Groups["date"].Value.Trim() + "\n"
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
                        var match = Regex.Match(inid.Trim(), @"(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.DOfPublication.date_45 = DateTime.Parse(match.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else Console.WriteLine($"{inid} -- 45");
                    }
                    else if (inid.StartsWith("(43)"))
                    {
                        var match = Regex.Match(inid.Trim(), @"(?<date>\d{4}\-\d{2}\-\d{2})");

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Date = match.Groups["date"].Value.Trim().Replace("-", "/").Trim();
                        }
                        else
                        {
                            var match2 = Regex.Match(inid.Trim(), @"(?<date>\d{2}.\d{2}.\d{4})");

                            if (match2.Success)
                            {
                                statusEvent.Biblio.Publication.Date = DateTime.Parse(match2.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }
                            else Console.WriteLine($"{inid} -- 43");
                        }
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
                        var match = Regex.Match(inid.Replace("(86)", "").Trim(),
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
                        var match = Regex.Match(inid.Replace("(74)", "").Trim(), @"(?<name>.+)\((?<english>\D+)\)");

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
                        var ipcs = Regex.Split(inid.Replace("(51)", "").Trim(), @";")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var ipc in ipcs)
                        {
                            statusEvent.Biblio.Ipcs.Add(new Ipc()
                            {
                                Class = ipc.Trim()
                            });
                        }
                    }
                    else if (inid.StartsWith("(30)"))
                    {
                        var priorities = Regex.Split(inid.Replace("(30)", "").Trim(), @"(?<=\s[A-Z]{2}\s)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var priority in priorities)
                        {
                            var prior = Regex.Match(priority,
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
                        var inventors = Regex.Split(inid.Replace("(72)", "").Trim(), @";")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inventor in inventors)
                        {
                            var match = Regex.Match(inventor.Trim(), @"(?<name>.+)\((?<code>\D{2})\)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim(),
                                    Language = match.Groups["code"].Value.Trim() switch
                                    {
                                        "VN" => "VI",
                                        _ => "EN"
                                    }
                                });
                            }
                            else Console.WriteLine($"{inventor} --- 72");
                        }
                    }
                    else if (inid.StartsWith("(87)"))
                    {
                        var match = Regex.Match(inid.Replace("(87)", "").Trim(),
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
                            var match1 = Regex.Match(inid.Replace("(87)", "").Trim(),
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
                            Source = "62",
                            Number = inid.Replace("(62)", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(67)"))
                    {
                        statusEvent.Biblio.Related.Add(new RelatedDocument()
                        {
                            Source = "67",
                            Number = inid.Replace("(67)", "").Trim()
                        });
                    }
                    else if (inid.StartsWith("(73)"))
                    {
                        var assignees = Regex.Split(inid.Replace("(73)", "").Trim(), @"(?=\d{1,3}\..+)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var assignee in assignees)
                        {
                            var match = Regex.Match(assignee.Trim(),
                                @"(\d{1,3}\.\s)?(?<name>.+)\((?<code>\D{2})\)\s(?<adress>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.Assignees.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim(),
                                    Language = match.Groups["code"].Value.Trim() switch
                                    {
                                        "VN" => "VI",
                                        _ => "EN"
                                    }
                                });
                            }
                            else Console.WriteLine($"{assignee} --- 73");
                        }
                    }
                    else if (inid.StartsWith("(76)"))
                    {
                        var invAppGrants = Regex.Split(inid.Replace("(76)", "").Trim(), @"(?=\d{1,3}\..+)")
                            .Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var inv in invAppGrants)
                        {
                            var match = Regex.Match(inv.Trim(),
                                @"(\d{1,3}\.\s)?(?<name>.+)\((?<code>\D{2})\)\s(?<adress>.+)");

                            if (match.Success)
                            {
                                statusEvent.Biblio.InvAppGrants.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim(),
                                    Language = match.Groups["code"].Value.Trim() switch
                                    {
                                        "VN" => "VI",
                                        _ => "EN"
                                    }
                                });
                            }
                            else Console.WriteLine($"{inv} --- 76");
                        }
                    }
                    else Console.WriteLine($"{inid}");
                }
            }
            else if (subCode is "16" or "17")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @".+ng.y\s(?<evDate>\d{2}\/\d{2}\/\d{4}).+đ.n\s(?<pubNum>\d+)\s?(?<noteDate>\d{2}\/\d{2}\/\d{4})\s.+(?<noteDate2>\d{2}\/\d{2}\/\d{4})\s.+?\)(?<name>.+)\((?<code>[A-Z]{2})\)\s?(?<adress>.+)");

                if (match.Success)
                {
                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["evDate"].Value.Trim(), culture).ToString("yyyy/MM/dd").Replace(".", "/").Trim();


                    if (subCode is "16") statusEvent.Biblio.Publication.Number = "1-" + match.Groups["pubNum"].Value.PadLeft(7, '0').Trim();
                    else statusEvent.Biblio.Publication.Number = "2-" + match.Groups["pubNum"].Value.PadLeft(7, '0').Trim();

                    statusEvent.Biblio.Assignees.Add(new PartyMember()
                    {
                        Name = match.Groups["name"].Value.Replace("Chủ văn bằng bảo hộ:", "").Trim(),
                        Country = match.Groups["code"].Value.Trim(),
                        Address1 = match.Groups["adress"].Value.Trim()
                    });

                    statusEvent.LegalEvent.Language = "VI";
                    statusEvent.LegalEvent.Note = "|| (15) Ngày cấp | " + DateTime
                                                      .Parse(match.Groups["noteDate"].Value.Trim(), culture)
                                                      .ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n"
                                                  + "|| Hiệu lực được duy trì đến | " +
                                                  DateTime.Parse(match.Groups["noteDate2"].Value.Trim(), culture)
                                                      .ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n";

                    statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                    {
                        Language = "EN",
                        Type = "INID",
                        Tr = "|| (15) Grant date | " + DateTime
                                 .Parse(match.Groups["noteDate"].Value.Trim(), culture)
                                 .ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n"
                             + "|| Valid until | " +
                             DateTime.Parse(match.Groups["noteDate2"].Value.Trim(), culture)
                                 .ToString("yyyy/MM/dd").Replace(".", "/").Trim() + "\n"
                    });
                }
                else Console.WriteLine($"{note.Replace("\r", "").Replace("\n", " ").Trim()}");
            }
            else if (subCode is "18" or "19")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @".+ng.y\s(?<evDate>\d{2}\/\d{2}\/\d{4}).+th.\s(?<pubNum>\d+)\s(?<noteDate>\d{1,2}\/\d{1,2}\/\d{4})\s(?<note>\d+)");

                if (match.Success)
                {
                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["evDate"].Value.Trim(), culture)
                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    if (subCode is "18") statusEvent.Biblio.Publication.Number = "1-" + match.Groups["pubNum"].Value.PadLeft(7, '0').Trim();
                    else statusEvent.Biblio.Publication.Number = "2-" + match.Groups["pubNum"].Value.PadLeft(7, '0').Trim();

                    var date = Regex.Match(match.Groups["noteDate"].Value.Trim(),
                        @"(?<day>\d+)\/(?<month>\d)\/(?<year>\d+)");

                    if (date.Success)
                    {
                        statusEvent.LegalEvent.Note =
                                "|| (15) Ngày bằng | " + date.Groups["year"].Value.Trim() + "/0" +
                                date.Groups["month"].Value.Trim() + "/" + date.Groups["day"].Value.Trim() + "\n"
                                + "|| Cấp lại lần thứ | " + match.Groups["note"].Value.Trim();
                        statusEvent.LegalEvent.Language = "VI";

                        statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                        {
                            Language = "EN",
                            Type = "INID",
                            Tr = "|| (15) Grant date | " + date.Groups["year"].Value.Trim() + "/0" +
                                 date.Groups["month"].Value.Trim() + "/" + date.Groups["day"].Value.Trim() + "\n"
                                 + "|| Reissue № | " + match.Groups["note"].Value.Trim()
                        });
                    }
                    else
                    {
                        statusEvent.LegalEvent.Note =
                            "|| (15) Ngày bằng | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n"
                            + "|| Cấp lại lần thứ | " + match.Groups["note"].Value.Trim();

                        statusEvent.LegalEvent.Language = "VI";

                        statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                        {
                            Language = "EN",
                            Type = "INID",
                            Tr = "|| (15) Grant date | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n"
                                 + "|| Reissue № | " + match.Groups["note"].Value.Trim()
                        });
                    }
                }
                else Console.WriteLine($"{note}");
            }
            else if (subCode is "20")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @".+ng.y\s(?<evDate>\d{2}\/\d{2}\/\d{4}).+:\s\d-(?<pubNum>\d.+)\s(?<noteDate>\d{2}\/\d{2}\/\d{4}).+?:(?<name>.+)Đ.a.+:(?<adress>.+)");

                if (match.Success)
                {
                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["evDate"].Value.Trim(), culture)
                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    statusEvent.Biblio.Publication.Number = "1-" + match.Groups["pubNum"].Value.PadLeft(7, '0').Trim();

                    statusEvent.NewBiblio.Agents.Add(new PartyMember()
                    {
                        Name = match.Groups["name"].Value.Trim(),
                        Address1 = match.Groups["adress"].Value.Trim(),
                        Country = "VN",
                        Language = "VI"
                    });

                    statusEvent.LegalEvent.Language = "VI";
                    statusEvent.LegalEvent.Note = "|| (15) Ngày cấp | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture)
                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                    {
                        Language = "EN",
                        Type = "INID",
                        Tr = "|| (15) Grant date | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture)
                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim()
                    });
                }
            }
            else if (subCode is "22")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @".y\s(?<evDate>\d{2}\/\d{2}\/\d{4}).+c.p:\s?(?<noteDate>\d{2}\/\d{2}\/\d{4}).+b.n\ss.\s(?<noteNum>\d+).+s.:\s?(?<pubNum>\d+).+chung:(?<name>\D+)\(?(?<adress>.+),(?<country>.+)\s");

                if (match.Success)
                {
                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["evDate"].Value.Trim(), culture)
                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    statusEvent.Biblio.Publication.Number = "1-" + match.Groups["pubNum"].Value.PadLeft(7, '0').Trim();

                    statusEvent.Biblio.Assignees.Add(new PartyMember()
                    {
                        Name = match.Groups["name"].Value.Trim(),
                        Address1 = match.Groups["adress"].Value.Replace("_", "").Trim(),
                        Language = MakeCountryCode(match.Groups["country"].Value.Trim()) switch
                        {
                            "VN" => "VI",
                            _ => "EN"
                        },
                        Country = MakeCountryCode(match.Groups["country"].Value.Trim())
                    });

                    if (MakeCountryCode(match.Groups["country"].Value.Trim()) is null) Console.WriteLine(match.Groups["country"].Value.Trim());

                    statusEvent.LegalEvent.Note = "|| (15) Ngày cấp | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture)
                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim() + '\n'
                        + "|| Cấp phó bản số | " + match.Groups["noteNum"].Value.Trim();

                    statusEvent.LegalEvent.Language = "VI";
                    statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                    {
                        Language = "EN",
                        Tr = "|| (15) Grant date | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim(), culture)
                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim() + '\n'
                        + "|| Digital copy number | " + match.Groups["noteNum"].Value.Trim(),
                        Type = "INID"
                    });
                }
                else Console.WriteLine($"{note}");
            }
            else if (subCode is "23" or "24")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"(?<appNum>.+?)\s(?<pubNum>.+)\s(?<pubDate>\d{2}\/\d{2}\/\d{4})\s(?<evDate>\d{2}\/\d{2}\/\d{4})\s(?<ipcs>.+\/\d+)\s?");

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
            else if (subCode is "29")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"ng.y\s(?<evDate>\d{2}\/\d{2}\/\d{4}).+?s.\s(?<pubNum>\d+)\s(?<noteDate>\d{2}\/\d+\/\d{4})\s(?<noteInf>\d+).+:(?<f73name>.+)\((?<f73Code>[A-Z]{2})\)(?<f73Adress>.+)");

                if (match.Success)
                {
                    statusEvent.Biblio.Publication.Number = "2-" + match.Groups["pubNum"].Value.PadLeft(7, '0').Trim();

                    statusEvent.Biblio.Assignees.Add(new PartyMember()
                    {
                        Name = match.Groups["f73name"].Value.Trim(),
                        Address1 = match.Groups["f73Adress"].Value.Trim(),
                        Country = match.Groups["f73Code"].Value.Trim()
                    });

                    statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["evDate"].Value.Trim())
                        .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                    var noteMatch = Regex.Match(match.Groups["noteDate"].Value.Trim(), @"(?<day>\d{2})\/(?<month>\d)\/(?<year>\d{4})");

                    if (noteMatch.Success)
                    {
                        statusEvent.LegalEvent.Language = "VI";
                        statusEvent.LegalEvent.Note = "|| (15) Ngày cấp bằng | " + noteMatch.Groups["year"].Value.Trim() + "/0" + noteMatch.Groups["month"].Value.Trim() + "/" + noteMatch.Groups["day"].Value.Trim() + "\n"
                                                      + "|| Cấp phó bản số | " + match.Groups["noteInf"].Value.Trim();

                        statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                        {
                            Language = "EN",
                            Type = "INID",
                            Tr = "|| (15) Grant date | " + noteMatch.Groups["year"].Value.Trim() + "/0" + noteMatch.Groups["month"].Value.Trim() + "/" + noteMatch.Groups["day"].Value.Trim() + "\n"
                            + "|| Digital copy number | " + match.Groups["noteInf"].Value.Trim()
                        });
                    }
                    else
                    {
                        statusEvent.LegalEvent.Language = "VI";
                        statusEvent.LegalEvent.Note = "|| (15) Ngày cấp bằng | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim()).ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n"
                                                      + "|| Cấp phó bản số | " + match.Groups["noteInf"].Value.Trim();

                        statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                        {
                            Language = "EN",
                            Type = "INID",
                            Tr = "|| (15) Grant date | " + DateTime.Parse(match.Groups["noteDate"].Value.Trim()).ToString("yyyy.MM.dd").Replace(".", "/").Trim() + "\n"
                                 + "|| Digital copy number | " + match.Groups["noteInf"].Value.Trim()
                        });
                    }
                }
            }

            //Add image to the record Abstract section
            AddAbstractScreenShot(statusEvent, imagesDictionary);

            return statusEvent;
        }

        private void AddAbstractScreenShot(LegalStatusEvent statusEvent, Dictionary<string, string> imagesDictionary)
        {
            var kind = string.Empty;
            if (statusEvent.SubCode is "12" or "13" or "14" or "15")
            {
                if (statusEvent.Biblio.Publication.Kind == "U")
                {
                    kind = "A";
                }
            }

            var patentKey = statusEvent.Biblio.Publication.Number + " " + kind;
            if (string.IsNullOrWhiteSpace(patentKey) || imagesDictionary == null)
            {
                return;
            }

            var patentImageInfo = imagesDictionary.Where(pair => pair.Value == patentKey)
                .Select(pair => pair.Key)
                .ToList();

            if (patentImageInfo.Any())
            {
                var pathToFolder = Path.GetDirectoryName(_currentFileName);
                if (pathToFolder != null)
                {
                    foreach (var image in patentImageInfo)
                    {
                        var pathToImageFile = Path.Combine(pathToFolder, image);
                        var imageString = ConvertTiffToPngString(pathToImageFile);
                        if (!string.IsNullOrWhiteSpace(imageString))
                        {
                            var imageValue = $"data:image/png;base64,{imageString}";
                            var id = GetUniqueScreenShotId();
                            var idText = $"<img id=\"{id}\">";
                            var tmpAbstract = statusEvent.Biblio.Abstracts.FirstOrDefault();
                            if (tmpAbstract == null)
                            {
                                return;
                            }
                            tmpAbstract.Text += idText;
                            statusEvent.Biblio.Abstracts = new List<Abstract> { tmpAbstract };
                            statusEvent.Biblio.ScreenShots.Add(new ScreenShot()
                            {
                                Id = id,
                                Data = imageValue
                            });
                        }
                    }
                }
            }
        }
        private string ConvertTiffToPngString(string path)
        {
            using var image = SixLabors.ImageSharp.Image.Load(path);
            using var extractedImageStream = new MemoryStream();
            image.SaveAsPng(extractedImageStream);
            var extractedImageFrameBytes = extractedImageStream.ToArray();
            return Convert.ToBase64String(extractedImageFrameBytes);
        }
        internal static string GetUniqueScreenShotId()
        {
            return $"{GetRandomString(4)}-{GetRandomString(12)}";
        }
        private static string GetRandomString(int stringLength)
        {
            var sb = new StringBuilder();
            var guIds = (stringLength - 1) / 32 + 1;
            for (var i = 1; i <= guIds; i++)
            {
                sb.Append(Guid.NewGuid().ToString("N"));
            }
            return sb.ToString(0, stringLength);
        }

        internal string MakeText(List<XElement> xElements, string subCode)
        {
            return xElements.Aggregate<XElement, string>(null, (current, xElement) => current + xElement.Value + " ").Trim();
        }
        internal string MakeCountryCode(string country) => country switch
        {
            "Japan" => "JP",
            _ => null
        };
        internal List<Diamond.Core.Models.LegalStatusEvent> MakeListPatent(string note, string subCode, string sectionCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legal = new();

            CultureInfo culture = new("ru-RU");

            if (subCode is "3" or "4")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"ng.y\s(?<evDate>\d{2}\/\d{2}\/\d{4}).+ch.\s..n:(?<f74>.+)N.i.+?\schuy.n\snh..ng:(?<f73>.+)B.n\s...c.+?:(?<f73new>.+?)\s..i\s.+?\s(?<f54>1.+)\sGi.");

                if (match.Success)
                {
                    var information = Regex.Split(match.Groups["f54"].Value.Trim(), @"(?=\d\s\D.+)")
                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var inf in information)
                    {
                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "VN",
                            SubCode = subCode,
                            SectionCode = sectionCode,
                            Id = _id++,
                            GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
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

                        statusEvent.Biblio.Agents.Add(new PartyMember()
                        {
                            Name = match.Groups["f74"].Value.Trim(),
                            Language = "VI"
                        });

                        statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["evDate"].Value.Trim(), culture)
                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        var assignes = Regex.Match(match.Groups["f73"].Value.Trim(),
                            @"(?<name>.+)\((?<code>[A-Z]{2})\)(?<adress>.+)");

                        if (assignes.Success)
                        {
                            if (assignes.Groups["code"].Value.Trim() is "VN")
                            {
                                statusEvent.Biblio.Assignees.Add(new PartyMember()
                                {
                                    Name = assignes.Groups["name"].Value.Trim(),
                                    Country = assignes.Groups["code"].Value.Trim(),
                                    Address1 = assignes.Groups["adress"].Value.Trim(),
                                    Language = "VI"
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Assignees.Add(new PartyMember()
                                {
                                    Name = assignes.Groups["name"].Value.Trim(),
                                    Country = assignes.Groups["code"].Value.Trim(),
                                    Address1 = assignes.Groups["adress"].Value.Trim(),
                                    Language = "EN"
                                });
                            }
                        }
                        else Console.WriteLine($"{match.Groups["f73"].Value.Trim()} ---- 73");

                        var assignesNew = Regex.Match(match.Groups["f73new"].Value.Trim(),
                            @"(?<name>.+)\((?<code>[A-Z]{2})\)(?<adress>.+)");

                        if (assignesNew.Success)
                        {
                            if (assignesNew.Groups["code"].Value.Trim() is "VN")
                            {
                                statusEvent.NewBiblio.Assignees.Add(new PartyMember()
                                {
                                    Name = assignesNew.Groups["name"].Value.Trim(),
                                    Country = assignesNew.Groups["code"].Value.Trim(),
                                    Address1 = assignesNew.Groups["adress"].Value.Trim(),
                                    Language = "VI"
                                });
                            }
                            else
                            {
                                statusEvent.NewBiblio.Assignees.Add(new PartyMember()
                                {
                                    Name = assignesNew.Groups["name"].Value.Trim(),
                                    Country = assignesNew.Groups["code"].Value.Trim(),
                                    Address1 = assignesNew.Groups["adress"].Value.Trim(),
                                    Language = "EN"
                                });
                            }
                        }
                        else Console.WriteLine($"{match.Groups["f73new"].Value.Trim()} ---- 73new");

                        var info = Regex.Match(inf.Trim(),
                            @"\d\s(?<f54>.+?)\s(?<f11>\d+)\s(?<note>\d{2}\/\d{2}\/\d{4})");

                        if (info.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "VI",
                                Text = info.Groups["f54"].Value.Trim()
                            });

                            if (subCode is "3") statusEvent.Biblio.Publication.Number = "1-" + info.Groups["f11"].Value.PadLeft(7, '0').Trim();
                            else statusEvent.Biblio.Publication.Number = "2-" + info.Groups["f11"].Value.PadLeft(7, '0').Trim();

                            statusEvent.LegalEvent.Note = "|| Ngày cấp | " + DateTime.Parse(info.Groups["note"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            statusEvent.LegalEvent.Language = "VI";

                            statusEvent.LegalEvent.Translations.Add(new NoteTranslation()
                            {
                                Language = "EN",
                                Tr = "|| Grant date | " + DateTime.Parse(info.Groups["note"].Value.Trim(), culture)
                                    .ToString("yyyy.MM.dd").Replace(".", "/").Trim(),
                                Type = "INID"
                            });
                        }
                        else Console.WriteLine($"{info} ---- note field");

                        legal.Add(statusEvent);
                    }
                }
            }
            else if (subCode is "7")
            {
                var match = Regex.Match(note.Replace("\r", "").Replace("\n", " ").Trim(),
                    @"ng.y\s(?<evDate>\d{2}\/\d{2}\/\d{4}).+đ.n(?<info>.+?)B.n.+?:(?<f71>.+)B.n.+?:(?<f71n>.+)");

                if (match.Success)
                {
                    var infoList = Regex.Split(match.Groups["info"].Value.Trim(), @"(?<=\d{2}\/\d{2}\/\d{4})")
                        .Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var inf in infoList)
                    {
                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "VN",
                            SubCode = subCode,
                            SectionCode = sectionCode,
                            Id = _id++,
                            GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
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

                        statusEvent.LegalEvent.Date = DateTime.Parse(match.Groups["evDate"].Value.Trim(), culture)
                            .ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        var applicants = Regex.Match(match.Groups["f71"].Value.Trim(),
                            @"(?<name>.+)\((?<code>[A-Z]{2})\)(?<adress>.+)");

                        if (applicants.Success)
                        {
                            if (applicants.Groups["code"].Value.Trim() is "VN")
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = applicants.Groups["name"].Value.Trim(),
                                    Country = applicants.Groups["code"].Value.Trim(),
                                    Address1 = applicants.Groups["adress"].Value.Replace("_", "").Trim(),
                                    Language = "VI"
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = applicants.Groups["name"].Value.Trim(),
                                    Country = applicants.Groups["code"].Value.Trim(),
                                    Address1 = applicants.Groups["adress"].Value.Replace("_", "").Trim(),
                                    Language = "EN"
                                });
                            }
                        }
                        else Console.WriteLine($"{match.Groups["f71"].Value.Trim()} ---- 71");

                        var applicantsNew = Regex.Match(match.Groups["f71n"].Value.Trim(),
                            @"(?<name>.+)\((?<code>[A-Z]{2})\)(?<adress>.+)");

                        if (applicantsNew.Success)
                        {
                            if (applicantsNew.Groups["code"].Value.Trim() is "VN")
                            {
                                statusEvent.NewBiblio.Applicants.Add(new PartyMember()
                                {
                                    Name = applicantsNew.Groups["name"].Value.Trim(),
                                    Country = applicantsNew.Groups["code"].Value.Trim(),
                                    Address1 = applicantsNew.Groups["adress"].Value.Replace("_", "").Trim(),
                                    Language = "VI"
                                });
                            }
                            else
                            {
                                statusEvent.NewBiblio.Applicants.Add(new PartyMember()
                                {
                                    Name = applicantsNew.Groups["name"].Value.Trim(),
                                    Country = applicantsNew.Groups["code"].Value.Trim(),
                                    Address1 = applicantsNew.Groups["adress"].Value.Replace("_", "").Trim(),
                                    Language = "EN"
                                });
                            }
                        }
                        else Console.WriteLine($"{match.Groups["f71n"].Value.Trim()} ---- 71new");

                        var matchInf = Regex.Match(inf.Trim(), @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                        if (matchInf.Success)
                        {
                            statusEvent.Biblio.Application.Number = matchInf.Groups["num"].Value.Trim();
                            statusEvent.Biblio.Application.Date = DateTime.Parse(matchInf.Groups["date"].Value.Trim(), culture)
                                .ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }

                        legal.Add(statusEvent);
                    }
                }
            }
            return legal;
        }
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
        private string GetImageString(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return null;
            }
            try
            {
                var data = File.ReadAllBytes(path);
                return Convert.ToBase64String(data);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Reading image to string failed. {e.Message}");
                throw;
            }
        }
    }
}
