using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_TR_Maksim_Excel
{
    internal class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        public List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string sub)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            foreach (FileInfo file in directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            foreach (string xlsFiles in files)
            {
                CurrentFileName = xlsFiles;

                if (sub == "10")
                {
                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    ISheet sheet = OpenedDocument.GetSheet("Sheet1");

                    for (int row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "FD",
                                Id = Id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = sheet.GetRow(row).GetCell(2).ToString()
                            });

                            Match match = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

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

                    ISheet sheet = OpenedDocument.GetSheet("Sheet1");

                    for (int row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "MM",
                                Id = Id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = sheet.GetRow(row).GetCell(2).ToString()
                            });

                            Match match = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

                            if (match.Success)
                            {
                                statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                            }

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
                else if (sub == "16")
                {
                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    ISheet sheet = OpenedDocument.GetSheet("Sheet1");

                    for (int row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "FD",
                                Id = Id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = sheet.GetRow(row).GetCell(2).ToString()
                            });

                            Match match = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

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

                    ISheet sheet = OpenedDocument.GetSheet("Sheet1");

                    for (int row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "FA",
                                Id = Id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = sheet.GetRow(row).GetCell(2).ToString()
                            });

                            Match match = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

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

                    ISheet sheet = OpenedDocument.GetSheet("Sheet1");

                    for (int row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "MK",
                                Id = Id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = sheet.GetRow(row).GetCell(2).ToString()
                            });

                            statusEvent.LegalEvent.Date = DateTime.Parse(sheet.GetRow(row).GetCell(3).ToString(), culture).ToString("yyyy.MM.dd").Replace(".","/").Trim();

                            statusEvents.Add(statusEvent);
                        }
                    }
                }
                else if (sub == "30")
                {
                    XSSFWorkbook OpenedDocument;

                    OpenedDocument = new(xlsFiles);

                    ISheet sheet = OpenedDocument.GetSheet("Sheet1");

                    for (int row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "EZ",
                                Id = Id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = sheet.GetRow(row).GetCell(2).ToString()
                            });

                            Match match = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

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

                    ISheet sheet = OpenedDocument.GetSheet("Sheet1");

                    for (int row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "EZ",
                                Id = Id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = sheet.GetRow(row).GetCell(2).ToString()
                            });

                            Match match = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

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

                    ISheet sheet = OpenedDocument.GetSheet("Sheet1");

                    for (int row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                                CountryCode = "TR",
                                SubCode = sub,
                                SectionCode = "EZ",
                                Id = Id++,
                                LegalEvent = new(),
                                Biblio = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "TR",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = sheet.GetRow(row).GetCell(2).ToString()
                            });

                            Match match = Regex.Match(Path.GetFileName(CurrentFileName.Replace(".xlsx", "")), @"(?<date>\d{8})");

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
