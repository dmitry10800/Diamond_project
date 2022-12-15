using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CZ
{
    class Process
    {
        public class Sub6
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub6 processing started");
                List<OutElements.Sub6> elements = new List<OutElements.Sub6>();
                Regex recordPattern = new Regex(@"\(97\)\s*(?<number>[^\(]+)");
                foreach (var record in v)
                {
                    var match = recordPattern.Matches(record.Value.Replace("\n", " "));
                    foreach (Match item in match)
                    {
                        elements.Add(new OutElements.Sub6
                        {
                            PubNumber = item.Value.Replace("(97)", "").Trim()
                        });
                    }
                }

                if (elements.Count > 0)
                {
                    Methods.SendToDiamond(ConvertToDiamond.Sub6(elements));
                    Console.WriteLine("Sub6 processing finished");
                }
                else
                    Console.WriteLine("Something went wrong...");
            }
        }
    }
}
