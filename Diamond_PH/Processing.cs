using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_PH
{
    public class Processing
    {
        public static List<Elements> SubCode7(List<string> elements)
        {
            List<Elements> elementsOut = new List<Elements>();

            foreach (var elem in elements)
            {
                var currentElement = new Elements();
                elementsOut.Add(currentElement);

                var title = elem.Trim();
                var strings = title.Split('\n');
                var matchesDate = Regex.Matches(elem, @"\d{2}\/\d{2}\/\d{4}");
                var matchNumber = Regex.Match(elem, @"\d{1}\/\d{4}\/\d+\.?");
                var index = elem.IndexOf(matchesDate[1].Value);
                var assignee = elem.Substring(index + matchesDate[1].Value.Length);
                assignee = assignee
                    .Replace("&nbsp;", "")
                    .Replace("_", "")
                    .Trim();
                title = title.Substring(0, elem.IndexOf(matchesDate[0].Value))
                    .Replace(strings[0], "")
                    .Replace(matchNumber.Value, "")
                    .Trim();

                var matchDots = Regex.Matches(assignee, @"\.{2,}", RegexOptions.Singleline);
                foreach (Match dot in matchDots)
                {
                    assignee = assignee.Replace(dot.Value, "").Trim();
                }

                var country = Regex.Match(assignee, @"((\[|\()[A-Z]{2}\]?|(\[|\()?[A-Z]{2}\])").Value;
                assignee = assignee.Replace(country, "").Trim();
                country = country.Replace("[", "").Replace("]", "");

                currentElement.AppNumber = matchNumber.Value.Replace(".", "");
                currentElement.AppDate = Methods.DateNormalize(matchesDate[0].Value);
                currentElement.EventDate = Methods.DateNormalize(matchesDate[1].Value);
                currentElement.Owner = new Owner { Name = assignee, Country = country };
                currentElement.Title = title;
            }

            return elementsOut;
        }
    }
}
