using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_MX
{
    class Process
    {
        public class SubSecondThird
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Second and third sub processing started");
                List<OutElements.SubSecondThird> elements = new List<OutElements.SubSecondThird>();
                var tmpString = string.Join("\n", v.Select(x => x.Value)).Replace("\n", " ").Trim();
                var patForSplit = new Regex(@"(?=[A-Z]{2}\/[a-z]{1}\/.*)");
                var patForMatch = new Regex(@"(?<appNumber>[A-Z]{2}\/[a-z]{1}\/\d{4}\/\d+)\s(?<pubNumber>\b\d+\b)*\s*(?<name>.*)?$");
                var spl = patForSplit.Split(tmpString).Where(x => x != "").ToList();
                foreach (var rec in spl)
                {
                    if (rec.Contains(" y ") || rec.Contains(" Y "))
                    {

                    }
                    var m = patForMatch.Match(rec);
                    if (m.Success)
                    {
                        var tmpRecord = new OutElements.SubSecondThird();
                        tmpRecord.AppNumber = m.Groups["appNumber"].Value.Trim();
                        tmpRecord.LePatNumber = m.Groups["pubNumber"].Value.Trim();
                        tmpRecord.PubNumber = m.Groups["pubNumber"].Value.Trim();
                        tmpRecord.New73Holder = m.Groups["name"].Value.Trim().Split(new string[] { " y ", " Y " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        //tmpRecord.New73Holder = new List<string> { m.Groups["name"].Value.Trim() };
                        elements.Add(tmpRecord);
                    }
                    else
                    {

                    }
                }
                if (elements.Count > 0)
                {
                    Methods.SendToDiamond(ConvertToDiamond.Sub1(elements));
                    Console.WriteLine("Subs 2&3 processing finished");
                }
                else
                    Console.WriteLine("Something went wrong...");
            }
        }
    }
}
