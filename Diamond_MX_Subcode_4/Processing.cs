using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_MX_Subcode_4
{
    class Processing
    {
        public static List<Elements> SubCode4(List<XElement> elements)
        {
            var elementsOut = new List<Elements>();

            if (elements != null && elements.Count > 0)
            {
                foreach (var elem in elements)
                {
                    var currentElement = new Elements();
                    var appNumber = Regex.Match(elem.Value, @"[A-Z]{2}\/[a-z]{1}\/\d{4}\/\d{6}");
                    if (appNumber.Success)
                    {
                        var text = elem.Value.Replace(appNumber.Value, "").Trim();
                        var pubNumber = Regex.Match(text, @"\d+");
                        var owner = text.Replace(pubNumber.Value, "").Trim();
                        var eventDate = Regex.Match(MX_main.currentFileName, @"20\d{6}");
                        var year = eventDate.Value.Substring(0, 4);
                        var month = eventDate.Value.Substring(4, 2);
                        var day = eventDate.Value.Substring(eventDate.Value.Length - 2);

                        currentElement.AppNumber = appNumber.Value;
                        currentElement.PubNumber = pubNumber.Value;
                        currentElement.EventDate = $"{year}/{month}/{day}";
                        currentElement.Owner = new Owner { Name = owner };
                        elementsOut.Add(currentElement);
                    }
                }
            }

            return elementsOut;
        }
    }
}
