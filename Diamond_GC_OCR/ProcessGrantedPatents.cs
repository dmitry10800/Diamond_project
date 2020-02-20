using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_GC_OCR
{
    public class ProcessGrantedPatents
    {
        private static readonly string I11 = "[11]";
        private static readonly string I19 = ")19(";
        private static readonly string I11Word = "[11] Patent No.:";
        private static readonly string I12 = "[12]";
        private static readonly string I21 = "[21]";
        private static readonly string I21Word = "[21] Application No.:";
        private static readonly string I22 = "[22]";
        private static readonly string I22Word = "[22] Filing Date:";
        private static readonly string I30 = "[30]";
        private static readonly string I30Word = "[30] Priority:";
        private static readonly string I31 = "[31]";
        private static readonly string I32 = "[32]";
        private static readonly string I33 = "[33]";
        private static readonly string I45 = "[45]";
        private static readonly string I45Word = "[45] Date of Publishing the Grant of the Patent:";
        private static readonly string I51 = "[51]";
        private static readonly string I51Word = "[51]IPC: Int. Cl.:";
        private static readonly string I54 = "[54]";
        private static readonly string I56 = "[56]";
        private static readonly string I56Word = "[56] Cited Documents:";
        private static readonly string I57 = "[57]";
        private static readonly string I57Word = "[57] Abstract:";
        private static readonly string I72 = "[72]";
        private static readonly string I72Word = "[72] Inventors:";
        private static readonly string I73 = "[73]";
        private static readonly string I73Word = "[73] Owners:";
        private static readonly string I74 = "[74]";
        private static readonly string I74Word = "[74] Agent:";
        private static readonly string INote = "Note:";
        public class ElementOut
        {
            public string I11 { get; set; }
            public string I12 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I31Number { get; set; }
            public string[] I32Date { get; set; }
            public string[] I33State { get; set; }
            public string I45 { get; set; }
            public string I51VersionYear { get; set; }
            public string[] I51ClasNumber { get; set; }

            public List<string> INotes = new List<string>();
            public string I54 { get; set; }
            public string I56 { get; set; }
            public string I57 { get; set; }
            public string[] I72N { get; set; }
            public string[] I73N { get; set; }
            public string[] I73A { get; set; }
            public string[] I73C { get; set; }
            public string I74 { get; set; }

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
                    if (value.StartsWith(I19))
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
                        && !elemList[tmpInc].Value.StartsWith(I19));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                            foreach (var inidCode in splittedRecord)
                            {
                                if (inidCode.StartsWith(I12))
                                {
                                    currentElement.I12 = inidCode.Replace(I12, "");
                                }
                                if (inidCode.StartsWith(I11))
                                {
                                    currentElement.I11 = inidCode.Replace(I11Word, "").Replace(I11Word, "").Replace("\n", "").Trim();
                                    Methods.CurrentAPNR = currentElement.I11;
                                }
                                if (inidCode.StartsWith(I21))
                                {
                                    currentElement.I21 = inidCode.Replace(I21Word, "").Replace(I21, "").Replace("\n", "").Trim();
                                }
                                if (inidCode.StartsWith(I22))
                                {
                                    currentElement.I22 = Methods.DateProcess(inidCode.Replace(I22Word, "").Replace(I22, "").Replace("\n", "").Trim());
                                }
                                if (inidCode.StartsWith(I30))
                                {
                                    var tmpPrio = Methods.PrioritySplit(inidCode.Replace(I30Word, "").Replace(I30, "").Trim());
                                    if (tmpPrio != null)
                                    {
                                        currentElement.I31Number = tmpPrio.Number;
                                        currentElement.I32Date = tmpPrio.Date;
                                        currentElement.I33State = tmpPrio.Country;
                                    }
                                }
                                if (inidCode.StartsWith(I45))
                                {
                                    string tmpValue = inidCode.Replace(I45Word, "").Replace(I45, "").Replace("\n", " ").Trim();
                                    string tmpDate = null;
                                    string noteFirst = null;
                                    string noteSecond = null;
                                    if (Regex.IsMatch(tmpValue, @"\d+\/\d+\/\d{4}"))
                                    {
                                        tmpDate = Regex.Match(tmpValue, @"\d+\/\d+\/\d{4}").Value;
                                        string[] tmpSplDate = tmpDate.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                                        if (tmpSplDate.Count() == 3)
                                        {
                                            currentElement.I45 = tmpSplDate[2] + "-" + tmpSplDate[1] + "-" + tmpSplDate[0];
                                        }
                                        if (tmpValue.Contains("Date of the Decision"))
                                        {
                                            noteSecond = tmpValue.Substring(tmpValue.IndexOf("Date of the Decision")).Trim();
                                            tmpValue = tmpValue.Remove(tmpValue.IndexOf("Date of the Decision")).Trim();
                                        }
                                        if (tmpValue.Contains("Number of the Decision"))
                                        {
                                            noteFirst = tmpValue.Substring(tmpValue.IndexOf("Number of the Decision")).Trim();
                                            tmpValue = tmpValue.Remove(tmpValue.IndexOf("Number of the Decision")).Trim();
                                        }
                                    }
                                    if (noteFirst != null)
                                        currentElement.INotes.Add(noteFirst);
                                    if (noteSecond != null)
                                        currentElement.INotes.Add(Methods.DateNoteProcess(noteSecond));
                                }
                                if (inidCode.StartsWith(I51))
                                {
                                    string tmpValue = inidCode.Replace("\n", "");
                                    tmpValue = Regex.Replace(tmpValue, @"\s+", " ");
                                    var tmpIPC = Methods.ClassificationInfoSplit(tmpValue.Replace(I51Word, "").Replace(I51, "").Trim());
                                    if (tmpIPC != null)
                                    {
                                        currentElement.I51ClasNumber = tmpIPC.Class;
                                        currentElement.I51VersionYear = tmpIPC.Date;
                                    }
                                }
                                if (inidCode.StartsWith(I54))
                                {
                                    currentElement.I54 = inidCode.Replace(I54, "").Replace("\n", " ").Trim();
                                }
                                if (inidCode.StartsWith(I56))
                                {
                                    string tmpValue = inidCode.Replace(I56Word, "").Replace(I56, "").Replace("\n", " ").Trim();
                                    if (inidCode.Contains("Examiner:")) currentElement.I56 = tmpValue.Remove(tmpValue.IndexOf("Examiner:"));
                                    else currentElement.I56 = tmpValue;

                                }
                                if (inidCode.StartsWith(I57))
                                {
                                    string tmpValue = inidCode.Replace(I57Word, "").Replace(I57, "").Trim();

                                    if (tmpValue.Contains("No. of claims:"))
                                    {
                                        if (tmpValue.Contains("No. of figures:"))
                                        {
                                            currentElement.INotes.Add(tmpValue.Substring(tmpValue.IndexOf("No. of figures:")).Trim());
                                            tmpValue = tmpValue.Remove(tmpValue.IndexOf("No. of figures:"));
                                        }
                                        currentElement.INotes.Add(tmpValue.Substring(tmpValue.IndexOf("No. of claims:")).Trim());
                                        tmpValue = tmpValue.Remove(tmpValue.IndexOf("No. of claims:"));
                                    }
                                    currentElement.I57 = tmpValue.Replace(I57Word, "").Replace(I57, "").Replace("\n", " ").Trim();

                                }
                                if (inidCode.StartsWith(I72))
                                {
                                    var tmpValue = inidCode.Replace(I72Word, "").Replace("[72] Inventor:", "").Replace(I72, "").Replace("\n", " ").Replace("،", " ").Trim();
                                    var inventors = Methods.InventorSplit(tmpValue);
                                    if (inventors != null)
                                        currentElement.I72N = inventors.Name;
                                }
                                if (inidCode.StartsWith(I73))
                                {
                                    string tmpValue = inidCode.Replace("[73] Owners:1-", "").Replace("[73] Owner:", "").Replace(I73Word, "").Replace(I73, "").Replace("\n", " ").Trim();
                                    var owners = Methods.OwnerSplit(tmpValue);
                                    if (owners != null && owners.Name.Count > 0)
                                    {
                                        currentElement.I73N = owners.Name.ToArray();
                                        currentElement.I73A = owners.Address.ToArray();
                                        currentElement.I73C = owners.Country.ToArray();
                                    }
                                }
                                if (inidCode.StartsWith(I74))
                                {
                                    string tmpValue = inidCode.Replace(I74Word, "").Replace(I74, "").Trim();
                                    currentElement.I74 = tmpValue;
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
