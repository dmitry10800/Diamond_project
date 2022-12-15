using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_GC_OCR
{
    public class ProcessLegalEvents
    {
        public class ElementOut
        {
            public string I21 { get; set; } // Application No
            public string I22 { get; set; } // Filling date
            public string EventDate { get; set; } // Decision date
            public string Note { get; set; } // Desigion No
        }

        public List<ElementOut> OutputValue(List<XElement> elemList)
        {
            List<ElementOut> ElementsOut = new List<ElementOut>();
            if (elemList != null)
            {
                for (int i = 0; i < elemList.Count; i++)
                {
                    var value = elemList[i].Value;
                    var pattern = @"(?<Counter>\d+)\s(?<AppNumber>\d+)\s(?<FillingDate>\d{2}\/\d{2}\/\d{4})\s(?<DecisionNo>\d+\/*\d+)\s(?<EventDate>\d{2}\/\d{2}\/\d{4})";
                    var patternA = @"(?<Counter>\d+)\s(?<AppNumber>\d+)\s(?<FillingDate>\d{4}\/\d{2}\/\d{2})\s(?<DecisionNo>\d+\/*\d+)\s(?<EventDate>\d{2}\/\d{2}\/\d{4})";
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
                    else if (Regex.IsMatch(value, patternA))
                    {
                        if (value.Contains("\n"))
                        {
                            var s = value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in s)
                            {
                                if (Regex.IsMatch(item, patternA))
                                {
                                    var recordLine = Regex.Match(item, patternA);
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
                            var k = Regex.Match(value, patternA);
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
                }
            }
            return ElementsOut;
        }

        public class Sub06ElementOut
        {
            public string I11 { get; set; } // Application No
            public string EventDate { get; set; } // Decision date
            public string LeNumber { get; set; } // Desigion No
        }
        public List<Sub06ElementOut> Sub06OutputValue(List<XElement> elemList)
        {
            List<Sub06ElementOut> ElementsOut = new List<Sub06ElementOut>();
            if (elemList != null)
            {
                for (int i = 0; i < elemList.Count; i++)
                {
                    var value = elemList[i].Value;
                    var pattern = @"(?<Counter>\d+)\s(?<PatNumber>[A-Z]+\d+)\s(?<DecisionNo>\d+\/*\d+)\s(?<DecisionDate>\d{2}\/\d{2}\/\d{4})";
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
                                    ElementsOut.Add(new Sub06ElementOut
                                    {
                                        EventDate = Methods.DateProcess(recordLine.Groups["DecisionDate"].Value),
                                        I11 = recordLine.Groups["PatNumber"].Value,
                                        LeNumber = recordLine.Groups["DecisionNo"].Value
                                    });
                                }
                            }
                        }
                        else
                        {
                            var k = Regex.Match(value, pattern);
                            ElementsOut.Add(new Sub06ElementOut
                            {
                                EventDate = Methods.DateProcess(k.Groups["DecisionDate"].Value),
                                I11 = k.Groups["PatNumber"].Value,
                                LeNumber = k.Groups["DecisionNo"].Value
                            });
                        }
                        Console.WriteLine();
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
