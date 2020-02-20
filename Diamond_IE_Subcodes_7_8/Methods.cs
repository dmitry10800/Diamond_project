using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_IE_Subcodes_7_8
{
    public class Methods
    {
        internal static string[] RecSplit(string recString)
        {
            string[] splittedRecords = null;

            string tempStrC = recString;

            if (!string.IsNullOrEmpty(tempStrC))
            {
                Regex regexPatOne = new Regex(@"(\d{5,}\s*|S\d{5,}\s*)", RegexOptions.IgnoreCase);
                MatchCollection matches = regexPatOne.Matches(tempStrC);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        tempStrC = tempStrC.Replace(match.Value, "***" + match.Value);
                    }
                }
                splittedRecords = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            }
            return splittedRecords;
        }

        internal static string GetDateFromNameGazette(string name)
        {
            var nameG = Regex.Match(name, @"(?<CountryCode>[A-Z]{2})_(?<Date>\d{8})_(?<Number>\d{4})\.pdf");
            return DateNormalize(nameG.Groups["Date"].Value);
        }

        internal static string DateNormalize(string s)
        {
            string dateNormalized = s;
            if (Regex.IsMatch(s, @"\d{2}\/*\-*\.*\d{2}\/*\-*\.*\d{4}"))
            {
                var date = Regex.Match(s, @"(?<year>\d{4})\/*\-*\.*(?<month>\d{2})\/*\-*\.*(?<day>\d{2})");
                dateNormalized = date.Groups["year"].Value + "-" + date.Groups["month"].Value + "-" + date.Groups["day"].Value;
            }
            return dateNormalized.Trim();
        }

        internal static List<IPC_Classification> IPCClassificationsNormalize(string ipcVersion, string ipcClassification)
        {
            List<IPC_Classification> outList = new List<IPC_Classification>();
            IPC_Classification _ipcClassification;
            var splittedIPCClass = ipcClassification.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            for (int i = 0; i < splittedIPCClass.Length; i++)
            {
                _ipcClassification = new IPC_Classification();
                _ipcClassification.Classification = splittedIPCClass[i].Trim();
                _ipcClassification.IPC_Version = ipcVersion;
                outList.Add(_ipcClassification);
            }
            return outList;
        }

        internal static List<string> AgentNameNormalize(string s)
        {
            List<string> agentNamesList = new List<string>();

            string[] splittedNames = s.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            for (int i = 0; i < splittedNames.Length; i++)
            {
                agentNamesList.Add(splittedNames[i]);
            }
            return agentNamesList;
        }
    }
}
