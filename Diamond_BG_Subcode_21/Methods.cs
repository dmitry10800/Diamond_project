using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Diamond_BG_Subcode_21
{
    class Methods
    {
        internal static string[] SplitRecords(string s)
        {
            string[] splittedRecords = null;
            string tempStr = s.Replace("\n", " ").Trim();
            if (!string.IsNullOrEmpty(tempStr))
            {
                Regex reg = new Regex(@"BG/EP \d{7}");
                MatchCollection matches = reg.Matches(tempStr);
                if(matches.Count == 0)
                {
                    reg = new Regex(@"EP \d{7}");
                    matches = reg.Matches(tempStr);
                }

                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        tempStr = tempStr.Replace(match.Value, "***" + match.Value);
                    }

                    splittedRecords = tempStr.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim()).ToArray();

                    return splittedRecords;
                }
            }
            return null;
        }

        internal static string GetLegalEventDate(string nameGazette)
        {
            var nameGazet = Regex.Match(nameGazette, @"(?<CountryCode>[A-Z]{2})_(?<Date>\d{8})_(?<Number>.*)\.pdf");
            return DateNormalize(nameGazet.Groups["Date"].Value);
        }

        internal static string DateNormalize(string s)
        {
            string dateNormalized = s;
            if (Regex.IsMatch(s, @"\d{4}\/*\-*\.*\d{2}\/*\-*\.*\d{2}"))
            {
                var date = Regex.Match(s, @"(?<year>\d{4})\/*\-*\.*(?<month>\d{2})\/*\-*\.*(?<day>\d{2})");
                dateNormalized = date.Groups["year"].Value + "-" + date.Groups["month"].Value + "-" + date.Groups["day"].Value;
            }
            return dateNormalized.Trim();
        }
    }
}
