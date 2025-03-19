using Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_MD_Maksim
{
    internal class Methods
    {
        private string _currentFileName;
        private int _id = 1;

        private const string I21 = "(21)";
        private const string I13 = "(13)";
        private const string I51 = "(51)";
        private const string I96 = "(96)";
        private const string I97 = "(97)";
        private const string I87 = "(87)";
        private const string I31 = "(31)";
        private const string I32 = "(32)";
        private const string I33 = "(33)";
        private const string I71 = "(71)";
        private const string I712 = "(712)";
        private const string I72 = "(72)";
        private const string I54 = "(54)";

        internal List<Diamond.Core.Models.LegalStatusEvent> Start(string path, string subCode)
        {
            var statusEvents = new List<Diamond.Core.Models.LegalStatusEvent>();

            var directory = new DirectoryInfo(path);

            var files = new List<string>();

            foreach (var file in directory.GetFiles("*.tetml", SearchOption.AllDirectories))
            {
                files.Add(file.FullName);
            }

            XElement tet;

            List<XElement> xElements = null;

            foreach (var tetml in files)
            {
                _currentFileName = tetml;

                tet = XElement.Load(tetml);

                if (subCode == "2")
                {
                    xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                        .SkipWhile(val => !val.Value.StartsWith("GEPI publică fiecare solicitare de validare după informarea de către OEB privind achitarea taxei"))
                        .TakeWhile(val => !val.Value.StartsWith("FF4A Brevete de invenţie acordate /"))
                        .ToList();

                    if (xElements.Count == 0)
                    {
                        xElements = tet.Descendants().Where(val => val.Name.LocalName == "Text")
                            .SkipWhile(val => !val.Value.StartsWith("GEPI shall publish any request for validation after it has been informed by the EPO that the"))
                            .TakeWhile(val => !val.Value.StartsWith("FF4A Brevete de invenţie acordate /"))
                            .ToList();
                    }

                    var notes = Regex.Split(MakeText(xElements, subCode), @"(?=\(21\))", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).Where(val => val.StartsWith("(21)")).ToList();

                    foreach (var note in notes)
                    {
                        statusEvents.Add(MakeConvertedPatent(note, subCode, "BB2A"));
                    }
                }
            }
            return statusEvents;
        }

        internal string MakeText(List<XElement> xElements, string subCode)
        {
            var sb = new StringBuilder();

            if (subCode == "2")
            {
                foreach (var xElement in xElements)
                {
                    sb = sb.Append(xElement.Value + "\n");
                }
            }
            return sb.ToString();
        }

        internal Diamond.Core.Models.LegalStatusEvent MakeConvertedPatent(string note, string subCode, string sectionCode)
        {
            Diamond.Core.Models.LegalStatusEvent legalStatus = new()
            {
                SubCode = subCode,
                SectionCode = sectionCode,
                CountryCode = "MD",
                GazetteName = Path.GetFileName(_currentFileName.Replace(".tetml", ".pdf")),
                Id = _id++,
                Biblio = new Biblio()
            };

            var euroPatent = new EuropeanPatent();

            var priorityNumbers = new List<string>();
            var priorityDates = new List<string>();
            var priorityCountries = new List<string>();

            foreach (var inid in MakeInids(note))
            {
                if (inid.StartsWith(I21))
                {
                    legalStatus.Biblio.Application.Number = inid.Replace(I21, "").Trim();
                }
                if (inid.StartsWith(I13))
                {
                    legalStatus.Biblio.Publication.Kind = inid.Replace(I13, "").Trim();
                }
                if (inid.StartsWith(I51))
                {
                    var inid51 = Regex.Replace(inid, @"(\(51\)\s?Int.\s?Cl.:)", "", RegexOptions.Singleline).Trim();
                    var inids = Regex.Split(inid51, @"(?<=\))", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var iinid in inids)
                    {
                        var match = Regex.Match(iinid.Trim(), @"(?<class>.+)\((?<date>.+)\)");
                        if (match.Success)
                        {
                            legalStatus.Biblio.Ipcs.Add(new Ipc
                            {
                                Class = match.Groups["class"].Value.Trim(),
                                Date = match.Groups["date"].Value.Replace(".", "/").Trim()
                            });
                        }
                        else
                        {
                            Console.WriteLine($"{iinid} in 51");
                        }
                    }
                }
                if (inid.StartsWith(I96))
                {
                    var match = Regex.Match(inid.Replace(I96, "").Trim(), @"(?<number>.+),\s(?<date>[0-9]{4}.[0-9]{2}.[0-9]{2})", RegexOptions.Singleline);

                    if (match.Success)
                    {
                        euroPatent.AppNumber = match.Groups["number"].Value.Trim();
                        euroPatent.AppDate = match.Groups["date"].Value.Replace(".", "/").Trim();
                    }
                    else
                        Console.WriteLine($"{inid} in 96");
                }
                if (inid.StartsWith(I97))
                {
                    var match = Regex.Match(inid.Replace(I97, "").Trim(), @"(?<number>.+),\s(?<date>[0-9]{4}.[0-9]{2}.[0-9]{2})", RegexOptions.Singleline);

                    if (match.Success)
                    {
                        euroPatent.PubNumber = match.Groups["number"].Value.Trim();
                        euroPatent.PubDate = match.Groups["date"].Value.Replace(".", "/").Trim();
                    }
                    else
                        Console.WriteLine($"{inid} in 97");
                }
                if (inid.StartsWith(I87))
                {
                    var match = Regex.Match(inid.Replace(I87, "").Trim(), @"(?<number>.+),\s(?<date>[0-9]{4}.[0-9]{2}.[0-9]{2})", RegexOptions.Singleline);

                    if (match.Success)
                    {
                        legalStatus.Biblio.IntConvention.PctPublNumber = match.Groups["number"].Value.Trim();
                        legalStatus.Biblio.IntConvention.PctPublDate = match.Groups["date"].Value.Replace(".", "/").Trim();
                    }
                    else
                        Console.WriteLine($"{inid} in 87");
                }
                if (inid.StartsWith(I31))
                {
                    priorityNumbers = Regex.Split(inid.Replace(I31, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();
                }
                if (inid.StartsWith(I32))
                {
                    priorityDates = Regex.Split(inid.Replace(I32, "").Replace(".", "/").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();
                }
                if (inid.StartsWith(I33))
                {
                    priorityCountries = Regex.Split(inid.Replace(I33, "").Trim(), ";").Where(val => !string.IsNullOrEmpty(val)).ToList();
                }
                if (inid.StartsWith(I71))
                {
                    var applicants = Regex.Split(inid.Replace(I71, "").Replace("\r", "").Replace("\n", " ").Trim(), ";", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var applicant in applicants)
                    {
                        var match = Regex.Match(applicant.Trim(), @"(?<name>.+)(?<country>[A-Z]{2})", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            legalStatus.Biblio.Applicants.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Replace("- ", "").Trim().TrimEnd(','),
                                Country = match.Groups["country"].Value.Trim()
                            });
                        }
                        else
                            Console.WriteLine($"{applicant} in 71");
                    }
                }
                if (inid.StartsWith(I72))
                {
                    var inventors = Regex.Split(inid.Replace(I72, "").Replace("\r", "").Replace("\n", " ").Trim(), ";", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var inventor in inventors)
                    {
                        var match = Regex.Match(inventor.Trim(), @"(?<name>.+)(?<country>[A-Z]{2})", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            legalStatus.Biblio.Inventors.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Replace("- ", "").Trim().TrimEnd(','),
                                Country = match.Groups["country"].Value.Trim()
                            });
                        }
                        else
                            Console.WriteLine($"{inventor} in 72");
                    }
                }
                if (inid.StartsWith(I712))
                {
                    var inventorsApplicants = Regex.Split(inid.Replace(I712, "").Replace("\r", "").Replace("\n", " ").Trim(), ";", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();

                    foreach (var inventorsApplicant in inventorsApplicants)
                    {
                        var match = Regex.Match(inventorsApplicant.Trim(), @"(?<name>.+)(?<country>[A-Z]{2})", RegexOptions.Singleline);

                        if (match.Success)
                        {
                            legalStatus.Biblio.Inventors.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Replace("- ", "").Trim().TrimEnd(','),
                                Country = match.Groups["country"].Value.Trim()
                            });
                            legalStatus.Biblio.Applicants.Add(new PartyMember
                            {
                                Name = match.Groups["name"].Value.Replace("- ", "").Trim().TrimEnd(','),
                                Country = match.Groups["country"].Value.Trim()
                            });
                        }
                        else
                            Console.WriteLine($"{inventorsApplicant} in 712");
                    }

                }
                if (inid.StartsWith(I54))
                {
                    var lines = Regex.Split(inid.Trim(), "\n").Where(_ => !string.IsNullOrEmpty(_)).ToList();

                    if (lines.Count % 3 == 0)
                    {
                        var firstThird = lines.Take(lines.Count / 3).ToList();

                        var str = string.Empty;
                        foreach (var strItem in firstThird)
                        {
                            str += strItem + " ";
                        }

                        legalStatus.Biblio.Titles.Add(new Title
                        {
                            Language = "RO",
                            Text = str.Replace(I54, "").Replace("- ", "").Trim()
                        });
                    }
                    else
                    {
                        var regexMatch = Regex.Match(inid.Trim(), @"\(54\)(?<inid54>.+)[A-Z][a-z]", RegexOptions.Singleline);

                        if (regexMatch.Success)
                        {
                            legalStatus.Biblio.Titles.Add(new Title
                            {
                                Language = "RO",
                                Text = regexMatch.Groups["inid54"].Value.Replace("\r", "").Replace("\n", " ").Replace("- ", "").Trim()
                            });
                        }
                        else
                            Console.WriteLine($"{inid} --- 54");
                    }
                }
                else
                {
                    Console.WriteLine($"{inid} don't processed");
                }
            }

            for (var i = 0; i < priorityNumbers.Count; i++)
            {
                legalStatus.Biblio.Priorities.Add(new Priority
                {
                    Number = priorityNumbers[i].Trim(),
                    Date = priorityDates[i].Trim(),
                    Country = priorityCountries[i].Trim()
                });
            }

            legalStatus.Biblio.EuropeanPatents.Add(euroPatent);

            return legalStatus;
        }
        internal List<string> MakeInids(string note)
        {
            var inids = new List<string>();
            if (note.Contains("(71)(72)"))
            {
                var match = Regex.Match(note.Trim(), @"(?<part1>.+)\s(?<inid7172>\(71.+)\s(?<part2>\(\d.+)",
                    RegexOptions.Singleline);

                if (match.Success)
                {
                    inids = Regex.Split(match.Groups["part1"].Value.Trim(), @"(?=\([0-9]{2}\)\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();
                    inids.Add(match.Groups["inid7172"].Value.Replace("(71)(72)", "(712)").Trim());
                    inids.Add(match.Groups["part2"].Value.Trim());
                }
                else
                    Console.WriteLine($"{note} --- not split");
            }
            else
            {
                inids = Regex.Split(note.Trim(), @"(?=\([0-9]{2}\)\s)", RegexOptions.Singleline).Where(val => !string.IsNullOrEmpty(val)).ToList();
            }
            return inids;
        }
    }
}
