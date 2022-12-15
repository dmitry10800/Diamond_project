﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_AP
{
    public class ProcessAppPendingGrant
    {
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I23 = "(23)";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I74 = "(74)";
        private static readonly string I75 = "(75)";
        private static readonly string I84 = "(84)";

        public class ElementOut
        {
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I23 { get; set; }
            public string[] I51V { get; set; }
            public string[] I51Y { get; set; }
            public string I54 { get; set; }
            public string I74 { get; set; }
            public string[] I75 { get; set; }
            public string[] I84 { get; set; }
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
                    if (value.StartsWith(I21))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        int tmpInc = i;
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
                                    string tmpValue = inidCode.Replace(I51, "").Trim();
                                    string[] splittedIPC = Methods.IPCSplit(tmpValue, out string[] ipcYear);
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
                                    string tmpValue = inidCode.Replace(I84, "").Trim();
                                    currentElement.I84 = Methods.OwnerSplit(tmpValue);
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
