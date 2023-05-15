using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_BY
{
    public class ProcessFirstList
    {
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I23 = "(23)";
        private static readonly string I31 = "(31)";
        private static readonly string I32 = "(32)";
        private static readonly string I33 = "(33)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I71 = "(71)";
        private static readonly string I72 = "(72)";

        public class ElementOut
        {

            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I23 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
            public string[] I51N { get; set; }
            public string[] I51D { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string[] I71N { get; set; }
            public string[] I71C { get; set; }
            public string[] I72N { get; set; }
            public string[] I72C { get; set; }
        }

        public List<ElementOut> OutputValue(List<XElement> elemList)
        {
            var ElementsOut = new List<ElementOut>();
            ElementOut currentElement;
            if (elemList != null)
            {

                for (var i = 0; i < elemList.Count; ++i)
                {
                    var element = elemList[i];
                    var value = element.Value;
                    string tmpRecordValue = null;
                    string[] splittedRecord = null;
                    int tmpInc;
                    string tmpRecValue;
                    if (value.StartsWith(I51) && Regex.IsMatch(value, @"\(51\)\s"))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elemList[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count() && !Regex.IsMatch(elemList[tmpInc].Value, @"\(51\)\s"));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*Resolution value*/
                            if (inidCode.StartsWith(I21))
                            {
                                tmpRecValue = inidCode.Replace(I21, "").Replace("\n", "").Trim();
                                currentElement.I21 = tmpRecValue.Remove(tmpRecValue.LastIndexOf(" ")).Trim();
                            }
                            /*51 international class*/
                            if (inidCode.StartsWith(I51))
                            {
                                string[] tmpSplittedValue = null;
                                string[] tmpIntClass = null;
                                string[] tmpVersion = null;
                                var datePattern = new Regex(@"\(\d{4}\.\d{2}\)");

                                tmpRecValue = inidCode.Replace(I51, "").Trim('\n');
                                //MatchCollection dateMatches = datePattern.Matches
                            }
                            /*22*/
                            if (inidCode.StartsWith(I22))
                            {
                                tmpRecValue = inidCode.Replace(I22, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, @"\d{4}\.\d{2}\.\d{2}"))
                                {
                                    currentElement.I22 = Methods.DateNormalize(Regex.Match(tmpRecValue, @"\d{4}\.\d{2}\.\d{2}").Value.Trim());
                                }
                            }
                            /*23*/
                            if (inidCode.StartsWith(I23))
                            {
                                tmpRecValue = inidCode.Replace(I23, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, @"\d{4}\.\d{2}\.\d{2}"))
                                {
                                    currentElement.I23 = Methods.DateNormalize(Regex.Match(tmpRecValue, @"\d{4}\.\d{2}\.\d{2}").Value.Trim());
                                }
                            }
                            /*31,32,33*/
                            if (inidCode.StartsWith(I31))
                            {
                                string[] tmpCC = null;
                                string[] tmpDate = null;
                                string[] tmpNumber = null;
                                string[] tmpSplittedValue = null;
                                var recordPattern = @".*\s*\d{2}\.\d{2}\.\d{4}\s*[A-Z]{2}$";
                                tmpRecValue = inidCode.Replace(I31, "").Replace(I32, "").Replace(I33, "").Trim();
                                if (tmpRecValue.Contains("\n"))
                                {
                                    tmpSplittedValue = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    foreach (var record in tmpSplittedValue)
                                    {
                                        if (Regex.IsMatch(record, recordPattern))
                                        {
                                            tmpCC = (tmpCC ?? Enumerable.Empty<string>()).Concat(new string[]
                                            {
                                                Regex.Match(record, @"[A-Z]{2}$").Value
                                            }).ToArray();
                                            tmpDate = (tmpDate ?? Enumerable.Empty<string>()).Concat(new string[]
                                            {
                                                Methods.DateNormalize(Regex.Match(record, @"\d{2}\.\d{2}\.\d{4}").Value)
                                            }).ToArray();
                                            tmpNumber = (tmpNumber ?? Enumerable.Empty<string>()).Concat(new string[]
                                            {
                                                record.Remove(record.IndexOf(Regex.Match(record, @"\d{2}\.\d{2}\.\d{4}").Value))
                                            }).ToArray();
                                        }
                                    }
                                }
                                else
                                {
                                    if (Regex.IsMatch(tmpRecValue, recordPattern))
                                    {
                                        tmpCC = (tmpCC ?? Enumerable.Empty<string>()).Concat(new string[]
                                        {
                                            Regex.Match(tmpRecValue, @"[A-Z]{2}$").Value
                                        }).ToArray();
                                        tmpDate = (tmpDate ?? Enumerable.Empty<string>()).Concat(new string[]
                                        {
                                            Methods.DateNormalize(Regex.Match(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}").Value)
                                        }).ToArray();
                                        tmpNumber = (tmpNumber ?? Enumerable.Empty<string>()).Concat(new string[]
                                        {
                                            tmpRecValue.Remove(tmpRecValue.IndexOf(Regex.Match(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}").Value))
                                        }).ToArray();
                                    }
                                }
                                if (tmpCC != null && tmpDate != null && tmpNumber != null)
                                {
                                    currentElement.I31 = tmpNumber;
                                    currentElement.I32 = tmpDate;
                                    currentElement.I33 = tmpCC;
                                }
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Replace("-\n", " ").Replace("\n", " ").Trim();
                                currentElement.I54 = tmpRecValue;
                            }
                            /*72 Inventor*/
                            if (inidCode.StartsWith(I72))
                            {

                            }
                        }
                    }
                }
            }
            return ElementsOut;
        }
    }
}
