using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Diamond_LK_Subcode_2
{
    public class Processing
    {
        public static List<Elements> Applications(List<string> elements)
        {
            var elementsOut = new List<Elements>();

            for (var i = 0; i < elements.Count; i++)
            {
                try
                {
                    var currentElem = new Elements();

                    var matchNumber = Regex.Match(elements[i], @"\d{5} \d{2}\.\d{2}\.\d{4}");

                    var split = elements[i].Split('\n');
                    var splitDandN = matchNumber.Value.Split(' ');

                    currentElem.PubNumber = splitDandN[0].Trim();
                    currentElem.AppDate = splitDandN[1].Trim();
                    currentElem.AssigneeName = split[0].Replace(matchNumber.Value, "").Trim();
                    currentElem.Title = elements[i].Replace(split[0], "").Replace("\n", " ").Trim();

                    elementsOut.Add(currentElem);
                }
                catch (Exception e)
                {

                }
            }
            return elementsOut;
        }
    }
}
