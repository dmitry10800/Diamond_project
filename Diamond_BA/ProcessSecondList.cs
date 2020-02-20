using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_BA
{
    public class ProcessSecondList
    {
        private static readonly string I11 = "(11)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I26 = "(26)";
        private static readonly string I31 = "(31)";
        private static readonly string I32 = "(32)";
        private static readonly string I33 = "(33)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I72 = "(72)";
        private static readonly string I73 = "(73)";
        private static readonly string I74 = "(74)";
        private static readonly string I96 = "(96)";
        private static readonly string I97 = "(97)";
        private static readonly string I99 = "(I99)";

        public class ElementOut
        {
            public string I11 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I26 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
            public string[] I51Version { get; set; }
            public string[] I51Class { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string[] I72N { get; set; }
            public string[] I72A { get; set; }
            public string[] I72C { get; set; }
            public string[] I73N { get; set; }
            public string[] I73C { get; set; }
            public string[] I74N { get; set; }
            public string[] I74C { get; set; }
            public string I96N { get; set; }
            public string I96D { get; set; }
            public string I97D { get; set; }
            public string I99 { get; set; }

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
                    if (value.StartsWith(I11) && Regex.IsMatch(value, @"\(11\)\s*BA\/EP\d+"))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elemList[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count() && !Regex.IsMatch(elemList[tmpInc].Value, @"\(11\)\s*BA\/EP\d+"));
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
                                currentElement.I22 = tmpRecValue;
                            }
                            /*26*/
                            if (inidCode.StartsWith(I26))
                            {
                                tmpRecValue = inidCode.Replace(I26, "").Replace("\n", "").Trim();
                                if (tmpRecValue != "")
                                {
                                    currentElement.I26 = tmpRecValue;
                                }
                            }
                            /*31,32,33*/
                            if (inidCode.StartsWith(I31))
                            {
                                var records = Methods.PrioritySplit(inidCode);
                                foreach (var item in records)
                                {
                                    currentElement.I31 = (currentElement.I31 ?? Enumerable.Empty<string>()).Concat(new string[] { item.Number }).ToArray();
                                    currentElement.I32 = (currentElement.I32 ?? Enumerable.Empty<string>()).Concat(new string[] { item.Date }).ToArray();
                                    currentElement.I33 = (currentElement.I33 ?? Enumerable.Empty<string>()).Concat(new string[] { item.Country }).ToArray();
                                }
                            }
                            /*51*/
                            if (inidCode.StartsWith(I51))
                            {
                                var tmpResults = Methods.ClassificationInfoSplit(inidCode.Replace("\n", "").Trim());
                                currentElement.I51Class = tmpResults.Classification;
                                currentElement.I51Version = tmpResults.Version;
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
                                tmpRecValue = inidCode.Replace(I57, "").Replace("\n", " ").Trim();
                                if (tmpRecValue != null && tmpRecValue.Contains(I99))
                                {
                                    currentElement.I57 = tmpRecValue.Remove(tmpRecValue.IndexOf(I99)).Trim();
                                    currentElement.I99 = tmpRecValue.Substring(tmpRecValue.IndexOf(I99)).Replace(I99, "").Trim();
                                }
                                else if (tmpRecValue != "")
                                {
                                    currentElement.I57 = tmpRecValue;
                                }
                            }
                            /*72*/
                            if (inidCode.StartsWith(I72))
                            {

                                var tmpValue = Methods.NameSplWithAddress(inidCode);
                                if (tmpValue.Name != null) currentElement.I72N = tmpValue.Name;
                                if (tmpValue.Country != null) currentElement.I72C = tmpValue.Country;
                                if (tmpValue.Address != null) currentElement.I72A = tmpValue.Address;
                            }
                            /*73*/
                            if (inidCode.StartsWith(I73))
                            {
                                var tmpValue = Methods.NameSpl(inidCode);
                                if (tmpValue.Name != null) currentElement.I73N = tmpValue.Name;
                                if (tmpValue.Country != null) currentElement.I73C = tmpValue.Country;
                            }
                            /*74*/
                            if (inidCode.StartsWith(I74))
                            {
                                var tmpValue = Methods.NameSpl(inidCode);
                                if (tmpValue.Name != null) currentElement.I74N = tmpValue.Name;
                                if (tmpValue.Country != null) currentElement.I74C = tmpValue.Country;
                            }
                            /*96*/
                            if (inidCode.StartsWith(I96))
                            {
                                string datePattern = @"\d{4}\-\d{2}\-\d{2}";
                                tmpRecValue = inidCode.Replace(I96, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, datePattern))
                                {
                                    currentElement.I96D = Regex.Match(tmpRecValue, datePattern).Value;
                                    currentElement.I96N = Regex.Replace(tmpRecValue, datePattern, "").Replace(",", "").Trim();
                                }
                            }
                            /*97*/
                            if (inidCode.StartsWith(I97))
                            {
                                string datePattern = @"\d{4}\-\d{2}\-\d{2}";
                                tmpRecValue = inidCode.Replace(I97, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, datePattern))
                                {
                                    currentElement.I97D = Regex.Match(tmpRecValue, datePattern).Value;
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
