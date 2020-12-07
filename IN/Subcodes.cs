using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IN
{
    public class Subcodes
    {
        public static readonly string I12 = "(12) PATENT";
        public static readonly string I13 = "(13)";
        public static readonly string I19 = "(19)";
        public static readonly string I21 = "(21) Application No.";
        public static readonly string I22 = "(22) Date of filing of Application";
        public static readonly string I31 = "(31) Priority Document No";
        public static readonly string I32 = "(32) Priority Date";
        public static readonly string I33 = "(33) Name of priority country";
        public static readonly string I43 = "(43) Publication Date :";
        public static readonly string I51 = "(51) International classification";
        public static readonly string I54 = "(54) Title of the invention";
        public static readonly string I57 = "(57) Abstract";
        public static readonly string I61 = "(61) Patent of Addition to Application Number";
        public static readonly string I62 = "(62) Divisional to Application Number";
        public static readonly string I71 = "(71)Name of Applicant";
        public static readonly string I72 = "(72)Name of Inventor";
        public static readonly string I86 = "(86) International Application No";
        public static readonly string I87 = "(87) International Publication No";
        public static readonly string I99 = "(99) No. of Pages";


        private static Regex _sub1Regex = new Regex(@"(?=\(12\)\sPATENT)");
        private static Regex _sub2Regex = new Regex(@"(?=\b\d+\b\n\b\d{5,8}\b\n)");
        public static List<Diamond.Core.Models.LegalStatusEvent> _patentRecordsList;
        public static Diamond.Core.Models.LegalStatusEvent _patentRecord;

        public static List<Diamond.Core.Models.LegalStatusEvent> Process1SubCode(List<string> elements, string subcode, string sectionCode, string gazetteName)
        {
            _patentRecordsList = new List<Diamond.Core.Models.LegalStatusEvent>();
            int tmpID = 0;
            var totalRecords = Methods.SplitStringByRegex(elements, _sub1Regex);
            foreach (var record in totalRecords)
            {
                var splittedRecord = Methods.SplitRecordsByInids(record);

                if (splittedRecord.Count > 0)
                {
                    _patentRecord = new Diamond.Core.Models.LegalStatusEvent();
                    _patentRecord.GazetteName = gazetteName;
                    _patentRecord.Biblio = new Integration.Biblio();
                    _patentRecord.Biblio.IntConvention = new Integration.IntConvention();
                    _patentRecord.LegalEvent = new Integration.LegalEvent();
                    _patentRecord.LegalEvent.Translations = new List<Integration.NoteTranslation>();
                    _patentRecord.SubCode = subcode;
                    _patentRecord.SectionCode = sectionCode;
                    _patentRecord.CountryCode = "IN";
                    _patentRecord.Id = tmpID++;
                    _patentRecord.Biblio.Publication.Kind = subcode;

                    foreach (var inid in splittedRecord)
                    {
                        var inidValue = inid.Replace(":", "").Trim();
                        if (inid.StartsWith(I12))
                        {
                            _patentRecord.Biblio.Publication.LanguageDesignation = inidValue.Replace(@"(12)", "").Trim();
                        }
                        else if (inid.StartsWith(I21))
                        {
                            var pattern = new Regex(@"(?<Number>.*)\s*(?<Kind>[A-Z]{1}$)");
                            var tmpValue = inidValue.Replace(I21, "").Trim();
                            var match = pattern.Match(tmpValue);
                            if (match.Success)
                            {
                                _patentRecord.Biblio.Application.Number = match.Groups["Number"].Value.Trim();
                                _patentRecord.Biblio.Publication.Kind = match.Groups["Kind"].Value.Trim();
                            }
                            else
                            {
                                _patentRecord.Biblio.Application.Number = tmpValue;
                            }
                        }
                        else if (inid.StartsWith(I22))
                        {
                            _patentRecord.Biblio.Application.Date = Methods.ConvertDate(inidValue.Replace(I22, "").Trim());
                        }
                        else if (inid.StartsWith(I31) && !inid.Contains("NA"))
                        {
                            try
                            {
                                _patentRecord.Biblio.Priorities = new List<Integration.Priority>
                                {
                                    new Integration.Priority
                                    {
                                        Number = inidValue.Replace(I31, "").Trim(),
                                        Date = Methods.ConvertDate(splittedRecord.Select(x => x).Where(x => x.StartsWith(I32)).FirstOrDefault().Replace(I32, "").Replace(":", "").Trim()),
                                        Country = Methods.ToCountry(splittedRecord.Select(x => x).Where(x => x.StartsWith(I33)).FirstOrDefault().Replace(I33, "").Replace(":", "").Trim())
                                    }
                                };
                            }
                            catch (Exception)
                            {
                                Console.WriteLine($"Priority (field 51) processing error, record: { _patentRecord.Biblio.Application.Number }");
                            }
                        }
                        else if (inid.StartsWith(I43))
                        {
                            _patentRecord.Biblio.Publication.Date = Methods.ConvertDate(inidValue.Replace(I43, "").Trim());
                        }
                        else if (inid.StartsWith(I51))
                        {
                            _patentRecord.Biblio.Ipcs = Methods.ConvertClassificationInfo(inidValue.Replace(I51, "").Trim());
                        }
                        else if (inid.StartsWith(I54))
                        {
                            _patentRecord.Biblio.Titles.Add(new Integration.Title
                            {
                                Text = inidValue.Replace(I54, "").Trim(),
                                Language = "EN"
                            });
                        }
                        else if (inid.StartsWith(I57))
                        {
                            _patentRecord.Biblio.Abstracts.Add(new Integration.Abstract
                            {
                                Text = inidValue.Replace(I57, "").Trim(),
                                Language = "EN"
                            });
                        }
                        else if (inid.StartsWith(I61) && !inid.Contains("NA"))
                        {
                            var tmp = Methods.GetDivisional(inidValue.Replace(I61, "").Trim());
                            _patentRecord.Biblio.Related = new List<Integration.RelatedDocument>
                            {
                                new Integration.RelatedDocument
                                {
                                    Number = tmp.Number,
                                    Date = tmp.Date,
                                    Source = "61"
                                }
                            };
                        }
                        else if (inid.StartsWith(I62) && !inid.Contains("NA"))
                        {
                            var tmp = Methods.GetDivisional(inidValue.Replace(I62, "").Trim());
                            _patentRecord.Biblio.Related = new List<Integration.RelatedDocument>
                            {
                                new Integration.RelatedDocument
                                {
                                    Number = tmp.Number,
                                    Date = tmp.Date,
                                    Source = "62"
                                }
                            };
                        }
                        else if (inid.StartsWith(I71))
                        {
                            _patentRecord.Biblio.Applicants = Methods.ConvertApplicants(inidValue.Replace(I71, "").Trim(), "71");
                        }
                        else if (inid.StartsWith(I72))
                        {
                            _patentRecord.Biblio.Inventors = Methods.ConvertInventors(inidValue.Replace(I72, "").Trim(), "72");
                        }
                        else if (inid.StartsWith(I86) && !inid.Contains("NA"))
                        {
                            var tmp = Methods.ConvertPCT(inidValue.Replace(I86, "").Trim());
                            _patentRecord.Biblio.IntConvention.PctApplNumber = tmp.Number;
                            _patentRecord.Biblio.IntConvention.PctApplDate = tmp.Date;
                        }
                        else if (inid.StartsWith(I87) && !inid.Contains("NA"))
                        {
                            var tmp = Methods.ConvertPCTv2(inidValue.Replace(I87, "").Trim());
                            _patentRecord.Biblio.IntConvention.PctPublNumber = tmp.Number;
                        }
                        else if (inid.StartsWith(I99))
                        {
                            _patentRecord.LegalEvent.Note = Methods.GetNote(inidValue.Replace(I99, "").Trim());
                            _patentRecord.LegalEvent.Language = "EN";
                        }
                        else
                        {
                            Console.WriteLine($"Inid missed in specification {inid}");
                        }
                    }
                    _patentRecordsList.Add(_patentRecord);
                }
            }
            return _patentRecordsList;
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> Process2SubCode(string filePath, string subcode, string sectionCode, string gazetteName)
        {
            XSSFWorkbook OpenedDocument;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                OpenedDocument = new XSSFWorkbook(file);
            }
            ISheet sheet = OpenedDocument.GetSheet("Лист1");


            _patentRecordsList = new List<Diamond.Core.Models.LegalStatusEvent>();
            int tmpID = 0;

            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null && sheet.GetRow(row).GetCell(0) != null)
                {
                    _patentRecord = new Diamond.Core.Models.LegalStatusEvent();
                    _patentRecord.GazetteName = gazetteName;
                    _patentRecord.Biblio = new Integration.Biblio();
                    //_patentRecord.Biblio.IntConvention = new Integration.IntConvention();
                    _patentRecord.LegalEvent = new Integration.LegalEvent();
                    _patentRecord.SubCode = subcode;
                    _patentRecord.SectionCode = sectionCode;
                    _patentRecord.CountryCode = "IN";
                    _patentRecord.Id = tmpID++;
                    _patentRecord.Biblio.Publication.Kind = "FG";

                    _patentRecord.Biblio.Publication.Number = sheet.GetRow(row).GetCell(1).ToString();
                    _patentRecord.Biblio.Application.Number = sheet.GetRow(row).GetCell(2).ToString();

                    _patentRecord.Biblio.Application.Date = Methods.ConvertDate(sheet.GetRow(row).GetCell(3).ToString());

                    try
                    {
                        if (!string.IsNullOrEmpty(sheet.GetRow(row).GetCell(4).ToString()))
                        {
                            _patentRecord.Biblio.Priorities = new List<Integration.Priority>
                            {
                                new Integration.Priority
                                {
                                    Date = Methods.ConvertDate(sheet.GetRow(row).GetCell(4).ToString())
                                }
                            };
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Priority processing error: {_patentRecord.Biblio.Publication.Number}, value: {sheet.GetRow(row).GetCell(4)}");
                    }

                    _patentRecord.Biblio.Titles.Add(new Integration.Title
                    {
                        Text = sheet.GetRow(row).GetCell(5).ToString(),
                        Language = "EN"
                    });
                    _patentRecord.Biblio.Assignees = new List<Integration.PartyMember>
                                { new Integration.PartyMember
                                    {
                                        Name = sheet.GetRow(row).GetCell(6).ToString()
                                    }
                                };
                    _patentRecord.LegalEvent.Note = $"|| Date of Publication of Abstract u/s 11(A) | {Methods.ConvertDate(sheet.GetRow(row).GetCell(7).ToString())}\n|| Appropriate Office | {sheet.GetRow(row).GetCell(8)}";
                    _patentRecord.LegalEvent.Language = "EN";

                    _patentRecordsList.Add(_patentRecord);
                }
            }
            return _patentRecordsList;
        }
    }
}
