using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Integration;

namespace Diamond_CL_Maksim_Excel
{
    internal class Methods
    {
        private string CurrentFileName;
        private int Idfor20 = 1;
        private int Idfor21 = 1;
        private int Idfor22 = 1;
        private int Idfor23 = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path)
        {
            var legalStatusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

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

                sheet = OpenedDocument.GetSheet("Applications-2021");

                CultureInfo culture = new("ru-RU");

                for (var row = 1; row <= sheet.LastRowNum; row++)
                {
                    var typeOfSubCode = sheet.GetRow(row).GetCell(11).ToString();

                    var sectionCode = typeOfSubCode switch
                    {
                        "Patente de invención" => "AA",
                        "Patente de invención PCT" => "AF",
                        "Modelo de utilidad PCT" => "AF",
                        "Modelo de utilidad" => "AA",
                        _ => null
                    };

                    var subCode = typeOfSubCode switch
                    {
                        "Patente de invención" => "20",
                        "Patente de invención PCT" => "21",
                        "Modelo de utilidad PCT" => "22",
                        "Modelo de utilidad" => "23",
                        _ => null
                    };

                    var idPatent = sectionCode switch
                    {
                        "AA" => Idfor20++.ToString(),
                        "AF" => Idfor21++.ToString(),
                        _ => null
                    };

                    if (sectionCode is not null)
                    {
                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "CL",
                            SectionCode = sectionCode,
                            SubCode = subCode,
                            Id = int.Parse(idPatent),
                            GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new(),
                            LegalEvent = new()
                        };
                        statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(0).ToString();

                        var ckeckApplicantsName = sheet.GetRow(row).GetCell(2);
                        if (ckeckApplicantsName is not null)
                        {
                            var applicantsName = ckeckApplicantsName.ToString();
                            var applicants = Regex.Split(applicantsName, @"(?=\(\D{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();
                            foreach (var applicant in applicants)
                            {
                                var match = Regex.Match(applicant.Trim(), @"\((?<code>\D{2})\)(?<name>.+)");
                                if (match.Success)
                                {
                                    if (subCode == "20")
                                    {
                                        statusEvent.Biblio.Applicants.Add(new PartyMember()
                                        {
                                            Country = match.Groups["code"].Value.Trim(),
                                            Name = match.Groups["name"].Value.Trim(),
                                            Address1 = sheet.GetRow(row).GetCell(14).ToString()
                                        });
                                    }
                                    else
                                    {
                                        statusEvent.Biblio.Applicants.Add(new PartyMember()
                                        {
                                            Country = match.Groups["code"].Value.Trim(),
                                            Name = match.Groups["name"].Value.Trim()
                                        });
                                    }
                                }
                            }
                        }

                        var checkAgentName = sheet.GetRow(row).GetCell(3);
                        if (checkAgentName is not null)
                        {
                            var agentName = checkAgentName.ToString();
                            var match = Regex.Match(agentName, @"\((?<code>\D{2})\)(?<name>.+)");
                            if (match.Success)
                            {
                                statusEvent.Biblio.Agents.Add(new PartyMember()
                                {
                                    Country = match.Groups["code"].Value.Trim(),
                                    Name = match.Groups["name"].Value.Trim()
                                });
                            }
                        }

                        var checkInventorsName = sheet.GetRow(row).GetCell(4);
                        if (checkInventorsName is not null)
                        {
                            var inventorsName = ckeckApplicantsName.ToString();
                            var inventors = Regex.Split(inventorsName, @"(?=\(\D{2}\).+)").Where(val => !string.IsNullOrEmpty(val)).ToList();
                            foreach (var inventor in inventors)
                            {
                                var match = Regex.Match(inventor.Trim(), @"\((?<code>\D{2})\)(?<name>.+)");
                                if (match.Success)
                                {
                                    statusEvent.Biblio.Inventors.Add(new PartyMember()
                                    {
                                        Country = match.Groups["code"].Value.Trim(),
                                        Name = match.Groups["name"].Value.Trim()
                                    });
                                }
                            }
                        }

                        var filingDate = sheet.GetRow(row).GetCell(5).ToString();
                        var matchFilingDate = Regex.Match(filingDate, @"(?<month>\d+)\/(?<day>\+)\/(?<year>\d{4}).+");
                        if (matchFilingDate.Success)
                        {
                            var month = matchFilingDate.Groups["month"].Value.Trim();
                            if (month.Length == 1) month = "0" + month;

                            var day = matchFilingDate.Groups["day"].Value.Trim();
                            if (day.Length == 1) day = "0" + day;

                            statusEvent.Biblio.Application.Date = matchFilingDate.Groups["year"].Value.Trim() + "/" + month + "/" + day;
                        }
                        else
                        {
                            var matchFilingDate2 = Regex.Match(filingDate, @"(?<day>\d+).(?<month>.+)-(?<year>\d{4})");
                            if (matchFilingDate2.Success)
                            {
                                statusEvent.Biblio.Application.Date = matchFilingDate2.Groups["year"].Value.Trim() + "/" 
                                    + MakeMonth(matchFilingDate2.Groups["month"].Value.Trim()) + "/" 
                                    + matchFilingDate2.Groups["day"].Value.Trim();
                            }
                        }

                        var pubDate = sheet.GetRow(row).GetCell(6).ToString();
                        var matchPubDate = Regex.Match(pubDate, @"(?<month>\d+)\/(?<day>\d+)\/(?<year>\d{4}).+");
                        if (matchPubDate.Success)
                        {
                            var month = matchPubDate.Groups["month"].Value.Trim();
                            if (month.Length == 1) month = "0" + month;

                            var day = matchPubDate.Groups["day"].Value.Trim();
                            if (day.Length == 1) day = "0" + day;

                            statusEvent.LegalEvent.Note = "|| Publication date | " + matchPubDate.Groups["year"].Value.Trim() + "/" + month + "/" + day;
                        }
                        else
                        {
                            var matchPubDate2 = Regex.Match(pubDate, @"(?<day>\d+).(?<month>.+).(?<year>\d{4})");
                            if (matchPubDate2.Success)
                            {
                                statusEvent.LegalEvent.Note = "|| Publication date | " + matchPubDate2.Groups["year"].Value.Trim() + "/"
                                                              + MakeMonth(matchPubDate2.Groups["month"].Value.Trim()) + "/"
                                                              + matchPubDate2.Groups["day"].Value.Trim();
                            }
                        }

                        statusEvent.LegalEvent.Language = "EN";

                        statusEvent.Biblio.Titles.Add(new Title()
                        {
                            Language = "ES",
                            Text = sheet.GetRow(row).GetCell(9).ToString()
                        });

                        if (sheet.GetRow(row).GetCell(16) is not null)
                        {
                            var pctAppDate = sheet.GetRow(row).GetCell(16).ToString();
                            var matchPctAppDate = Regex.Match(pctAppDate, @"(?<month>\d+)\/(?<day>\d+)\/(?<year>\d{4})");
                            if (matchPctAppDate.Success)
                            {
                                var month = matchPctAppDate.Groups["month"].Value.Trim();
                                if (month.Length == 1) month = "0" + month;

                                var day = matchPctAppDate.Groups["day"].Value.Trim();
                                if (day.Length == 1) day = "0" + day;

                                statusEvent.Biblio.IntConvention.PctApplDate = matchPctAppDate.Groups["year"].Value.Trim() + "/" + month + "/" + day;
                            }
                            else
                            {
                                var matchPctAppDate2 = Regex.Match(pctAppDate, @"(?<day>\d+).(?<month>.+)-(?<year>\d{4})");
                                if (matchPctAppDate2.Success)
                                {
                                    statusEvent.Biblio.IntConvention.PctApplDate = matchPctAppDate2.Groups["year"].Value.Trim() + "/"
                                        + MakeMonth(matchPctAppDate2.Groups["month"].Value.Trim()) + "/"
                                        + matchPctAppDate2.Groups["day"].Value.Trim();
                                }
                            }
                        }

                        if (sheet.GetRow(row).GetCell(17) is not null)
                        {
                            var pctPubDate = sheet.GetRow(row).GetCell(17).ToString();
                            var matchPctPubDate = Regex.Match(pctPubDate, @"(?<month>\d+)\/(?<day>\d+)\/(?<year>\d{4})");
                            if (matchPctPubDate.Success)
                            {
                                var month = matchPctPubDate.Groups["month"].Value.Trim();
                                if (month.Length == 1) month = "0" + month;

                                var day = matchPctPubDate.Groups["day"].Value.Trim();
                                if (day.Length == 1) day = "0" + day;

                                statusEvent.Biblio.IntConvention.PctPublDate = matchPctPubDate.Groups["year"].Value.Trim() + "/" + month + "/" + day;
                            }
                            else
                            {
                                var matchPctPubDate2 = Regex.Match(pctPubDate, @"(?<day>\d+).(?<month>.+)-(?<year>\d{4})");
                                if (matchPctPubDate2.Success)
                                {
                                    statusEvent.Biblio.IntConvention.PctApplDate = matchPctPubDate2.Groups["year"].Value.Trim() + "/"
                                        + MakeMonth(matchPctPubDate2.Groups["month"].Value.Trim()) + "/"
                                        + matchPctPubDate2.Groups["day"].Value.Trim();
                                }
                            }
                        }

                        if (sheet.GetRow(row).GetCell(18) is not null)
                        {
                            var priorities = Regex.Split(sheet.GetRow(row).GetCell(18).ToString(), @";").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            foreach (var priority in priorities)
                            {
                                var matchPriority = Regex.Match(priority, @"\((?<code>\D{2})\)\s(?<num>.+)\s(?<day>\d{2}).(?<month>\d{2}).(?<year>\d{4})");
                                if (matchPriority.Success)
                                {
                                    statusEvent.Biblio.Priorities.Add(new Priority()
                                    {
                                        Country = matchPriority.Groups["code"].Value.Trim(),
                                        Number = matchPriority.Groups["num"].Value.Trim(),
                                        Date = matchPriority.Groups["year"].Value.Trim() + "/" + matchPriority.Groups["month"].Value.Trim() + "/" + matchPriority.Groups["day"].Value.Trim()
                                    });
                                }
                            }
                        }

                        legalStatusEvents.Add(statusEvent);
                    }
                }
            }
            return legalStatusEvents;
        }

        internal string MakeMonth(string month) => month switch
        {
            "Jan" => "01",
            "Feb" => "02",
            "Mar" => "03",
            "Apr" => "04",
            "May" => "05",
            "June" => "06",
            "July" => "07",
            "Aug" => "08",
            "Sept" => "09",
            "Sep" => "09",
            "Oct" => "10",
            "Nov" => "11",
            "Dec" => "12",
            _ => null
        };
        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool SendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                var url = SendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
                HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                _ = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
