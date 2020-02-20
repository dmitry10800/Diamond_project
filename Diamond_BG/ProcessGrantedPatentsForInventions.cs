using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_BG
{
    public class ProcessGrantedPatentsForInventions
    {
        private static readonly string I11 = "(11)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I24 = "(24)";
        private static readonly string I31 = "(31)";
        private static readonly string I32 = "(32)";
        private static readonly string I33 = "(33)";
        private static readonly string I41 = "(41)";
        private static readonly string I51 = "(51) Int. Cl.";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I72 = "(72)";
        private static readonly string I73 = "(73)";
        private static readonly string I74 = "(74)";
        private static readonly string I86 = "(86)";
        private static readonly string I87 = "(87)";

        public class ElementOut
        {
            public string I11 { get; set; }
            public string I13 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I24 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
            public string I41 { get; set; }
            public string[] I51N { get; set; }
            public string[] I51D { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string[] I72 { get; set; }
            public string[] I73N { get; set; }
            public string[] I73A { get; set; }
            public string[] I73C { get; set; }
            public string[] I74N { get; set; }
            public string[] I74A { get; set; }
            public string[] I74C { get; set; }
            public string I86Number { get; set; }
            public string I86Date { get; set; }
            public string I87Number { get; set; }
            public string I87Date { get; set; }
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
                    if (value.StartsWith(I11) && Regex.IsMatch(value, @"\(11\)\s\d+"))
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
                        && !Regex.IsMatch(elemList[tmpInc].Value, @"\(11\)\s\d+")
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
                            splittedRecord = Methods.RecSplit(tmpRecordValue.Replace("(73 )", I73));
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*Resolution value*/
                            if (inidCode.StartsWith(I11))
                            {
                                tmpRecValue = inidCode.Replace(I11, "").Replace("\n", "").Trim();
                                if (tmpRecValue.Contains(" "))
                                {
                                    currentElement.I11 = tmpRecValue.Remove(tmpRecValue.LastIndexOf(" ")).Trim();
                                    currentElement.I13 = tmpRecValue.Substring(tmpRecValue.LastIndexOf(" ")).Trim();
                                }
                                else
                                {
                                    currentElement.I11 = tmpRecValue;
                                }
                            }
                            /*31,32,33*/
                            if (inidCode.StartsWith(I31))
                            {
                                string[] tmpCC = null;
                                string[] tmpDate = null;
                                string[] tmpNumber = null;
                                string[] tmpSplittedValue = null;
                                string recordPattern = @".*\s*\d{2}\.\d{2}\.\d{4}\s*[A-Z]{2}$";
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
                            /*57*/
                            if (inidCode.StartsWith(I57))
                            {
                                tmpRecValue = inidCode.Replace(I57, "").Trim();
                                currentElement.I57 = tmpRecValue;
                            }
                            /*21*/
                            if (inidCode.StartsWith(I21))
                            {
                                tmpRecValue = inidCode.Replace(I21, "").Replace("\n", "").Trim();
                                currentElement.I21 = tmpRecValue;
                            }
                            /*22*/
                            if (inidCode.StartsWith(I22))
                            {
                                tmpRecValue = inidCode.Replace(I22, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}"))
                                {
                                    currentElement.I22 = Methods.DateNormalize(Regex.Match(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}").Value.Trim());
                                }
                            }
                            /*24*/
                            if (inidCode.StartsWith(I24))
                            {
                                tmpRecValue = inidCode.Replace(I24, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}"))
                                {
                                    currentElement.I24 = Methods.DateNormalize(Regex.Match(tmpRecValue, @"\d{2}\.\d{2}\.\d{4}").Value.Trim());
                                }
                            }
                            /*41*/
                            if (inidCode.StartsWith(I41))
                            {
                                if (Regex.IsMatch(inidCode, @"\d{2}\.\d{2}\.\d{4}"))
                                {
                                    currentElement.I41 = Methods.DateNormalize(Regex.Match(inidCode, @"\d{2}\.\d{2}\.\d{4}").Value.Trim());
                                }
                            }
                            /*73*/
                            if (inidCode.StartsWith(I73))
                            {
                                string tmpAddress = null;
                                string tmpCountryCode = null;
                                string[] tmpSplittedValue = null;
                                tmpRecValue = inidCode.Replace(I73, "").Replace("-\n", "").Replace("\n", " ").Trim();
                                if (tmpRecValue.Contains(";"))
                                {
                                    tmpSplittedValue = tmpRecValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (tmpSplittedValue != null)
                                    {
                                        Array.Reverse(tmpSplittedValue);
                                        foreach (var record in tmpSplittedValue)
                                        {
                                            if (record.Contains(","))
                                            {
                                                string tmpSplName = null;
                                                string tmpSplAddr = null;
                                                if (Regex.IsMatch(record.ToLower(), @"(\,.*\sllc\,)|(\,.*\sgmbh\,)|(\,.*\sinc(\.|\,)*\,)|(\,.*\sltd(\.|\,)*\,)|(\,.*\ss\.a\.\,)|(\,.*\ss\.l\.\,)|(\,.*\ss\.a\.u\.\,)"))
                                                {
                                                    if (record.Count(f => f == ',') >= 2)
                                                    {
                                                        int firstIndex = record.IndexOf(",");
                                                        int secondIndex = record.IndexOf(",", firstIndex + 1);
                                                        tmpSplName = record.Remove(secondIndex).Trim();
                                                        tmpSplAddr = record.Substring(secondIndex).Trim(',').Trim();
                                                    }
                                                }
                                                else
                                                {
                                                    tmpSplName = record.Remove(record.IndexOf(",")).Trim();
                                                    tmpSplAddr = record.Substring(record.IndexOf(",")).Trim(',').Trim();
                                                }
                                                currentElement.I73N = (currentElement.I73N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplName }).ToArray();
                                                if (Regex.IsMatch(tmpSplAddr, @"\([A-Z]{2}\)$"))
                                                {
                                                    try
                                                    {
                                                        tmpCountryCode = Regex.Match(tmpSplAddr, @"\([A-Z]{2}\)$").Value.Replace("(", "").Replace(")", "").Trim();
                                                        tmpAddress = Regex.Replace(tmpSplAddr, @"\([A-Z]{2}\)$", "").Trim(',').Trim();
                                                    }
                                                    catch (Exception)
                                                    {
                                                        Console.WriteLine("73 field address or CC error");
                                                    }
                                                }
                                                else if (!Regex.IsMatch(record, @"\([A-Z]{2}\)$") && tmpCountryCode != null)
                                                {
                                                    tmpAddress = Regex.Replace(record.Substring(record.IndexOf(",")), @"\([A-Z]{2}\)$", "").Trim(',').Trim();
                                                }
                                                else
                                                {
                                                    tmpAddress = tmpSplAddr;
                                                }
                                                if (tmpAddress != null)
                                                {
                                                    currentElement.I73A = (currentElement.I73A ?? Enumerable.Empty<string>()).Concat(new string[] { tmpAddress }).ToArray();
                                                }
                                                if (tmpCountryCode != null)
                                                {
                                                    currentElement.I73C = (currentElement.I73C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpCountryCode }).ToArray();
                                                }
                                            }
                                            else
                                            {
                                                currentElement.I73N = (currentElement.I73N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray();
                                                currentElement.I73A = (currentElement.I73A ?? Enumerable.Empty<string>()).Concat(new string[] { "" }).ToArray();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (tmpRecValue.Contains(","))
                                    {
                                        string tmpSplName = null;
                                        string tmpSplAddr = null;
                                        if (Regex.IsMatch(tmpRecValue.ToLower(), @"(\,.*\sllc\,)|(\,.*\sgmbh\,)|(\,.*\sinc(\.|\,)*\,)|(\,.*\sltd(\.|\,)*\,)|(\,.*\ss\.a\.\,)|(\,.*\ss\.l\.\,)|(\,.*\ss\.a\.u\.\,)"))
                                        {
                                            if (tmpRecValue.Count(f => f == ',') >= 2)
                                            {
                                                int firstIndex = tmpRecValue.IndexOf(",");
                                                int secondIndex = tmpRecValue.IndexOf(",", firstIndex + 1);
                                                tmpSplName = tmpRecValue.Remove(secondIndex).Trim();
                                                tmpSplAddr = tmpRecValue.Substring(secondIndex).Trim(',').Trim();
                                            }
                                        }
                                        else
                                        {
                                            tmpSplName = tmpRecValue.Remove(tmpRecValue.IndexOf(",")).Trim();
                                            tmpSplAddr = tmpRecValue.Substring(tmpRecValue.IndexOf(",")).Trim(',').Trim();
                                        }
                                        currentElement.I73N = (currentElement.I73N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpSplName }).ToArray();
                                        if (Regex.IsMatch(tmpSplAddr, @"\([A-Z]{2}\)$"))
                                        {
                                            try
                                            {
                                                tmpCountryCode = Regex.Match(tmpSplAddr, @"\([A-Z]{2}\)$").Value.Replace("(", "").Replace(")", "").Trim();
                                                tmpAddress = Regex.Replace(tmpSplAddr, @"\([A-Z]{2}\)$", "").Trim(',').Trim();
                                            }
                                            catch (Exception)
                                            {
                                                Console.WriteLine("73 field address or CC error");
                                            }
                                        }
                                        else
                                        {
                                            tmpAddress = tmpSplAddr;
                                        }
                                        if (tmpAddress != null)
                                        {
                                            currentElement.I73A = (currentElement.I73A ?? Enumerable.Empty<string>()).Concat(new string[] { tmpAddress }).ToArray();
                                        }
                                        if (tmpCountryCode != null)
                                        {
                                            currentElement.I73C = (currentElement.I73C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpCountryCode }).ToArray();
                                        }
                                    }
                                    else
                                    {
                                        currentElement.I73N = (currentElement.I73N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray(); ;
                                    }
                                }
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Replace("-\n", "").Replace("\n", " ").Trim();
                                currentElement.I54 = tmpRecValue;
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
                            if (inidCode.StartsWith(I86))
                            {
                                string tmpValue = inidCode.Replace(I86, "").Trim();
                                if (tmpValue.Contains(","))
                                {
                                    currentElement.I86Number = tmpValue.Remove(tmpValue.IndexOf(",")).Trim();
                                    currentElement.I86Date = Methods.DateNormalize(tmpValue.Substring(tmpValue.IndexOf(",") + 1).Trim());
                                }
                                else
                                {
                                    currentElement.I86Number = tmpValue;
                                }
                            }
                            if (inidCode.StartsWith(I87))
                            {
                                string tmpValue = inidCode.Replace(I87, "").Trim();
                                if (tmpValue.Contains(","))
                                {
                                    currentElement.I87Number = tmpValue.Remove(tmpValue.IndexOf(",")).Trim();
                                    currentElement.I87Date = Methods.DateNormalize(tmpValue.Substring(tmpValue.IndexOf(",") + 1).Trim());
                                }
                                else
                                {
                                    currentElement.I87Number = tmpValue;
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
