using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_VN
{
    class Subcodes
    {
        private static readonly string I11 = "(11)";
        private static readonly string I15 = "(15)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I30 = "(30)";
        private static readonly string I43 = "(43)";
        private static readonly string I45 = "(45)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I62 = "(62)";
        private static readonly string I67 = "(67)";
        private static readonly string I71 = "(71)";
        private static readonly string I72 = "(72)";
        private static readonly string I73 = "(73)";
        private static readonly string I74 = "(74)";
        private static readonly string I75 = "(75)";
        private static readonly string I76 = "(76)";
        private static readonly string I85 = "(85)";
        private static readonly string I86 = "(86)";
        private static readonly string I87 = "(87)";
        private static readonly string I01 = "(01)"; //Ngày yêu cầu thẩm định nội dung:
        private static readonly string I02 = "(02)"; //Ngày yêu cầu công bố sớm:

        private static Regex _sub12Regex = new Regex(@"(?=\(11\)\s\d+\b)");

        public static List<Diamond.Core.Models.LegalStatusEvent> _patentRecordsList;
        public static Diamond.Core.Models.LegalStatusEvent _patentRecord;
        public static string GetKindValue(string subcode)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string> { ["12"] = "A", ["13"] = "U", ["14"] = "B", ["15"] = "Y"};
            return pairs[subcode];
        }
        public static List<Diamond.Core.Models.LegalStatusEvent> ProcessSubCodes(List<XElement> xElements, string subcode, string sectionCode, string gazetteName)
        {
            _patentRecordsList = new List<Diamond.Core.Models.LegalStatusEvent>();
            int tmpID = 0;
            foreach (var record in SplitStringByRegex(xElements, _sub12Regex))
            {
                var splittedRecord = SplitRecordsByInids(record);
                //var splittedRecord = SplitRecordsByInids(Methods.ConvertText(record)); //Old Text format
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
                    _patentRecord.CountryCode = "VN";
                    _patentRecord.Id = tmpID++;
                    _patentRecord.Biblio.Publication.Kind = GetKindValue(subcode);
                    Integration.NoteTranslation noteTranslation = new Integration.NoteTranslation();

                    foreach (var inid in splittedRecord)
                    {
                        var inidValue = Regex.Replace(inid, @"^\(\d{2}\)\s", "").Replace("\n", " ").Trim();
                        if (inid.StartsWith(I11))
                        {
                            var numberRegex = new Regex(@"(?<Number>[\d,\-*]+)\b\s*(?<Kind>[A-Z]{1})");
                            var match = numberRegex.Match(inidValue);
                            if (match.Success)
                            {
                                _patentRecord.Biblio.Publication.Number = match.Groups["Number"].Value;
                                _patentRecord.Biblio.Publication.Kind = match.Groups["Kind"].Value;
                            }
                            else
                                _patentRecord.Biblio.Publication.Number = inidValue;
                        }
                        else if (inid.StartsWith(I21))
                        {
                            _patentRecord.Biblio.Application.Number = inidValue;
                        }
                        else if (inid.StartsWith(I22))
                        {
                            _patentRecord.Biblio.Application.Date = ConvertDate(inidValue);
                        }
                        else if (inid.StartsWith(I30))
                        {
                            _patentRecord.Biblio.Priorities = ConvertPriorities(Regex.Replace(inid, @"^\(\d{2}\)\s", "").Trim());
                        }
                        else if (inid.StartsWith(I43))
                        {
                            _patentRecord.Biblio.Publication.Date = ConvertDate(inidValue);
                        }
                        else if (inid.StartsWith(I45))
                        {
                            _patentRecord.Biblio.DOfPublication = new Integration.DOfPublication { date_45 = ConvertDate(inidValue) };
                        }
                        else if (inid.StartsWith(I51))
                        {
                            _patentRecord.Biblio.Ipcs = ConvertClassificationInfo(inidValue);
                            //_patentRecord.Biblio.Ipcs = OldConvertClassificationInfo(inidValue);
                        }
                        else if (inid.StartsWith(I54))
                        {
                            _patentRecord.Biblio.Titles.Add(new Integration.Title 
                            {
                                Text = inidValue,
                                Language = "VI"
                            });
                        }
                        else if (inid.StartsWith(I57))
                        {
                            _patentRecord.Biblio.Abstracts.Add(new Integration.Abstract 
                            {
                                Text = inidValue,
                                Language = "VI"
                            });
                        }
                        else if (inid.StartsWith(I62))
                        {
                            _patentRecord.Biblio.Related = new List<Integration.RelatedDocument>
                            {
                                new Integration.RelatedDocument
                                {
                                    Number = inidValue,
                                    Source = "62"
                                }
                            };
                        }
                        else if (inid.StartsWith(I67))
                        {
                            _patentRecord.Biblio.Related = new List<Integration.RelatedDocument>
                            {
                                new Integration.RelatedDocument
                                {
                                    Number = inidValue,
                                    Source = "67"
                                }
                            };
                        }
                        else if (inid.StartsWith(I71))
                        {
                            _patentRecord.Biblio.Applicants = ConvertApplicants(inidValue, "71");
                        }
                        else if (inid.StartsWith(I72))
                        {
                            _patentRecord.Biblio.Inventors = ConvertInventors(inidValue, "72");
                            //_patentRecord.Biblio.Inventors = OldConvertInventors(inidValue, "72");
                        }
                        else if (inid.StartsWith(I73))
                        {
                            _patentRecord.Biblio.Assignees = ConvertApplicants(inidValue, "73");
                        }
                        else if (inid.StartsWith(I74))
                        {
                            _patentRecord.Biblio.Agents = ConvertAgent(inidValue, "74");
                        }
                        else if (inid.StartsWith(I75))
                        {
                            _patentRecord.Biblio.InvOrApps = ConvertApplicants(inidValue, "75");
                        }
                        else if (inid.StartsWith(I76))
                        {
                            _patentRecord.Biblio.InvAppGrants = ConvertApplicants(inidValue, "76");
                        }
                        else if (inid.StartsWith(I85))
                        {
                            _patentRecord.Biblio.IntConvention.PctNationalDate = ConvertDate(inidValue);
                        }
                        else if (inid.StartsWith(I86))
                        {
                            var tmp = ConvertPCT(inidValue);
                            _patentRecord.Biblio.IntConvention.PctApplNumber = tmp.Number;
                            _patentRecord.Biblio.IntConvention.PctApplDate = tmp.Date;
                        }
                        else if (inid.StartsWith(I87))
                        {
                            var tmp = ConvertPCT(inidValue);
                            _patentRecord.Biblio.IntConvention.PctPublNumber = tmp.Number;
                            _patentRecord.Biblio.IntConvention.PctPublDate = tmp.Date;
                            _patentRecord.Biblio.IntConvention.PctPublKind = tmp.Kind;
                        }
                        else if (inid.StartsWith(I01))
                        {
                            var tmpValue = ConvertNote(inidValue).value;
                            var tmpDate = ConvertNote(inidValue).date;
                            _patentRecord.LegalEvent.Note += ("\n|| " + tmpValue + " | " + tmpDate).Trim();
                            _patentRecord.LegalEvent.Language = "VI";
                            noteTranslation.Language = "EN";
                            noteTranslation.Tr += ("\n|| Date of request for substantive examination | " + tmpDate).Trim();
                            noteTranslation.Type = "INID";
                        }
                        else if (inid.StartsWith(I02))
                        {
                            var tmpDate = ConvertNote(inidValue).date;
                            _patentRecord.LegalEvent.Note += ("\n|| " + ConvertNote(inidValue).value + " | " + tmpDate).Trim();
                            _patentRecord.LegalEvent.Language = "VI";
                            noteTranslation.Language = "EN";
                            noteTranslation.Tr += ("\n|| Date of request for early publication | " + tmpDate).Trim();
                            noteTranslation.Type = "INID";
                        }
                        else if (inid.StartsWith(I15))
                        {
                            _patentRecord.LegalEvent.Note += ("\n|| (15)Ngày cấp | " + inidValue).Trim();
                            _patentRecord.LegalEvent.Language = "VI";
                            noteTranslation.Language = "EN";
                            noteTranslation.Tr += ("\n|| (15) Date of grant | " + inidValue).Trim();
                            noteTranslation.Type = "INID";
                        }
                        else
                        {
                            Console.WriteLine($"Inid missed in specification {inid}");
                        }
                    }
                    _patentRecord.LegalEvent.Translations.Add(noteTranslation);
                    _patentRecordsList.Add(_patentRecord);
                }
            }
            return _patentRecordsList;
        }
        public static (string value, string date) ConvertNote(string note)
        {
            var spl = note?.Split(':').ToList();
            return (spl.FirstOrDefault().Trim(), spl.LastOrDefault().Trim());
        }
        public struct PCT
        {
            public string Number { get; set; }
            public string Date { get; set; }
            public string Kind { get; set; }
        }
        public static PCT ConvertPCT(string pct)
        {
            var intConvention = new PCT();
            var pctRegex = new Regex(@"(?<Number>[^\s]+)(\s(?<Kind>[A-Z]{1}\d{1}))*\s(?<Date>\d+[\/\-\.,]\d+[\/\-\.,]\d{4})");
            var numberRegex = new Regex(@"(?<Number>.*)(?<Kind>[A-Z]{1}\d{1}$)");
            try
            {
                var match = pctRegex.Match(pct);
                if (match.Success)
                {
                    intConvention.Number = match.Groups["Number"].Value.Trim();
                    intConvention.Date = ConvertDate(match.Groups["Date"].Value.Trim());
                    intConvention.Kind = match.Groups["Kind"]?.Value.Trim();
                }
                else
                    Console.WriteLine($"PCT format doesn't match regex: {pct}");

                var numMatch = numberRegex.Match(intConvention.Number);
                if (numMatch.Success)
                {
                    intConvention.Number = numMatch.Groups["Number"].Value.Trim();
                    intConvention.Kind = numMatch.Groups["Kind"].Value.Trim();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"PCT processing error: {e.Message}, PCT value: {pct}");
            }
            return intConvention;
        }
        public static List<Integration.PartyMember> ConvertAgent(string agent, string type) // 74
        {
            var agentList = new List<Integration.PartyMember>();
            var agentRegex = new Regex(@"(?<Name>.*)\((?<VN_Name>.*)\)");
            var match = agentRegex.Match(agent);
            if (match.Success)
            {
                agentList.Add(new Integration.PartyMember
                {
                    Name = match.Groups["Name"].Value,
                    Language = "VI",
                    Country = "VN",
                    Translations = new List<Integration.Translation> 
                    {
                        new Integration.Translation
                        { 
                            Language = "EN",
                            TrName = match.Groups["VN_Name"].Value.Trim(),
                            Type = type
                        }
                    }
                });
            }
            else
            {
                Console.WriteLine($"Agent value doesn't match pattern: {agent}");
            }
            return agentList;
        }
        public static List<Integration.PartyMember> OldConvertInventors(string inventors, string type) // 72
        {
            var invList = new List<Integration.PartyMember>();
            inventors = inventors.Replace("),", ");");
            var multipleInventors = inventors?.Split(';').Select(x => x.Replace(",", "").Trim()).ToList();
            var invRegex = new Regex(@"(?<Name>.*)\((?<Country>[A-Z]{2})\)");
            foreach (var inventor in multipleInventors)
            {
                var match = invRegex.Match(inventor);
                if (match.Success)
                {
                    var tmpInv = new Integration.PartyMember();
                    tmpInv.Name = match.Groups["Name"].Value.Trim();
                    tmpInv.Country = match.Groups["Country"].Value;
                    if (tmpInv.Country == "VN")
                    {
                        tmpInv.Language = "VI";
                        tmpInv.Translations = new List<Integration.Translation>
                        {
                            new Integration.Translation
                            {
                                TrName = ConvertViToEn(tmpInv.Name),
                                Language = "EN",
                                Type = type
                            }
                        };
                    }
                    else
                        tmpInv.Language = "EN";
                    invList.Add(tmpInv);
                }
                else
                {
                    Console.WriteLine($"Inventor value doesn't match pattern: {inventor}");
                }
            }
            return invList;
        }
        public static List<Integration.PartyMember> ConvertInventors(string inventors, string type) // 72
        {
            var invList = new List<Integration.PartyMember>();
            
            var multipleInventors = inventors?.Split(';').Select(x => x.Replace(",", "").Trim()).ToList();
            var invRegex = new Regex(@"(?<Name>.*)\((?<Country>[A-Z]{2})\)");
            foreach (var inventor in multipleInventors)
            {
                var match = invRegex.Match(inventor);
                if (match.Success)
                {
                    var tmpInv = new Integration.PartyMember();
                    tmpInv.Name = match.Groups["Name"].Value.Trim();
                    tmpInv.Country = match.Groups["Country"].Value;
                    if (tmpInv.Country == "VN")
                    {
                        tmpInv.Language = "VI";
                        tmpInv.Translations = new List<Integration.Translation>
                        {
                            new Integration.Translation
                            {
                                TrName = ConvertViToEn(tmpInv.Name),
                                Language = "EN",
                                Type = type
                            }
                        };
                    }
                    else
                        tmpInv.Language = "EN";
                    invList.Add(tmpInv);
                }
                else
                {
                    Console.WriteLine($"Inventor value doesn't match pattern: {inventor}");
                }
            }
            return invList;
        }
        public static List<Integration.PartyMember> ConvertApplicants(string applicants, string type) // 71,73
        {
            var appList = new List<Integration.PartyMember>();

            var multipleApplicants = Regex.Split(applicants, @"\b\d{1,2}\b\.\s")
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Trim())
                .ToList();
            var appRegex = new Regex(@"(?<Name>.*)\((?<Country>[A-Z]{2})\)(?<Address>.*)");
            foreach (var applicant in multipleApplicants)
            {
                var match = appRegex.Match(applicant);
                if (match.Success)
                {
                    var tmpApp = new Integration.PartyMember();
                    tmpApp.Name = match.Groups["Name"].Value.Trim();
                    tmpApp.Address1 = match.Groups["Address"].Value.Trim();
                    tmpApp.Country = match.Groups["Country"].Value.Trim();

                    if (tmpApp.Country == "VN")
                    {
                        tmpApp.Language = "VI";
                        tmpApp.Translations = new List<Integration.Translation>
                        {
                            new Integration.Translation
                            {
                                TrName = ConvertViToEn(tmpApp.Name),
                                Language = "EN",
                                Type = type
                            }
                        };
                    }
                    else
                        tmpApp.Language = "EN";

                    appList.Add(tmpApp);
                }
                else
                {
                    Console.WriteLine($"Applicant value doesn't match pattern: {applicant}");
                }
            }
            return appList;
        }
        public static List<Integration.Ipc> ConvertClassificationInfo(string classificationInfo) //51
        {
            classificationInfo = Regex.Replace(classificationInfo, @"\.*\,*", "");
            var ips = new List<Integration.Ipc>();
            int tmpEdition = 0;
            var ipsRegex = new Regex(@"^(?<Edition>\d{1})\s(?<Value>[A-Z]{1}\d{2}[A-Z]{1}.*)");
            var match = ipsRegex.Match(classificationInfo);
            if (match.Success)
            {
                classificationInfo = match.Groups["Value"].Value;
                tmpEdition = Int32.Parse(match.Groups["Edition"].Value);
            }
            var splittedIps = classificationInfo?.Split(';').Select(x => x.Trim()).ToList();
            foreach (var item in splittedIps)
            {
                var tmpIps = new Integration.Ipc();
                tmpIps.Class = item;
                if (tmpEdition != 0)
                    tmpIps.Edition = tmpEdition;
                ips.Add(tmpIps);
            }
            return ips;
        }
        public static List<Integration.Ipc> OldConvertClassificationInfo(string classificationInfo) //51
        {
            var ips = new List<Integration.Ipc>();
            int tmpEdition = 0;
            string tmpValueFirst = null;
            var ipsRegex = new Regex(@"^((?<Edition>\d{1}|\d{2})\s)*(?<Value>.*)");
            var valueFirst = new Regex(@"[A-Z]{1}\d{2}[A-Z]{1}");
            var valueSecond = new Regex(@"\b\d+\/\d+\b");
            var match = ipsRegex.Match(classificationInfo);
            if (match.Success)
            {
                classificationInfo = match.Groups["Value"].Value.Replace(",","");
                if (match.Groups["Edition"].Value != "")
                    tmpEdition = Int32.Parse(match.Groups["Edition"].Value);
                var splittedIps = classificationInfo?.Split(' ').Select(x => x.Trim()).ToList();
                var tmpIps = new Integration.Ipc();
                foreach (var item in splittedIps)
                {
                    if (tmpEdition != 0)
                        tmpIps.Edition = tmpEdition;
                    if (valueFirst.Match(item).Success)
                    {
                        tmpValueFirst = item;
                    }
                    else if (valueSecond.Match(item).Success)
                    {
                        if (tmpValueFirst != null)
                        {
                            tmpIps.Class = tmpValueFirst + " " +  item;
                            ips.Add(tmpIps);
                            tmpIps = new Integration.Ipc();
                        }
                    }
                }
            }
            else
            {
                
            }
            return ips;
        }
        public static List<Integration.Priority> ConvertPriorities(string priority)
        {
            var priorities = new List<Integration.Priority>();
            var regex = new Regex(@"(?<Number>.*?)\s(?<Date>\d+[\/\-\.,]\d+[\/\-\.,\s]\d{4})\s(?<Country>[A-Z]{2})");
            var matches = regex.Matches(priority);
            if (matches.Count > 0)
            {
                foreach (Match prio in matches)
                {
                    var match = regex.Match(prio.Value.Trim());
                    if (match.Success)
                    {
                        priorities.Add(new Integration.Priority
                        {
                            Number = match.Groups["Number"].Value.Trim(),
                            Date = ConvertDate(match.Groups["Date"].Value.Trim()),
                            Country = match.Groups["Country"].Value.Trim()
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Wrong priority format: {prio}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Wrong priority Regex: {priority}");
            }
            return priorities;

        }
        public static List<string> SplitStringByRegex(List<XElement> xElements, Regex regex)
        {
            var mergedElements = String.Join(" ", xElements.Select(x => x.Value).ToList())
                //.Replace(@"Ngày yêu cầu thẩm định nội dung:", "(01) Ngày yêu cầu thẩm định nội dung:")
                .Replace(@"Ngày yêu cầu thẩm định nội dung:", "(01) Ngày yêu cầu thẩm định nội dung:")
                .Replace(@"Ngμy yªu cÇu thÈm ®Þnh néi dung:", "(01) Ngày yêu cầu thẩm định nội dung:")
                .Replace(@"Ngày yêu cầu công bố sớm:", "(02) Ngày yêu cầu công bố sớm:");

            return regex.Split(mergedElements).Where(x => x.Trim().StartsWith(I11)).Select(x => x.Trim()).ToList();
        }
        public static List<string> SplitRecordsByInids(string record)
        {
            var tmpList = new List<string>();
            Regex regex = new Regex(@"\(\d{2}\)", RegexOptions.IgnoreCase);
            string tmpI57 = null;
            string tmpI87 = null;
            if (record.Contains(I57))
            {
                tmpI57 = record.Substring(record.IndexOf(I57)).Trim();
                record = record.Remove(record.IndexOf(I57)).Trim();
            }
            if (record.Contains(I87))
            {
                try
                {
                    var I87Regex = new Regex(@"\(87\).*?\d+[\/\-\.,\s]\d+[\/\-\.,\s]\d{4}");
                    tmpI87 = I87Regex.Match(record).Value;
                    record = record.Replace(tmpI87, "").Trim();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Record splitting error. Message:{e.Message}");
                }
            }
            MatchCollection matches = regex.Matches(record);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                    record = record.Replace(match.Value, "***" + match.Value);
                tmpList = record.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (tmpI57 != null)
                {
                    tmpList = tmpList.Concat(new string[] { tmpI57 }).ToList();
                }
                if (tmpI87 != null)
                {
                    tmpList = tmpList.Concat(new string[] { tmpI87 }).ToList();
                }
            }
            else
            {
                Console.WriteLine($"Record splitting failed. {record}");
            }
            return tmpList;
        }
        static string DateZeroAdd(string s)
        {
            return s.Length == 1 ? 0 + s : s; 
        }
        public static string ConvertDate(string s)
        {
            var datePat = new Regex(@"(?<day>\d+)[\/\-\.,\s](?<month>\d+)[\/\-\.,\s](?<year>\d{4})");
            var a = datePat.Match(s);
            if (a.Success)
            {
                return a.Groups["year"].Value + "-" + a.Groups["month"].Value + "-" + DateZeroAdd(a.Groups["day"].Value);
            }
            else
                Console.WriteLine($"Date pattern doesn't match\t {s}");
            return s;
        }

        public static string ConvertViToEn(string s)
        {
            char[] vi = { 'Ă', 'Á', 'À', 'Â', 'Ã', 'È', 'É', 'Ê', 'Ì', 'Í', 'Ò', 'Ó', 'Ô', 'Õ', 'Ù', 'Ú', 'Ý', 'ă', 'â', 'à', 'á', 'ã', 'è', 'é', 'ê', 'ì', 'í', 'ò', 'ó', 'ô', 'õ', 'ù', 'ý', 'Ỳ', 'Ỹ', 'ỳ', 'ỹ', 'Ỷ', 'ỷ', 'Ỵ', 'ỵ', 'ự', 'Ự', 'ử', 'Ử', 'ữ', 'Ữ', 'ừ', 'Ừ', 'ứ', 'Ứ', 'ư', 'Ư', 'ụ', 'Ụ', 'ủ', 'Ủ', 'ũ', 'Ũ', 'ợ', 'Ợ', 'ở', 'Ở', 'ỡ', 'Ỡ', 'ờ', 'Ờ', 'ớ', 'Ớ', 'ơ', 'Ơ', 'ộ', 'Ộ', 'ổ', 'Ổ', 'ỗ', 'Ỗ', 'ồ', 'Ồ', 'ố', 'Ố', 'ọ', 'Ọ', 'ỏ', 'Ỏ', 'ị', 'Ị', 'ỉ', 'Ỉ', 'ĩ', 'Ĩ', 'ệ', 'Ệ', 'ể', 'Ể', 'ễ', 'Ễ', 'ề', 'Ề', 'ế', 'Ế', 'ẹ', 'Ẹ', 'ẻ', 'Ẻ', 'ẽ', 'Ẽ', 'ặ', 'Ặ', 'ẳ', 'Ẳ', 'ẵ', 'Ẵ', 'ằ', 'Ằ', 'ắ', 'Ắ', 'ă', 'Ă', 'ậ', 'Ậ', 'ẩ', 'Ẩ', 'ẫ', 'Ẫ', 'ầ', 'Ầ', 'ấ', 'Ấ', 'ạ', 'Ạ', 'ả', 'Ả', 'đ', '₫', 'Đ' };
            char[] en = { 'A', 'A', 'A', 'A', 'A', 'E', 'E', 'E', 'I', 'I', 'O', 'O', 'O', 'O', 'U', 'U', 'Y', 'a', 'a', 'a', 'a', 'a', 'e', 'e', 'e', 'i', 'i', 'o', 'o', 'o', 'o', 'u', 'y', 'Y', 'Y', 'y', 'y', 'Y', 'y', 'Y', 'y', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'i', 'I', 'i', 'I', 'i', 'I', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'd', 'd', 'D' };
            StringBuilder strB = new StringBuilder(s);
            for (int i = 0; i < vi.Length; i++)
            {
                strB.Replace(vi[i], en[i]);
            }
            return strB.ToString();
        }
    }
}
