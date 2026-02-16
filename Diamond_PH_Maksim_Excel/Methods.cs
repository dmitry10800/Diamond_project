using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Integration;

namespace Diamond_PH_Maksim_Excel
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            var legalStatusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directory = new DirectoryInfo(path);

            var files = directory.GetFiles("*.xlsx", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

            foreach (var xlsxFile in files)
            {
                _currentFileName = xlsxFile;

                XSSFWorkbook OpenedDocument;

                using (FileStream file = new(xlsxFile, FileMode.Open, FileAccess.Read))
                {
                    OpenedDocument = new XSSFWorkbook(file);
                }

                var sheet = OpenedDocument.GetSheet("Sheet1") ?? OpenedDocument.GetSheet("Лист1");
                if (sheet == null) continue;
                var culture = new CultureInfo("ru-RU");

                if (subCode == "7")
                {
                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        var rowObj = sheet.GetRow(row);
                        if (rowObj == null) continue;

                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "PH",
                            SectionCode = "MK",
                            SubCode = subCode,
                            Id = _id++,
                            GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new Biblio { DOfPublication = new DOfPublication() },
                            LegalEvent = new LegalEvent()
                        };

                        statusEvent.Biblio.Application.Number = GetCellStringValue(rowObj, 0);

                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Text = GetCellStringValue(rowObj, 1),
                            Language = "EN"
                        });

                        statusEvent.Biblio.Application.Date = ParseDateString(GetCellStringValue(rowObj, 2), row).ToString("yyyy/MM/dd");
                        statusEvent.LegalEvent.Date = ParseDateString(GetCellStringValue(rowObj, 3), row).ToString("yyyy/MM/dd");

                        var assignersRaw = GetCellStringValue(rowObj, 4);

                        if (!string.IsNullOrWhiteSpace(assignersRaw))
                        {
                            var assignersList = Regex.Split(assignersRaw, @"(?<=[A-Z]{2}\])")
                                .Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                            foreach (var assigner in assignersList)
                            {
                                var match = Regex.Match(assigner.Trim(), @"(?<name>.+)\s\[(?<code>[A-Z]{2})", RegexOptions.Singleline);
                                if (match.Success)
                                {
                                    statusEvent.Biblio.Assignees.Add(new PartyMember()
                                    {
                                        Name = match.Groups["name"].Value.Trim().TrimStart(';').TrimStart(',').Trim(),
                                        Country = match.Groups["code"].Value.Trim()
                                    });
                                }
                            }
                        }
                        legalStatusEvents.Add(statusEvent);
                    }
                }
                if (subCode == "5")
                {
                    for (var row = 1; row <= sheet.LastRowNum; row++)
                    {
                        var rowObj = sheet.GetRow(row);
                        if (rowObj == null) continue;

                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "PH",
                            SectionCode = "MM",
                            SubCode = subCode,
                            Id = _id++,
                            GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new Biblio { DOfPublication = new DOfPublication() },
                            LegalEvent = new LegalEvent()
                        };

                        statusEvent.Biblio.Application.Number = GetCellStringValue(rowObj, 0);

                        var assigneesRaw = GetCellStringValue(rowObj, 1);
                        if (!string.IsNullOrWhiteSpace(assigneesRaw))
                        {
                            var assigneersList = Regex.Split(assigneesRaw, @";")
                                .Where(val => !string.IsNullOrWhiteSpace(val)).ToList();

                            foreach (var assigneer in assigneersList)
                            {
                                var match73 = Regex.Match(assigneer, @"(?<name>.+)\[(?<code>\D{2})");
                                if (match73.Success)
                                {
                                    statusEvent.Biblio.Assignees.Add(new PartyMember()
                                    {
                                        Name = match73.Groups["name"].Value.Trim(),
                                        Country = match73.Groups["code"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    var match73Second = Regex.Match(assigneer, @"(?<name>.+)\((?<code>\D{2})");
                                    if (match73Second.Success)
                                    {
                                        statusEvent.Biblio.Assignees.Add(new PartyMember()
                                        {
                                            Name = match73Second.Groups["name"].Value.Trim(),
                                            Country = match73Second.Groups["code"].Value.Trim()
                                        });
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{assigneer} --- not processed");
                                    }
                                }
                            }
                        }

                        var pubDateString = GetCellStringValue(rowObj, 2);
                        if (!string.IsNullOrWhiteSpace(pubDateString))
                        {
                            var pubDate = ParseDateString(pubDateString, row);
                            statusEvent.Biblio.Publication.Date = pubDate.ToString("yyyy/MM/dd");
                        }

                        var pctPubDateString = GetCellStringValue(rowObj, 3);
                        if (!string.IsNullOrWhiteSpace(pctPubDateString))
                        {
                            var pctPubDate = ParseDateString(pctPubDateString, row);
                            statusEvent.Biblio.IntConvention.PctPublDate = pctPubDate.ToString("yyyy/MM/dd");
                        }

                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "EN",
                            Text = GetCellStringValue(rowObj, 5)
                        });

                        var match = Regex.Match(_currentFileName, @"_(?<date>\d{8})_");
                        if (match.Success)
                        {
                            statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                        }

                        legalStatusEvents.Add(statusEvent);
                    }
                }
                if (subCode == "12")
                {
                    for (var row = 1; row <= sheet.LastRowNum; row++)
                    {
                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "PH",
                            SectionCode = "KA",
                            SubCode = subCode,
                            Id = _id++,
                            GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new()
                            {
                                DOfPublication = new()
                            },
                            LegalEvent = new()
                        };

                        var countryCodeForCheck = string.Empty;

                        var applicantsList = Regex.Split(sheet.GetRow(row).GetCell(1).ToString(), @"and").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicantsList)
                        {
                            var applicantMatch = Regex.Match(applicant.Trim(), @"(?<name>.+)\s?\[(?<code>\D{2})");

                            if (applicantMatch.Success)
                            {
                                countryCodeForCheck = applicantMatch.Groups["code"].Value.Trim();

                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicantMatch.Groups["name"].Value.Trim(),
                                    Country = applicantMatch.Groups["code"].Value.Trim()
                                });
                            }
                        }

                        if (countryCodeForCheck is not "PH")
                        {
                            var applicationData = Regex.Match(sheet.GetRow(row).GetCell(0).ToString(), @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (applicationData.Success)
                            {
                                statusEvent.Biblio.Application.Number = applicationData.Groups["num"].Value.Trim();

                                statusEvent.Biblio.IntConvention.PctNationalDate = DateTime.Parse(applicationData.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }

                            statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(sheet.GetRow(row).GetCell(2).ToString(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else
                        {
                            var applicationData = Regex.Match(sheet.GetRow(row).GetCell(0).ToString(), @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (applicationData.Success)
                            {
                                statusEvent.Biblio.Application.Number = applicationData.Groups["num"].Value.Trim();

                                statusEvent.Biblio.Application.Date = DateTime.Parse(applicationData.Groups["date"].Value.Trim(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }

                            statusEvent.Biblio.Publication.Date = DateTime.Parse(sheet.GetRow(row).GetCell(2).ToString(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }

                        statusEvent.Biblio.DOfPublication.date_45 = DateTime.Parse(sheet.GetRow(row).GetCell(3).ToString(), culture).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "EN",
                            Text = sheet.GetRow(row).GetCell(4).ToString()
                        });

                        statusEvent.LegalEvent.Note = "|| ANNUITY DUE | " + sheet.GetRow(row).GetCell(5).ToString();

                        var match = Regex.Match(_currentFileName, @"_(?<date>\d{8})_");

                        if (match.Success)
                        {
                            statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                        }
                        legalStatusEvents.Add(statusEvent);
                    }
                }
                if (subCode == "22")
                {
                    for (var row = 1; row <= sheet.LastRowNum; row++)
                    {
                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "PH",
                            SectionCode = "FA",
                            SubCode = subCode,
                            Id = _id++,
                            GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new Biblio
                            {
                                DOfPublication = new DOfPublication()
                            },
                            LegalEvent = new LegalEvent()
                        };

                        statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                        var originalDate = sheet.GetRow(row).GetCell(1).ToString();
                        originalDate = originalDate?.Replace("Sept", "Sep");
                        var parsedDate = DateTime.ParseExact(originalDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                        var formattedDate = parsedDate.ToString("yyyy/MM/dd");
                        statusEvent.Biblio.Application.Date = formattedDate;

                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "EN",
                            Text = sheet.GetRow(row).GetCell(2).ToString()
                        });

                        var applicants = Regex.Split(sheet.GetRow(row).GetCell(3).ToString(), ";")
                            .Where(x => !string.IsNullOrEmpty(x)).ToList();

                        foreach (var applicant in applicants)
                        {
                            var matchApplicant = Regex.Match(applicant.Trim(), @"(?<name>.+)\((?<country>\D{2})\)");

                            if (matchApplicant.Success)
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = matchApplicant.Groups["name"].Value.Trim(),
                                    Country = matchApplicant.Groups["country"].Value.Trim()
                                });
                            }
                            else
                            {
                                statusEvent.Biblio.Applicants.Add(new PartyMember()
                                {
                                    Name = applicant
                                });
                            }
                        }

                        var match = Regex.Match(_currentFileName, @"_(?<date>\d{8})_");
                        if (match.Success)
                        {
                            statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                        }
                        legalStatusEvents.Add(statusEvent);
                    }
                }
            }
            return legalStatusEvents;
        }

        private static string GetCellStringValue(IRow row, int cellIndex)
        {
            var cell = row.GetCell(cellIndex);
            return cell?.ToString().Trim();
        }

        private static DateTime ParseDateString(string dateString, int rowNumber)
        {
            var exactFormats = new[] {"dd.MM.yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd/MM/yyyy", "dd-MMM-yyyy", "dd-MMMM-yyyy", "dd MMM yyyy", "dd MMMM yyyy"};

            var supportedCultures = new[] {CultureInfo.InvariantCulture, new CultureInfo("en-US"),new CultureInfo("en-GB")};

            if (string.IsNullOrWhiteSpace(dateString))
            {
                throw new Exception($"Empty date in row {rowNumber + 1}");
            }

            dateString = Normalize(dateString);

            foreach (var culture in supportedCultures)
            {
                if (DateTime.TryParseExact(
                        dateString,
                        exactFormats,
                        culture,
                        DateTimeStyles.None,
                        out var parsedExact))
                {
                    return parsedExact;
                }
            }

            foreach (var culture in supportedCultures)
            {
                if (DateTime.TryParse(
                        dateString,
                        culture,
                        DateTimeStyles.None,
                        out var parsed))
                {
                    return parsed;
                }
            }

            throw new Exception($"Invalid date '{dateString}' in row {rowNumber + 1}");
        }

        private static string Normalize(string input)
        {
            var value = input.Trim();

            value = value.Replace("Sept", "Sep", StringComparison.OrdinalIgnoreCase);

            while (value.Contains("  "))
            {
                value = value.Replace("  ", " ");
            }

            return value;
        }
    }
}
