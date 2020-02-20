using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diamond_AL_granted_patents
{
    class Diamond_AL_granted_patents
    {
        private static readonly string I11 = "( 11 )";
        private static readonly string I11A = "( 11)";
        private static readonly string I18 = "( 18 )";
        private static readonly string I18A = "( 18)";
        private static readonly string I21 = "( 21 )";
        private static readonly string I21A = "( 21)";
        private static readonly string I22 = "( 22 )";
        private static readonly string I22A = "( 22)";
        private static readonly string I30 = "( 30 )";
        private static readonly string I30A = "( 30)";
        private static readonly string I54 = "( 54 )";
        private static readonly string I54A = "( 54)";
        private static readonly string I57 = "( 57 )";
        private static readonly string I57A = "( 57)";
        private static readonly string I71 = "( 71 )";
        private static readonly string I71A = "( 71)";
        private static readonly string I72 = "( 72 )";
        private static readonly string I72A = "( 72)";
        private static readonly string I73 = "( 73 )";
        private static readonly string I73A = "( 73)";
        private static readonly string I74 = "( 74 )";
        private static readonly string I74A = "( 74)";
        private static readonly string I96 = "( 96 )";
        private static readonly string I96A = "( 96)";
        private static readonly string I97 = "( 97 )";
        private static readonly string I97A = "( 97)";

        class ElementOut
        {
            public string I11 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I30N { get; set; }
            public string[] I30D { get; set; }
            public string[] I30C { get; set; }
            public string I45 { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string I71N { get; set; }
            public string I71A { get; set; }
            public string I71C { get; set; }
            //public string I71MN { get; set; }
            //public string I71MA { get; set; }
            //public string I71MC { get; set; }
            //public string I73N { get; set; }
            //public string I73A { get; set; }
            public string[] I72N { get; set; }
            public string[] I72A { get; set; }
            public string I74N { get; set; }
            public string I74A { get; set; }
            public string I74C { get; set; }
            public string I96N { get; set; }
            public string I96D { get; set; }
            public string I97N { get; set; }
            public string I97D { get; set; }
        }

        /*72 split*/
        static (string[] ownerName, string[] ownerAdr) OwnerSplit(string tmpOwnerValue)
        {
            string[] tmpMultiOwn = null;
            string[] ownerName = null;
            string[] ownerAdr = null;
            string tmpOwnName = null;
            string tmpOwnAdr = null;
            /*If MultiOwner*/
            /*Split by ";" char*/
            if (tmpOwnerValue.Contains(";"))
            {
                tmpMultiOwn = tmpOwnerValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();

                if (tmpMultiOwn != null && tmpMultiOwn.Count() > 0)
                {
                    foreach (var ownerRec in tmpMultiOwn)
                    {
                        if (ownerRec.Contains("("))
                        {
                            tmpOwnName = ownerRec.Remove(ownerRec.IndexOf("(")).Trim();
                            tmpOwnAdr = ownerRec.Replace(tmpOwnName, "").Replace("(", "").Replace(")", "").Trim();
                        }
                        if (tmpOwnName != null && tmpOwnAdr != null)
                        {
                            ownerName = (ownerName ?? Enumerable.Empty<string>()).Concat(new[] { tmpOwnName }).ToArray();
                            ownerAdr = (ownerAdr ?? Enumerable.Empty<string>()).Concat(new[] { tmpOwnAdr }).ToArray();
                        }
                    }
                }
            }
            else if (tmpOwnerValue.Contains("("))
            {
                tmpOwnName = tmpOwnerValue.Remove(tmpOwnerValue.IndexOf("(")).Trim();
                tmpOwnAdr = tmpOwnerValue.Replace(tmpOwnName, "").Replace("(", "").Replace(")", "").Trim();
                if (tmpOwnName != null && tmpOwnAdr != null)
                {
                    ownerName = (ownerName ?? Enumerable.Empty<string>()).Concat(new[] { tmpOwnName }).ToArray();
                    ownerAdr = (ownerAdr ?? Enumerable.Empty<string>()).Concat(new[] { tmpOwnAdr }).ToArray();
                }
            }
            return (ownerName, ownerAdr);
        }

        /*Priority split*/
        static (string[] prioNumber, string[] prioCountry, string[] prioDate) PrioritySplit(string tmpPriorityValue)
        {
            string[] tmpMultiPrio = null;
            string[] prioNumber = null;
            string[] prioCountry = null;
            string[] prioDate = null;
            string datePattern = @"\s\d{2}\/\d{2}\/\d{4}\s";
            string tmpDateValue = null;
            string[] tmpPrioRecordValue = null;
            tmpPriorityValue = tmpPriorityValue.Replace(" and ", ";").Replace(" AND ", ";");
            /*If MultiPriority*/
            /*Split by ";" char*/
            if (tmpPriorityValue.Contains(";"))
            {
                tmpMultiPrio = tmpPriorityValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                if (tmpMultiPrio != null && tmpMultiPrio.Count() > 0)
                {
                    foreach (var prioRecord in tmpMultiPrio)
                    {
                        tmpDateValue = Regex.Match(prioRecord.Trim(), datePattern).Value;
                        if (tmpDateValue != null)
                        {
                            tmpPrioRecordValue = prioRecord.Split(new string[] { tmpDateValue }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            if (tmpPrioRecordValue != null && tmpPrioRecordValue.Count() == 2)
                            {
                                prioNumber = (prioNumber ?? Enumerable.Empty<string>()).Concat(new[] { tmpPrioRecordValue[0].Trim() }).ToArray();
                                prioDate = (prioDate ?? Enumerable.Empty<string>()).Concat(new[] { DateNormalize(tmpDateValue.Trim()) }).ToArray();
                                prioCountry = (prioCountry ?? Enumerable.Empty<string>()).Concat(new[] { tmpPrioRecordValue[1].Trim() }).ToArray();
                            }
                        }
                    }
                }
            }
            else
            {
                tmpDateValue = Regex.Match(tmpPriorityValue.Trim(), datePattern).Value;
                if (tmpDateValue != null)
                {
                    tmpPrioRecordValue = tmpPriorityValue.Split(new string[] { tmpDateValue }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (tmpPrioRecordValue != null && tmpPrioRecordValue.Count() == 2)
                    {
                        prioNumber = (prioNumber ?? Enumerable.Empty<string>()).Concat(new[] { tmpPrioRecordValue[0].Trim() }).ToArray();
                        prioDate = (prioDate ?? Enumerable.Empty<string>()).Concat(new[] { DateNormalize(tmpDateValue.Trim()) }).ToArray();
                        prioCountry = (prioCountry ?? Enumerable.Empty<string>()).Concat(new[] { tmpPrioRecordValue[1].Trim() }).ToArray();
                    }
                }
            }
            return (prioNumber, prioCountry, prioDate);
        }

        static string[] RecSplit(string recString)
        {
            string[] splittedRecord = null;
            string tempStrC = recString/*.Replace("\n", " ")*/;
            if (recString != "")
            {
                //if (recString.Contains("\n"))
                //{
                //    recString = recString.Replace("\n", " ");
                //}
                Regex regexPatOne = new Regex(@"\(\s+\d{2}\s*\)", RegexOptions.IgnoreCase);
                MatchCollection matchesClass = regexPatOne.Matches(recString);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
            }
            return splittedRecord;
        }
        /*Date*/
        static string DateNormalize(string tmpDate)
        {
            string swapDate = tmpDate;
            string[] splitDate = null;
            if (tmpDate.Contains("/"))
            {
                splitDate = tmpDate.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitDate.Count() == 3)
                {
                    return swapDate = splitDate[2] + "-" + splitDate[1] + "-" + splitDate[0];
                }
            }
            return tmpDate;
        }
        static List<ElementOut> ElementsOut = new List<ElementOut>();
        static void Main(string[] args)
        {

        }
    }
}
