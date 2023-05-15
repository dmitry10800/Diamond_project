using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;

namespace Diamond_GC_Maksim_Excel
{
    internal class Methods
    {
        private string CurrentFileName;
        private int ID = 1;

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            List<Diamond.Core.Models.LegalStatusEvent> statusEvents = new();

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

                sheet = OpenedDocument.GetSheet("Sheet1") ?? OpenedDocument.GetSheet("Лист1");

                CultureInfo culture = new("ru-RU");

                if (subCode is "14")
                {
                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "GC",
                            SectionCode = "FD",
                            SubCode = subCode,
                            Id = ID++,
                            GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new(),
                            LegalEvent = new()
                        };

                        statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(1).ToString();

                        statusEvent.Biblio.Application.Date = DateTime.Parse(sheet.GetRow(row).GetCell(2).ToString(), culture).ToString("yyyy.MM.dd").Replace(".","/");

                        statusEvent.LegalEvent.Language = "EN";

                        statusEvent.LegalEvent.Note = "|| Decision No | " + sheet.GetRow(row).GetCell(3).ToString() + "\n"
                            + "|| Decision Date | " + DateTime.Parse(sheet.GetRow(row).GetCell(4).ToString(), culture).ToString("yyyy.MM.dd").Replace(".", "/");

                        var match = Regex.Match(CurrentFileName, @"_(?<date>\d{8})_");

                        if (match.Success)
                        {
                            statusEvent.LegalEvent.Date = match.Groups["date"].Value.Insert(4, "/").Insert(7, "/").Trim();
                        }

                        statusEvents.Add(statusEvent);
                    }
                }
                else if (subCode is "26")
                {
                    for (var row = 0; row <= sheet.LastRowNum; row++)
                    {
                        Diamond.Core.Models.LegalStatusEvent statusEvent = new()
                        {
                            CountryCode = "GC",
                            SectionCode = "FC",
                            SubCode = subCode,
                            Id = ID++,
                            GazetteName = Path.GetFileName(CurrentFileName.Replace(".xlsx", ".pdf")),
                            Biblio = new(),
                            LegalEvent = new()
                        };

                        statusEvent.Biblio.Application.Number = sheet.GetRow(row).GetCell(1).ToString();

                        statusEvent.Biblio.Application.Date = DateTime.Parse(sheet.GetRow(row).GetCell(4).ToString(), culture).ToString("yyyy.MM.dd").Replace(".", "/");

                        statusEvent.LegalEvent.Language = "EN";

                        statusEvent.LegalEvent.Note = "|| Decision No | " + sheet.GetRow(row).GetCell(6).ToString() + "\n";

                        statusEvent.LegalEvent.Date = DateTime.Parse(sheet.GetRow(row).GetCell(8).ToString(), culture).ToString("yyyy.MM.dd").Replace(".", "/");

                        statusEvents.Add(statusEvent);
                    }
                }
            }
            return statusEvents;
        }

        internal void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events, bool sendToProduction)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                string url;
                url = sendToProduction == true ? @"https://diamond.lighthouseip.online/external-api/import/legal-event" : @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event";
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
