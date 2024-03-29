﻿using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_SG_Maksim
{
    internal class Methods
    {
        private string CurrentFileName;
        private int Id = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start (string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalStatusEvents = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            if (subCode is "5")
            {
                foreach (var file in directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
                {
                    files.Add(file.FullName);
                }

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

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "SG",
                            SectionCode = "ND",
                            SubCode = subCode,
                            Id = Id++,
                            GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new(),
                            LegalEvent = new()
                        };

                        statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString().Trim();

                        var applicantsList = Regex.Split(sheet.GetRow(row).GetCell(1).ToString(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicantsList)
                        {
                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = applicant.Trim()
                            });
                        }

                        statusEvent.LegalEvent.Note = "|| The year patents renewed for | " + sheet.GetRow(row).GetCell(2).ToString().Trim();
                        statusEvent.LegalEvent.Language = "EN";

                        var match = Regex.Match(CurrentFileName, @"_(?<date>\d{8})_");

                        if (match.Success)
                        {
                            statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4,"/").Insert(7,"/").Trim();
                        }

                        legalStatusEvents.Add(statusEvent);
                    }
                }
            }
            else if (subCode is "7" or "6")
            {
                foreach (var file in directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
                {
                    files.Add(file.FullName);
                }

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

                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "SG",
                            SectionCode = "MK",
                            SubCode = subCode,
                            Id = Id++,
                            GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new(),
                            LegalEvent = new()
                        };

                        statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString().Trim();

                        var applicantsList = Regex.Split(sheet.GetRow(row).GetCell(1).ToString(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                        foreach (var applicant in applicantsList)
                        {
                            statusEvent.Biblio.Applicants.Add(new Integration.PartyMember
                            {
                                Name = applicant.Trim()
                            });
                        }

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
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
