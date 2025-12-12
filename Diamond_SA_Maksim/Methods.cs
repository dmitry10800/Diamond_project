using Integration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;
using Diamond.Core.Models;

namespace Diamond_SA_Maksim;

public class Methods
{
    private string _currentFileName;
    private int _id = 1;

    internal List<LegalStatusEvent> Start(string path, string subCode)
    {
        var legalStatusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();
        var directory = new DirectoryInfo(path);
        var files = directory.GetFiles("*.xlsx", SearchOption.AllDirectories).Select(file => file.FullName);

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

            if (subCode == "1")
            {
                for (var row = 0; row <= sheet.LastRowNum; row++)
                {
                    var currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue;

                    var cell0 = GetCellValue(currentRow.GetCell(0));
                    var cell1 = GetCellValue(currentRow.GetCell(1));
                    if (string.IsNullOrWhiteSpace(cell0) || string.IsNullOrWhiteSpace(cell1)) continue;

                    var statusEvent = new Diamond.Core.Models.LegalStatusEvent()
                    {
                        CountryCode = "SA",
                        SectionCode = "FD",
                        SubCode = subCode,
                        Id = _id++,
                        GazetteName = Path.GetFileName(_currentFileName.Replace(".xlsx", ".pdf")),
                        Biblio = new Biblio(),
                        LegalEvent = new LegalEvent()
                    };

                    statusEvent.Biblio.Application.Number = cell1;
                    statusEvent.Biblio.Titles.Add(new Title()
                    {
                        Text = cell0,
                        Language = "AR"
                    });

                    var date = Regex.Match(Path.GetFileName(_currentFileName.Replace(".xlsx", "")), @"[0-9]{8}");
                    if (date.Success)
                    {
                        statusEvent.LegalEvent.Date = date.Value.Insert(4, "/").Insert(7, "/").Trim();
                    }

                    legalStatusEvents.Add(statusEvent);
                }
            }
            OpenedDocument.Close();
        }
        return legalStatusEvents;
    }

    private string GetCellValue(ICell cell)
    {
        if (cell == null) return string.Empty;

        return cell.ToString()?.Trim() ?? string.Empty;
    }
}