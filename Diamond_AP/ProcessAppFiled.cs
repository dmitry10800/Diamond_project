using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_AP
{
    public class ProcessAppFiled
    {
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I23 = "(23)";
        private static readonly string I31 = "(31)";
        private static readonly string I32 = "(32)";
        private static readonly string I33 = "(33)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I71 = "(71)";
        private static readonly string I72 = "(72)";
        private static readonly string I74 = "(74)";
        private static readonly string I75 = "(75)";
        private static readonly string I84 = "(84)";
        private static readonly string I86 = "(86)";
        private static readonly string I96 = "(96)";

        public class ElementOut
        {
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I23 { get; set; }
            public string[] I51V { get; set; }
            public string[] I51Y { get; set; }
            public string I54 { get; set; }
            public string[] I71 { get; set; }
            public string[] I75 { get; set; }
            public string[] I72 { get; set; }
            public string I74 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
            public string[] I84 { get; set; }
            public string I86N { get; set; }
            public string I86D { get; set; }
            public string I96N { get; set; }
            public string I96D { get; set; }
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
                    if (value.StartsWith(I21))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        var tmpInc = i;
                        tmpRecordValue = "";
                        do
                        {
                            tmpRecordValue += elemList[tmpInc].Value + " ";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count()
                        && !elemList[tmpInc].Value.StartsWith(I21));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                            foreach (var inidCode in splittedRecord)
                            {
                                if (inidCode.StartsWith(I21))
                                {
                                    currentElement.I21 = inidCode.Replace(I21, "").Trim();
                                }
                                if (inidCode.StartsWith(I22))
                                {
                                    currentElement.I22 = Methods.DateNormalize(inidCode.Replace(I22, "").Trim());
                                }
                                if (inidCode.StartsWith(I23))
                                {
                                    currentElement.I23 = Methods.DateNormalize(inidCode.Replace(I23, "").Trim());
                                }
                                if (inidCode.StartsWith(I51))
                                {
                                    var tmpValue = inidCode.Replace(I51, "").Trim();
                                    var splittedIPC = Methods.IPCSplit(tmpValue, out var ipcYear);
                                    currentElement.I51Y = ipcYear;
                                    foreach (var item in splittedIPC)
                                    {
                                        currentElement.I51V = (currentElement.I51V ?? Enumerable.Empty<string>()).Concat(new string[] { item.Trim() }).ToArray();
                                    }
                                }
                                if (inidCode.StartsWith(I54))
                                {
                                    currentElement.I54 = Methods.DateNormalize(inidCode.Replace(I54, "").Trim());
                                }
                                if (inidCode.StartsWith(I31))
                                {
                                    currentElement.I31 = (currentElement.I31 ?? Enumerable.Empty<string>()).Concat(new string[] { inidCode.Replace(I31, "").Trim() }).ToArray();
                                }
                                if (inidCode.StartsWith(I32))
                                {
                                    currentElement.I32 = (currentElement.I32 ?? Enumerable.Empty<string>()).Concat(new string[] { Methods.DateNormalize(inidCode.Replace(I32, "").Trim()) }).ToArray();
                                }
                                if (inidCode.StartsWith(I33))
                                {
                                    currentElement.I33 = (currentElement.I33 ?? Enumerable.Empty<string>()).Concat(new string[] { inidCode.Replace(I33, "").Trim() }).ToArray();
                                }
                                if (inidCode.StartsWith(I71))
                                {
                                    currentElement.I71 = Methods.OwnerSplit(inidCode.Replace(I71, "").Trim());
                                }
                                if (inidCode.StartsWith(I72))
                                {
                                    currentElement.I72 = Methods.OwnerSplit(inidCode.Replace(I72, "").Trim());
                                }
                                if (inidCode.StartsWith(I74))
                                {
                                    currentElement.I74 = inidCode.Replace(I74, "").Trim();
                                }
                                if (inidCode.StartsWith(I75))
                                {
                                    currentElement.I75 = Methods.OwnerSplit(inidCode.Replace(I75, "").Trim());
                                }
                                if (inidCode.StartsWith(I84))
                                {
                                    var tmpValue = inidCode.Replace(I84, "").Trim();
                                    currentElement.I84 = Methods.OwnerSplit(tmpValue);
                                }
                                if (inidCode.StartsWith(I86))
                                {
                                    string pctNumber = null;
                                    var tmpValue = inidCode.Replace(I86, "").Trim();
                                    currentElement.I86D = Methods.PctSplit(tmpValue, out pctNumber);
                                    currentElement.I86N = pctNumber;

                                }
                                if (inidCode.StartsWith(I96))
                                {
                                    var tmpValue = inidCode.Replace(I96, "").Trim();
                                    string pctNumber = null;
                                    currentElement.I96D = Methods.PctSplit(tmpValue, out pctNumber);
                                    currentElement.I96N = pctNumber;
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
