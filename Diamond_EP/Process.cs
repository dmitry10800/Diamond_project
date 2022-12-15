using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_EP
{
    class Process
    {
        public class Sub10
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub10 processing started");
                List<OutElements.Sub10> elements = new List<OutElements.Sub10>();
                List<string> splittedRecords = new List<string>();
                foreach (var item in v)
                {
                    string value = item.Value;
                    string[] tmpSplValue = null;
                    if (value.Contains("\n"))
                    {
                        tmpSplValue = value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var rec in tmpSplValue)
                        {
                            splittedRecords.Add(rec.Trim());
                        }
                    }
                    else
                    {
                        splittedRecords.Add(value.Trim());
                    }
                }
                if (splittedRecords != null && splittedRecords.Count > 0)
                {
                    foreach (var record in splittedRecords)
                    {
                        string pattern = @"[A-Z]{2}\/[A-Z]{1}\/\d{4}\/\d{6}\s+\d{2}\.\d{2}\.\d{4}\s+\d{2}\.\d{2}\.\d{4}";
                        if (Regex.IsMatch(record, pattern))
                        {
                            string tmpMatchedValue = Regex.Match(record, pattern).Value.Trim();
                            string[] splittedMatchedValue = tmpMatchedValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            if (splittedMatchedValue.Count() == 3)
                            {
                                elements.Add(new OutElements.Sub10
                                {
                                    AppNumber = splittedMatchedValue[0],
                                    DateFeePaid = Methods.DateNormalize(splittedMatchedValue[1]),
                                    ValidUntil = Methods.DateNormalize(splittedMatchedValue[2]),
                                    Anniversary = record.Replace(tmpMatchedValue, "").Trim()
                                });
                            }
                        }
                    }
                }
                if (elements.Count > 0)
                {
                    Methods.SendToDiamond(ConvertToDiamond.Sub10(elements));
                    Console.WriteLine("Sub10 processing finished");
                }
                else
                    Console.WriteLine("Something went wrong...");
            }
        }
        public class Sub7
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub7 processing started");
                List<OutElements.Sub7> elements = new List<OutElements.Sub7>();
                List<string> splittedRecords = new List<string>();
                foreach (var item in v)
                {
                    string value = item.Value;
                    string[] tmpSplValue = null;
                    if (value.Contains("\n"))
                    {
                        tmpSplValue = value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var rec in tmpSplValue)
                        {
                            splittedRecords.Add(rec.Trim());
                        }
                    }
                    else
                    {
                        splittedRecords.Add(value.Trim());
                    }
                }
                if (splittedRecords != null && splittedRecords.Count > 0)
                {
                    foreach (var record in splittedRecords)
                    {
                        var pattern = new Regex(@"(?<patNumber>[A-Z]{2}\s*\d+)\s*(?<appNumber>[A-Z]{2}\/[A-Z]\/\d+\/\d+)\s*(?<dateFeePaid>\d{2}\.\d{2}\.\d{4})\s*(?<validUntil>\d{2}\.\d{2}\.\d{4})\s*(?<anniversary>.*)$");
                        var a = pattern.Match(record);
                        if (a.Success)
                        {
                            {
                                elements.Add(new OutElements.Sub7
                                {
                                    PatNumber = a.Groups["patNumber"].Value.Trim(),
                                    AppNumber = a.Groups["appNumber"].Value.Trim(),
                                    DateFeePaid = Methods.DateNormalize(a.Groups["dateFeePaid"].Value.Trim()),
                                    ValidUntil = Methods.DateNormalize(a.Groups["validUntil"].Value.Trim()),
                                    Anniversary = a.Groups["anniversary"].Value.Trim()
                                });
                            }
                        }
                        else
                            Console.WriteLine("Pattern doesn't match!");
                    }
                }
                if (elements.Count > 0)
                {
                    Methods.SendToDiamond(ConvertToDiamond.Sub7(elements));
                    Console.WriteLine("Sub7 processing finished");
                }
                else
                    Console.WriteLine("Something went wrong...");
            }
        }
    }
}
