using System.Globalization;
using System.Text.RegularExpressions;
using Integration;
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
        if (!files.Any())
        {
            files = directory.GetFiles("*.xls", SearchOption.AllDirectories).Select(file => file.FullName);
        }

        foreach (var xlsxFile in files)
        {
            _currentFileName = xlsxFile;

            ISheet sheet;

            XSSFWorkbook openedDocument;

            using (FileStream file = new(xlsxFile, FileMode.Open, FileAccess.Read))
            {
                openedDocument = new XSSFWorkbook(file);
            }

            sheet = openedDocument.GetSheet("Sheet1") ?? openedDocument.GetSheet("Лист1");

            if (subCode == "13")
            {
                var series = string.Empty;
                var fileNumber = string.Empty;
                var culture = new CultureInfo("ru-RU");
                for (var row = 1; row <= sheet.LastRowNum; row++)
                {
                    var statusEvent = new Diamond.Core.Models.LegalStatusEvent()
                    {
                        CountryCode = "PK",
                        SectionCode = "MK",
                        SubCode = subCode,
                        Id = _id++,
                        GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                        Biblio = new Biblio(),
                        LegalEvent = new LegalEvent()
                    };

                    var priority = new Priority();

                    var cellValueFileNumber = sheet.GetRow(row).GetCell(0)?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(cellValueFileNumber))
                    {
                        fileNumber = cellValueFileNumber;
                    }

                    var cellValueSeries = sheet.GetRow(row).GetCell(1)?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(cellValueSeries))
                    {
                        series = cellValueSeries;
                    }

                    var cellValuePublicationNumber = sheet.GetRow(row).GetCell(2)?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(cellValuePublicationNumber))
                    {
                        statusEvent.Biblio.Publication.Number = cellValuePublicationNumber;
                    }

                    var cellValueApplicationDate = sheet.GetRow(row).GetCell(3)?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(cellValueApplicationDate))
                    {
                        try
                        {
                            statusEvent.Biblio.Application.Date = DateTime.Parse(cellValueApplicationDate, culture)
                                .ToString("yyyy/MM/dd")
                                .Trim();
                        }
                        catch
                        {
                            var matchDate = Regex.Match(cellValueApplicationDate,
                                @"(?<day>\d{2}).?(?<month>\d{2}).?(?<year>\d{4})");

                            statusEvent.Biblio.Application.Date = matchDate.Groups["year"].Value.Trim() + "/" +
                                                                  matchDate.Groups["month"].Value.Trim() + "/" +
                                                                  matchDate.Groups["day"].Value.Trim();
                        }
                    }

                    var cellValuePriorityDate = sheet.GetRow(row).GetCell(4)?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(cellValuePriorityDate))
                    {
                        try
                        {
                            priority.Date = DateTime.Parse(cellValuePriorityDate, culture)
                                .ToString("yyyy/MM/dd")
                                .Trim();
                        }
                        catch
                        {
                            var matchDate = Regex.Match(cellValuePriorityDate,
                                @"(?<day>\d{2}).?(?<month>\d{2}).?(?<year>\d{4})");

                            priority.Date = matchDate.Groups["year"].Value.Trim() + "/" +
                                            matchDate.Groups["month"].Value.Trim() + "/" +
                                            matchDate.Groups["day"].Value.Trim();
                        }
                    }

                    var cellValuePriorityCountry = sheet.GetRow(row).GetCell(5)?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(cellValuePriorityCountry))
                    {
                        priority.Country = cellValuePriorityCountry;
                    }

                    var cellValueLegalEventDate = sheet.GetRow(row).GetCell(6)?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(cellValueLegalEventDate))
                    {
                        try
                        {
                            statusEvent.LegalEvent.Date = DateTime.Parse(cellValueLegalEventDate, culture)
                                .ToString("yyyy/MM/dd")
                                .Trim();
                        }
                        catch
                        {
                            var matchDate = Regex.Match(cellValueLegalEventDate,
                                @"(?<day>\d{2}).?(?<month>\d{2}).?(?<year>\d{4})");

                            statusEvent.LegalEvent.Date = matchDate.Groups["year"].Value.Trim() + "/" +
                                                          matchDate.Groups["month"].Value.Trim() + "/" +
                                                          matchDate.Groups["day"].Value.Trim();
                        }
                    }

                    if (priority.Date == null && priority.Country == null)
                    {
                        statusEvent.LegalEvent.Note = "|| Series | " + series + '\n'
                                                      + "|| File No | " + fileNumber + '\n'
                                                      + "|| Priorities | None";
                    }
                    else
                    {
                        statusEvent.LegalEvent.Note = "|| Series | " + series + '\n'
                                                      + "|| File No | " + fileNumber;
                    }

                    if (priority.Country != null || priority.Date != null)
                    {
                        statusEvent.Biblio.Priorities.Add(priority);
                    }

                    fileNumber = string.Empty;
                    legalStatusEvents.Add(statusEvent);
                }
            }
        }
        return legalStatusEvents;
    }
}