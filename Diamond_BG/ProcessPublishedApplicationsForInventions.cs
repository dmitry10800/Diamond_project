using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_BG
{
    public class ProcessPublishedApplicationsForInventions
    {
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I51 = "(51) Int. Cl.";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I71 = "(71)";
        private static readonly string I72 = "(72)";
        private static readonly string I74 = "(74)";
        private static readonly string I86 = "(86)";
        private static readonly string I87 = "(87)";

        public class ElementOut
        {
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I51N { get; set; }
            public string[] I51D { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string[] I71N { get; set; }
            public string[] I71A { get; set; }
            public string[] I72 { get; set; }
            public string[] I74N { get; set; }
            public string[] I74A { get; set; }
            public string[] I74C { get; set; }
            public string I86N { get; set; }
            public string I86D { get; set; }
            public string I87N { get; set; }
            public string I87D { get; set; }
            public string INote { get; set; }
        }

        public List<ElementOut> OutputValue(List<XElement> elemList)
        {
            List<ElementOut> ElementsOut = new List<ElementOut>();
            ElementOut currentElement;
            if (elemList != null)
            {

                for (int i = 0; i < elemList.Count; ++i)
                {
                    var element = elemList[i];
                    string value = element.Value;
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
                        } while (tmpInc < elemList.Count()
                        //&& !elements[tmpInc].Value.StartsWith(I11)
                        && !Regex.IsMatch(elemList[tmpInc].Value, @"\(51\)\s")
                        && !Regex.IsMatch(elemList[tmpInc].Value, @"(\d{1}|\d{2})\s*претенц"));
                        if (Regex.IsMatch(elemList[tmpInc].Value, @"(\d{1}|\d{2})\s*претенц"))
                        {
                            if (elemList[tmpInc].Value.Contains("\n"))
                            {
                                currentElement.INote = elemList[tmpInc].Value.Remove(elemList[tmpInc].Value.IndexOf("\n"));
                            }
                            else currentElement.INote = elemList[tmpInc].Value;
                        }
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
                                currentElement.I21 = tmpRecValue;
                            }
                            /*22 */
                            if (inidCode.StartsWith(I22))
                            {
                                tmpRecValue = inidCode.Replace(I22, "").Replace("\n", "").Trim();
                                currentElement.I22 = Methods.DateNormalize(tmpRecValue);
                            }
                            /*51 international class*/
                            if (inidCode.StartsWith(I51))
                            {
                                string[] tmpSplittedValue = null;
                                string[] tmpIntClass = null;
                                string[] tmpVersion = null;
                                string datePattern = @"\(\d{4}\.\d{2}\)";
                                tmpRecValue = inidCode.Replace(I51, "").Trim('\n');
                                if (tmpRecValue.Contains("\n"))
                                {
                                    tmpSplittedValue = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplittedValue != null)
                                    {
                                        foreach (var record in tmpSplittedValue)
                                        {
                                            if (Regex.IsMatch(record, datePattern))
                                            {
                                                tmpVersion = (tmpVersion ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(record, datePattern).Value.Replace("(", "").Replace(")", "") }).ToArray();
                                                tmpIntClass = (tmpIntClass ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Replace(record, datePattern, "").Replace(" ", "").Insert(4, " ").Trim() }).ToArray();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Regex.IsMatch(tmpRecValue, datePattern))
                                    {
                                        tmpVersion = (tmpVersion ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Match(tmpRecValue, datePattern).Value.Replace("(", "").Replace(")", "") }).ToArray();
                                        tmpIntClass = (tmpIntClass ?? Enumerable.Empty<string>()).Concat(new string[] { Regex.Replace(tmpRecValue, datePattern, "").Replace(" ", "").Insert(4, " ").Trim() }).ToArray();
                                    }
                                }
                                if (tmpIntClass != null && tmpVersion != null)
                                {
                                    currentElement.I51N = tmpIntClass;
                                    currentElement.I51D = tmpVersion;
                                }
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Replace("-\n", "").Replace("\n", " ").Trim();
                                currentElement.I54 = tmpRecValue;
                            }
                            /*57 description*/
                            if (inidCode.StartsWith(I57))
                            {
                                tmpRecValue = inidCode.Replace(I57, "").Replace("-\n", " ").Replace("\n", " ").Trim();
                                currentElement.I57 = tmpRecValue;
                            }
                            /*71*/
                            if (inidCode.StartsWith(I71))
                            {
                                string[] tmpName = null;
                                string[] tmpAddr = null;
                                tmpRecValue = inidCode.Replace(I71, "").Replace("\n", " ").Trim();
                                if (tmpRecValue.Contains(";"))
                                {
                                    string[] tmpSplValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    foreach (var item in tmpSplValue)
                                    {
                                        if (item.Contains(","))
                                        {
                                            tmpName = (tmpName ?? Enumerable.Empty<string>()).Concat(new string[] { item.Remove(item.IndexOf(",")).Trim() }).ToArray();
                                            tmpAddr = (tmpAddr ?? Enumerable.Empty<string>()).Concat(new string[] { item.Substring(item.IndexOf(",")).Trim().Trim(',').Trim() }).ToArray();
                                        }
                                        else
                                        {
                                            tmpName = (tmpName ?? Enumerable.Empty<string>()).Concat(new string[] { item.Trim() }).ToArray();
                                            tmpAddr = (tmpAddr ?? Enumerable.Empty<string>()).Concat(new string[] { item.Trim() }).ToArray();
                                        }
                                    }
                                }
                                else
                                {
                                    if (tmpRecValue.Contains(","))
                                    {
                                        tmpName = (tmpName ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Remove(tmpRecValue.IndexOf(",")).Trim() }).ToArray();
                                        tmpAddr = (tmpAddr ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Substring(tmpRecValue.IndexOf(",")).Trim().Trim(',').Trim() }).ToArray();
                                    }
                                    else
                                    {
                                        tmpName = (tmpName ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Trim() }).ToArray();
                                        tmpAddr = (tmpAddr ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Trim() }).ToArray();
                                    }
                                }
                                if (tmpName != null && tmpAddr != null)
                                {
                                    currentElement.I71N = tmpName;
                                    currentElement.I71A = tmpAddr;
                                }
                            }
                            /*72 Inventor*/
                            if (inidCode.StartsWith(I72))
                            {
                                tmpRecValue = inidCode.Replace(I72, "").Replace("\n", " ").Replace(",", " ").Replace("  ", " ").Trim();
                                if (tmpRecValue.Contains(";"))
                                {
                                    string[] tmpSplValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue.Count() > 0)
                                    {
                                        foreach (var inventor in tmpSplValue)
                                        {
                                            currentElement.I72 = (currentElement.I72 ?? Enumerable.Empty<string>()).Concat(new string[] { inventor.Trim() }).ToArray();
                                        }
                                    }
                                }
                                else
                                {
                                    currentElement.I72 = (currentElement.I72 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray();
                                }
                            }
                            /*74*/
                            if (inidCode.StartsWith(I74))
                            {
                                string tmpAddr = null;
                                string[] splittedValue = null;
                                tmpRecValue = inidCode.Replace(I74, "").Replace("\n", " ");
                                if (tmpRecValue.Contains(";"))
                                {
                                    splittedValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    Array.Reverse(splittedValue);
                                    foreach (var item in splittedValue)
                                    {
                                        if (item.Contains(","))
                                        {
                                            currentElement.I74N = (currentElement.I74N ?? Enumerable.Empty<string>()).Concat(new string[] { item.Remove(item.IndexOf(",")).Trim() }).ToArray();
                                            currentElement.I74A = (currentElement.I74A ?? Enumerable.Empty<string>()).Concat(new string[] { item.Substring(item.IndexOf(",")).Trim().Trim(',').Trim() }).ToArray();
                                            currentElement.I74C = (currentElement.I74C ?? Enumerable.Empty<string>()).Concat(new string[] { "BG" }).ToArray();
                                            tmpAddr = item.Substring(item.IndexOf(",")).Trim().Trim(',').Trim();
                                        }
                                        else if (tmpAddr != null)
                                        {
                                            currentElement.I74N = (currentElement.I74N ?? Enumerable.Empty<string>()).Concat(new string[] { item.Trim() }).ToArray();
                                            currentElement.I74A = (currentElement.I74A ?? Enumerable.Empty<string>()).Concat(new string[] { tmpAddr.Trim() }).ToArray();
                                            currentElement.I74C = (currentElement.I74C ?? Enumerable.Empty<string>()).Concat(new string[] { "BG" }).ToArray();
                                        }
                                    }
                                }
                                else if (tmpRecValue.Contains(","))
                                {
                                    currentElement.I74N = (currentElement.I74N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Remove(tmpRecValue.IndexOf(",")).Trim() }).ToArray();
                                    currentElement.I74A = (currentElement.I74A ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Substring(tmpRecValue.IndexOf(",")).Trim().Trim(',').Trim() }).ToArray();
                                    currentElement.I74C = (currentElement.I74C ?? Enumerable.Empty<string>()).Concat(new string[] { "BG" }).ToArray();

                                }
                            }
                            /*86*/
                            if (inidCode.StartsWith(I86))
                            {
                                string tmpDateValue = null;
                                string datePattern = @"\d{2}\.\d{2}\.\d{4}";
                                string tmpNumber = null;
                                tmpRecValue = inidCode.Replace(I86, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, datePattern))
                                {
                                    tmpDateValue = Regex.Match(tmpRecValue, datePattern).Value;
                                    tmpNumber = Regex.Replace(tmpRecValue, datePattern, "").Trim().Trim('/');
                                    currentElement.I86N = tmpNumber != null ? tmpNumber : tmpRecValue.Replace(tmpDateValue, "").Trim().Trim(',').Trim();
                                    currentElement.I86D = Methods.DateNormalize(tmpDateValue);
                                }
                                else
                                {
                                    currentElement.I86N = tmpRecValue;
                                }

                            }
                            /*87*/
                            if (inidCode.StartsWith(I87))
                            {
                                string tmpNumber = null;
                                string datePattern = @"\d{2}\.\d{2}\.\d{4}";
                                tmpRecValue = inidCode.Replace(I87, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, datePattern))
                                {
                                    tmpNumber = Regex.Replace(tmpRecValue, datePattern, "").Trim().Trim('/');
                                    currentElement.I87D = Methods.DateNormalize(Regex.Match(tmpRecValue, datePattern).Value);
                                    currentElement.I87N = tmpNumber != null ? tmpNumber : Regex.Replace(tmpRecValue, datePattern, "").Replace(",", "").Trim();
                                }
                                else
                                {
                                    currentElement.I87N = tmpRecValue;
                                }
                            }
                        }
                    }
                }
            }
            return ElementsOut;
        }
    }
}
