using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_US
{
    class Process
    {
        public class Sub1
        {
            public static void Run(List<XElement> v)
            {
                Console.WriteLine("Sub1 processing started");
                List<string> splValues = new List<string>();
                string eventDate = null;
                List<OutElements.Sub1> elements = new List<OutElements.Sub1>();
                /*Get Event Date start*/
                var a = v.Where(x => x.Value.StartsWith("PATENTS WHICH EXPIRED ON")).Select(x => x.Value).FirstOrDefault();
                if (a != null)
                {
                    eventDate = Methods.DateProcess(a.Remove(a.IndexOf("DUE TO")).Replace("PATENTS WHICH EXPIRED ON", "")).Trim();
                }
                else
                    Console.WriteLine("Legal Event Date not found!");
                /*Get Event Date end*/
                /*Clear elements - exclude all that doesn't contain date*/
                v = v.Where(x => Regex.IsMatch(x.Value, @"\d+\/\d+\/\d+")).ToList();
                /*Clear end*/
                /*Concat all values to string and spliting to list of single values*/
                splValues = string.Join("\n", v.Select(x => x.Value)).Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                /*Concat end*/
                foreach (var record in splValues)
                {
                    Regex pattern = new Regex(@"(?<pubNumber>[\d,]+)\s(?<appNumber>[\d,\/]+)\s(?<date>\d+\/\d+\/\d+)");
                    var b = pattern.Match(record);
                    if (b.Success)
                    {
                        elements.Add(new OutElements.Sub1
                        {
                            AppNumber = b.Groups["appNumber"].Value,
                            PubNumber = b.Groups["pubNumber"].Value,
                            IssueDate = Methods.DateNormalize(b.Groups["date"].Value),
                            EventDate = eventDate
                        });
                    }
                    else
                        Console.WriteLine($"Record pattern doen't match!\t{record}");
                }
                if (elements.Count > 0)
                {
                    Methods.SendToDiamond(ConvertToDiamond.Sub1(elements));
                    Console.WriteLine("Sub1 processing finished");
                }
                else
                    Console.WriteLine("Something went wrong...");
            }
        }
    }
}
