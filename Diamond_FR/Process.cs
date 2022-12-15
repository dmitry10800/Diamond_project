using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_FR
{
    class Process
    {
        /*APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED*/
        public class FirstList
        {
            public static void Run(List<XElement> v)
            {
                List<OutElements.FirstList> elements = new List<OutElements.FirstList>();
                var pat = new Regex(@"(?<country>[A-Z]{2})\s*(?<appNum>\d+\b)\s*(?<noteNum>\d+)");
                var patA = new Regex(@"(?<country>[A-Z]{2})\s*(?<appNum>\d+[A-Z]\d+\b)\s*(?<noteNum>\d+)");
                foreach (var item in v)
                {
                    var match = pat.Match(item.Value);
                    var matchA = patA.Match(item.Value);
                    if (match.Success)
                    {
                        elements.Add(new OutElements.FirstList
                        {
                            AppNumber = match.Groups["appNum"].Value,
                            LeNoteCountry = match.Groups["country"].Value,
                            LeNoteNumber = match.Groups["noteNum"].Value,
                            LePatNumber = match.Groups["appNum"].Value
                        });
                    }
                    else if (matchA.Success)
                    {
                        elements.Add(new OutElements.FirstList
                        {
                            AppNumber = match.Groups["appNum"].Value,
                            LeNoteCountry = match.Groups["country"].Value,
                            LeNoteNumber = match.Groups["noteNum"].Value,
                            LePatNumber = match.Groups["appNum"].Value
                        });
                    }
                    else
                        Console.WriteLine($"Pattern doesn't match for record:\t{item.Value}");
                }
                Methods.SendToDiamond(ConvertToDiamond.SubCode1Convert(elements));
            }
        }
        /*Sub 7*/
        public class SecondList
        {
            public static void Run(List<XElement> v)
            {
                List<OutElements.SecondList> elements = new List<OutElements.SecondList>();
                var pattern = new Regex(@"(?<NatureOfApplication>[A-Z]{2})\s(?<AppNumber>\d+)\s(?<RegNumber>\d+)");
                foreach (var record in v)
                {
                    var match = pattern.Match(record.Value);
                    if (match.Success)
                    {
                        elements.Add(new OutElements.SecondList
                        {
                            AppNumber = match.Groups["AppNumber"].Value,
                            RegNumber = match.Groups["RegNumber"].Value,
                            NatureOfApplication = match.Groups["NatureOfApplication"].Value
                        });
                    }
                    else
                        Console.WriteLine("Record pattern doesn't match");
                }

                Methods.SendToDiamond(ConvertToDiamond.SubCode7Convert(elements));
            }
        }
        /*Sub 4*/
        public class ThirdList
        {
            public static void Run(List<XElement> v)
            {
                List<OutElements.SecondList> elements = new List<OutElements.SecondList>();
                var pattern = new Regex(@"(?<NatureOfApplication>[A-Z]{2})\s(?<AppNumber>\d+)\s(?<RegNumber>\d+)");
                foreach (var record in v)
                {
                    var match = pattern.Match(record.Value);
                    if (match.Success)
                    {
                        elements.Add(new OutElements.SecondList
                        {
                            AppNumber = match.Groups["AppNumber"].Value,
                            RegNumber = match.Groups["RegNumber"].Value,
                            NatureOfApplication = match.Groups["NatureOfApplication"].Value
                        });
                    }
                    else
                        Console.WriteLine("Record pattern doesn't match" + record);
                }

                Methods.SendToDiamond(ConvertToDiamond.SubCode4Convert(elements));
            }
        }
    }
}
