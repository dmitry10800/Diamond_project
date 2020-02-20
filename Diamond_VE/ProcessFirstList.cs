using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_VE
{
    public class ProcessFirstList
    {
        private static readonly string I11 = "(11)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I30 = "(30)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I72 = "(72)";
        private static readonly string I73 = "(73)";
        private static readonly string I74 = "(74)";

        public class ElementOut
        {
            public string I11 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
            public string[] I51Version { get; set; }
            public string[] I51Class { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string[] I72N { get; set; }
            public string[] I73N { get; set; }
            public string[] I73A { get; set; }
            public string[] I73C { get; set; }
            public string I74N { get; set; }
            public string I86N { get; set; }
            public string I86D { get; set; }
            public string I87N { get; set; }
            public string I87D { get; set; }

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
                    if (value.StartsWith(I11) && Regex.IsMatch(value, @"\(11\)\s*\d{3}.*"))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elemList[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count() && !Regex.IsMatch(elemList[tmpInc].Value, @"\(11\)\s\d{3}"));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*11*/
                            if (inidCode.StartsWith(I11))
                            {
                                tmpRecValue = inidCode.Replace(I11, "").Replace("\n", "").Trim();
                                currentElement.I11 = tmpRecValue;
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
                                currentElement.I22 = Methods.DateNormalize(tmpRecValue);
                            }
                            /*31,32,33*/
                            if (inidCode.StartsWith(I30))
                            {
                                var records = Methods.PrioritySplit(inidCode.Replace("\n", ""));
                                if (records != null)
                                {
                                    foreach (var item in records)
                                    {
                                        currentElement.I31 = (currentElement.I31 ?? Enumerable.Empty<string>()).Concat(new string[] { item.Number }).ToArray();
                                        currentElement.I32 = (currentElement.I32 ?? Enumerable.Empty<string>()).Concat(new string[] { item.Date }).ToArray();
                                        currentElement.I33 = (currentElement.I33 ?? Enumerable.Empty<string>()).Concat(new string[] { item.Country }).ToArray();
                                    }
                                }
                            }
                            /*51*/
                            if (inidCode.StartsWith(I51))
                            {
                                var tmpResults = Methods.ClassificationInfoSplit(inidCode.Replace("\n", "").Trim());
                                currentElement.I51Class = tmpResults.Classification;
                                //currentElement.I51Version = tmpResults.Version;
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Replace("-\n", " ").Replace("\n", " ").Trim();
                                currentElement.I54 = tmpRecValue;
                            }
                            /*57*/
                            if (inidCode.StartsWith(I57))
                            {
                                tmpRecValue = inidCode.Replace(I57, "").Replace("\n", "").Trim();
                                if (tmpRecValue != "")
                                {
                                    currentElement.I57 = tmpRecValue;
                                }
                            }
                            /*72*/
                            if (inidCode.StartsWith(I72))
                            {
                                var tmpValue = inidCode.Replace(I72, "").Trim();
                                if (tmpValue.Contains(";"))
                                {
                                    currentElement.I72N = tmpValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().Trim(',').Trim()).ToArray();
                                }
                                else
                                {
                                    currentElement.I72N = (currentElement.I72N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValue.Trim() }).ToArray();
                                }
                            }
                            /*73*/
                            if (inidCode.StartsWith(I73))
                            {
                                var tmpValue = Methods.AssigneeSplit(inidCode);
                                if (tmpValue != null)
                                {
                                    currentElement.I73N = tmpValue.Name;
                                    currentElement.I73A = tmpValue.Address;
                                    currentElement.I73C = tmpValue.Country;
                                }
                            }
                            /*74*/
                            if (inidCode.StartsWith(I74))
                            {
                                currentElement.I74N = inidCode.Replace(I74, "").Trim();
                            }
                            /*86*/
                            //if (inidCode.StartsWith(I86))
                            //{
                            //    string datePattern = @"\d{4}\-\d{2}\-\d{2}";
                            //    tmpRecValue = inidCode.Replace(I87, "").Replace("\n", "").Trim();
                            //    if (Regex.IsMatch(tmpRecValue, datePattern))
                            //    {
                            //        currentElement.I86D = Regex.Match(tmpRecValue, datePattern).Value;
                            //        currentElement.I86N = Regex.Replace(tmpRecValue, datePattern, "").Replace(",", "").Trim();
                            //    }
                            //}
                            ///*87*/
                            //if (inidCode.StartsWith(I87))
                            //{
                            //    string datePattern = @"\d{4}\-\d{2}\-\d{2}";
                            //    tmpRecValue = inidCode.Replace(I87, "").Replace("\n", "").Trim();
                            //    if (Regex.IsMatch(tmpRecValue, datePattern))
                            //    {
                            //        currentElement.I87D = Regex.Match(tmpRecValue, datePattern).Value;
                            //        currentElement.I87N = Regex.Replace(tmpRecValue, datePattern, "").Replace(",", "").Trim();
                            //    }
                            //}
                        }
                    }
                }
            }
            return ElementsOut;
        }
    }
}
