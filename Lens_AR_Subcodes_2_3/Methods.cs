using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lens_AR_Subcodes_2_3
{
    public class Methods
    {
        internal static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString.Replace(@"\n", " ").Trim();
            string[] keysArray = new[] { @"(10)", @"(11)", @"(21)", @"(22)", @"(24)", @"(--)", @"(30)", @"(47)", @"(51)", @"(54)", @"(57)", @"(71)", @"(72)", @"(74)", @"(45)", "Siguen " };

            tempStrC = Regex.Replace(tempStrC, @"-+", "").Trim();

            if (!string.IsNullOrEmpty(tempStrC))
            {
                foreach (var item in keysArray)
                {
                    //tempStrC = Regex.Replace(tempStrC, item, $"***{item}");
                    tempStrC = tempStrC.Replace(item, $"***{item}");
                }
            }
            splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
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
            return dateNormalized.Trim();
        }

        internal static List<PriorityInformation> GetNormalizedPriorities(string strLine)
        {
            List<PriorityInformation> outList = new List<PriorityInformation>();
            PriorityInformation priorityInformation = null;

            var splittedPriorities = strLine.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            if (splittedPriorities.Length > 0)
            {
                foreach (var splittedPriority in splittedPriorities)
                {
                    var onePriority = Regex.Match(splittedPriority, @"(?<authority>[A-Z])\s*(?<number>.+)\s*(?<date>\d{2}\/\d{2}\/\d{4})");
                    if (onePriority.Success)
                    {
                        outList.Add(new PriorityInformation
                        {
                            PriorityNumber = onePriority.Groups["number"].Value,
                            PriorityDate = Methods.DateNormalize(onePriority.Groups["date"].Value),
                            PriorityCountryCode = onePriority.Groups["authority"].Value
                        });
                    }
                }
            }
            return outList;
        }

        internal static List<PersonInformation> GetNormolizedAgentInformation(string strLine)
        {
            List<PersonInformation> outList = new List<PersonInformation>();
            PersonInformation agent = null;

            var splittedAgents = strLine.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            if (splittedAgents.Length > 0)
            {
                foreach (var oneAgent in splittedAgents)
                {
                    if (!string.IsNullOrEmpty(oneAgent))
                    {
                        outList.Add(new PersonInformation
                        {
                            Name = oneAgent.Trim()
                        });
                    }
                }
            }
            return outList;
        }

        internal static List<PersonInformation> GetNormalizedInventorsInformation(string strLine)
        {
            List<PersonInformation> outList = new List<PersonInformation>();
            PersonInformation inventor = null;

            var splittedInventors = strLine.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            if (splittedInventors.Length > 0)
            {
                foreach (var oneInventor in splittedInventors)
                {
                    if (!string.IsNullOrEmpty(oneInventor))
                    {
                        outList.Add(new PersonInformation
                        {
                            Name = oneInventor.Trim()
                        });
                    }
                }
            }

            return outList;
        }

        internal static (List<Classification_Field51>, List<string>) NormalizedClassificationField(string classificationStr, string patentNumber)
        {
            List<Classification_Field51> outList = new List<Classification_Field51>();
            List<string> errorsList = new List<string>();
            string ipcVersion;
            string LNNL;
            string tempLNNL;
            string NNN_NNN;
            string fullClassification;


            var splittedClassifications = classificationStr.Split(',', ';');
            if (splittedClassifications.Length > 0)
            {
                var tempFirstClassification = splittedClassifications.First();
                string tempLastClassification = null;
                ipcVersion = Regex.Match(tempFirstClassification, @"\(?.*\d{4}.{1}\s*\d{2}\)?", RegexOptions.IgnoreCase).Value;
                if (string.IsNullOrEmpty(ipcVersion))
                {
                    tempLastClassification = splittedClassifications.Last();
                    ipcVersion = Regex.Match(tempLastClassification, @"\(?.*\d{4}.{1}\s*\d{2}\)?", RegexOptions.IgnoreCase).Value;
                }

                if (tempFirstClassification.Contains("["))
                    errorsList.Add(patentNumber);

                tempLNNL = Regex.Match(tempFirstClassification, @"[A-Z]{1}\d{2}[A-Z]{1}", RegexOptions.IgnoreCase).Value;
                ipcVersion = NormalizeIpcVersion(ipcVersion);

                foreach (var splittedClassification in splittedClassifications)
                {
                    var tempIpcVersion = Regex.Match(splittedClassification, @"\d{4}\s*.{0,1}\s*\d{2}", RegexOptions.IgnoreCase).Value;
                    if (string.IsNullOrEmpty(tempIpcVersion))
                        tempIpcVersion = ipcVersion;

                    LNNL = Regex.Match(splittedClassification, @"[A-Z]{1}\d{2}[A-Z]{1}", RegexOptions.IgnoreCase).Value;
                    if (string.IsNullOrEmpty(LNNL))
                        LNNL = tempLNNL;
                    NNN_NNN = Regex.Match(splittedClassification, @"\d+\/\d+", RegexOptions.IgnoreCase).Value;
                    fullClassification = $"{LNNL} {NNN_NNN}";
                    outList.Add(new Classification_Field51
                    {
                        Classification = fullClassification,
                        IPC_Version = tempIpcVersion
                    });
                }
            }

            return (outList, errorsList);
        }

        private static string NormalizeIpcVersion(string line)
        {
            string ipcVersion = null;
            if (!string.IsNullOrEmpty(line))
            {
                ipcVersion = Regex.Match(line, @"\d{4}\s*.{0,1}\s*\d{2}", RegexOptions.IgnoreCase).Value;
            }
            return ipcVersion;
        }
    }
}
