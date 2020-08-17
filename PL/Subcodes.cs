using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PL
{
    class Subcodes
    {
        public static readonly string I11 = "(11)";
        public static readonly string I13 = "(13)";
        public static readonly string I97 = "(97)";
        public static readonly string I22 = "(22)";
        public static readonly string I30 = "(30)";

        private static Regex _sub10Regex = new Regex(@"(?=\([A-Z]{1}\d{1}\)\s*\(11\)\s\d+\b)");
        private static Regex _sub10RecordRegex = new Regex(@"\((?<Kind>[\w,\d]+)\)\s*\(11\)\s*(?<Number>\d+)\s*(?<Date>\d{4}\s*\d{2}\s*\d{2})\s*(?<Note>.*)");
        private static Regex _sub25Regex = new Regex(@"(?=\(11\)\s\d+\b)");
        private static Regex _sub32Regex = new Regex(@"(?=\(11\)\s\d+\b)");


        public static List<Diamond.Core.Models.LegalStatusEvent> _patentRecordsList;
        public static Diamond.Core.Models.LegalStatusEvent _patentRecord;

        public static List<Diamond.Core.Models.LegalStatusEvent> ProcessSub_32(List<XElement> xElements, string subcode, string sectionCode, string gazetteName)
        {
            _patentRecordsList = new List<Diamond.Core.Models.LegalStatusEvent>();
            int tmpID = 0;
            foreach (var record in Methods.SplitStringByRegex(xElements, _sub10Regex))
            {
                var splittedRecord = _sub10RecordRegex.Match(record);
                if (splittedRecord.Success)
                {
                    _patentRecord = new Diamond.Core.Models.LegalStatusEvent();
                    _patentRecord.GazetteName = gazetteName;
                    _patentRecord.Biblio = new Integration.Biblio();
                    _patentRecord.Biblio.IntConvention = new Integration.IntConvention();
                    _patentRecord.LegalEvent = new Integration.LegalEvent();
                    _patentRecord.LegalEvent.Translations = new List<Integration.NoteTranslation>();
                    _patentRecord.SubCode = subcode;
                    _patentRecord.SectionCode = sectionCode;
                    _patentRecord.CountryCode = "PL";
                    _patentRecord.Id = tmpID++;

                    _patentRecord.Biblio.Publication.Number = splittedRecord.Groups["Number"].Value;
                    _patentRecord.Biblio.Publication.Kind = splittedRecord.Groups["Kind"].Value;
                    _patentRecord.LegalEvent.Number = splittedRecord.Groups["Date"].Value;
                    _patentRecord.LegalEvent.Note = $"|| Zakres wygaśnięcia | {splittedRecord.Groups["Note"].Value}";
                    _patentRecord.LegalEvent.Language = "PL";
                    _patentRecord.LegalEvent.Translations.Add(new Integration.NoteTranslation 
                    {
                        Language = "EN",
                        Tr = $"|| Expiry range | Patent has expired completely"
                    });

                    _patentRecordsList.Add(_patentRecord);
                }
                else
                {
                    Console.WriteLine($"Regex verification failed on: {record}");
                }
            }
            return _patentRecordsList;
        }

    }
}
