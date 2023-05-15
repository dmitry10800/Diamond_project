using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_SM
{
    class ProcessFirstList
    {
        public static readonly string I21 = "(219) N. Domanda:";
        public static readonly string I22 = "(220) Data Deposito:";
        public static readonly string I30 = "Priorità:";
        public static readonly string I54 = "Titolo";
        public static readonly string I57 = "(57) Riassunto:";
        public static readonly string I72 = "(720) Inventori/Inventors:";
        public static readonly string I73 = "(730) Richiedenti/Applicants:";
        public static readonly string I74 = "(740) Consulente/Agent:";
        public static readonly string I96Number = "N. Domanda Internazionale:";
        public static readonly string I96Date = "Data Deposito Internazionale:";
        public static readonly string I97 = "N. Pubblicazione EP:";


        public class ElementOut
        {
            public string I21Number { get; set; }
            public string I22Date { get; set; }
            public string I54Title { get; set; }
            public string I57Absract { get; set; }
            public List<PrioStruct> I30Prio { get; set; }
            public List<InventorsStruct> I72Inventors { get; set; }
            public List<ApplicantsStruct> I73Applicants { get; set; }
            public AgentStruct I74Agent { get; set; }
            public string I96Number { get; set; }
            public string I96Date { get; set; }
            public string I97 { get; set; }
            public struct PrioStruct { public string Number { get; set; } public string Date { get; set; } public string Country { get; set; } }
            public struct InventorsStruct { public string Name { get; set; } }
            public struct ApplicantsStruct { public string Name { get; set; } public string Address { get; set; } public string Country { get; set; } }
            public struct AgentStruct { public string Name { get; set; } public string Address { get; set; } }

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
                    if (value.StartsWith(I21))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elemList[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elemList.Count() && !elemList[tmpInc].Value.StartsWith(I21));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                        }
                        if (splittedRecord.Count() < 9)
                        {
                            Console.WriteLine("TETml issue to fix!");
                        }
                        foreach (var record in splittedRecord)
                        {
                            if (record.StartsWith(I21))
                            {
                                currentElement.I21Number = record.Replace(I21, "").Trim();
                            }
                            if (record.StartsWith(I22))
                            {
                                currentElement.I22Date = Methods.DateNormalize(record.Replace(I22, "").Trim());
                            }
                            if (record.StartsWith(I30))
                            {
                                currentElement.I30Prio = Methods.PriorityProcess(record.Replace(I30, "").Trim());
                            }
                            if (record.StartsWith(I54))
                            {
                                currentElement.I54Title = record.Replace(I54, "").Trim();
                            }
                            if (record.StartsWith(I57))
                            {
                                currentElement.I57Absract = record.Replace(I57, "").Trim();
                            }
                            if (record.StartsWith(I72))
                            {
                                currentElement.I72Inventors = Methods.InventorsProcess(record.Replace(I72, "").Trim());
                            }
                            if (record.StartsWith(I73))
                            {
                                try
                                {
                                    currentElement.I73Applicants = Methods.ApplicantsProcess(record.Replace(I73, "").Trim());
                                    if (currentElement.I73Applicants[0].Name == null)
                                    {
                                        Console.WriteLine("Error in 730 (Applicant) identification:\t" + currentElement.I21Number);
                                    }
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Error with I73 identification:\t" + currentElement.I21Number);
                                }
                            }
                            if (record.StartsWith(I74))
                            {
                                currentElement.I74Agent = Methods.AgentProcess(record.Replace(I74, "").Trim());
                            }
                            if (record.StartsWith(I96Number))
                            {
                                var tmpValue = record.Replace(I96Number, "").Trim();
                                if (tmpValue != "") currentElement.I96Number = tmpValue;
                            }
                            if (record.StartsWith(I96Date))
                            {
                                var tmpValue = record.Replace(I96Date, "").Trim();
                                if (tmpValue != "") currentElement.I96Date = Methods.DateNormalize(tmpValue);
                            }
                            if (record.StartsWith(I97))
                            {
                                var tmpValue = record.Replace(I97, "").Trim();
                                if (tmpValue != "") currentElement.I97 = tmpValue;
                            }
                        }
                    }
                }
            }
            return ElementsOut;
        }

    }
}
