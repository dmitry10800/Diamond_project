using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_GB
{
    class Process
    {
        public class Sub8
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub8 processing started");
                var elements = new List<OutElements.Sub8>();
                var checkPattern = new Regex(@"[A-Z]{2}\d+");
                foreach (var record in v)
                {
                    if (checkPattern.Match(record.Value).Success)
                    {
                        elements.Add(new OutElements.Sub8
                        {
                            AppNumber = record.Value.Trim(),
                            LePatNumber = record.Value.Trim()
                        });
                    }
                    else
                        Console.WriteLine($"Pattern doesn't match\t{record.Value}");
                }
                Methods.SendToDiamond(ConvertToDiamond.Sub8(elements));
                Console.WriteLine("Sub8 processing finished");
            }
        }
        public class Sub9
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub9 processing started");
                var elements = new List<OutElements.Sub9>();
                var splElements = new List<string>();
                var checkPattern = new Regex(@"(?<pubNumber>[A-Z]{2}\d+)\s(?<appNumber>[A-Z]{2}\d+\.\d+)$");
                foreach (var elValue in v)
                {
                    splElements = splElements.Concat(elValue.Value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList()).ToList();
                }
                foreach (var rec in splElements)
                {
                    var m = checkPattern.Match(rec);
                    if (m.Success)
                    {
                        elements.Add(new OutElements.Sub9
                        {
                            PubNumber = m.Groups["pubNumber"].Value.Trim(),
                            AppNumber = m.Groups["appNumber"].Value.Trim(),
                            LePatNumber = m.Groups["pubNumber"].Value.Trim()
                        });
                    }
                    else
                        Console.WriteLine($"Pattern doesn't match\t{rec}");
                }
                Methods.SendToDiamond(ConvertToDiamond.Sub9(elements));
                Console.WriteLine("Sub9 processing finished");
            }
        }
        public class Sub10
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub10 processing started");
                var elements = new List<OutElements.Sub10>();
                var checkPattern = new Regex(@"(?<date>\d+\s\w+\s\d{4})\s(?<pubNumber>[A-Z]{2}\d+)\s(?<appNumber>[A-Z]{2}\d+\.*(\d+)*)$");
                foreach (var record in v)
                {
                    var m = checkPattern.Match(record.Value);
                    if (m.Success)
                    {
                        elements.Add(new OutElements.Sub10
                        {
                            PubNumber = m.Groups["pubNumber"].Value.Trim(),
                            LeDate = Methods.DateProcess(m.Groups["date"].Value.Trim()),
                            AppNumber = m.Groups["appNumber"].Value.Trim(),
                            LePatNumber = m.Groups["pubNumber"].Value.Trim()
                        });
                    }
                    else
                        Console.WriteLine($"Pattern doesn't match\t{record.Value}");
                }
                Methods.SendToDiamond(ConvertToDiamond.Sub10(elements));
                Console.WriteLine("Sub10 processing finished");
            }
        }
        public class Sub11
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub11 processing started");
                var elements = new List<OutElements.Sub11>();
                var datePattern = new Regex(@"\d+\s\w+\s\d{4}");
                var recPatter = new Regex(@"[A-Z]{2}\d+");
                string dateValue = null;
                foreach (var record in v)
                {
                    var m = datePattern.Match(record.Value);
                    if (m.Success)
                    {
                        dateValue = Methods.DateProcess(record.Value);
                    }
                    else if (recPatter.Match(record.Value).Success)
                    {
                        var splValues = record.Value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var rec in splValues)
                        {
                            elements.Add(new OutElements.Sub11
                            {
                                PubNumber = rec.Trim(),
                                LeDate = dateValue,
                                LePatNumber = rec.Trim()
                            });
                        }
                    }
                }
                Methods.SendToDiamond(ConvertToDiamond.Sub11(elements));
                Console.WriteLine("Sub11 processing finished");
            }
        }
        public class Sub12
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub12 processing started");
                var elements = new List<OutElements.Sub12>();
                var datePattern = new Regex(@"\d+\s\w+\s\d{4}");
                var recPatter = new Regex(@"[A-Z]{2}\d+");
                string dateValue = null;
                foreach (var record in v)
                {
                    var m = datePattern.Match(record.Value);
                    if (m.Success)
                    {
                        dateValue = Methods.DateProcess(record.Value);
                    }
                    else if (recPatter.Match(record.Value).Success)
                    {
                        var splValues = record.Value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var rec in splValues)
                        {
                            elements.Add(new OutElements.Sub12
                            {
                                PubNumber = rec.Trim(),
                                LeDate = dateValue,
                                LePatNumber = rec.Trim()
                            });
                        }
                    }
                }
                Methods.SendToDiamond(ConvertToDiamond.Sub12(elements));
                Console.WriteLine("Sub12 processing finished");
            }
        }
        public class Sub42
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub42 processing started");
                var elements = new List<OutElements.Sub42>();
                var datePattern = new Regex(@"\d+\s\w+\s\d{4}");
                var recPatter = new Regex(@"[A-Z]{2}\d+");
                string dateValue = null;
                var allText = string.Join(" ", v.Cast<XElement>().Select(x => x.Value).ToList()).Replace("European Patents Ceased - cont.", "");
                var splAllText = new Regex(@"(\d+\s\w+\s\d{4})").Split(allText).Select(x => x.Trim()).Where(x => x != "").ToList();
                foreach (var record in splAllText)
                {
                    var m = datePattern.Match(record);
                    if (m.Success)
                    {
                        dateValue = Methods.DateProcess(record);
                    }
                    else
                    if (recPatter.Match(record.Replace("European Patents Ceased - cont.", "").Trim()).Success)
                    {
                        var splValues = record.Replace("European Patents Ceased - cont.", "").Trim().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var rec in splValues)
                        {
                            elements.Add(new OutElements.Sub42
                            {
                                PubNumber = rec.Trim(),
                                LeDate = dateValue,
                                LePatNumber = rec.Trim()
                            });
                        }
                    }
                }
                Methods.SendToDiamond(ConvertToDiamond.Sub42(elements));
                Console.WriteLine("Sub42 processing finished");
            }
        }
        public class Sub43
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub43 processing started");
                var elements = new List<OutElements.Sub43>();
                var datePattern = new Regex(@"\d+\s\w+\s\d{4}");
                var recPatter = new Regex(@"[A-Z]{2}\d+");
                string dateValue = null;
                foreach (var record in v)
                {
                    var m = datePattern.Match(record.Value);
                    if (m.Success)
                    {
                        dateValue = Methods.DateProcess(record.Value);
                    }
                    else
                    if (recPatter.Match(record.Value).Success)
                    {
                        var splValues = record.Value.Replace("UK Patents Ceased - cont.", "").Trim().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var rec in splValues)
                        {
                            elements.Add(new OutElements.Sub43
                            {
                                PubNumber = rec.Trim(),
                                LeDate = dateValue,
                                LePatNumber = rec.Trim()
                            });
                        }
                    }
                }
                Methods.SendToDiamond(ConvertToDiamond.Sub43(elements));
                Console.WriteLine("Sub43 processing finished");
            }
        }
    }
}
