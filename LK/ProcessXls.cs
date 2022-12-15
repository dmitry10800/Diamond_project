using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace LK
{
    class ProcessXls
    {
        public static string DateNormalize(string tmpDate)
        {
            Regex patternDate = new Regex(@"(?<day>\d+)\s*\.*\/*\-*(?<month>\d+)\s*\.*\/*\-*(?<year>\d{4})");
            var a = patternDate.Match(tmpDate);
            if (a.Success)
            {
                return a.Groups["year"].Value + "-" + a.Groups["month"].Value + "-" + a.Groups["day"].Value;
            }
            else return tmpDate;
        }
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                string tmpValue = JsonConvert.SerializeObject(rec);
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; // STAGING
                string url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; // PRODUCTION
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
        public struct Elements
        {
            public string Number { get; set; }
            public string Date { get; set; }
            public string Owner { get; set; }
            public string Title { get; set; }
        }
        public static List<Elements> Records = new List<Elements>();
        public static void Read(string pathToXls)
        {
            using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(pathToXls)))
            {
                var firstSheet = xlPackage.Workbook.Worksheets.First(); //select sheet here
                var totalRows = firstSheet.Dimension.End.Row;
                var totalColumns = firstSheet.Dimension.End.Column;
                Elements record = new Elements();
                if (totalColumns != 4)
                    Console.WriteLine("Ошибка заполнения xls файла, лишние данные (должно быть 4 столбца)");

                var sb = new StringBuilder(); //this is your data
                for (int rowNum = 1; rowNum <= totalRows; rowNum++) //select starting row here
                {
                    var row = firstSheet.Cells[rowNum, 1, rowNum, totalColumns]/*.Select(c => c.Value == null ? string.Empty : c.Value.ToString())*/.ToList();
                    if (row.Count != 4)
                        Console.WriteLine($"Wrong count of cells in one row (should be 4). Manualy correct line {rowNum} of xlsx file.");
                    if (!string.IsNullOrEmpty(row[0].Text))
                    {
                        record.Number = row[0].Value.ToString();
                        record.Date = DateNormalize(row[1].Text);
                        record.Owner = row[2].Value.ToString();
                        record.Title = row[3].Value.ToString();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(row[2].Text))
                            record.Owner += $" {row[2].Value.ToString()}";
                        if (!string.IsNullOrEmpty(row[3].Text))
                            record.Title += $" {row[3].Value.ToString()}";
                    }
                    if (rowNum + 1 < totalRows && !string.IsNullOrEmpty(firstSheet.Cells[rowNum + 1, 1, rowNum, totalColumns].ToList()[0].Text) || rowNum == totalRows)
                    {
                        Records.Add(record);
                    }
                    //sb.AppendLine(string.Join(",", row));
                }
            }
            Console.WriteLine();

            SendToDiamond(ConvertToDiamond.Sub2Convertation(Records));

            //HSSFWorkbook OpenedDocument;
            //using (FileStream file = new FileStream(pathToXls, FileMode.Open, FileAccess.Read))
            //{
            //    OpenedDocument = new HSSFWorkbook(file);
            //}
            //ISheet sheet = OpenedDocument.GetSheet("CONCEDIDAS");

            //for (int row = 7; row <= sheet.LastRowNum; row++)
            //{
            //    if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
            //    {
            //        //OutElements.Add(new OutPutRecords
            //        //{
            //        //    APNR = sheet.GetRow(row).GetCell(1).ToString(),
            //        //    RGNR = sheet.GetRow(row).GetCell(6).ToString(),
            //        //    RGDA = DateNormalize(sheet.GetRow(row).GetCell(8).ToString()),
            //        //    TMNM = sheet.GetRow(row).GetCell(4).ToString(),
            //        //    TMTY = "W",
            //        //    OWNN = sheet.GetRow(row).GetCell(3).ToString(),
            //        //    CLAS = ClassZeroAdd(sheet.GetRow(row).GetCell(2).ToString().Trim()),
            //        //    DESC = "S",
            //        //    NOTE = "DESC missing"
            //        //});
            //    }
            //}
        }
    }
}
