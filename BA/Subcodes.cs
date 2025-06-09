using Diamond.Core.Models;
using Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BA
{
    public class Subcodes
    {
        private static readonly string I11 = "(11)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I26 = "(26)";
        private static readonly string I31 = "(31)";
        private static readonly string I32 = "(32)";
        private static readonly string I33 = "(33)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I72 = "(72)";
        private static readonly string I73 = "(73)";
        private static readonly string I74 = "(74)";
        private static readonly string I96 = "(96)";
        private static readonly string I97 = "(97)";
        private static readonly string I99 = "(I99)";

        private static readonly Regex StartKeyPatternSub1 = new Regex(@"(?=\(11\)\s*BA\/EP\d+)");
        private static readonly Regex StartKeyPatternSub2 = new Regex(@"(?=\(11\)\s*BA\/EP\d+)");
        private static readonly Regex StartKeyPatternSub3 = new Regex(@"(?=\(11\)\s*BA\/EP\d+)");
        private static readonly Regex StartKeyPatternSub4 = new Regex(@"(?=\(11\)\s*BA\/EP\d+)");

        private static List<string> GetRecordsFromTetml(List<XElement> elemList, Regex subPattern)
        {
            var textData = string.Join(" ", elemList.Cast<XElement>().Select(x => x.Value).ToList());
            var splittedRecords = subPattern.Split(textData).ToList();
            if (splittedRecords.Count == 0)
            {
                Console.WriteLine($"Eror getting text from Tetml and splitting to records by pattern");
                return null;
            }
            return splittedRecords;
        }

        public static List<LegalStatusEvent> ProcessSubcode4(List<XElement> elemList, string gazetteName)
        {
            var datePattern = @"\d{4}\-\d{2}\-\d{2}";
            var processedRecords = new List<LegalStatusEvent>();
            LegalStatusEvent patent;
            var patentCounter = 1;
            if (elemList != null && elemList.Count > 0)
            {
                var records = GetRecordsFromTetml(elemList, StartKeyPatternSub4).Where(x => !string.IsNullOrEmpty(x));

                foreach (var record in records)
                {
                    var splittedRecords = Methods.RecSplit(record);
                    if (splittedRecords.Count == 0)
                    {
                        Console.WriteLine($"Error splitting record by Inids: {record}");
                        continue;
                    }
                    patent = new LegalStatusEvent();
                    var biblioData = new Biblio();
                    var euPatent = new EuropeanPatent();
                    patent.GazetteName = gazetteName;
                    patent.SubCode = "4";
                    patent.SectionCode = "BA";
                    patent.CountryCode = "BA";
                    patent.Id = patentCounter++;

                    foreach (var rec in splittedRecords)
                    {
                        if (rec.StartsWith(I11))
                        {
                            biblioData.Publication.Number = rec.Replace(I11, "").Replace("\n", "").Trim();
                            if (biblioData.Publication.Number == null)
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I11} data is missing in {rec}");
                            }
                        }
                        else if (rec.StartsWith(I21))
                        {
                            biblioData.Application.Number = rec.Replace(I21, "").Replace("\n", "").Trim();
                            if (biblioData.Application.Number == null)
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I21} data is missing in {rec}");
                            }
                        }
                        else if (rec.StartsWith(I22))
                        {
                            biblioData.Application.Date = rec.Replace(I22, "").Replace("\n", "").Trim();
                            if (biblioData.Application.Date == null)
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I22} data is missing in {rec}");
                            }
                        }
                        else if (rec.StartsWith(I26))
                        {
                            var temp = rec.Replace(I26, "").Replace("\n", "").Trim();
                            if (!string.IsNullOrEmpty(temp))
                                biblioData.Publication.Authority = temp;
                        }
                        else if (rec.StartsWith(I31))
                        {
                            biblioData.Priorities = Methods.PrioritySplit(rec);
                            if (biblioData.Priorities == null || biblioData.Priorities.Count == 0)
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I31} data is missing in {rec}");
                            }
                        }
                        else if (rec.StartsWith(I51))
                        {
                            biblioData.Ipcs = Methods.ClassificationInfoSplit(rec.Replace("\n", "").Trim());
                            if (biblioData.Ipcs == null || biblioData.Ipcs.Count == 0)
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I51} data is missing in {rec}");
                            }
                        }
                        else if (rec.StartsWith(I54))
                        {
                            biblioData.Titles.Add(new Title
                            {
                                Language = "HR",
                                Text = rec.Replace(I54, "").Replace("-\n", " ").Replace("\n", " ").Trim()
                            });
                            if (biblioData.Titles == null || biblioData.Titles.Count == 0)
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I54} data is missing in {rec}");
                            }

                        }
                        else if (rec.StartsWith(I57))
                        {
                            var tmpAbstract = rec.Replace(I57, "").Replace("\n", " ").Trim();
                            if (tmpAbstract.Contains(I99))
                            {
                                var tmpNotes = tmpAbstract.Substring(tmpAbstract.IndexOf(I99)).Replace(I99, "").Trim();
                                patent.LegalEvent = new LegalEvent
                                {
                                    Note = $"|| Broj ostalih patentnih zahtjeva | {tmpNotes}",
                                    Language = "HR",
                                    Translations = new List<NoteTranslation>
                                    {
                                        new NoteTranslation
                                        {
                                            Language = "EN",
                                            Tr = $"|| The number of other claims | {tmpNotes}",
                                            Type = "INID"
                                        }
                                    }
                                };

                                tmpAbstract = tmpAbstract.Remove(tmpAbstract.IndexOf(I99)).Trim();
                            }

                            biblioData.Claims.Add(new DiamondProjectClasses.Claim 
                            {
                                Language = "HR",
                                Text = tmpAbstract,
                                Number ="1"
                            });
                            if (biblioData.Claims == null || biblioData.Claims.Count == 0)
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I57} data is missing in {rec}");
                            }
                        }
                        else if (rec.StartsWith(I72))
                        {
                            biblioData.Inventors = Methods.GetPersonsInfo(rec.Replace(I72, "").Trim());
                            if (biblioData.Inventors == null || biblioData.Inventors.Count == 0)
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I72} data is missing in {rec}");
                            }
                        }
                        else if (rec.StartsWith(I73))
                        {
                            biblioData.Assignees = Methods.GetPersonsInfo(rec.Replace(I73, "").Trim());
                            if (biblioData.Assignees == null || biblioData.Assignees.Count == 0)
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I73} data is missing in {rec}");
                            }
                        }
                        else if (rec.StartsWith(I74))
                        {
                            biblioData.Agents = Methods.GetPersonsShortInfo(rec.Replace(I74, "").Trim());
                            if (biblioData.Agents == null || biblioData.Agents.Count == 0)
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I74} data is missing in {rec}");
                            }
                        }
                        else if (rec.StartsWith(I96))
                        {
                            var tmp = rec.Replace(I96, "").Replace("\n", "").Trim();
                            if (Regex.IsMatch(tmp, datePattern))
                            {
                                euPatent.AppDate = Regex.Match(tmp, datePattern).Value;
                                euPatent.AppNumber = Regex.Replace(tmp, datePattern, "").Replace(",", "").Trim();
                            }
                            else
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I96} data is missing in {rec}");
                            }
                        }
                        else if (rec.StartsWith(I97))
                        {
                            var tmp = rec.Replace(I97, "").Replace("\n", "").Trim();
                            if (Regex.IsMatch(tmp, datePattern))
                            {
                                euPatent.PubDate = Regex.Match(tmp, datePattern).Value;
                            }
                            else
                            {
                                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I97} data is missing in {rec}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Element's Inid is missing: {rec}");
                        }
                    }
                    if (euPatent.AppNumber != null || euPatent.AppDate != null || euPatent.PubDate != null)
                    {
                        biblioData.EuropeanPatents = new List<EuropeanPatent> { euPatent };
                    }
                    patent.Biblio = biblioData;
                    processedRecords.Add(patent);
                }
            }
            return processedRecords;
        }
    }
}
