using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_MY
{
    class ProcessGrants
    {
        public static readonly string I11 = "(11)";
        public static readonly string I12 = "(12)";
        public static readonly string I21 = "Application No. :";
        public static readonly string I22 = "Filing Date :";
        public static readonly string I30 = "Priority Data :";
        public static readonly string I47 = "Date of Publication and Grant :";
        public static readonly string I51 = "Classification, INT CL";
        public static readonly string I54 = "Title :";
        public static readonly string I56 = "Prior Art :";
        public static readonly string I57 = "Abstract :";
        public static readonly string I72 = "Inventors :";
        public static readonly string I73 = "Patent Owner :";
        public static readonly string I74 = "Agent :";

        public class ElementOut
        {
            public struct I11Struct { public string Number { get; set; } public string Kind { get; set; } }
            public I11Struct I11Values { get; set; }
            public string I12Value { get; set; }
            public string I21App { get; set; }
            public string I22Date { get; set; }
            public string I47Date { get; set; }
            public string I54Title { get; set; }
            public string I57Absract { get; set; }
            public struct PrioStruct { public string Number { get; set; } public string Date { get; set; } public string Country { get; set; } }
            public struct IntClassStruct { public string Number { get; set; } public string Date { get; set; } public string DigitClass { get; set; } }
            public struct PrioArtStruct { public string Country { get; set; } public string Number { get; set; } public string Kind { get; set; } }
            public struct InventorsStruct { public string Name { get; set; } }
            public struct OwnersStruct { public string Name { get; set; } public string Address { get; set; } public string Country { get; set; } }
            public struct AgentStruct { public string Name { get; set; } public string Address { get; set; } }
            public AgentStruct I74Agent { get; set; }
            public List<PrioStruct> I30Prio { get; set; }
            public List<IntClassStruct> I51IntCl { get; set; }
            public List<PrioArtStruct> I56PrioArt { get; set; }
            public List<InventorsStruct> I72Inventors { get; set; }
            public List<OwnersStruct> I73Owners { get; set; }
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
                    if (value.StartsWith(I12))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elemList[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count() && !elemList[tmpInc].Value.StartsWith(I12));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                        }
                        foreach (var record in splittedRecord)
                        {
                            if (record.StartsWith(I12))
                            {
                                string tmpValue = record.Trim();
                                if (tmpValue.Contains(I11))
                                {
                                    currentElement.I11Values = Methods.I11Process(record.Substring(record.IndexOf(I11)).Replace(I11, "").Trim());
                                    tmpValue = tmpValue.Remove(tmpValue.IndexOf(I11)).Trim();
                                }
                                else
                                {

                                }
                                currentElement.I12Value = tmpValue.Replace(I12, "").Trim();
                            }
                            if (record.StartsWith(I21))
                            {
                                //currentElement.I21App = record.Replace(I21, "").Trim();
                            }
                            if (record.StartsWith(I22))
                            {
                                //currentElement.I22Date = Methods.DateNormalize(record.Replace(I22, "").Trim());
                            }
                            if (record.StartsWith(I30))
                            {
                                //currentElement.I30Prio = Methods.PriorityProcess(record.Replace(I30, "").Trim());
                            }
                            if (record.StartsWith(I47))
                            {
                                currentElement.I47Date = Methods.DateNormalize(record.Replace(I47, "").Trim());
                            }
                            if (record.StartsWith(I51))
                            {
                                currentElement.I51IntCl = Methods.IntClasProcess(record.Replace(I51, "").Trim());
                            }
                            if (record.StartsWith(I54))
                            {
                                //currentElement.I54Title = record.Replace(I54, "").Replace("\n", " ");
                            }
                            if (record.StartsWith(I57))
                            {
                                //currentElement.I57Absract = record.Replace(I57, "").Replace("\n", " ");
                            }
                            if (record.StartsWith(I72))
                            {
                                //currentElement.I72Inventors = Methods.InventorsProcess(record.Replace(I72, "").Trim());
                            }
                            if (record.StartsWith(I73))
                            {
                                //currentElement.I73Owners = Methods.OwnersProcess(record.Replace(I73, "").Trim());
                            }
                            if (record.StartsWith(I74))
                            {
                                //currentElement.I74Agent = Methods.AgentProcess(record.Replace(I74, "").Trim());
                            }
                        }
                    }
                }
            }
            return ElementsOut;
        }
    }
}
