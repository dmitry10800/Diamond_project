using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Diamond_NZ
{
    class Process
    {
        /*APPLICATIONS WITHDRAWN, REFUSED, TAKEN TO BE ABANDONED*/
        public class FirstList
        {
            public static List<OutElements.Subcode2Elements> Run(List<XElement> v)
            {
                var elements = new List<OutElements.Subcode2Elements>();
                var pattern = new System.Text.RegularExpressions.Regex(@".*Patent Lapsed:\s(?<Number>\d+)\s.*");
                foreach (var item in v)
                {
                    var match = pattern.Match(item.Value);
                    if (match.Success)
                    {
                        elements.Add(new OutElements.Subcode2Elements { PatNumber = match.Groups["Number"].Value });
                    }
                    else
                    {
                        Console.WriteLine("Match isn't success!");
                    }
                }
                return elements;
            }
        }
    }
}
