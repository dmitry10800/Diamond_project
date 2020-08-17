using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PL
{
    class Methods
    {
        public static List<FileInfo> GetTetmlFiles(string path)
        {
            return Directory.GetFiles(path, @"*.tetml", SearchOption.TopDirectoryOnly).Select(x => new FileInfo(x)).ToList();
        }
        public static List<string> SplitStringByRegex(List<XElement> xElements, Regex regex)
        {
            var mergedElements = String.Join(" ", xElements.Select(x => x.Value).ToList()).Replace("\n", " ");

            return regex.Split(mergedElements).Where(x => x.Trim().Contains(Subcodes.I11)).Select(x => x.Trim()).ToList();
        }
        public static List<string> SplitRecordsByInids(string record)
        {
            var tmpList = new List<string>();
            Regex regex = new Regex(@"\(\d{2}\)", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(record);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                    record = record.Replace(match.Value, "***" + match.Value);
                tmpList = record.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else
            {
                Console.WriteLine($"Record splitting failed. {record}");
            }
            return tmpList;
        }
    }
}
