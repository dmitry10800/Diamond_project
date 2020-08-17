using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_SM
{
    public class ProcessSecondtList
    {
        public static string GetDate(string tmpFileName)
        {
            FileInfo fName = new FileInfo(tmpFileName);
            string name = fName.Name;
            string datePatternBig = @"^[A-Z]{2}_\d{8}_";
            string datePatternSmall = @"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})";
            if (Regex.IsMatch(name, datePatternBig))
                return
                    Regex.Match(name, datePatternSmall).Groups["year"].Value + "-" +
                    Regex.Match(name, datePatternSmall).Groups["month"].Value + "-" +
                    Regex.Match(name, datePatternSmall).Groups["day"].Value;
            else return null;
        }
        public static string date45 = GetDate(Diamond_SM_main.CurrentFileName);

        public class ElementOut
        {
            public List<string> RenewalsNumbers { get; set; }
            public string GazetteDate = date45;
        }
        public ElementOut OutputValue(List<XElement> elemList)
        {
            ElementOut ElementsOut = new ElementOut();
            Regex pattern = new Regex(@"[A-Z]{2}\s*\-\s*[A-Z]{1}\s*\-\s*(\d\s*)+");
            List<string> numList = new List<string>();
            if (elemList != null)
            {
                foreach (var el in elemList)
                {
                    if (pattern.Match(el.Value).Success)
                    {
                        MatchCollection k = pattern.Matches(el.Value);
                        foreach (Match item in k)
                        {
                            numList.Add(item.Value.Replace(" ", "").Trim());
                        }
                    }
                }
            }
            if (numList != null && numList.Count > 0)
            {
                ElementsOut.RenewalsNumbers = numList;
            }
            return ElementsOut;
        }
    }
}
