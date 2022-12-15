using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Diamond_RS
{
    class Methods
    {
        public static List<string> RecSplit(string s, string[] parameters)
        {
            foreach (var param in parameters)
            {
                if (!string.IsNullOrEmpty(param))
                    s = s.Replace(param, $"***{param}");
            }

            return s.Split("***", StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static string DateNormalize(string s)
        {
            string dateNormalized = s;
            var date = Regex.Match(s, @"(?<day>\d{2})[^0-9]*(?<month>\d{1,2})[^0-9]*(?<year>\d{4})");
            var day = date.Groups["day"].Value;
            if (date.Groups["day"].Length == 1)
                day = $"0{day}";
            dateNormalized = date.Groups["year"].Value + date.Groups["month"].Value + day;

            if (!string.IsNullOrEmpty(dateNormalized))
            {
                dateNormalized = $"{date.Groups["year"].Value}-{date.Groups["month"].Value}-{day}";
            }

            return dateNormalized;
        }
    }
}
