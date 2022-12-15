using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Diamond_IL
{
    class ProcessNumbers
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
        public static string date45 = GetDate(Diamond_IL_main.CurrentFileName);
        public class ElementOut
        {
            public string I11 { get; set; }
            public string I45 = date45;
        }
        public List<ElementOut> OutputValue(string elemList)
        {
            elemList = elemList.Replace("\r\n", "\n\t").Replace("\n\t", "\t");
            List<ElementOut> ElementsOut = new List<ElementOut>();
            ElementOut currentElement = null;
            if (elemList != null)
            {
                ElementsOut.Clear();
                if (Regex.IsMatch(elemList, @"[\b,\n,^,\t]*\d{6}[\b,\n,$,\t]*"))
                {
                    Regex pattern = new Regex(@"[\b,\n,^,\t]*\d{6}[\b,\n,$,\t]*");
                    MatchCollection numbers = pattern.Matches(elemList);
                    if (numbers != null && numbers.Count > 0)
                    {
                        foreach (Match item in numbers)
                        {
                            currentElement = new ElementOut();
                            ElementsOut.Add(currentElement);
                            currentElement.I11 = item.Value.Trim();
                        }
                    }
                }
            }
            return ElementsOut;
        }
    }
}
