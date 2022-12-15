using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_ME
{
    class ProcessPublicationsOfEuroApp
    {
        private static readonly string I31 = "(31)";
        private static readonly string I32 = "(32)";
        private static readonly string I33 = "(33)";
        private static readonly string I51 = "(51) MKP";
        private static readonly string I54 = "(54)";
        private static readonly string I71 = "(71)";
        private static readonly string I72 = "(72)";
        private static readonly string I96 = "(96)";
        private static readonly string I97 = "(97)";
        public class ElementOut
        {
            public struct IPCStruct { public string Number { get; set; } public string Version { get; set; } }
            public struct OwnerStruct { public string Name { get; set; } public string Address { get; set; } public string Country { get; set; } }
            public struct PriorityStruct { public string Number { get; set; } public string Date { get; set; } public string Country { get; set; } }
            public struct I96Struct { public string Number { get; set; } public string Date { get; set; } }
            public struct I97Struct { public string Number { get; set; } public string Date { get; set; } }
            public List<IPCStruct> IPCValues { get; set; }
            public List<OwnerStruct> Applicants { get; set; } //71
            public List<OwnerStruct> Inventors { get; set; } //72
            public List<PriorityStruct> Priority { get; set; }
            public I96Struct I96 { get; set; }
            public I97Struct I97 { get; set; }
            public string I51 { get; set; }
            public string I54 { get; set; }
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
                    string[] splittedRecord = null;
                    int tmpInc;
                    if (value.StartsWith(I51))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        string tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elemList[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count() && !elemList[tmpInc].Value.StartsWith(I51));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                        }
                        foreach (var record in splittedRecord)
                        {
                            if (record.StartsWith(I51))
                            {
                                currentElement.IPCValues = Methods.IPCProcess(record.Replace(I51, "").Trim());
                            }
                            if (record.StartsWith(I96))
                            {
                                currentElement.I96 = Methods.I96Process(record.Replace(I96, "").Trim());
                            }
                            if (record.StartsWith(I97))
                            {
                                currentElement.I97 = Methods.I97Process(record.Replace(I97, "").Trim());
                            }
                            if (record.StartsWith(I31))
                            {
                                currentElement.Priority = Methods.PriorityProcess(record.Replace(I31, "").Trim());
                            }
                            if (record.StartsWith(I54))
                            {
                                currentElement.I54 = record.Replace(I54, "").Trim();
                            }
                            if (record.StartsWith(I71))
                            {
                                currentElement.Applicants = Methods.ApplicantsProcess(record.Replace(I71, "").Trim());
                            }
                            if (record.StartsWith(I72))
                            {
                                currentElement.Inventors = Methods.InventorsProcess(record.Replace(I72, "").Trim());
                            }
                        }
                    }
                }
            }
            return ElementsOut;
        }
    }
}
