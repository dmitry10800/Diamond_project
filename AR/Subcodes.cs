using Diamond.Core.Models;
using Integration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AR
{
    public class Subcodes
    {
        private static readonly string I10 = "(10)";
        private static readonly string I11 = "(11)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I24 = "(24)";
        private static readonly string I24A = "(--)";
        private static readonly string I30 = "(30)";
        private static readonly string I47 = "(47)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I71 = "(71)";
        private static readonly string I72 = "(72)";
        private static readonly string I74 = "(74)";
        private static readonly string I45 = "(45)";
        private static readonly string I62 = "(62)";
        private static readonly string note = "Sigue";

        private static readonly Regex StartKeyPattern = new Regex(@"(?=\<Primera\>)");
        private static readonly Regex StartKeyPatternSub2 = new Regex(@"(?=\(10\)\s*Patente de Invención)");
        private static readonly Regex StartKeyPatternSub3 = new Regex(@"(?=\(10\)\s*Modelo de Utilidad)");
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

        public static List<LegalStatusEvent> ProcessSubcode(List<XElement> elemList, string gazetteName)
        {
            string datePattern = @"\d{4}\-\d{2}\-\d{2}";
            List<LegalStatusEvent> processedRecords = new List<LegalStatusEvent>();
            int patentCounter = 1;
            if (elemList != null && elemList.Count > 0)
            {
                var records = GetRecordsFromTetml(elemList, StartKeyPattern).Where(x => !string.IsNullOrEmpty(x));

                foreach (string record in records)
                {
                    List<string> splittedRecords = Methods.RecSplit(record);
                    if (splittedRecords.Count == 0)
                    {
                        Console.WriteLine($"Error splitting record by Inids: {record}");
                        continue;
                    }

                    LegalStatusEvent legalEvent = new LegalStatusEvent();

                    legalEvent.CountryCode = "AR";
                    legalEvent.Id = patentCounter++;
                    legalEvent.GazetteName = gazetteName;
                    legalEvent.SectionCode = "FG";


                    Biblio biblioData = new Biblio();

                    biblioData.EuropeanPatents = new List<EuropeanPatent>();

                    EuropeanPatent europeanPatent = new EuropeanPatent();

                    IntConvention intConvention = new IntConvention();

                    CultureInfo cultureInfo = new CultureInfo("ru-Ru");

                    LegalEvent legal = new LegalEvent();

                    NoteTranslation noteTranslation = new NoteTranslation();

                    DOfPublication dOfPublication = new DOfPublication();

                    foreach (string element in splittedRecords)
                    {

                        if (element.StartsWith(I10))
                        {
                            string text = ReplaceInid(element, I10);

                            if (text.Contains("Patente "))
                            {
                                legalEvent.SubCode = "2";
                                biblioData.Publication.LanguageDesignation = text;
                            }
                            else
                            if (text.Contains("Modelo ")) {
                                legalEvent.SubCode = "3";
                                biblioData.Publication.LanguageDesignation = text;
                            }
                            else
                            {
                                Console.WriteLine($"В 10-м поле вот этот элемент не разбился - [{element}]");
                            }
                        }
                        else
                        if (element.StartsWith(I11))
                        {
                            string text = ReplaceInid(element, I11);

                            Match match = Regex.Match(text, @"([A-Z]{2}\d+\D\d)");

                            if (match.Success)
                            {
                                string tmp = match.Value.Trim();
                                Match match1 = Regex.Match(tmp, @"(?<number>[A-Z]{2}\d+)(?<kind>[A-Z]\d+)");

                                if (match1.Success)
                                {
                                    biblioData.Publication.Number = match1.Groups["number"].Value.Trim();
                                    biblioData.Publication.Kind = match1.Groups["kind"].Value.Trim();
                                }
                                else Console.WriteLine($"В 11-м поле в match не разбились на группы- [{tmp}]");

                            }
                            else Console.WriteLine($"В 11-м поле не найдет Match - [{element}]");
                        }
                        else
                        if (element.StartsWith(I21))
                        {
                            string text = ReplaceInid(element, I21);

                            Match match = Regex.Match(text, @"[A-Z]\s\d+");

                            if (match.Success)
                            {
                                biblioData.Application.Number = match.Value.Trim();
                            }
                            else Console.WriteLine($"В 21 поле не был найден App Number - [{text}]");
                        }
                        else
                        if (element.StartsWith(I22))
                        {
                            string text = ReplaceInid(element, I22);

                            Match match = Regex.Match(text, @"(\d{2}\/\d{2}\/\d{4})");

                            if (match.Success)
                            {
                                biblioData.Application.Date = DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace('.', '/').Trim();
                            }
                            else Console.WriteLine($"В 22 поле не был найден App Date - [{text}]");
                        }
                        else
                        if (element.StartsWith(I24))
                        {
                            string text = ReplaceInid(element, I24);

                            Match match = Regex.Match(text, @"(\d{2}\/\d{2}\/\d{4})");

                            if (match.Success)
                            {
                                biblioData.Application.EffectiveDate = DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace('.', '/').Trim();
                            }
                            else Console.WriteLine($"В 24 поле не был найден App Effect Date - [{text}]");
                        }
                        else
                        if (element.StartsWith(I24A))
                        {
                            string text = ReplaceInid(element, I24A);

                            Match match = Regex.Match(text, @"(?<note>.+)\s(?<date>\d{2}\/\d{2}\/\d{4})");

                            if (match.Success)
                            {
                                legal.Note = "|| " + match.Groups["note"].Value.Trim() + " | " + DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                                legal.Language = "ES";
                                noteTranslation.Language = "EN";
                                noteTranslation.Tr = "|| Expiration date | " + DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy/MM/dd").Replace(".", "/").Trim();
                                noteTranslation.Type = "note";
                            }
                            else Console.WriteLine($"В -- поле не был найден LegalEvent.Date - [{text}]");
                        }
                        else
                        if (element.StartsWith(I30))
                        {
                            string text = ReplaceInid(element, I30);

                            List<string> notes = Regex.Split(text, @";").ToList();

                            biblioData.Priorities = new List<Priority>();

                            foreach (string note in notes)
                            {
                                Match match = Regex.Match(note, @"(?<code>[A-Z]{2})\s(?<number>.+)\s(?<date>\d{2}\/\d{2}\/\d{3,4})");

                                if (match.Success)
                                {
                                    biblioData.Priorities.Add(new Priority
                                    {
                                        Country = match.Groups["code"].Value.Trim(),
                                        Number = match.Groups["number"].Value.Trim(),
                                        Date = DateTime.Parse(match.Groups["date"].Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace('.', '/').Trim()
                                    });
                                }
                                else Console.WriteLine($"В 30-ом поле match не сработал на эту запись  - [{note}]");
                            }
                        }
                        else
                        if (element.StartsWith(I47))
                        {
                            string text = ReplaceInid(element, I47);

                            Match match = Regex.Match(text, @"(\d{2}\/\d{2}\/\d{4})");

                            if (match.Success)
                            {
                                dOfPublication.date_47 = DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace('.', '/').Trim();
                            }
                            else Console.WriteLine($"В 47 поле не был найден App Effect Date - [{text}]");
                        }
                        else
                        if (element.StartsWith(I51))
                        {
                            string text = ReplaceInid(element, I51).Replace("\r","").Replace("\n"," ").Trim();

                            biblioData.Ipcs = new List<Ipc>();

                            Match match = Regex.Match(text, @"([A-Z]\d+[A-Z].+)");

                            string temp = match.Value.Replace(",", " ").Replace(":", " ").Trim();

                            List<string> firstIpcs = Regex.Split(temp, @"\s").Where(val =>!string.IsNullOrEmpty(val)).ToList();

                            List<string> secondIpcs = new List<string>();

                            foreach (string firstIpc in firstIpcs)
                            {
                                string tmp = firstIpc.TrimEnd(',').TrimEnd(';');


                                if(Regex.IsMatch(tmp, @"(?<gr1>[A-Z]\d+\D)(?<gr2>\d+\/\d+)"))
                                {
                                    Match match1 = Regex.Match(tmp, @"(?<gr1>[A-Z]\d+\D)(?<gr2>\d+\/\d+)");

                                    secondIpcs.Add(match1.Groups["gr1"].Value.Trim());

                                    secondIpcs.Add(match1.Groups["gr2"].Value.Trim());
                                }
                                else secondIpcs.Add(tmp);

                            }

                            List<string> finalIpcs = new List<string>();

                            foreach (string secondIpc in secondIpcs)
                            {
                                if (secondIpc.Contains("/")) 
                                {
                                    if(Regex.IsMatch(finalIpcs[finalIpcs.Count-1], @"(?<gr1>[A-Z]\d+\D)\s(?<gr2>\d+\/\d+)"))
                                    {
                                        Match match1 = Regex.Match(finalIpcs[finalIpcs.Count - 1], @"(?<gr1>[A-Z]\d+\D)\s(?<gr2>\d+\/\d+)");

                                        finalIpcs.Add(match1.Groups["gr1"].Value.Trim() + " " + secondIpc);

                                    }
                                    else
                                    {
                                        finalIpcs[finalIpcs.Count - 1] = finalIpcs[finalIpcs.Count - 1] + " " + secondIpc;
                                    }
                                }
                                else
                                {
                                    finalIpcs.Add(secondIpc);
                                }
                            }

                            foreach (string item in finalIpcs)
                            {
                                biblioData.Ipcs.Add(new Ipc
                                {
                                    Class = item
                                });
                            }
                        }
                        else
                        if (element.StartsWith(I54))
                        {
                            string text = ReplaceInid(element, I54);

                            Match match = Regex.Match(text, @"([A-Z]{2}.+)");

                            if (match.Success)
                            {
                                biblioData.Titles.Add(new Title
                                {
                                    Text = match.Value.TrimEnd('.').Trim(),
                                    Language = "ES"
                                });
                            }
                            else Console.WriteLine($"В 54 поле не был найден Title - [{text}]");
                        }
                        else
                        if (element.StartsWith(I57))
                        {
                            string text = ReplaceInid(element, I57);

                            biblioData.Claims.Add(new DiamondProjectClasses.Claim
                            {
                                Text = text,
                                Number = "1",
                                Language ="ES"
                            });
                        }
                        else
                        if (element.StartsWith(note)||element.StartsWith("<>"))
                        {
                            if (element != "<>")
                            {
                                Match match = Regex.Match(element.Trim(), @"(?<gr1>.+)\s(?<gr2>.+)\s(?<gr3>.+)");

                                legal.Note = legal.Note + " || REIVINDICACIONES | " + element.Trim();

                                noteTranslation.Tr = noteTranslation.Tr + " || Claims | " + match.Groups["gr2"].Value.Trim() + " Claims follow";

                                legal.Translations = new List<NoteTranslation> { noteTranslation };
                                legalEvent.LegalEvent = legal;
                            }
                            else
                            {
                                legal.Translations = new List<NoteTranslation> { noteTranslation };
                                legalEvent.LegalEvent = legal;
                            }
                        }
                        else
                        if (element.StartsWith(I71))
                        {
                            string text = ReplaceInid(element, I71);

                            List<string> applicants = Regex.Split(text, @"(?<=,\s[A-Z]{2}\n?$)").Where(val => !string.IsNullOrEmpty(val)).ToList();

                            biblioData.Applicants = new List<PartyMember>();

                            foreach (string applicant in applicants)
                            {
                                string tmp = applicant.Replace("\r", "").Replace("\n", " ").Trim();
                                Match match = Regex.Match(tmp, @"(?<adress>[A-Z]{2}.+)\s(?<code>[A-Z]{2}$)");

                                if (match.Success)
                                {
                                    biblioData.Applicants.Add(new PartyMember
                                    {
                                        Address1 = match.Groups["adress"].Value.Trim().TrimEnd(','),
                                        Country = match.Groups["code"].Value.Trim()
                                    });
                                }
                                else
                                {
                                    biblioData.Applicants.Add(new PartyMember
                                    {
                                        Address1 = tmp
                                    });
                                }
                            }

                        }
                        else
                        if (element.StartsWith(I72))
                        {
                            string text = ReplaceInid(element, I72).Replace("\r", "").Replace("\n", " ").Trim();

                            Match match = Regex.Match(text, @"([A-Z]{2}.+)");

                            biblioData.Inventors = new List<PartyMember>();

                            List<string> names = Regex.Split(match.Value, @"\s").ToList();

                            List <string> inventors = new List<string>();

                            for (int i = 0; i < names.Count; i++)
                            {
                                if (names[i].Contains(","))
                                {
                                    string tmp = names[i].TrimEnd('-');
                                    inventors.Add(tmp);
                                }
                                else
                                {
                                    if (inventors.Count != 0)
                                    {
                                        string tmp = names[i].TrimEnd('-');
                                        inventors[inventors.Count - 1] = inventors[inventors.Count - 1] + " " + tmp;
                                    }
                                    else
                                    {
                                        string tmp = names[i].TrimEnd('-');
                                        inventors.Add(tmp);
                                    }

                                }
                            }
                            foreach (string inventor in inventors)
                            {
                                biblioData.Inventors.Add(new PartyMember
                                {
                                    Name = inventor
                                });
                            }
                        }
                        else
                        if (element.StartsWith(I74))
                        {

                            Match match = Regex.Match(element, @"(?<inid>\(\d{2}\))\s(?<info>.+)\s(?<numbers>\d+)");

                            if (match.Success)
                            {
                                legal.Note = legal.Note + " || " + match.Groups["inid"].Value.Trim() + " | " + match.Groups["info"].Value.Trim() + " | " + match.Groups["numbers"].Value.Trim();
                                noteTranslation.Tr = noteTranslation.Tr + " || " + match.Groups["inid"].Value.Trim() + " | Agent's number | " + match.Groups["numbers"].Value.Trim();
                            }

                            biblioData.Agents = new List<PartyMember>() { new PartyMember { Name = match.Groups["numbers"].Value.Trim() } };


                        }
                        else
                        if (element.StartsWith(I45))
                        {
                            string text = ReplaceInid(element, I45);

                            Match match = Regex.Match(text, @"(\d{2}\/\d{2}\/\d{4})");

                            if (match.Success)
                            {
                                dOfPublication.date_45 = DateTime.Parse(match.Value.Trim(), cultureInfo).ToString("yyyy.MM.dd").Replace('.', '/').Trim();

                                biblioData.DOfPublication = dOfPublication;
                            }
                            else Console.WriteLine($"В 45 поле не был найден App Effect Date - [{text}]");
                        }
                        else
                        if (element.StartsWith(I62))
                        {
                            string text = ReplaceInid(element, I62);

                            Match match = Regex.Match(text, @"(?<number>[A-R]{2}\d+)(?<kind>[A-Z]\d+)");

                            biblioData.Related = new List<RelatedDocument>();

                            if (match.Success)
                            {
                                biblioData.Related.Add(new RelatedDocument
                                {
                                    Number = match.Groups["number"].Value.Trim(),
                                    Type = match.Groups["kind"].Value.Trim(),
                                    Source = "62"
                                });
                            }
                        }
                        else Console.WriteLine($"Этот inid не обработан {element}");
                    }
                    legalEvent.Biblio = biblioData;
                    processedRecords.Add(legalEvent);



                    //    patent.SubCode = "4";

                    //    foreach (var rec in splittedRecords)
                    //    {
                    //        if (rec.StartsWith(I11))
                    //        {
                    //            biblioData.Publication.Number = rec.Replace(I11, "").Replace("\n", "").Trim();
                    //            if (biblioData.Publication.Number == null)
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I11} data is missing in {rec}");
                    //            }
                    //        }
                    //        else if (rec.StartsWith(I21))
                    //        {
                    //            biblioData.Application.Number = rec.Replace(I21, "").Replace("\n", "").Trim();
                    //            if (biblioData.Application.Number == null)
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I21} data is missing in {rec}");
                    //            }
                    //        }
                    //        else if (rec.StartsWith(I22))
                    //        {
                    //            biblioData.Application.Date = rec.Replace(I22, "").Replace("\n", "").Trim();
                    //            if (biblioData.Application.Date == null)
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I22} data is missing in {rec}");
                    //            }
                    //        }
                    //        else if (rec.StartsWith(I26))
                    //        {
                    //            var temp = rec.Replace(I26, "").Replace("\n", "").Trim();
                    //            if (!string.IsNullOrEmpty(temp))
                    //                biblioData.Publication.Authority = temp;
                    //        }
                    //        else if (rec.StartsWith(I31))
                    //        {
                    //            biblioData.Priorities = Methods.PrioritySplit(rec);
                    //            if (biblioData.Priorities == null || biblioData.Priorities.Count == 0)
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I31} data is missing in {rec}");
                    //            }
                    //        }
                    //        else if (rec.StartsWith(I51))
                    //        {
                    //            biblioData.Ipcs = Methods.ClassificationInfoSplit(rec.Replace("\n", "").Trim());
                    //            if (biblioData.Ipcs == null || biblioData.Ipcs.Count == 0)
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I51} data is missing in {rec}");
                    //            }
                    //        }
                    //        else if (rec.StartsWith(I54))
                    //        {
                    //            biblioData.Titles.Add(new Title
                    //            {
                    //                Language = "HR",
                    //                Text = rec.Replace(I54, "").Replace("-\n", " ").Replace("\n", " ").Trim()
                    //            });
                    //            if (biblioData.Titles == null || biblioData.Titles.Count == 0)
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I54} data is missing in {rec}");
                    //            }

                    //        }
                    //        else if (rec.StartsWith(I57))
                    //        {
                    //            var tmpAbstract = rec.Replace(I57, "").Replace("\n", " ").Trim();
                    //            if (tmpAbstract.Contains(I99))
                    //            {
                    //                var tmpNotes = tmpAbstract.Substring(tmpAbstract.IndexOf(I99)).Replace(I99, "").Trim();
                    //                patent.LegalEvent = new LegalEvent
                    //                {
                    //                    Note = $"|| Broj ostalih patentnih zahtjeva | {tmpNotes}",
                    //                    Language = "HR",
                    //                    Translations = new List<NoteTranslation>
                    //                    {
                    //                        new NoteTranslation
                    //                        {
                    //                            Language = "EN",
                    //                            Tr = $"|| The number of other claims | {tmpNotes}",
                    //                            Type = "note"
                    //                        }
                    //                    }
                    //                };

                    //                tmpAbstract = tmpAbstract.Remove(tmpAbstract.IndexOf(I99)).Trim();
                    //            }

                    //            biblioData.Abstracts.Add(new Abstract
                    //            {
                    //                Language = "HR",
                    //                Text = tmpAbstract
                    //            });
                    //            if (biblioData.Abstracts == null || biblioData.Abstracts.Count == 0)
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I57} data is missing in {rec}");
                    //            }
                    //        }
                    //        else if (rec.StartsWith(I72))
                    //        {
                    //            biblioData.Inventors = Methods.GetPersonsInfo(rec.Replace(I72, "").Trim());
                    //            if (biblioData.Inventors == null || biblioData.Inventors.Count == 0)
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I72} data is missing in {rec}");
                    //            }
                    //        }
                    //        else if (rec.StartsWith(I73))
                    //        {
                    //            biblioData.Assignees = Methods.GetPersonsInfo(rec.Replace(I73, "").Trim());
                    //            if (biblioData.Assignees == null || biblioData.Assignees.Count == 0)
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I73} data is missing in {rec}");
                    //            }
                    //        }
                    //        else if (rec.StartsWith(I74))
                    //        {
                    //            biblioData.Agents = Methods.GetPersonsShortInfo(rec.Replace(I74, "").Trim());
                    //            if (biblioData.Agents == null || biblioData.Agents.Count == 0)
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I74} data is missing in {rec}");
                    //            }
                    //        }
                    //        else if (rec.StartsWith(I96))
                    //        {
                    //            var tmp = rec.Replace(I96, "").Replace("\n", "").Trim();
                    //            if (Regex.IsMatch(tmp, datePattern))
                    //            {
                    //                euPatent.AppDate = Regex.Match(tmp, datePattern).Value;
                    //                euPatent.AppNumber = Regex.Replace(tmp, datePattern, "").Replace(",", "").Trim();
                    //            }
                    //            else
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I96} data is missing in {rec}");
                    //            }
                    //        }
                    //        else if (rec.StartsWith(I97))
                    //        {
                    //            var tmp = rec.Replace(I97, "").Replace("\n", "").Trim();
                    //            if (Regex.IsMatch(tmp, datePattern))
                    //            {
                    //                euPatent.PubDate = Regex.Match(tmp, datePattern).Value;
                    //            }
                    //            else
                    //            {
                    //                Console.WriteLine($"Patent: {biblioData.Publication.Number}. Inid {I97} data is missing in {rec}");
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Console.WriteLine($"Element's Inid is missing: {rec}");
                    //        }
                    //    }
                    //    if (euPatent.AppNumber != null || euPatent.AppDate != null || euPatent.PubDate != null)
                    //    {
                    //        biblioData.EuropeanPatents = new List<EuropeanPatent> { euPatent };
                    //    }
                    //    patent.Biblio = biblioData;
                    //    processedRecords.Add(patent);
                }
            }
            return processedRecords;
        }
        internal static string ReplaceInid(string text, string inid) => text.Replace(inid, "").Trim();
    }
}
