using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Diamond_TR_Maksim_Excel
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        public List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string sub)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (var file in directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            foreach (var xlsFiles in files)
            {
                _currentFileName = xlsFiles;

                if (sub == "10")
                {
                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    var sheet = OpenedDocument.GetSheet("Sheet1");

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "FD",
                                Id = _id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();
                            
                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            var applicants = Regex.Split(sheet.GetRow(row).GetCell(2).ToString().Trim(), @"\\").Where(x => !string.IsNullOrEmpty(x)).ToList();
                            foreach (var applicant in applicants)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicant.Trim()
                                });
                            }

                            var match = Regex.Match(Path.GetFileName(_currentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                            if (match.Success)
                            {
                                statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4,"/").Insert(7,"/").Trim();
                            }

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
                else if (sub == "13")
                {
                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    var sheet = OpenedDocument.GetSheet("Sheet1");

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "MM",
                                Id = _id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            var applicants = Regex.Split(sheet.GetRow(row).GetCell(2).ToString().Trim(), @"\\").Where(x => !string.IsNullOrEmpty(x)).ToList();
                            foreach (var applicant in applicants)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicant.Trim()
                                });
                            }

                            var match = Regex.Match(Path.GetFileName(_currentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                            if (match.Success)
                            {
                                statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                            }

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
                else if (sub is "16" or "15")
                {
                    var sectionCode = sub switch
                    {
                        "16" => "FD",
                        "15" => "DC",
                        _ => null
                    };

                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    var sheet = OpenedDocument.GetSheet("Sheet1");

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = sectionCode,
                                Id = _id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            var applicants = Regex.Split(sheet.GetRow(row).GetCell(2).ToString().Trim(), @"\\").Where(x => !string.IsNullOrEmpty(x)).ToList();
                            foreach (var applicant in applicants)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicant.Trim()
                                });
                            }

                            var match = Regex.Match(Path.GetFileName(_currentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                            if (match.Success)
                            {
                                statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                            }

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
                else if (sub == "17")
                {
                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    var sheet = OpenedDocument.GetSheet("Sheet1");

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "FA",
                                Id = _id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            var applicants = Regex.Split(sheet.GetRow(row).GetCell(2).ToString().Trim(), @"\\").Where(x => !string.IsNullOrEmpty(x)).ToList();
                            foreach (var applicant in applicants)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicant.Trim()
                                });
                            }

                            var match = Regex.Match(Path.GetFileName(_currentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                            if (match.Success)
                            {
                                statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                            }

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
                else if (sub == "19")
                {
                    XSSFWorkbook OpenedDocument;

                    CultureInfo culture = new("ru-RU");

                    OpenedDocument = new(xlsFiles);

                    var sheet = OpenedDocument.GetSheet("Sheet1");

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "MK",
                                Id = _id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            var applicants = Regex.Split(sheet.GetRow(row).GetCell(2).ToString().Trim(), @"\\").Where(x => !string.IsNullOrEmpty(x)).ToList();
                            foreach (var applicant in applicants)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicant.Trim()
                                });
                            }

                            statusEvent.LegalEvent.Date = DateTime.Parse(sheet.GetRow(row).GetCell(3).ToString(), culture).ToString("yyyy.MM.dd").Replace(".","/").Trim();

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
                else if (sub == "30")
                {
                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    var sheet = OpenedDocument.GetSheet("Sheet1");

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "EZ",
                                Id = _id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            var applicants = Regex.Split(sheet.GetRow(row).GetCell(2).ToString().Trim(), @"\\").Where(x => !string.IsNullOrEmpty(x)).ToList();
                            foreach (var applicant in applicants)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicant.Trim()
                                });
                            }

                            var match = Regex.Match(Path.GetFileName(_currentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                            if (match.Success)
                            {
                                statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                            }

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
                else if (sub == "31")
                {
                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    var sheet = OpenedDocument.GetSheet("Sheet1");

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "EZ",
                                Id = _id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            var applicants = Regex.Split(sheet.GetRow(row).GetCell(2).ToString().Trim(), @"\\").Where(x => !string.IsNullOrEmpty(x)).ToList();
                            foreach (var applicant in applicants)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicant.Trim()
                                });
                            }

                            var match = Regex.Match(Path.GetFileName(_currentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                            if (match.Success)
                            {
                                statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                            }

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
                else if (sub == "39")
                {
                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    var sheet = OpenedDocument.GetSheet("Sheet1");

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "EZ",
                                Id = _id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            var applicants = Regex.Split(sheet.GetRow(row).GetCell(2).ToString().Trim(), @"\\").Where(x => !string.IsNullOrEmpty(x)).ToList();
                            foreach (var applicant in applicants)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicant.Trim()
                                });
                            }

                            var match = Regex.Match(Path.GetFileName(_currentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                            if (match.Success)
                            {
                                statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                            }

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
                else if (sub == "41")
                {
                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    var sheet = OpenedDocument.GetSheet("Sheet1");

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "NB",
                                Id = _id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            var applicants = Regex.Split(sheet.GetRow(row).GetCell(2).ToString().Trim(), @"\\").Where(x => !string.IsNullOrEmpty(x)).ToList();
                            foreach (var applicant in applicants)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicant.Trim()
                                });
                            }

                            var match = Regex.Match(Path.GetFileName(_currentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                            if (match.Success)
                            {
                                statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                            }

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
                else if (sub == "47")
                {
                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    var sheet = OpenedDocument.GetSheet("Sheet1");

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "MA",
                                Id = _id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            var applicants = Regex.Split(sheet.GetRow(row).GetCell(2).ToString().Trim(), @"\\").Where(x => !string.IsNullOrEmpty(x)).ToList();
                            foreach (var applicant in applicants)
                            {
                                statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                                {
                                    Name = applicant.Trim()
                                });
                            }

                            var match = Regex.Match(Path.GetFileName(_currentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                            if (match.Success)
                            {
                                statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                            }

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
            }
            return statusEvents;
        }
    }
}
