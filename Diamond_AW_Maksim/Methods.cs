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

namespace Diamond_AW_Maksim
{
    internal class Methods
    {
        private string CurrentFileName;
        private int Id = 1;
        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> legalStatuses = new();

            DirectoryInfo directory = new(path);

            List<string> files = new();

            if (subCode is "1")
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

                    sheet = OpenedDocument.GetSheet("qry_uitvinders");

                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "AW",
                            SectionCode = "FG",
                            SubCode = subCode,
                            Id = Id++,
                            GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new(),
                            LegalEvent = new()
                        };

                        statusEvent.Biblio.Publication.Number = sheet.GetRow(row).GetCell(0).ToString().Replace("OCT-", "").Trim();

                        if(sheet.GetRow(row).GetCell(0).ToString().Replace("OCT-", "").Trim() == sheet.GetRow(row+1).GetCell(0).ToString().Replace("OCT-", "").Trim())
                        {
                            Match match = Regex.Match(sheet.GetRow(row+1).GetCell(2).ToString(), @"(?<day>\d{2})-(?<month>.+)-(?<year>\d{4})");

                            if (match.Success)
                            {
                                string month = MakeMonth(match.Groups["month"].Value.Trim());
                                if (month is not null)
                                {
                                    statusEvent.Biblio.Application.Date = match.Groups["year"].Value.Trim() + "/"
                                        + month + "/"
                                        + match.Groups["day"].Value.Trim();
                                }
                                else Console.WriteLine($"{match.Groups["month"].Value.Trim()}");
                            }

                            for (int i = 3; i < sheet.GetRow(row).LastCellNum; i++)
                            {
                                if (sheet.GetRow(row).GetCell(i).ToString() is "")
                                {
                                    break;
                                }

                                statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                                {
                                    Name = sheet.GetRow(row).GetCell(i).ToString()
                                });
                            }

                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "EN",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            row++;
                        }
                        else
                        {
                            statusEvent.Biblio.Titles.Add(new Integration.Title
                            {
                                Language = "EN",
                                Text = sheet.GetRow(row).GetCell(1).ToString()
                            });

                            Match match = Regex.Match(sheet.GetRow(row).GetCell(2).ToString(), @"(?<day>\d{2})-(?<month>.+)-(?<year>\d{4})");

                            if (match.Success)
                            {
                                string month = MakeMonth(match.Groups["month"].Value.Trim());
                                if (month is not null)
                                {
                                    statusEvent.Biblio.Application.Date = match.Groups["year"].Value.Trim() + "/"
                                        + month + "/"
                                        + match.Groups["day"].Value.Trim();
                                }
                                else Console.WriteLine($"{match.Groups["month"].Value.Trim()}");
                            }

                            for (int i = 3; i < sheet.GetRow(row).LastCellNum; i++)
                            {
                                if (sheet.GetRow(row).GetCell(i).ToString() is "")
                                {
                                    break;
                                }

                                statusEvent.Biblio.Assignees.Add(new Integration.PartyMember
                                {
                                    Name = sheet.GetRow(row).GetCell(i).ToString()
                                });
                            }
                        }
                        legalStatuses.Add(statusEvent);
                    }             
                }
            }
            return legalStatuses;
        }

        internal string MakeMonth(string month) => month switch
        {
            "янв." => "01",
            "февр." => "02",
            "мар." => "03",
            "апр."=> "04",
            "мая" => "05",
            "июн." => "06",
            "июл." => "07",
            "авг." => "08",
            "сент." => "09",
            "окт." => "10",
            "нояб." => "11",
            "дек." => "12",
            "Mar" => "03",
            "Feb" => "02",
            "Aug" => "08",
            "Nov" => "11",
            "Jul" => "07",
            "Apr" => "04",
            "Jun" => "06",
            "May" => "05",
            "Jan" => "01",
            "Sep" => "09",
            "Oct" => "10",
            "Dec" => "12",
            _ => null
        };
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
