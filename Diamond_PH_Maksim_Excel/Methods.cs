using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

            List<string> files = new();

            if (subCode is "12")
            {
                foreach (FileInfo file in directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
                {
                    files.Add(file.FullName);
                }

                foreach (string xlsxFile in files)
                {
                    CurrentFileName = xlsxFile;

                    ISheet sheet;

                    XSSFWorkbook OpenedDocument;

                    using (FileStream file = new(xlsxFile, FileMode.Open, FileAccess.Read))
                    {
                        OpenedDocument = new XSSFWorkbook(file);
                    }

                    sheet = OpenedDocument.GetSheet("Sheet1");

                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "SG",
                            SectionCode = "ND",
                            SubCode = subCode,
                            Id = Id++,
                            GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new()
                            {
                                DOfPublication = new()
                            },
                            LegalEvent = new()
                        };

                        string countryCodeForCheck = string.Empty;

                        List<string> applicantsList = Regex.Split(sheet.GetRow(row).GetCell(1).ToString(), @"and").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (string applicant in applicantsList)
                        {
                            Match applicantMatch = Regex.Match(applicant.Trim(), @"(?<name>.+)\s?\[(?<code>\D{2})");

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
                            Match applicationData = Regex.Match(sheet.GetRow(row).GetCell(0).ToString(), @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (applicationData.Success)
                            {
                                statusEvent.Biblio.Application.Number = applicationData.Groups["num"].Value.Trim();

                                statusEvent.Biblio.IntConvention.PctNationalDate = DateTime.Parse(applicationData.Groups["date"].Value.Trim()).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }

                            statusEvent.Biblio.IntConvention.PctPublDate = DateTime.Parse(sheet.GetRow(row).GetCell(2).ToString()).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }
                        else
                        {
                            Match applicationData = Regex.Match(sheet.GetRow(row).GetCell(0).ToString(), @"(?<num>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (applicationData.Success)
                            {
                                statusEvent.Biblio.Application.Number = applicationData.Groups["num"].Value.Trim();

                                statusEvent.Biblio.Application.Date = DateTime.Parse(applicationData.Groups["date"].Value.Trim()).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                            }

                            statusEvent.Biblio.Publication.Date = DateTime.Parse(sheet.GetRow(row).GetCell(2).ToString()).ToString("yyyy.MM.dd").Replace(".", "/").Trim();
                        }

                        statusEvent.Biblio.DOfPublication.date_45 = DateTime.Parse(sheet.GetRow(row).GetCell(3).ToString()).ToString("yyyy.MM.dd").Replace(".", "/").Trim();

                        statusEvent.Biblio.Titles.Add(new Integration.Title
                        {
                            Language = "EN",
                            Text = sheet.GetRow(row).GetCell(4).ToString()
                        });

                        statusEvent.LegalEvent.Note = "|| ANNUITY DUE | " + sheet.GetRow(row).GetCell(5).ToString();

                        Match match = Regex.Match(CurrentFileName, @"_(?<date>\d{8})_");

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
