﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Diamond_BA
{
    public class ProcessFirstList
    {
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I26 = "(26)";
        private static readonly string I31 = "(31)";
        private static readonly string I32 = "(32)";
        private static readonly string I33 = "(33)";
        private static readonly string I43 = "(43)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I71 = "(71)";
        private static readonly string I72 = "(72)";
        private static readonly string I73 = "(73)";
        private static readonly string I74 = "(74)";
        private static readonly string I75 = "(75)";
        private static readonly string I86 = "(86)";
        private static readonly string I87 = "(87)";

        public class ElementOut
        {
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I26 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
            public string I43 { get; set; }
            public string[] I51Version { get; set; }
            public string[] I51Class { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string[] I71N { get; set; }
            public string[] I71C { get; set; }
            public string[] I72N { get; set; }
            public string[] I72A { get; set; }
            public string[] I72C { get; set; }
            public string[] I73N { get; set; }
            public string[] I73C { get; set; }
            public string[] I74N { get; set; }
            public string[] I74C { get; set; }
            public string[] I75N { get; set; }
            public string[] I75A { get; set; }
            public string[] I75C { get; set; }
            public string I86N { get; set; }
            public string I86D { get; set; }
            public string I87N { get; set; }
            public string I87D { get; set; }

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
                    if (value.StartsWith(I21) && Regex.IsMatch(value, @"\(21\)\s"))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elemList[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count() && !Regex.IsMatch(elemList[tmpInc].Value, @"^\(21\)\s"));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
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
                            /*43*/
                            if (inidCode.StartsWith(I43))
                            {
                                tmpRecValue = inidCode.Replace(I43, "").Replace("\n", "").Trim();
                                if (tmpRecValue != "")
                                {
                                    currentElement.I43 = tmpRecValue;
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
                                if (tmpRecValue != "")
                                {
                                    currentElement.I57 = tmpRecValue;
                                }
                            }
                            /*71*/
                            if (inidCode.StartsWith(I71))
                            {
                                var tmpValue = Methods.NameSpl(inidCode);
                                if (tmpValue.Name != null) currentElement.I71N = tmpValue.Name;
                                if (tmpValue.Country != null) currentElement.I71C = tmpValue.Country;
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
                            /*75*/
                            if (inidCode.StartsWith(I75))
                            {
                                var tmpValue = Methods.NameSplWithAddress(inidCode);
                                if (tmpValue.Name != null) currentElement.I75N = tmpValue.Name;
                                if (tmpValue.Country != null) currentElement.I75C = tmpValue.Country;
                                if (tmpValue.Address != null) currentElement.I75A = tmpValue.Address;
                            }
                            /*86*/
                            if (inidCode.StartsWith(I86))
                            {
                                var datePattern = @"\d{4}\-\d{2}\-\d{2}";
                                tmpRecValue = inidCode.Replace(I87, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, datePattern))
                                {
                                    currentElement.I86D = Regex.Match(tmpRecValue, datePattern).Value;
                                    currentElement.I86N = Regex.Replace(tmpRecValue, datePattern, "").Replace(",", "").Trim();
                                }
                            }
                            /*87*/
                            if (inidCode.StartsWith(I87))
                            {
                                var datePattern = @"\d{4}\-\d{2}\-\d{2}";
                                tmpRecValue = inidCode.Replace(I87, "").Replace("\n", "").Trim();
                                if (Regex.IsMatch(tmpRecValue, datePattern))
                                {
                                    currentElement.I87D = Regex.Match(tmpRecValue, datePattern).Value;
                                    currentElement.I87N = Regex.Replace(tmpRecValue, datePattern, "").Replace(",", "").Trim();
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
