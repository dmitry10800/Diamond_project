using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Diamond_AR
{
    class ProcessApplications
    {
        private static readonly string I10 = "(10)";
        private static readonly string I21 = "(21)";
        private static readonly string I22 = "(22)";
        private static readonly string I30 = "(30)";
        private static readonly string I41 = "(41) Fecha:";
        private static readonly string I51 = "(51)";
        private static readonly string I54 = "(54)";
        private static readonly string I57 = "(57)";
        private static readonly string I62 = "(62)";
        private static readonly string I71 = "(71)";
        private static readonly string I72 = "(72)";
        private static readonly string I74 = "(74)";
        private static readonly string I83 = "(83)";

        public class ElementOut
        {
            public string I10 { get; set; }
            public string I13 { get; set; }
            public string I21 { get; set; }
            public string I22 { get; set; }
            public string[] I30C { get; set; }
            public string[] I30N { get; set; }
            public string[] I30D { get; set; }
            public string I41 { get; set; }
            public string I41B { get; set; }
            public string[] I51 { get; set; }
            public string I54 { get; set; }
            public string I57 { get; set; }
            public string I62 { get; set; }
            public string I62Kind { get; set; }
            public string[] I71N { get; set; }
            public string[] I71A { get; set; }
            public string[] I71C { get; set; }
            public string[] I72 { get; set; }
            public string I74 { get; set; }
            public string I83 { get; set; }
        }

        public List<ElementOut> OutputValue(List<XElement> elements)
        {
            List<ElementOut> ElementsOut = new List<ElementOut>();
            ElementOut currentElement;
            if (elements != null)
            {
                for (int i = 0; i < elements.Count; ++i)
                {
                    var element = elements[i];
                    string value = element.Value;
                    string tmpRecordValue = null;
                    string tmpRecValue;
                    string[] splittedRecord = null;
                    int tmpInc;
                    if (value.StartsWith(I10) && Regex.IsMatch(value, @"\(10\)\s"))
                    {
                        currentElement = new ElementOut();
                        ElementsOut.Add(currentElement);
                        tmpRecordValue = "";
                        tmpInc = i;
                        do
                        {
                            tmpRecordValue += elements[tmpInc].Value + "\n";
                            ++tmpInc;
                        } while (tmpInc < elements.Count()
                        //&& !elements[tmpInc].Value.StartsWith(I10)
                        && !Regex.IsMatch(elements[tmpInc].Value, @"^\(10\)\s"));
                        if (tmpRecordValue != null)
                        {
                            splittedRecord = Methods.RecSplit(tmpRecordValue);
                        }
                        foreach (var inidCode in splittedRecord)
                        {
                            tmpRecValue = null;
                            /*Resolution value*/
                            if (inidCode.StartsWith(I10))
                            {
                                tmpRecValue = inidCode.Replace(I10, "").Replace("\n", "").Trim();
                                if (tmpRecValue.Contains(" "))
                                {
                                    currentElement.I10 = tmpRecValue.Remove(tmpRecValue.LastIndexOf(" ")).Trim();
                                    currentElement.I13 = tmpRecValue.Substring(tmpRecValue.LastIndexOf(" ")).Trim();
                                }
                                else
                                {
                                    currentElement.I10 = tmpRecValue;
                                }
                            }
                            /*Act number*/
                            if (inidCode.StartsWith(I21))
                            {
                                tmpRecValue = inidCode.Replace(I21, "").Replace("\n", "").Trim();
                                currentElement.I21 = tmpRecValue;
                            }
                            /*22 Date*/
                            if (inidCode.StartsWith(I22))
                            {
                                tmpRecValue = Methods.DateNormalize(inidCode.Replace(I22, "").Replace("\n", "").Trim());
                                currentElement.I22 = tmpRecValue;
                            }
                            /*30 Priority*/
                            if (inidCode.StartsWith(I30))
                            {
                                string[] splPriorityRecord = null;
                                tmpRecValue = inidCode.Replace(I30, "").Trim();
                                if (tmpRecValue.Contains("\n"))
                                {
                                    splPriorityRecord = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                }
                                if (splPriorityRecord != null && splPriorityRecord.Count() > 1)
                                {
                                    foreach (var prioRecord in splPriorityRecord)
                                    {
                                        if (prioRecord.Contains(" "))
                                        {
                                            if (!prioRecord.StartsWith("PCT"))
                                            {
                                                try
                                                {
                                                    currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { prioRecord.Remove(prioRecord.IndexOf(" ")).Trim() }).ToArray();
                                                    currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { prioRecord.Remove(prioRecord.LastIndexOf(" ")).Substring(prioRecord.IndexOf(" ")).Trim() }).ToArray();
                                                    currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { Methods.DateNormalize(prioRecord.Substring(prioRecord.LastIndexOf(" ")).Trim()) }).ToArray();
                                                }
                                                catch (Exception)
                                                {
                                                    Console.WriteLine("Priority identification error in:\t" + currentElement.I10);
                                                }
                                            }
                                            else
                                            {
                                                currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { "" }).ToArray();
                                                currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { prioRecord.Remove(prioRecord.LastIndexOf(" ")).Trim() }).ToArray();
                                                currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { Methods.DateNormalize(prioRecord.Substring(prioRecord.LastIndexOf(" ")).Trim()) }).ToArray();
                                            }
                                        }
                                    }
                                }
                                else if (tmpRecValue.Contains(" "))
                                {
                                    try
                                    {
                                        currentElement.I30C = (currentElement.I30C ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Remove(tmpRecValue.IndexOf(" ")).Trim() }).ToArray();
                                        currentElement.I30N = (currentElement.I30N ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Remove(tmpRecValue.LastIndexOf(" ")).Substring(tmpRecValue.IndexOf(" ")).Trim() }).ToArray();
                                        currentElement.I30D = (currentElement.I30D ?? Enumerable.Empty<string>()).Concat(new string[] { Methods.DateNormalize(tmpRecValue.Substring(tmpRecValue.LastIndexOf(" ")).Trim()) }).ToArray();
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("Priority identification error in:\t" + currentElement.I10);
                                    }
                                }
                                //currentElement.I30 = tmpRecValue;
                            }
                            /*41*/
                            if (inidCode.StartsWith(I41))
                            {
                                tmpRecValue = inidCode.Replace(I41, "").Replace("\n", " ").Trim();
                                if (tmpRecValue.Contains("Bol. Nro.:"))
                                {
                                    string tRec = tmpRecValue.Substring(tmpRecValue.IndexOf("Bol. Nro.:")).Replace("Bol. Nro.:", "").Replace("SOLICITUDES E PATENTE", "").Trim();
                                    currentElement.I41B = tRec.Trim();
                                    currentElement.I41 = Methods.DateNormalize(tmpRecValue.Remove(tmpRecValue.IndexOf("Bol. Nro.:")).Trim());
                                }
                                else
                                {
                                    currentElement.I41 = tmpRecValue;
                                }
                            }
                            /*51 international class*/
                            if (inidCode.StartsWith(I51))
                            {
                                string[] tmpIntClass = null;
                                string tmpCodeValue = null;
                                tmpRecValue = inidCode.Replace(I51, "").Replace("\n", " ").Replace("//", ",").Trim();
                                string ipcPattern = @"([A-Z]{1}\d{2}[A-Z]{1}).*?";
                                string[] splIPC = Regex.Split(tmpRecValue, ipcPattern).Where(s => s != String.Empty).ToArray();
                                if (splIPC != null && splIPC.Count() % 2 == 0)
                                {
                                    for (int k = 0; k < splIPC.Count(); k++)
                                    {
                                        if (Regex.IsMatch(splIPC[k].Trim(), @"[A-Z]{1}\d{2}[A-Z]{1}"))
                                        {
                                            tmpCodeValue = splIPC[k];
                                        }
                                        if (splIPC[k].Contains(","))
                                        {
                                            tmpIntClass = splIPC[k].Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                            foreach (var item in tmpIntClass)
                                            {
                                                if (tmpCodeValue != null)
                                                {
                                                    currentElement.I51 = (currentElement.I51 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpCodeValue + " " + item.Trim() }).ToArray();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (tmpCodeValue != null && !Regex.IsMatch(splIPC[k].Trim(), @"[A-Z]{1}\d{2}[A-Z]{1}"))
                                            {
                                                currentElement.I51 = (currentElement.I51 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpCodeValue + " " + splIPC[k].Trim() }).ToArray();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    currentElement.I51 = (currentElement.I51 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue }).ToArray();
                                }
                            }
                            /*54 title*/
                            if (inidCode.StartsWith(I54))
                            {
                                tmpRecValue = inidCode.Replace(I54, "").Replace("-", " ").Replace("\n", " ").Trim();
                                currentElement.I54 = tmpRecValue;
                            }
                            /*57 description*/
                            if (inidCode.StartsWith(I57))
                            {
                                tmpRecValue = inidCode.Replace(I57, "").Replace("\n", " ").Trim();
                                currentElement.I57 = tmpRecValue;
                            }
                            /*62 division*/
                            if (inidCode.StartsWith(I62))
                            {
                                tmpRecValue = inidCode.Replace(I62, "").Replace("\n", " ").Trim();
                                if (Regex.IsMatch(tmpRecValue, @"^.*[A-Z]{1}\d{1}$"))
                                {
                                    var num = Regex.Match(tmpRecValue, @"(^.*)([A-Z]{1}\d{1})$");
                                    currentElement.I62 = num.Groups[1].Value;
                                    currentElement.I62Kind = num.Groups[2].Value;
                                }
                                else currentElement.I62 = tmpRecValue;
                            }
                            /*71 titular*/
                            if (inidCode.StartsWith(I71))
                            {
                                tmpRecValue = inidCode.Replace(I71, "");
                                var applicant = Methods.ApplicantIdentification(tmpRecValue);
                                if (applicant != null)
                                {
                                    currentElement.I71N = applicant.Name.ToArray();
                                    currentElement.I71A = applicant.Address.ToArray();
                                    //currentElement.I71A = "HEGENHEIMERMATTWEG 91, CH-4123 ALLSCHWIL";
                                    currentElement.I71C = applicant.CountryCode.ToArray();
                                    //currentElement.I71C = "CH";
                                }
                                else
                                {
                                    Console.WriteLine("Error with 71 field identification:\t" + currentElement.I10);
                                }
                                //if (tmpRecValue.Contains("\n"))
                                //{
                                //    string[] tmpSplValue = tmpRecValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                //    if (tmpSplValue.Count() > 1)
                                //    {
                                //        string tmpSplValueTitular = "";
                                //        currentElement.I71N = tmpSplValue[0].Replace("-", " ").Trim();
                                //        for (int s = 1; s < tmpSplValue.Count(); s++)
                                //        {
                                //            tmpSplValueTitular += " " + tmpSplValue[s];
                                //        }
                                //        if (tmpSplValueTitular != "")
                                //        {
                                //            if (Regex.IsMatch(tmpSplValueTitular.Trim(), @"\s[A-Z]{2}$"))
                                //            {
                                //                currentElement.I71A = Regex.Replace(tmpSplValueTitular.Trim(), @"\s[A-Z]{2}$", "").Trim().Trim(',').Trim();
                                //                currentElement.I71C = Regex.Match(tmpSplValueTitular.Trim(), @"\s[A-Z]{2}$").Value.Trim();
                                //            }
                                //            else
                                //            {
                                //                currentElement.I71A = tmpSplValueTitular.Trim();
                                //                currentElement.I71C = "";
                                //            }
                                //        }
                                //    }
                                //}
                            }
                            /*72 Inventor*/
                            if (inidCode.StartsWith(I72))
                            {
                                tmpRecValue = inidCode.Replace(I72, "").Replace("\n", " ");
                                if (tmpRecValue.Contains(" - "))
                                {
                                    string[] tmpSplValue = tmpRecValue.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (tmpSplValue.Count() > 0)
                                    {
                                        foreach (var inventor in tmpSplValue)
                                        {
                                            currentElement.I72 = (currentElement.I72 ?? Enumerable.Empty<string>()).Concat(new string[] { inventor.Trim() }).ToArray();
                                        }
                                    }
                                }
                                else if (tmpRecValue.Length > 5)
                                {
                                    currentElement.I72 = (currentElement.I72 ?? Enumerable.Empty<string>()).Concat(new string[] { tmpRecValue.Trim() }).ToArray();
                                }
                            }
                            /*74 Agent*/
                            if (inidCode.StartsWith(I74))
                            {
                                tmpRecValue = inidCode.Replace(I74, "").Replace("\n", " ").Trim();
                                currentElement.I74 = tmpRecValue;
                            }
                            /*83 Agent*/
                            if (inidCode.StartsWith(I83))
                            {
                                tmpRecValue = inidCode.Replace(I83, "").Replace("\n", " ").Trim();
                                currentElement.I83 = tmpRecValue;
                            }
                        }
                    }
                }
            }
            return ElementsOut;
        }
    }
}
