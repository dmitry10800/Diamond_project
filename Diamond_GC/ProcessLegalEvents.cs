using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_GC
{
    public class ProcessLegalEvents
    {
        public class ElementOut
        {
            public string I21 { get; set; } // Application No
            public string I22 { get; set; } // Filling date
            public string EventDate { get; set; } // Decision date
            public string Note { get; set; } // Desigion No
            public string Title { get; set; } // Title (only for 3rd version of 14th subcode


        }

        public List<ElementOut> OutputValue(List<XElement> elemList)
        {
            var ElementsOut = new List<ElementOut>();
            if (elemList != null)
            {
                for (var i = 0; i < elemList.Count; i++)
                {
                    var value = elemList[i].Value;
                    var pattern = @"(?<Counter>\d+)\s(?<AppNumber>\d+)\s(?<FillingDate>\d{2}\/\d{2}\/\d{4})\s(?<DecisionNo>\d+\/*\d+)\s(?<EventDate>\d{2}\/\d{2}\/\d{4})";
                    //var patArab = @"(?<EventDate>\d{2}\/\d{2}\/\d{4})\s(?<DecisionNo>\d+\/*\d+)\s(?<FillingDate>\d{2}\/\d{2}\/\d{4})\s(?<AppNumber>\d+)\s(?<Counter>\d+)";
                    var patOldVersion = @"(?<Counter>\d+)\s(?<AppNumber>\d+)\s(?<Title>.*)\s(?<AppDate>\d{4}\/\d+\/\d+)\s(?<DecisionNo>.*)\s(?<DecisionDate>\d+\/\d+\/\d+)";
                    if (Regex.IsMatch(value, pattern))
                    {
                        if (value.Contains("\n"))
                        {
                            var s = value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in s)
                            {
                                if (Regex.IsMatch(item, pattern))
                                {
                                    var recordLine = Regex.Match(item, pattern);
                                    ElementsOut.Add(new ElementOut
                                    {
                                        EventDate = Methods.DateProcess(recordLine.Groups["EventDate"].Value),
                                        I21 = recordLine.Groups["AppNumber"].Value,
                                        I22 = Methods.DateProcess(recordLine.Groups["FillingDate"].Value),
                                        Note = recordLine.Groups["DecisionNo"].Value
                                    });
                                }
                            }
                        }
                        else
                        {
                            var k = Regex.Match(value, pattern);
                            ElementsOut.Add(new ElementOut
                            {
                                EventDate = Methods.DateProcess(k.Groups["EventDate"].Value),
                                I21 = k.Groups["AppNumber"].Value,
                                I22 = Methods.DateProcess(k.Groups["FillingDate"].Value),
                                Note = k.Groups["DecisionNo"].Value
                            });
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Pattern doesn't match for\t" + value);
                    }

                    if (Regex.IsMatch(value, patOldVersion))
                    {
                        if (value.Contains("\n"))
                        {
                            var s = value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in s)
                            {
                                var k = Regex.Match(item, patOldVersion);
                                ElementsOut.Add(new ElementOut
                                {
                                    EventDate = "2007-03-31",
                                    I21 = k.Groups["AppNumber"].Value,
                                    I22 = Methods.DateProcess(k.Groups["AppDate"].Value.Replace("/", "-")),
                                    Title = k.Groups["Title"].Value,
                                    Note = "|| Decision No | " + k.Groups["DecisionNo"].Value + "\n" + "|| Decision Date | " + k.Groups["DecisionDate"].Value
                                });
                            }
                        }
                        else
                        {
                            var k = Regex.Match(value, patOldVersion);
                            ElementsOut.Add(new ElementOut
                            {
                                EventDate = "2007-03-31",
                                I21 = k.Groups["AppNumber"].Value,
                                I22 = Methods.DateProcess(k.Groups["AppDate"].Value.Replace("/", "-")),
                                Title = k.Groups["Title"].Value,
                                Note = "|| Decision No | " + k.Groups["DecisionNo"].Value + "\n" + "|| Decision Date | " + k.Groups["DecisionDate"].Value
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine("Pattern doesn't match for\t" + value);
                    }
                }
            }
            return ElementsOut;
        }

        public class Sub06v2ElementOut
        {
            public string I11 { get; set; } // PubNumber
            public string I54 { get; set; } // Title
            public string DecisionNo { get; set; } // LE Note
            public string DecisionDate { get; set; } // EventDate
        }
        public List<Sub06v2ElementOut> Sub06OutputValue(List<XElement> elemList)
        {
            var ElementsOut = new List<Sub06v2ElementOut>();
            if (elemList != null)
            {
                for (var i = 0; i < elemList.Count; i++)
                {
                    var value = elemList[i].Value.Replace("\n", " ");
                    if (!value.Contains("Falling of a Patent for the"))
                        continue;
                    var pattern = new Regex(@"\d+\s(?<PubNumber>[A-Z]{2}\d+)\s(?<Title>.*)\s(?<DecisionNo>\d+\/\d+)\s(?<DecisionDate>\d{4}\/\d+\/\d+)\s(?<KeyPhrase>.*)$");
                    var match = pattern.Match(value);
                    if (match.Success)
                    {
                        ElementsOut.Add(new Sub06v2ElementOut
                        {
                            I11 = match.Groups["PubNumber"].Value,
                            DecisionDate = match.Groups["DecisionDate"].Value.Replace("/", "-"),
                            DecisionNo = match.Groups["DecisionNo"].Value,
                            I54 = match.Groups["Title"].Value
                        });
                    }
                    else
                    {
                        Console.WriteLine("Pattern doesn't match for\t" + value);
                    }
                }
            }
            return ElementsOut;
        }
    }
}
