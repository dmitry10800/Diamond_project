using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Integration;

namespace Diamond_PH_Maksim_Excel
{
    internal class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalStatusEvents = new();

            DirectoryInfo directory = new(path);

            var files = directory.GetFiles("*.xlsx", SearchOption.AllDirectories).Select(file => file.FullName).ToList();

            foreach (var xlsxFile in files)
            {
                    CurrentFileName = xlsxFile;

                    ISheet sheet;

                    XSSFWorkbook OpenedDocument;

                    using (FileStream file = new(xlsxFile, FileMode.Open, FileAccess.Read))
                    {
                        OpenedDocument = new XSSFWorkbook(file);
                    }

                    sheet = OpenedDocument.GetSheet("Sheet1");

                    CultureInfo culture = new("ru-RU");

                    if (subCode is "7")
                    {
                        for (int row = 0; row <= sheet.LastRowNum; row++)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                            CountryCode = "PH",
                            SectionCode = "MK",
                            SubCode = subCode,
                            Id = Id++,
                            GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new()
                            {
                                DOfPublication = new()
                            },
                            LegalEvent = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Text = sheet.GetRow(row).GetCell(1).ToString(),
                                Language = "EN"
                            });

                            statusEvent.Biblio.Application.Date = DateTime
                                .Parse(sheet.GetRow(row).GetCell(2).ToString(), culture).ToString("yyyy.MM.dd")
                                .Replace(".", "/").Trim();

                            statusEvent.LegalEvent.Date = DateTime
                                .Parse(sheet.GetRow(row).GetCell(3).ToString(), culture).ToString("yyyy.MM.dd")
                                .Replace(".", "/").Trim();

                            var assignersList = Regex.Split(sheet.GetRow(row).GetCell(4).ToString(), @"(?<=[A-Z]{2}\])")
                                .Where(x => !string.IsNullOrEmpty(x)).ToList();

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
                            legalStatusEvents.Add(statusEvent);
                        }
                    }
                    else if (subCode == "5")
                    {
                        for (int row = 1; row <= sheet.LastRowNum; row++)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                                CountryCode = "PH",
                                SectionCode = "MM",
                                SubCode = subCode,
                                Id = Id++,
                                GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                                Biblio = new()
                                {
                                    DOfPublication = new()
                                },
                                LegalEvent = new()
                            };

                            statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                            List<string> aasigneers = Regex.Split(sheet.GetRow(row).GetCell(1).ToString(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (string assigneer in aasigneers)
                            {
                                Match match73 = Regex.Match(assigneer, @"(?<name>.+)\[(?<code>\D{2})");

                                if (match73.Success)
                                {
                                    statusEvent.Biblio.Assignees.Add(new PartyMember()
                                    {
                                        Name = match73.Groups["name"].Value.Trim(),
                                        Country = match73.Groups["code"].Value.Trim()
                                    });
                                }
                                else Console.WriteLine($"{assigneer} --- not process");
                            }

                            var pubDate = sheet.GetRow(row).GetCell(2);
                                
                            if (pubDate != null)
                            {
                                statusEvent.Biblio.Publication.Date = DateTime
                                    .Parse(sheet.GetRow(row).GetCell(2).ToString(), culture).ToString("yyyy.MM.dd")
                                    .Replace(".", "/").Trim();
                            }

                            statusEvent.Biblio.IntConvention.PctPublDate = DateTime
                                .Parse(sheet.GetRow(row).GetCell(3).ToString(), culture).ToString("yyyy.MM.dd")
                                .Replace(".", "/").Trim();

                            statusEvent.Biblio.Titles.Add(new Title()
                            {
                                Language = "EN",
                                Text = sheet.GetRow(row).GetCell(5).ToString()
                            });

                            var match = Regex.Match(CurrentFileName, @"_(?<date>\d{8})_");

                            if (match.Success)
                            {
                                statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                            }

                            legalStatusEvents.Add(statusEvent);
                        }   
                    }
                    else if (subCode is "12")
                    {
                        for (int row = 1; row <= sheet.LastRowNum; row++)
                        {
                            Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                            {
                            CountryCode = "PH",
                            SectionCode = "KA",
                            SubCode = subCode,
                            Id = Id++,
                            GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new()
                            {
                                DOfPublication = new()
                            },
                            LegalEvent = new()
                            };

                        var countryCodeForCheck = string.Empty;

                        var applicantsList = Regex.Split(sheet.GetRow(row).GetCell(1).ToString(), @"and").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicantsList)
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

                        var match = Regex.Match(CurrentFileName, @"_(?<date>\d{8})_");

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
                HttpResponseMessage result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
