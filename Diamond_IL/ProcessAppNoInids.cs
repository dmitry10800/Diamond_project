using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Diamond_IL
{
    class ProcessAppNoInids
    {
        public class ElementOut
        {
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I31 { get; set; } // number
            public string[] I32 { get; set; } // date
            public string[] I33 { get; set; } // country
            public string I51Version { get; set; }
            public string I51Number { get; set; }
            public string I54Eng { get; set; }
            public string I54Hebrew { get; set; }
            public string I71NameENG { get; set; }
            public string I71NameIL { get; set; }
            public string I86 { get; set; }
            public string I87 { get; set; }
        }

        public List<ElementOut> OutputValue(string elemList)
        {
            var ElementsOut = new List<ElementOut>();
            ElementOut currentElement = null;
            if (elemList != null)
            {
                ElementsOut.Clear();
                var pattern = @"(?=\(\d{4}\.\d{2}\)\s*[A-Z]{1}\d{2}[A-Z]{1}(\s*|\t*)\d{6})";
                var repText = Regex.Replace(elemList, pattern, m => "ISePaRaToR" + m.Groups[1].Value);
                var splittedRecords = Regex.Split(repText.Replace("\r\n", "\n"), @"ISePaRaToR").Select(x => x.Trim()).Where(x => x != "" && x != "  " && x != "\n" && x != " ");
                foreach (var record in splittedRecords)
                {
                    var splPattern = @"(?<=.*\[\d{2}\])"; /*works*/
                    var nSpl = Regex.Split(record, splPattern).Where(x => !x.Contains(@"__________")).Select(x => x.Trim().Trim('\t'));

                    foreach (var splittedRec in nSpl)
                    {
                        var splittedRecClear = splittedRec.Replace("\n", " ").Trim().Trim('\t');
                        var pos = splittedRec.IndexOf(Regex.Match(splittedRec, @"\d{2}\/\d{2}\/\d{4}").Value) + 10;
                        var tmpSplittedRec = splittedRec.Insert(pos, "\t");
                        /*Search and extract 51, 21, 22 fields with deleting from original string*/
                        var groupPatternR = new Regex(@"\((?<classInfoDate>\d{4}\.\d{2})\)\s*(?<classInfoType>[A-Z]{1}\d{2}[A-Z]{1})\t(?<appNumber>\d{6})\n*(?<appDate>\d{2}\/\d{2}\/\d{4})\t");
                        var groupsMatch = groupPatternR.Match(tmpSplittedRec);
                        if (groupsMatch.Success)
                        {
                            currentElement = new ElementOut();
                            ElementsOut.Add(currentElement);
                            var priorityInfo = new List<string>();
                            var pctInfo = new List<string>();
                            var woInfo = new List<string>();
                            /**/
                            currentElement.I51Version = groupsMatch.Groups["classInfoDate"].Value;
                            currentElement.I51Number = groupsMatch.Groups["classInfoType"].Value;
                            currentElement.I21 = groupsMatch.Groups["appNumber"].Value;
                            currentElement.I22 = Methods.NormalizeDate(groupsMatch.Groups["appDate"].Value);
                            tmpSplittedRec = groupPatternR.Replace(tmpSplittedRec, "").Trim();

                            /*Search and extract 30x fields if found with deleting from original string*/
                            if (Regex.IsMatch(tmpSplittedRec, @"\t{3}\d{2}\/\d{2}\/\d{4}.*(\n|$)"))
                            {
                                var x = new Regex(@"\t{3}\d{2}\/\d{2}\/\d{4}.*(\n|$)");
                                var priorities = x.Matches(tmpSplittedRec);
                                foreach (Match prio in priorities) { priorityInfo.Add(prio.Value.Trim()); }
                                if (priorityInfo.Count > 0)
                                {
                                    var prioPattern = new Regex(@"(?<date>.*)\t(?<number>.*)\t(?<country>.*)");
                                    foreach (var prioLine in priorityInfo)
                                    {
                                        var prioMatch = prioPattern.Match(prioLine);
                                        if (prioMatch.Success)
                                        {
                                            currentElement.I31 = (currentElement.I31 ?? Enumerable.Empty<string>()).Concat(new string[] { prioMatch.Groups["number"].Value.Trim() }).ToArray();
                                            currentElement.I32 = (currentElement.I32 ?? Enumerable.Empty<string>()).Concat(new string[] { Methods.NormalizeDate(prioMatch.Groups["date"].Value.Trim()) }).ToArray();
                                            currentElement.I33 = (currentElement.I33 ?? Enumerable.Empty<string>()).Concat(new string[] { prioMatch.Groups["country"].Value.Trim() }).ToArray();
                                        }
                                        else
                                        {
                                            Console.WriteLine("priority identification error");
                                        }
                                    }
                                }
                                tmpSplittedRec = Regex.Replace(tmpSplittedRec, @"\t{3}\d{2}\/\d{2}\/\d{4}.*(\n|$)", "");
                            }
                            /*Search and extract 86 field if found with deleting from original string*/
                            if (Regex.IsMatch(tmpSplittedRec, "\n\tPCT.*\n"))
                            {
                                var x = new Regex(@"\n\tPCT.*\n");
                                var pctValues = x.Matches(tmpSplittedRec);
                                if (pctValues.Count == 1) currentElement.I86 = pctValues[0].Value.Trim();
                                //foreach (Match pct in pctValues) {pctInfo.Add(pct.Value.Trim()); }
                                tmpSplittedRec = Regex.Replace(tmpSplittedRec, @"\n\tPCT.*\n", "");
                            }
                            /*Search and extract 87 field if found with deleting from original string*/
                            if (Regex.IsMatch(tmpSplittedRec, "\tWO.*$"))
                            {
                                var x = new Regex(@"\tWO.*$");
                                var woValues = x.Matches(tmpSplittedRec);
                                if (woValues.Count == 1) currentElement.I87 = woValues[0].Value.Trim();
                                //foreach (Match pct in pctValues) { pctInfo.Add(pct.Value.Trim()); }
                                tmpSplittedRec = Regex.Replace(tmpSplittedRec, @"\tWO.*$", "");
                            }
                            if (tmpSplittedRec.Contains("\t"))
                            {
                                var elementsSplitted = tmpSplittedRec.Replace("\n", "\t").Trim().Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                if (elementsSplitted != null && elementsSplitted.Count() == 4)
                                {
                                    currentElement.I54Hebrew = elementsSplitted[0].Trim();
                                    currentElement.I54Eng = elementsSplitted[1].Trim();
                                    currentElement.I71NameIL = elementsSplitted[2].Trim();
                                    currentElement.I71NameENG = elementsSplitted[3].Trim();
                                }
                                if (elementsSplitted != null && elementsSplitted.Count() == 3)
                                {
                                    currentElement.I54Hebrew = elementsSplitted[0].Trim();
                                    currentElement.I54Eng = elementsSplitted[1].Trim();
                                    currentElement.I71NameENG = elementsSplitted[2].Trim();
                                }
                                if (elementsSplitted.Count() != 3 && elementsSplitted.Count() != 4)
                                {
                                    Console.WriteLine("Error in 54/71 identification!\t" + currentElement.I21);
                                }
                            }
                        }
                    }
                }
            }
            return ElementsOut;
        }
    }
}
