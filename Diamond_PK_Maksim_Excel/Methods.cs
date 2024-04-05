using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Integration;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;


namespace Diamond_PK_Maksim_Excel;

public class Methods
{
    private string _currentFileName;
    private int _id = 1;

    internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
    {
        var legalStatusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

        var directory = new DirectoryInfo(path);

        var files = directory.GetFiles("*.xlsx", SearchOption.AllDirectories).Select(file => file.FullName); 
        if(!files.Any())
        {
            files = directory.GetFiles("*.xls", SearchOption.AllDirectories).Select(file => file.FullName);
        }

        foreach (var xlsxFile in files)
        {
            _currentFileName = xlsxFile;

            ISheet sheet;

            XSSFWorkbook OpenedDocument;

            using (FileStream file = new(xlsxFile, FileMode.Open, FileAccess.Read))
            {
                OpenedDocument = new XSSFWorkbook(file);
            }

            sheet = OpenedDocument.GetSheet("Sheet1") ?? OpenedDocument.GetSheet("Лист1");

            if (subCode == "13")
            {
                for (var row = 1; row <= sheet.LastRowNum; row++)
                {
                    const string sectionCode = "MK";

                    Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                    {
                        CountryCode = "PK",
                        SectionCode = sectionCode,
                        SubCode = subCode,
                        Id = _id++,
                        GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                        Biblio = new Biblio(),
                        LegalEvent = new LegalEvent()
                    };

                    statusEvent.LegalEvent.Note = $"|| Series | " + sheet.GetRow(row).GetCell(1).ToString().Trim() + " || File No. | " + sheet.GetRow(row).GetCell(2).ToString().Trim();

                    if (sheet.GetRow(row).GetCell(3).ToString().Trim() != null)
                    {
                        var appDateMatch = Regex.Match(sheet.GetRow(row).GetCell(3).ToString(), @"(?<day>\d{2})-(?<month>.+)-(?<year>\d{4})");
                        if (appDateMatch.Success)
                        {
                            var day = appDateMatch.Groups["day"].Value.Trim();
                            if (day.Length == 1)
                            {
                                day = "0" + day;
                            }

                            Console.WriteLine(appDateMatch.Groups["month"].Value.Trim());
                            var month = MakeMonth(appDateMatch.Groups["month"].Value.Trim());
                            if (month == null) Console.WriteLine(appDateMatch.Groups["month"].Value.Trim());

                            //tatusEvent.LegalEvent.Date = appDateMatch.Groups["year"].Value.Trim() + "/" + month + "/" + day;
                        }
                    }

                    if (sheet.GetRow(row).GetCell(4).ToString().Trim() != null)
                    {
                        statusEvent.Biblio.Publication.Number = sheet.GetRow(row).GetCell(4).ToString();
                    }
                        
                    if (sheet.GetRow(row).GetCell(5).ToString().Trim() != null)
                    {
                        var priorities = Regex.Split(sheet.GetRow(row).GetCell(5).ToString(), @";").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                        foreach (var priority in priorities)
                        {
                            var matchPriority = Regex.Match(priority.Trim(), @"(?<num>.+?)\s\s?(?<day>\d{2})\/(?<month>\d{2})\/(?<year>\d{4})\s\s?(?<code>\D{2})");

                            if (matchPriority.Success)
                            {
                                var code = matchPriority.Groups["code"].Value.Trim();
                                if (code == "UK")
                                {
                                    code = "GB";
                                }
                                statusEvent.Biblio.Priorities.Add(new Priority()
                                {
                                    Country = code,
                                    Number = matchPriority.Groups["num"].Value.Trim(),
                                    Date = matchPriority.Groups["year"].Value.Trim() + "/" + matchPriority.Groups["month"].Value.Trim() + "/" + matchPriority.Groups["day"].Value.Trim()
                                });
                            }
                        }
                    }

                    if (sheet.GetRow(row).GetCell(6).ToString().Trim() != null)
                    {
                        var dateMatch = Regex.Match(sheet.GetRow(row).GetCell(6).StringCellValue.Trim(), @"(?<month>\d+)\/(?<day>\d+)\/(?<year>\d{4})");
                        if (dateMatch.Success)
                        {
                            var day = dateMatch.Groups["day"].Value.Trim();
                            if (day.Length == 1)
                            {
                                day = "0" + day;
                            }

                            var month = dateMatch.Groups["month"].Value.Trim();
                            if (month.Length == 1)
                            {
                                month = "0" + month;
                            }

                            statusEvent.LegalEvent.Date =dateMatch.Groups["year"].Value.Trim() + "/" + month + "/" + day;
                        }
                    }

                    legalStatusEvents.Add(statusEvent);
                }
            }
        }
        return legalStatusEvents;
    }

    private string? MakeMonth(string month) => month switch
    {
        "Feb" => "02",
        _ => null
    };

    internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool sendToProduction)
    {
        foreach (var rec in events)
        {
            var tmpValue = JsonConvert.SerializeObject(rec);
            var url = sendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
            HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri(url);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            StringContent content = new(tmpValue.ToString(), Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync("", content).Result;
            _ = result.Content.ReadAsStringAsync().Result;
        }
    }
}