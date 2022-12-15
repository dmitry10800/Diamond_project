using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Diamond_ID
{
    public class ProcessFirstList
    {
        private static readonly string I11 = "(11) No Pengumuman :";
        private static readonly string I13 = "(13)";
        private static readonly string I20 = "(20) RI Permohonan Paten";
        private static readonly string I21 = "(21) No. Permohonan Paten :";
        private static readonly string I22 = "(22) Tanggal Penerimaan Permohonan Paten :";
        private static readonly string I30 = "(30) Data Prioritas :";
        private static readonly string I43 = "(43) Tanggal Pengumuman Paten :";
        private static readonly string I51 = "(51) I.P.C :";
        private static readonly string I54 = "(54) Judul Invensi :";
        private static readonly string I57 = "(57) Abstrak :";
        private static readonly string I71 = "(71) Nama dan Alamat yang Mengajukan Permohonan Paten :";
        private static readonly string I72 = "(72) Nama Inventor :";
        private static readonly string I74 = "(74) Nama dan Alamat Konsultan Paten :";

        public class ElementOut
        {
            public string I11 { get; set; }
            public string I13 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string I43 { get; set; }
            public string[] I51D { get; set; }
            public string[] I51C { get; set; }
            public string I51Notes { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string I71 { get; set; }
            public string[] I72N { get; set; }
            public string[] I72C { get; set; }
            public string I74 { get; set; }
            public string[] I31 { get; set; }
            public string[] I32 { get; set; }
            public string[] I33 { get; set; }
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
                    if (value.StartsWith(I20))
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
                        && !elemList[tmpInc].Value.StartsWith(I20));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                            foreach (var inidCode in splittedRecord)
                            {
                                if (inidCode.StartsWith(I11))
                                {
                                    currentElement.I11 = inidCode.Replace(I11, "").Replace("\n", "").Trim();
                                }
                                if (inidCode.StartsWith(I13))
                                {
                                    currentElement.I13 = inidCode.Replace(I13, "").Replace("\n", "").Trim();
                                }
                                if (inidCode.StartsWith(I21))
                                {
                                    currentElement.I21 = inidCode.Replace(I21, "").Replace("\n", "").Trim();
                                }
                                if (inidCode.StartsWith(I22))
                                {
                                    currentElement.I22 = Methods.DateNormalize(inidCode.Replace(I22, "").Replace("\n", "").Trim());
                                }
                                if (inidCode.StartsWith(I30))
                                {
                                    var processedPrio = Methods.PrioritySplit(inidCode);
                                    if (processedPrio != null)
                                    {
                                        currentElement.I31 = processedPrio.Number;
                                        currentElement.I32 = processedPrio.Date;
                                        currentElement.I33 = processedPrio.Country;
                                    }
                                }
                                if (inidCode.StartsWith(I43))
                                {
                                    currentElement.I43 = Methods.DateNormalize(inidCode.Replace(I43, "").Replace("\n", "").Trim());
                                }
                                if (inidCode.StartsWith(I51))
                                {
                                    var processedValue = Methods.ClassificationInfoSplit(inidCode);
                                    if (processedValue != null && processedValue.Class != null)
                                    {
                                        if (processedValue.Date != null) currentElement.I51D = processedValue.Date;
                                        currentElement.I51C = processedValue.Class;
                                        if (processedValue.NotesValue != null) currentElement.I51Notes = processedValue.NotesValue;
                                    }
                                }
                                if (inidCode.StartsWith(I54))
                                {
                                    currentElement.I54 = inidCode.Replace(I54, "").Replace("\n", " ").Trim();
                                }
                                if (inidCode.StartsWith(I57))
                                {
                                    currentElement.I57 = inidCode.Replace(I57, "").Replace("\n", " ").Trim();
                                }
                                if (inidCode.StartsWith(I71))
                                {
                                    currentElement.I71 = inidCode.Replace(I71, "").Replace("\n", " ").Trim();
                                }
                                if (inidCode.StartsWith(I72))
                                {
                                    var inventors = Methods.InventorSplit(inidCode);
                                    currentElement.I72N = inventors.Name;
                                    currentElement.I72C = inventors.Country;
                                }
                                if (inidCode.StartsWith(I74))
                                {
                                    if (inidCode.Replace(I74, "").Replace("\n", " ").Trim() != "")
                                    {
                                        currentElement.I74 = inidCode.Replace(I74, "").Replace("\n", " ").Trim();
                                    }
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
