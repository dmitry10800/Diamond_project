using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_PH
{
    class Methods
    {
        public static string DateNormalize(string s)
        {
            var date = Regex.Match(s, @"(?<day>\d{2})\/*\-*\.*(?<month>\d{2})\/*\-*\.*(?<year>\d{4})");
            var dateNormalized = "";
            if (date.Success)
            {
                dateNormalized = $"{date.Groups["year"].Value}-{date.Groups["month"].Value}-{date.Groups["day"].Value}";
            }

            return dateNormalized;
        }
    }
}
