using Diamond.Core.Models;
using Integration;
using SixLabors.ImageSharp;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_CY_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<LegalStatusEvent> Start(string path, string subCode)
        {
            var statusEvents = new List<LegalStatusEvent>();
            var directory = new DirectoryInfo(path);
            var files = directory.GetFiles("*.tetml", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

            XElement tet;
            var xElements = new List<XElement>();

            foreach (var tetml in files)
            {
                _currentFileName = tetml;
                tet = XElement.Load(tetml);

                if (subCode == "3")
                {
                    var imageFiles = GetImages(tet);
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("Β. ΑΝΑΚΟΙΝΩΣΗ ΓΙΑ ΚΑΤΑΧΩΡΗΣΗ ΜΕΤΑΦΡΑΣΗΣ ΕΥΡΩΠΑΪΚΩΝ ΔΙΠΛΩΜΑΤΩΝ ΕΥΡΕΣΙΤΕΧΝΙΑΣ"))
                        .TakeWhile(val => !val.Value.StartsWith("ΚΕΦΑΛΑΙΟ 3"))
                        .ToList();

                    var text = MakeText(xElements, subCode);

                    var notes = Regex.Split(text, @"(?=\(11\).+)", RegexOptions.Singleline)
                                             .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("(11)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakeNotes(note, subCode, "FG", imageFiles));
                    }
                }

                if (subCode == "57")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("ββ) Οι πιο κάτω Μεταφράσεις Ευρωπαϊκών"))
                        .TakeWhile(val => !val.Value.StartsWith("ΔΙΑΓΡΑΦΕΣ ΣΥΜΠΛΗΡΩΜΑΤΙΚΩΝ ΠΙΣΤΟΠΟΙΗΤΙΚΩΝ ΠΡΟΣΤΑΣΙΑΣ ΓΙΑ ΤΑ ΦΑΡΜΑΚΑ (ΣΠΠΦ)"))
                        .ToList();

                    var text = MakeText(xElements, subCode);

                    var notes = Regex.Split(text, @"(?=CY\s?\d+\s\()", RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("CY")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakeNotes(note, subCode, "MK"));
                    }
                }

                if (subCode == "39")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("αα) Τα πιο κάτω Διπλώματα Ευρεσιτεχνίας"))
                        .TakeWhile(val => !val.Value.StartsWith("ΔΙΑΓΡΑΦΕΣ ΜΕΤΑΦΡΑΣΕΩΝ ΕΥΡΩΠΑΪΚΩΝ ΔΙΠΛΩΜΑΤΩΝ ΕΥΡΕΣΙΤΕΧΝΙΑΣ"))
                        .ToList();

                    var text = MakeText(xElements, subCode);

                    var notes = Regex.Split(text, @"(?=CY\s?\d+\s\()", RegexOptions.Singleline)
                        .Where(val => !string.IsNullOrEmpty(val) && val.StartsWith("CY")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakeNotes(note, subCode, "MK"));
                    }
                }
            }
            return statusEvents;
        }
        private static string MakeText(List<XElement> xElements, string subCode)
        {
            var text = new StringBuilder();

            switch (subCode)
            {
                case "3":
                    {
                        foreach (var xElement in xElements)
                        {
                            text = text.AppendLine(xElement.Value + "\n");
                        }
                        break;
                    }
                case "57":
                {
                    foreach (var xElement in xElements)
                    {
                        text = text.AppendLine(xElement.Value + "\n");
                    }
                    break;
                }
                case "39":
                {
                    foreach (var xElement in xElements)
                    {
                        text = text.AppendLine(xElement.Value + "\n");
                    }
                    break;
                }
            }
            return text.ToString();
        }
        private Dictionary<string, string> GetImages(XElement tet)
        {
            var result = new Dictionary<string, string>();
            try
            {
                var iDAndFilenameInfo = new Dictionary<string, string>();

                var imagesList = tet.Descendants()
                    .Where(x => x.Name.LocalName == "Image")
                    .Select(x => (x.Attribute("id")?.Value, x.Attribute("filename")?.Value))
                    .ToList();

                foreach (var i in imagesList.Where(i => !iDAndFilenameInfo.ContainsKey(i.Item1)))
                {
                    iDAndFilenameInfo.Add(i.Item1, i.Item2);
                }

                var iDAndPatentKeyInfo = new Dictionary<string, string>();
                var pages = tet.Descendants().Elements().Where(x => x.Name.LocalName == "Page").ToList();

                var numberPattern = new Regex(@"(?=\(11\).+\s(?<Number>CY\d+\s?\D\d+))");
                var numberPattern_v2 = new Regex(@"(?<Number>CY\d+\s?\D\d+)");

                for (var i = 0; i < pages.Count; i++)
                {
                    var page = pages[i];
                    var pageImages = page.Descendants()
                        .Where(x => x.Name.LocalName == "PlacedImage")
                        .ToList();

                    if (!pageImages.Any())
                        continue;

                    string FindPatentNumberOnPage(XElement pageToCheck)
                    {
                        var texts = pageToCheck.Descendants().Where(x => x.Name.LocalName == "Text").ToList();
                        var patentTexts = texts.Where(x => numberPattern.Match(x.Value).Success).ToList();
                        if (patentTexts.Count == 0)
                        {
                            var patentTexts_v2 = texts.Where(x => numberPattern_v2.Match(x.Value).Success).ToList();
                            if (patentTexts_v2.Count == 1)
                            {
                                return numberPattern_v2.Match(patentTexts_v2.First().Value).Groups["Number"].Value.Trim();
                            }
                        }
                        if (patentTexts.Count == 1)
                        {
                            return numberPattern.Match(patentTexts.First().Value).Groups["Number"].Value.Trim();
                        }

                        return null;
                    }

                    var patentNumberValue = FindPatentNumberOnPage(page);

                    if (string.IsNullOrWhiteSpace(patentNumberValue) && i > 0)
                    {
                        patentNumberValue = FindPatentNumberOnPage(pages[i - 1]);
                    }
                    if (!string.IsNullOrWhiteSpace(patentNumberValue))
                    {
                        foreach (var item in pageImages)
                        {
                            var imageId = item.Attribute("image")?.Value;
                            if (imageId != null && !iDAndPatentKeyInfo.ContainsKey(imageId))
                            {
                                iDAndPatentKeyInfo.Add(imageId, patentNumberValue);
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

        internal LegalStatusEvent MakeNotes(string note, string subCode, string sectionCode,
            Dictionary<string, string> imagesDictionary = null)
        {
            LegalStatusEvent statusEvent = new()
            {
                CountryCode = "CY",
                SubCode = subCode,
                SectionCode = sectionCode,
                Id = _id++,
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Biblio = new Biblio(),
                LegalEvent = new LegalEvent()
            };

            var euPatent = new EuropeanPatent();

            if (subCode == "3")
            {
                var inids = Regex.Split(note.Trim(), @"(?=\(\d{2}\).+)", RegexOptions.Singleline).Where(_ => !string.IsNullOrEmpty(_)).ToList();

                foreach (var inid in inids)
                {
                    if (inid.StartsWith("(11)"))
                    {
                        var match = Regex.Match(inid, @"\(11\).+\s(?<number>CY\d+)\s?(?<kind>\D\d)",
                            RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Number = match.Groups["number"].Value.Trim();
                            statusEvent.Biblio.Publication.Kind = match.Groups["kind"].Value.Trim();
                        }
                        else
                        {
                            Console.WriteLine($"{inid} --- 11");
                        }
                    }
                    if (inid.StartsWith("(43)"))
                    {
                        var match = Regex.Match(inid, @"\(43\).+\s(?<date>\d{2}\/\d{2}\/\d{4})",
                            RegexOptions.Singleline);
                        if (match.Success)
                        {
                            statusEvent.Biblio.Publication.Date = DateTime
                                .ParseExact(match.Groups["date"].Value.Trim(), "dd/MM/yyyy",
                                    CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                        }
                        else
                        {
                            Console.WriteLine($"{inid} --- 43");
                        }
                    }
                    if (inid.StartsWith("(21)"))
                    {
                        var match = Regex.Match(inid, @"\(21\).+\s(?<number>CY\d+)",
                            RegexOptions.Singleline);
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Number = match.Groups["number"].Value.Trim();
                        }
                        else
                        {
                            Console.WriteLine($"{inid} --- 21");
                        }
                    }
                    if (inid.StartsWith("(22)"))
                    {
                        var match = Regex.Match(inid, @"\(22\).+\s(?<date>\d{2}\/\d{2}\/\d{4})",
                            RegexOptions.Singleline);
                        if (match.Success)
                        {
                            statusEvent.Biblio.Application.Date = DateTime
                                .ParseExact(match.Groups["date"].Value.Trim(), "dd/MM/yyyy",
                                    CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                        }
                        else
                        {
                            Console.WriteLine($"{inid} --- 22");
                        }
                    }
                    if (inid.StartsWith("(65)"))
                    {
                        if (inid.StartsWith("(65) Αριθμός Χορήγησης"))
                        {
                            var match = Regex.Match(inid, @"\(65\).+\s(?<number>EP\s?\d+\s?\d+\s?\d+)",
                                RegexOptions.Singleline);
                            if (match.Success)
                            {
                                euPatent.PubNumber = match.Groups["number"].Value.Trim();
                            }
                        }

                        if (inid.StartsWith("(65) Ημερομηνία Δημοσίευσης"))
                        {
                            var match2 = Regex.Match(inid, @"\(65\).+\s(?<number>EP\s?\d+\s?\d+\s?\d+).+(?<date>\d{2}\/\d{2}\/\d{4})",
                                RegexOptions.Singleline);
                            if (match2.Success)
                            {
                                euPatent.PubNumber = match2.Groups["number"].Value.Trim();
                                euPatent.PubDate = DateTime
                                    .ParseExact(match2.Groups["date"].Value.Trim(), "dd/MM/yyyy",
                                        CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                            }
                            else
                            {
                                match2 = Regex.Match(inid, @"\(65\).+\s(?<date>\d{2}\/\d{2}\/\d{4})",
                                    RegexOptions.Singleline);

                                if (match2.Success)
                                {
                                    euPatent.PubDate = DateTime
                                        .ParseExact(match2.Groups["date"].Value.Trim(), "dd/MM/yyyy",
                                            CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                                }
                                else
                                {
                                    Console.WriteLine($"{inid} --- (65) Ημερομηνία Δημοσίευσης");
                                }
                            }
                        }

                        if (inid.StartsWith("(65) Αριθμός Ευρωπ"))
                        {
                            var match3 = Regex.Match(inid, @"\(65\).+\s(?<number>EP\d+\.?\d+)",
                                RegexOptions.Singleline);
                            if (match3.Success)
                            {
                                euPatent.AppNumber = match3.Groups["number"].Value.Trim();
                            }
                            else
                            {
                                Console.WriteLine($"{inid} --- (65) Αριθμός Ευρωπ");
                            }
                        }

                        if (inid.StartsWith("(65) Ημερομηνία Κατάθεσης"))
                        {
                            var match4 = Regex.Match(inid, @"\(65\).+\s(?<number>EP\d+\.?\d+).+(?<date>\d{2}\/\d{2}\/\d{4})",
                                RegexOptions.Singleline);
                            if (match4.Success)
                            {
                                euPatent.AppNumber = match4.Groups["number"].Value.Trim();
                                euPatent.AppDate = DateTime
                                    .ParseExact(match4.Groups["date"].Value.Trim(), "dd/MM/yyyy",
                                        CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                            }
                            else
                            {
                                match4 = Regex.Match(inid, @"\(65\).+\s(?<date>\d{2}\/\d{2}\/\d{4})",
                                    RegexOptions.Singleline);

                                if (match4.Success)
                                {
                                    euPatent.AppDate = DateTime
                                        .ParseExact(match4.Groups["date"].Value.Trim(), "dd/MM/yyyy",
                                            CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                                }
                                else
                                {
                                    Console.WriteLine($"{inid} --- (65) Ημερομηνία Κατάθεσης");
                                }
                            }
                        }
                    }
                    if (inid.StartsWith("(54)"))
                    {
                        var match = Regex.Match(inid, @"\(54\).+\s�(?<title>.+)",
                            RegexOptions.Singleline);

                        if (match.Success)
                        {
                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Text = match.Groups["title"].Value.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Trim(),
                                Language = "EL"
                            });
                        }
                    }
                    if (inid.StartsWith("(73)"))
                    {
                        var cleanedInid = Regex.Replace(inid, @"^\s*$[\r\n]*", "", RegexOptions.Multiline);

                        var match = Regex.Match(cleanedInid, @"(?<name>.+)\n(?<adress>.+),(?<country>.+)");
                        if (match.Success)
                        {
                            var countryCode = MakeCountry(match.Groups["country"].Value.Trim());
                            if (countryCode == null)
                            {
                                Console.WriteLine(match.Groups["country"].Value.Trim());
                            }
                            else
                            {
                                statusEvent.Biblio.Assignees.Add(new PartyMember()
                                {
                                    Name = match.Groups["name"].Value.Trim(),
                                    Address1 = match.Groups["adress"].Value.Trim(),
                                    Country = countryCode
                                });
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{inid} --- 73");
                        }
                    }
                    if (inid.StartsWith("(72)"))
                    {
                        var pairs = ParseInventors(inid);
                        foreach (var pair in pairs)
                        {
                            var countryCode = MakeCountry(pair.Value);
                            if (countryCode == null)
                            {
                                Console.WriteLine(pair.Value);
                            }
                            else
                            {
                                statusEvent.Biblio.Inventors.Add(new PartyMember()
                                {
                                    Name = pair.Key,
                                    Country = countryCode
                                });
                            }

                        }
                    }
                    if (inid.StartsWith("(30)"))
                    {
                        var cleanInid = Regex.Match(inid, @"\(30\).+�(?<inid>.+)", RegexOptions.Singleline);

                        if (cleanInid.Success)
                        {
                            var priorities = Regex.Split(cleanInid.Groups["inid"].Value, "\n").Where(x => !string.IsNullOrEmpty(x) && !x.StartsWith("\r")).ToList();
                            foreach (var priority in priorities)
                            {
                                var match = Regex.Match(priority, @"(?<date>\d+\/\d+\/\d+)\s?(?<code>\D{2})\s?(?<num>.+)");
                                if (match.Success)
                                {
                                    statusEvent.Biblio.Priorities.Add(new Priority()
                                    {
                                        Country = match.Groups["code"].Value.Trim(),
                                        Number = match.Groups["num"].Value.Trim(),
                                        Date = DateTime.ParseExact(match.Groups["date"].Value.Trim(), "dd/MM/yyyy",
                                                CultureInfo.InvariantCulture).ToString("yyyy/MM/dd")
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"{priority} --- 30");
                                }
                            }
                        }
                    }
                    if (inid.StartsWith("(51)"))
                    {
                        var cleanInid = Regex.Match(inid, @"\(51\).+�(?<inid>.+)", RegexOptions.Singleline);

                        if (cleanInid.Success)
                        {
                            var ipcs = Regex.Split(cleanInid.Groups["inid"].Value.Trim(), @"(?<=\))")
                                .Where(x => !string.IsNullOrEmpty(x)).ToList();

                            foreach (var ipc in ipcs)
                            {
                                var match = Regex.Match(ipc.Trim(), @"(?<class>.+)\((?<date>.+)\)");
                                if (match.Success)
                                {
                                    statusEvent.Biblio.Ipcs.Add(new Ipc()
                                    {
                                        Class = match.Groups["class"].Value.Trim(),
                                        Date = match.Groups["date"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"{ipc} --- 51");
                                }
                            }
                        }
                    }
                    if (inid.StartsWith("(52)"))
                    {
                        var match = Regex.Match(inid, @"\(52\).+�(?<ipcrs>.+\/\d{2})", RegexOptions.Singleline);
                        if (match.Success)
                        {
                            var ipcrs = Regex.Split(match.Groups["ipcrs"].Value.Trim(), @"(?=\D\d{2}\D\s?\d+\/\d+)")
                                .Where(x => !string.IsNullOrEmpty(x)).ToList();

                            foreach (var ipcr in ipcrs)
                            {
                                statusEvent.Biblio.Ipcrs.Add(new Ipcr()
                                {
                                    ClassValue = ipcr.Trim()
                                });
                            }
                        }
                    }
                    if (inid.StartsWith("(74)"))
                    {
                        var cleanedInid = Regex.Replace(inid, @"^\s*$[\r\n]*", "", RegexOptions.Multiline);

                        var match = Regex.Match(cleanedInid, @".+�(?<name>.+?)\n(?<adress>.+)", RegexOptions.Singleline);
                        if (match.Success)
                        {
                            statusEvent.Biblio.Agents.Add(new PartyMember()
                            {
                                Name = match.Groups["name"].Value.Trim(),
                                Address1 = match.Groups["adress"].Value.Replace("\r", "").Replace("\n", " ").Trim(),
                            });
                        }
                        else
                        {
                            Console.WriteLine($"{inid} --- 74");
                        }
                    }

                    if (inid.StartsWith("(57)"))
                    {
                        var match = Regex.Match(inid, @"\(57\)\sΠερίληψη(?<text>.+)", RegexOptions.Singleline);
                        if (match.Success)
                        {
                            statusEvent.Biblio.Abstracts.Add(new Abstract()
                            {
                                Language = "EL",
                                Text = match.Groups["text"].Value.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Trim()
                            });
                        }
                    }
                }
                statusEvent.Biblio.EuropeanPatents.Add(euPatent);
                AddAbstractScreenShot(statusEvent, imagesDictionary);
            }

            if (subCode == "57" || subCode =="39")
            {
                var match = Regex.Match(note,
                    @"(?<pubnum>CY\d+)\s?\((?<euPubNum>.+)\)\s?(?<inid73>.+)\s?(?<evDate>\d{2}\/\d{2}\/\d{4})",
                    RegexOptions.Singleline);

                if (match.Success)
                {
                    statusEvent.Biblio.Publication.Number = match.Groups["pubnum"].Value.Trim();
                    euPatent.PubNumber = match.Groups["pubnum"].Value.Trim();

                    var assignees = Regex.Split(match.Groups["inid73"].Value.Replace("\r","").Replace("\n","").Trim(), @"\d+\.\s*(.*?)(?=\s*\d+\.|$)")
                        .Where(x => !string.IsNullOrEmpty(x)).ToList();

                    foreach (var assignee in assignees)
                    {
                        statusEvent.Biblio.Assignees.Add(new PartyMember()
                        {
                            Language = "EN",
                            Name = assignee
                        });
                    }

                    statusEvent.LegalEvent.Date = DateTime
                        .ParseExact(match.Groups["evDate"].Value.Trim(), "dd/MM/yyyy",
                            CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                }
                else
                {
                    var match2 = Regex.Match(note,
                        @"(?<pubnum>CY\d+)\s?\((?<euPubNum>.+)\)\s?(?<inid73>.+)",
                        RegexOptions.Singleline);

                    if (match2.Success)
                    {

                    }
                    else Console.WriteLine(note);
                }
                statusEvent.Biblio.EuropeanPatents.Add(euPatent);
            }
            return statusEvent;
        }

        internal string MakeCountry(string country) => country switch
        {
            "France" => "FR",
            "Belgium" => "BE",
            "Cyprus" => "CY",
            "Sweden" => "SE",
            "Germany" => "DE",
            "Denmark" => "DK",
            "Italy" => "IT",
            "Greece" => "GR",
            "United States of America" => "US",
            "United Kingdom" => "GB",
            "Poland" => "PL",
            "Canada" => "CA",
            "The Netherlands" => "NL",
            "Spain" => "ES",
            "Japan" => "JP",
            "Republic of Korea" => "KR",
            "Slovakia" => "SK",
            _ => null,
        };

        public static Dictionary<string, string> ParseInventors(string input)
        {
            var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            var countryIndex = lines.FindIndex(line =>
                !line.Contains(",") &&
                !line.Contains("(") &&
                !line.Contains("�") &&
                !line.Contains("Εφευρέτης"));

            if (countryIndex < 1)
            {
                Console.WriteLine("Error - 72");
            }

            var names = lines.Skip(2).Take(countryIndex - 2).ToList();
            var countries = lines.Skip(countryIndex).ToList();
            var dictionary = new Dictionary<string, string>();
            for (var i = 0; i < names.Count; i++)
            {
                dictionary[names[i]] = countries[i];
            }

            return dictionary;
        }

        private void AddAbstractScreenShot(LegalStatusEvent statusEvent, Dictionary<string, string> imagesDictionary)
        {
            var patentKey = statusEvent.Biblio.Publication.Number + " " + statusEvent.Biblio.Publication.Kind;
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
    }
}
