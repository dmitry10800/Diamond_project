using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_SI
{
    public class Methods
    {
        internal static string[] RecordsSplit(string s)
        {
            string tempS = s;
            tempS = tempS.Replace("****", "").Trim();

            var splittedRecord = tempS.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            return splittedRecord;
        }

        internal static string DateNormalize(string s)
        {
            string dateNormalized = s;
            if (Regex.IsMatch(s, @"\d{2}\/*\-*\.*\d{2}\/*\-*\.*\d{4}"))
            {
                var date = Regex.Match(s, @"(?<day>\d{2})\/*\-*\.*(?<month>\d{2})\/*\-*\.*(?<year>\d{4})");
                dateNormalized = date.Groups["year"].Value + "-" + date.Groups["month"].Value + "-" + date.Groups["day"].Value;
            }
            return dateNormalized;
        }
    }
}
